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

    /* TODO:
     * 
     *              Version 1.0.0
     * multiply creditsWorth by current discount to get price (Terminal -> itemSalesPercentages)
     * RadarBooster gets 2 scanNodes, first radar booster doesnt, but the next ones - yes
     * test all items in v49
     * 
     * Possibly:
     * allow user to change price of items (so I can sell bought items for more or less than 50%)
     * add modded items
    */

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
    }
    //test if loot doesnt become NOTscrap
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemPatch
    {
        private static List<string> PatchableItems = new List<string> { "FlashlightItem",
            "PatcherTool", "Shovel", "WalkieTalkie","BoomboxItem",
            "StunGrenadeItem","JetpackItem","ShotgunItem","LockPicker","ExtensionLadderItem",//"RadarBoosterItem",
            "SprayPaintItem","TetraChemicalItem"};

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
                    scanNodeProperties.minRange = 3; // need to set it to the value which scrap has. (value 2 - can scan in your own hands when looking up) (value 3 seems fine, but scan disappears when coming close)
                    scanNodeProperties.maxRange = 7;
                    scanNodeProperties.requiresLineOfSight = true;
                    scanNodeProperties.creatureScanID = -1;
                }
                else
                {
                    Debug.LogError($"Couldn't add scanNodeProperties to instance of object named: {__instance.GetType().Name}");
                }

                __instance.itemProperties.isScrap = true;

                __instance.SetScrapValue(__instance.itemProperties.creditsWorth/2); // need to also multiply it by current discount in Terminal class
                                                                                    // what if I buy item on discounts, then change moon to get this item for full price?

                __instance.itemProperties.isScrap = false ;


            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }

        

    }

    // only activated radar can be sold. otherwise it gets 2 scanNodes and everything is broken possibly
    
    [HarmonyPatch(typeof(RadarBoosterItem))]
    internal class RadarBoosterPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Postfix(RadarBoosterItem __instance)
        {
            if (__instance != null && __instance.itemProperties != null)
            {
                __instance.itemProperties.isScrap = true;

                __instance.SetScrapValue(__instance.itemProperties.creditsWorth / 2); // need to also multiply it by current discount in Terminal class

                __instance.itemProperties.isScrap = false ;

            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }



    }

}
