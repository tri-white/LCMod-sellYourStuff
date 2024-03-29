﻿using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace SellYourStuff.Patches
{


    /// <summary>
    /// This class contains list of names of items that will be patched by this plugin.
    /// </summary>
    internal class PatchableItemsList
    {
        protected static readonly List<string> PatchableItems = new List<string> { "Flashlight","Extension ladder","Lockpicker","Jetpack",
            "Pro-flashlight","TZP-Inhalant","Stun grenade", "Boombox","Spray paint",
            "Shovel","Walkie-talkie","Zap gun","Radar-booster"};
    }

    /// <summary>
    /// This class makes items sellable when placed on the counter in the company building. Before being patched by this class, all store-bought items are set to not be considered as a scrap
    /// </summary>
    [HarmonyPatch(typeof(DepositItemsDesk))]
    internal class SellPatch : PatchableItemsList
    {
        [HarmonyPatch("PlaceItemOnCounter")]
        static void Prefix(DepositItemsDesk __instance, [HarmonyArgument(0)] PlayerControllerB playerWhoTriggered)
        {
            if (playerWhoTriggered != null)
            {
                GrabbableObject item = playerWhoTriggered.currentlyHeldObjectServer;

                if (item != null && item.itemProperties != null && PatchableItems.Contains(item.itemProperties.itemName))
                {
                    item.itemProperties.isScrap = true;
                }
            }
        }
    }

    /// <summary>
    /// Class that applies scanNodes for items and sets their prices.
    /// </summary>
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemPatch : PatchableItemsList
    {
        private const int NodeType = 2;
        private const int MinRange = 3;
        private const int MaxRange = 12;
        private const int creatureScanId = -1;
        private const bool requiresLineOfSight = false;

        // Function that does all stuff. Applies scanNode to item, sets its value
        [HarmonyPatch("Start")]
        static void Postfix(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && PatchableItems.Contains(__instance.itemProperties.itemName))
            {
                TryAddScanNode(__instance);

                __instance.itemProperties.isScrap = true;

                TrySetScrapValue(__instance);

                __instance.itemProperties.isScrap = false;
            }
        }

        // Function that sets scrap value for item, considering it's store price with discounts, and applies also fee (50% by default) on its price
        private static void TrySetScrapValue(GrabbableObject __instance)
        {
            try
            {
                Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                for (int k = 0; k < UnityEngine.Object.FindObjectOfType<Terminal>().buyableItemsList.Length; k++)
                {

                    if (terminal.buyableItemsList[k].itemName == __instance.itemProperties.itemName)
                    {
                        int percent = terminal.itemSalesPercentages[k];

                        __instance.SetScrapValue(
                                    (__instance.itemProperties.creditsWorth * percent) / 200
                            ); // formula is written that way to avoid float/double values
                    }
                }
            }
            catch
            {
                Debug.LogError("Item not found in the current terminal store or other error occured");
            }

        }

        // Function that adds scanNode to item if it doesn't have one yet.
        private static void TryAddScanNode(GrabbableObject __instance)
        {
            try
            {
                object node = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText;

            }
            catch
            {
                GameObject ScanNode;

                ScanNode = ((Component)UnityEngine.Object.FindObjectOfType<ScanNodeProperties>()).gameObject;
                if (ScanNode != null)
                {
                    GameObject val = UnityEngine.Object.Instantiate<GameObject>(ScanNode, ((Component)__instance).transform.position, Quaternion.Euler(Vector3.zero), ((Component)__instance).transform);

                    ScanNodeProperties scanNodeProperties = val.GetComponent<ScanNodeProperties>();

                    if (scanNodeProperties != null)
                    {
                        scanNodeProperties.headerText = __instance.itemProperties.itemName;
                        scanNodeProperties.nodeType = NodeType;
                        scanNodeProperties.minRange = MinRange;
                        scanNodeProperties.maxRange = MaxRange;
                        scanNodeProperties.requiresLineOfSight = requiresLineOfSight;
                        scanNodeProperties.creatureScanID = creatureScanId;
                    }
                }
                else
                {
                    Debug.LogError($"Couldn't create scanNode for object");
                }

            }
        }

    }
}
