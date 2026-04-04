using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(ShipBuildModeManager))]
    internal class ShipBuildModeManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateGhostObjectAndHighlight")]
        private static void CreateGhostObjectAndHighlight_Postfix()
        {
            string modifiedText = HUDManager.Instance.buildModeControlTip.text.Replace("Confirm", "배치");
            modifiedText = modifiedText.Replace("Rotate", "회전");
            modifiedText = modifiedText.Replace("Store", "보관");
            HUDManager.Instance.buildModeControlTip.text = modifiedText;
        }
    }
}
