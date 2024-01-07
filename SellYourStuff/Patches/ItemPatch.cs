using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace SellYourStuff.Patches
{
    // ensure that nothing is null, when you try to access it 

    /*
     * Description:
     * Mod allows to sell store-bought items (flashlights, tzp, zap gun, shovels, etc.
     * Items can be sold for 50% of their bought price
     * Items can be scanned by RMB
     * Items are not counted as scrap, and you can't gain EXP for them (WIP)
     * Compatible with ShipoverallPrice
     * Mod is compatible with custom store-items (only grabbable items) (WIP)
     */

    /* TODO:
     * 
     *              Version 1.0.0
     * multiply creditsWorth by current discount to get price (Terminal -> itemSalesPercentages)
     * test what will happen in the end stats, if amount of scrap is higher than overall amount
     * RadarBooster has wrong price, and it has price on its' separate parts. when it is activated - it gets name and price (funny, you can sell Louie :D)
     *    
     *              Version 1.1.0
     *              
     * don't count as scrap, until placed in company to sell
     * test grabbing item after placing in store, and trying to place it on ship (will it register scrap?)
     * test grabbing item in store, and placing it back
     * 
     *              Version 1.2.0
     *          
     * Check items if they're in store, and if they have credit worth. Instead of going through PatchableItems List<String> ?
     * test if decor has any price when scanning
     * test if suits have price
     * test if modded items have price and can be sold (and they shouldn't be counted as scrap on ship)
     * test if default items have price and can be sold (and they shouldn't be counted as scrap on ship)
     * 
     * Possibly:
     * allow user to change price of items (so I can sell bought items for more or less than 50%)
     * allow to change if items should be counted when scanning ship
    */
    /*[HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabPatch
    {
        private static List<string> PatchableItems = new List<string> { "FlashlightItem",
            "PatcherTool", "Shovel", "WalkieTalkie","BoomboxItem",
            "StunGrenadeItem","JetpackItem","ShotgunItem","LockPicker","ExtensionLadderItem",
        "RadarBoosterItem","SprayPaintItem","TetraChemicalItem"};

        [HarmonyPatch("GrabItem")]
        static void Postfix(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && PatchableItems.Contains(__instance.GetType().Name))
            {
                __instance.itemProperties.isScrap = false;

            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }
    }

    [HarmonyPatch(typeof(DepositItemsDesk))]
    internal class SellPatch
    {
        private static List<string> PatchableItems = new List<string> { "FlashlightItem",
        "PatcherTool", "Shovel", "WalkieTalkie","BoomboxItem",
        "StunGrenadeItem","JetpackItem","ShotgunItem","LockPicker","ExtensionLadderItem",
    "RadarBoosterItem","SprayPaintItem","TetraChemicalItem"};

        [HarmonyPatch("PlaceItemOnCounter")]
        static void Prefix(DepositItemsDesk __instance, [HarmonyArgument(0)] PlayerControllerB playerWhoTriggered)
        {
            // I need to put this thing inside "if" in this method, and also, before calling other methods.
            if (playerWhoTriggered != null)
            {
                GrabbableObject item = playerWhoTriggered.currentlyHeldObjectServer;

                if (item != null && item.itemProperties != null && PatchableItems.Contains(item.GetType().Name))
                {
                    item.itemProperties.isScrap = true;
                }
            }

        }
    }*/


    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemPatch
    {
        private static List<string> PatchableItems = new List<string> { "FlashlightItem",
            "PatcherTool", "Shovel", "WalkieTalkie","BoomboxItem",
            "StunGrenadeItem","JetpackItem","ShotgunItem","LockPicker","ExtensionLadderItem",
        "RadarBoosterItem","SprayPaintItem","TetraChemicalItem"};

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Postfix(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && PatchableItems.Contains(__instance.GetType().Name))
            {
                GameObject ScanNode;

                ScanNode = ((Component)UnityEngine.Object.FindObjectOfType<ScanNodeProperties>()).gameObject;

                GameObject val = UnityEngine.Object.Instantiate<GameObject>(ScanNode, ((Component)__instance).transform.position, Quaternion.Euler(Vector3.zero), ((Component)__instance).transform);

                ScanNodeProperties scanNodeProperties = val.GetComponent<ScanNodeProperties>();

                if (scanNodeProperties != null)
                {
                    scanNodeProperties.headerText = __instance.GetType().Name;
                    scanNodeProperties.nodeType = 2;
                    scanNodeProperties.minRange = 3;
                    scanNodeProperties.maxRange = 10;
                    scanNodeProperties.requiresLineOfSight = true;
                    scanNodeProperties.creatureScanID = -1;
                }
                else
                {
                    Debug.LogError($"Couldn't add scanNodeProperties to instance of object named: {__instance.GetType().Name}");
                }

                __instance.itemProperties.isScrap = true;

                __instance.SetScrapValue(__instance.itemProperties.creditsWorth/2);
            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }

        

    }
}
