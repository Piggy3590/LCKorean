using HarmonyLib;
using TMPro;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    internal class QuickMenuManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void Update_Prefix(ref TextMeshProUGUI ___settingsBackButton)
        {
            if (___settingsBackButton != null)
            {
                ___settingsBackButton.text = ___settingsBackButton.text.Replace("Discard changes", "변경 사항 취소");
                ___settingsBackButton.text = ___settingsBackButton.text.Replace("Back", "뒤로");
            }
        }
    }
}
