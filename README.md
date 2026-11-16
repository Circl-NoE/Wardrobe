# Wardrobe
A mod for A Township Tale that expands the number of customization preset slots you can use.

## How it works
After installing the mod (simply put it in your mods folder), you'll see two arrow buttons in front of the preset shelf in the customization area, main menu or on a server. These buttons allow you to go back and forth between pages. New pages are made as previous ones are filled.

Presets will be shifted back in the list to occupy as little space as possible, so deleting an outfit will cause all others to be shifted back by one slot once another page is loaded.

Slots are always given the smallest id number possible, this can result in the order of outfits shifting slightly if you delete one on one page then save a new one on the next page, as it may be assigned to the id of the deleted one and be put in its proper spot when another page is loaded.

### Melon Preferences
This mod has two config options in the melonpreferences file.
  
Burn Trophies : Represents how long it takes for the burning animation of trophies to play as you switch pages. Setting this to 0 may decrease jitteriness when switching pages

Debug Keybinds : Toggles keybinds for switching pages (- to go back = to go forward)

## Current future plans
Visual of what page you're on (ex: 2/3)
