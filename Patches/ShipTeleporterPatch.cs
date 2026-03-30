using HarmonyLib;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    internal class ShipTeleporterPatch
    {
        /*
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(ref InteractTrigger ___buttonTrigger)
        {
            ___buttonTrigger.disabledHoverTip = ___buttonTrigger.disabledHoverTip.Replace("Cooldown", "재사용 대기 중");
            ___buttonTrigger.disabledHoverTip = ___buttonTrigger.disabledHoverTip.Replace(" sec.", "초 남음.");
        }
        */
    }
}
