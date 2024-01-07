using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SellYourStuff.Patches
{
    // rewrite mod, so it works for any future new items. If item is picked up and it's not scrap - then use my mod.
    // or maybe just add Item classes to List, and check if item is in list - then use my mod

    // ensure that nothing is null, when you try to access it 

    /* TODO:
     * show scanNode
     * don't count as scrap, until placed in company to sell
     * reset isScrap to false when grabbed item
     * bind patch to all items, not just flashlight
    */
    [HarmonyPatch(typeof(FlashlightItem))]
    internal class ItemPatch
    {
        private static List<string> AllItemsList = new List<string> { "FlashlightItem", "Shovel", "BoomboxItem", "WalkieTalkie" };

        /*[HarmonyTargetMethod]
        static MethodBase TargetMethod(Type targetType)
        {
            if (AllItemsList.Contains(targetType.Name) && targetType.GetMethod("Start") != null && targetType.GetMethod("GrabItem") != null && targetType.GetMethod("SellItem") != null)
            {
                return AccessTools.Method(targetType, "Start");
            }

            return null; 
        }*/

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void IsSpawned(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && AllItemsList.Contains(__instance.GetType().Name))
            {
                __instance.gameObject.AddComponent<ScanNodeProperties>();
                ScanNodeProperties scanNodeProperties = __instance.gameObject.GetComponent<ScanNodeProperties>();
                if (scanNodeProperties != null)
                {
                    scanNodeProperties.headerText = __instance.GetType().Name;
                    scanNodeProperties.minRange = 0;
                    scanNodeProperties.maxRange = 9;
                    scanNodeProperties.requiresLineOfSight = true;
                    scanNodeProperties.creatureScanID = -1;
                    scanNodeProperties.nodeType = 0;
                }
                else
                {
                    Debug.LogError($"Couldn't add scanNodeProperties to instance of object named: {__instance.GetType().Name}");
                }

                __instance.SetScrapValue(__instance.itemProperties.creditsWorth/2);
            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }

       /* [HarmonyPatch("S")]
        [HarmonyPostfix]
        static void IsSelling(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && AllItemsList.Contains(__instance.GetType().Name))
            {

                __instance.itemProperties.isScrap = true;
            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }*/


       /*
        [HarmonyPatch("GrabItem")]
        [HarmonyPostfix]
        static void NotScrap(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && AllItemsList.Contains(__instance.GetType().Name))
            {
                __instance.itemProperties.isScrap = false;

            }
            else
            {
                Debug.LogError("One of the required objects (__instance, __instance.itemProperties) is null.");
            }
        }*/

    }
}
