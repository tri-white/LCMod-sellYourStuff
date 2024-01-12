# Project description
This project is a mod for game "Lethal Company". It's written with the help of BepInExPlugin, and it's posted on thunderstore community website.

It's purpose is to let players sell store-bought items for 50% of their original price

Items can't be collected as scrap, so they don't count in overall value at the end of each round/day

## Introduction
You bought whole lot of equipment for you and your crew, but as the deadline approaches, you're falling short of meeting your quota?

##### This mod is for you!

### Features:
+ **scan** store-bought items to see their name and price   
  
+ **sell** store-bought items at company building and get *credits* and *quota progress*!  

  + items are sold for **50%** of their price, including discounts

### Warning
This mod is not fully tested. I appreciate any help

### Noticed issues

- If item was bought for 140 but delivered on the day when its price was at 200 - then it will cost 200

# plans
-Add the configuration classes where user can:

  -Set the radius of scanning for items
  -Set the % at which his items will be sold
  -Set the list of items that can be sold (so it will be easier to make mod compatible with other mods that add items)
-Maybe optimize the plugin:

  -Apply plugin specifically to the classes of store-bought items. It means there will be lot of repeating lines of code, but in the end it may make performance better, because as of now, my plugin gets called each time when GrabbableItem instance is created and method Start() called for it

#### [For support](https://www.buymeacoffee.com/axeron)  
I really appreciate any support and kind words!

