using HarmonyLib;
using TMPro;
using UnityEngine;

namespace LCKR.Patches
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
        private static void SkipToFinalSetting_Postfix(PreInitSceneScript __instance, ref TextMeshProUGUI ___headerText)
        {
            LoadingManager loadingManager = __instance.gameObject.AddComponent<LoadingManager>();
            loadingManager.text = ___headerText;
            loadingManager.buttons = GameObject.Find("LANOrOnline");
            ___headerText.text = "실행 모드";
        }
    }
}
