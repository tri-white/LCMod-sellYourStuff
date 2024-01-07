
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SellYourStuff.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellYourStuff
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class SellYourStuffModBase : BaseUnityPlugin
    {
        private const string modGUID = "Axeron.SellYourStuff";
        private const string modName = "SellYourStuff";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal ManualLogSource mls;

        private static SellYourStuffModBase Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo($"SellYourStuff plugin loaded. Version {modVersion}");

            harmony.PatchAll(typeof(SellYourStuffModBase));
            harmony.PatchAll(typeof(ItemPatch));
        }

    }
}
