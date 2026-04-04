using HarmonyLib;
using TMPro;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(ManualCameraRenderer))]
    internal class ManualCameraRendererPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(UnlockableSuit __instance)
        {
            TextMeshProUGUI t = StartOfRound.Instance.mapScreenPlayerName;
            t.text = TranslationManager.ReplaceArrayText(t.text, "Ingame", "MONITORING");
        }
    }
}
