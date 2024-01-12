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

    [HarmonyPatch(typeof(DepositItemsDesk))]
    internal class SellPatch
    {
        private static List<string> PatchableItems = new List<string> { "Flashlight","Extension ladder","Lockpicker","Jetpack",
            "Pro-flashlight","TZP-Inhalant","Stun grenade", "Boombox","Spray paint",
            "Shovel","Walkie-talkie","Zap gun","Radar-booster"};

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
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemPatch
    {
        // list of names of patchable items. I don't use classes, as it will cause conflicts (ex. Shovel class has instances of: Yield sign, Stop sign, Shovel, which causes slight problems)
        // but when I patch GrabbableObject as a whole class, then my method Postfix() will be called for each grabbableObject on map when they spawn.
            // I can write this method separately for all the items classes like FlashlightItem, RadarBoosterItem etc, but it will take memory and plugin will be several times bigger
            // maybe somehow put array of classes into [HarmonyPatch(typeof(...))]

        private static List<string> PatchableItems = new List<string> { "Flashlight","Extension ladder","Lockpicker","Jetpack",
            "Pro-flashlight","TZP-Inhalant","Stun grenade", "Boombox","Spray paint",
            "Shovel","Walkie-talkie","Zap gun","Radar-booster"};

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Postfix(GrabbableObject __instance)
        {
            if (__instance != null && __instance.itemProperties != null && PatchableItems.Contains(__instance.itemProperties.itemName))
            {
                // if object doesn't have a scanNode already then catch() will be done
                try 
                {
                    object node = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText; //checking if scanNode exists by existence of header text on scanNode

                    Debug.Log($"Instance of {__instance.GetType().Name} has scanNode already.");
                }
                catch
                {
                    Debug.Log($"Instance of {__instance.GetType().Name} doesnt have scanNode. Creating node");
                    GameObject ScanNode;

                    ScanNode = ((Component)UnityEngine.Object.FindObjectOfType<ScanNodeProperties>()).gameObject;
                    if (ScanNode != null)
                    {
                        GameObject val = UnityEngine.Object.Instantiate<GameObject>(ScanNode, ((Component)__instance).transform.position, Quaternion.Euler(Vector3.zero), ((Component)__instance).transform);

                        ScanNodeProperties scanNodeProperties = val.GetComponent<ScanNodeProperties>();

                        if (scanNodeProperties != null)
                        {
                            scanNodeProperties.headerText = __instance.itemProperties.itemName;
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
                    }
                    else
                    {
                        Debug.LogError($"Couldn't create scanNode for object");
                    }

                }

                __instance.itemProperties.isScrap = true;

                try
                {
                    for (int k = 0; k < UnityEngine.Object.FindObjectOfType<Terminal>().buyableItemsList.Length; k++)
                    {

                        if (UnityEngine.Object.FindObjectOfType<Terminal>().buyableItemsList[k].itemName == __instance.itemProperties.itemName)
                        {
                            Debug.Log($"Found item {UnityEngine.Object.FindObjectOfType<Terminal>().buyableItemsList[k].itemName} with sales {UnityEngine.Object.FindObjectOfType<Terminal>().itemSalesPercentages[k]}");

                            __instance.SetScrapValue(
                                        (__instance.itemProperties.creditsWorth * UnityEngine.Object.FindObjectOfType<Terminal>().itemSalesPercentages[k]) / 200
                                );

                            Debug.Log($"Set item value to: {(__instance.itemProperties.creditsWorth * UnityEngine.Object.FindObjectOfType<Terminal>().itemSalesPercentages[k]) / 200}");

                        }
                    }
                }
                catch
                {
                    __instance.SetScrapValue(0);
                    Debug.Log("Item not found in the current terminal store or other error occured. For safety purposes, its' price is set to 0");
                }

                __instance.itemProperties.isScrap = false ;


            }
            else
            {
                Debug.LogError($"One of the required objects (__instance, __instance.itemProperties) is null");
            }
        }

        

    }

}
