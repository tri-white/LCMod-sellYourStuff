using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellYourStuff.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class ItemPatch
    {
        // nameof(PlayerControllerB.Update - if it's not private, this way is better. Otherwise:
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infiniteSprintPatch(ref float __sprintMeter)
        {
            __sprintMeter = 1f;
            GameNetworkManager.Instance.localPlayerController;
        }
    }
}
