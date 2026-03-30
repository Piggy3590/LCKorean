using HarmonyLib;
using TMPro;

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
            if (_loadingText == null)
            {
                _loadingText = HUDManager.Instance.LoadingScreen.transform.Find("LoadText").GetComponent<TextMeshProUGUI>();
            }
            _loadingText.text = TranslationManager.ReplaceArrayText(_loadingText.text, "HUD", "LoadText");
        }
        
        [HarmonyPrefix]
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        private static void GenerateNewLevelClientRpc_Prefix()
        {
            _loadingText.text = TranslationManager.ReplaceArrayText(_loadingText.text, "HUD", "LoadText");
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        private static void GenerateNewLevelClientRpc_Postfix()
        {
            _loadingText.text = TranslationManager.ReplaceArrayText(_loadingText.text, "HUD", "LoadText");
            if (HUDManager.Instance.loadingText.text.Contains("Random seed"))
            {
                TextMeshProUGUI t = HUDManager.Instance.loadingText;
                t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", "Random seed");
            }
        }
    }
}
