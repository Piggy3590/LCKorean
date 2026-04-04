using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartMatchLeverPatch
    {
        /*
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void Update_Prefix(ref InteractTrigger ___triggerScript)
        {
            if (___triggerScript.hoverTip == "Land ship : [LMB]")
            {
                ___triggerScript.hoverTip = "함선 착륙하기 : [LMB]";
            }
            else if (___triggerScript.hoverTip == "Start game : [LMB]")
            {
                ___triggerScript.hoverTip = "게임 시작하기 : [LMB]";
            }
            else if (___triggerScript.hoverTip == "Start ship : [LMB]")
            {
                ___triggerScript.hoverTip = "함선 출발하기 : [LMB]";
            }

            if (___triggerScript.disabledHoverTip == "[Wait for ship to land]")
            {
                ___triggerScript.disabledHoverTip = "[함선이 완전히 이착륙할 때까지 기다리세요]";
            }
            if (___triggerScript.disabledHoverTip == "[Ship in motion]")
            {
                ___triggerScript.disabledHoverTip = "[함선이 이동하고 있습니다]";
            }
        }
        */
    }
}
