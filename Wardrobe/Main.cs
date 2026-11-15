using Alta.Api.DataTransferModels.Models.Responses;
using Alta.Customization;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

[assembly:MelonInfo(typeof(Wardrobe.Main), "Wardrobe", "0.0.1", "Circl")]

namespace Wardrobe
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("More slots! :D");
        }

        public override void OnUpdate()
        {
            if (PageManager.Manager == null)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.minusKey.wasPressedThisFrame)
            {
                PageManager.PreviousPage(PageManager.Manager);
            }

            if (Keyboard.current != null && Keyboard.current.equalsKey.wasPressedThisFrame)
            {
                PageManager.NextPage(PageManager.Manager);
            }
        }
    }

    public static class PageManager
    {
        public static CosmeticPresetsManager Manager;

        public static List<UserPresetDataInfo> Presets = new List<UserPresetDataInfo>();

        public static int Page = 0;

        public static int MaxPage => Presets.Count / 10;

        public static async Task Refresh()
        {
            var result = await ApiAccess.ApiClient.UserClient.GetAllCosmeticsPresets();

            Presets = result.OrderBy(p => p.PresetId).ToList();
        }

        public static int GetNextAvailableId(HashSet<int> usedIds)
        {
            int id = 0;

            while (usedIds.Contains(id))
                id++;

            usedIds.Add(id);
            return id;
        }

        public static async Task LoadPage(CosmeticPresetsManager instance, int page)
        {
            try
            {
                foreach (CosmeticPresetSlot slot in instance.slots)
                {
                    slot.Clear();
                }
                if (LoginManager.IsLoggedIn && !instance.isDestroyed)
                {
                    await PageManager.Refresh();

                    Page = (page % (MaxPage + 1) + (MaxPage + 1)) % (MaxPage + 1);

                    HashSet<int> usedIds = new HashSet<int>(PageManager.Presets.Select(p => p.PresetId));

                    for (int i = 0; i < instance.slots.Length; i++)
                    {
                        int index = PageManager.Page * 10 + i;

                        if (index >= PageManager.Presets.Count)
                        {
                            instance.slots[i].Index = PageManager.GetNextAvailableId(usedIds);

                            continue;
                        }

                        UserPresetDataInfo preset = PageManager.Presets[index];

                        instance.slots[i].Index = preset.PresetId;

                        CosmeticPresetTrophy trophy = instance.GetNewTrophy();

                        trophy.Apply(preset.Data, preset.ByteSize);

                        instance.slots[i].RegisterTrophy(trophy);

                        MelonLogger.Msg($"Registered trophy to slot {index} id {preset.PresetId}");
                    }

                    instance.SetupWithManager(instance.baseHandler.Manager);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }

        public static async Task NextPage(CosmeticPresetsManager instance)
        {
            await LoadPage(instance, Page + 1);
        }
        public static async Task PreviousPage(CosmeticPresetsManager instance)
        {
            await LoadPage(instance, Page - 1);
        }
    }

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(CosmeticPresetsManager), nameof(CosmeticPresetsManager.LoggedIn)), HarmonyPrefix]
        public static bool LogInPrefix(CosmeticPresetsManager __instance)
        {
            PageManager.Manager = __instance;
            LoadPresets(__instance);
            return false;
        }
        private static async void LoadPresets(CosmeticPresetsManager instance)
        {
            await PageManager.LoadPage(instance, 0);
        }
    }
}
