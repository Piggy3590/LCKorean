using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(PreInitSceneScript))]
    internal class PreInitSceneScriptPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("PressContinueButton")]
        private static void PressContinueButton_Postfix(ref int ___currentLaunchSettingPanel, ref GameObject[] ___LaunchSettingsPanels,
            ref Animator ___blackTransition, ref GameObject ___continueButton, ref TextMeshProUGUI ___headerText)
        {
            if (___headerText.text == "LAUNCH MODE")
            {
                ___headerText.text = "실행 모드";
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SkipToFinalSetting")]
        private static void SkipToFinalSetting_Postfix(ref TextMeshProUGUI ___headerText)
        {
            ___headerText.text = "실행 모드";
        }
    }
}
