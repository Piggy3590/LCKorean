using HarmonyLib;
using TMPro;
using UnityEngine;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        private static TextMeshProUGUI _loadingText;
        
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void Start_Prefix()
        {
            if (GameNetworkManager.Instance.gameVersionNum >= 80 && _loadingText == null)
            {
                _loadingText = GameObject.Find("LoadText").GetComponent<TextMeshProUGUI>();
                _loadingText.text = TranslationManager.GetArrayTranslation("HUD", "LoadText");
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        private static void GenerateNewLevelClientRpc_Postfix()
        {
            if (HUDManager.Instance.loadingText.text.Contains("Random seed"))
            {
                HUDManager.Instance.loadingText.text = HUDManager.Instance.loadingText.text.Replace("Random seed", "무작위 시드");
            }
        }
    }
}
