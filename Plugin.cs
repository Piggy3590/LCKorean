using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using LCKorean.Patches;
using UnityEngine;
using System.IO;
using BepInEx.Configuration;

namespace LCKorean
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Piggy.LCKorean";
        private const string modName = "LCKorean";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource mls;
        public static AssetBundle Bundle;

        public static bool translatePlanet;

        public static string PluginDirectory;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Plugin.PluginDirectory = base.Info.Location;

            //LoadAssets();

            translatePlanet = (bool)base.Config.Bind<bool>("번역", "행성 내부 이름 번역", false, "코드에서 사용되는 행성의 내부 이름을 한글화합니다. 게임 플레이에서 달라지는 부분은 없지만, true로 하면 모드 인테리어의 구성을 변경할 때 행성 명을 한글로 입력해야 합니다. false로 두면 그대로 영어로 입력하시면 됩니다.").Value;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LC Korean is loaded");

            harmony.PatchAll(typeof(Plugin));

            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(TerminalPatch));
            harmony.PatchAll(typeof(PreInitSceneScriptPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));
            harmony.PatchAll(typeof(SaveFileUISlotPatch));
            harmony.PatchAll(typeof(InteractTriggerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(StartMatchLeverPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(GrabbableObjectPatch));

            harmony.PatchAll(typeof(TMP_DropdownPatch));
            harmony.PatchAll(typeof(TextMeshProUGUIPatch));
        }
        private void LoadAssets()
        {
            try
            {
                Plugin.Bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Plugin.PluginDirectory), "radarzoom"));
            }
            catch (Exception ex)
            {
                Plugin.mls.LogError("Couldn't load asset bundle: " + ex.Message);
                return;
            }
            try
            {
                //Plugin.ButtonPrefab = Plugin.Bundle.LoadAsset<GameObject>("CameraZoomSwitchButton.prefab");
                base.Logger.LogInfo("Successfully loaded assets!");
            }
            catch (Exception ex2)
            {
                base.Logger.LogError("Couldn't load assets: " + ex2.Message);
            }
        }
    }
}
