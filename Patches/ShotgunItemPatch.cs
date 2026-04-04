using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {
        /*
        [HarmonyPostfix, HarmonyPatch("SetSafetyControlTip")]
        private static void SetSafetyControlTip_Postfix(ShotgunItem __instance, ref bool ___safetyOn)
        {
            string text;
            if (___safetyOn)
            {
                text = "안전 모드 해제하기: [Q]";
            }
            else
            {
                text = "안전 모드 설정하기: [Q]";
            }
            if (__instance.IsOwner)
            {
                HUDManager.Instance.ChangeControlTip(3, text, false);
            }
        }
        */
    }
}
