# Project description
This project is a mod for game "Lethal Company". It's written with the help of BepInExPlugin, and it's posted on thunderstore community website.

It's purpose is to let players sell store-bought items for 50% of their original price, which by default can't be nor scanned nor sold at company

# Future plans
Make mod compatible with mods that add items to store

Make items not count as scrap, so you can't get EXP from them

Make sell percentage configurable (default: 50%)

# Bugs

RadarBooster contains 2 parts, each of them have their own price. But there should be only one price.
So you bought radarBooster for 60$. You activated it. Now it consists of:
{
  name: "Lueui",
  price: "30$"
},
{
  name: "RadarBoosterItem",
  price: "60$"
}
But it should be:
{
  name: "RadarBoosterItem",
  price: "30$"
}
And nothing else.
