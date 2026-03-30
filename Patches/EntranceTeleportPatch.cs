using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(EntranceTeleport))]
    internal class EntranceTeleportPatch
    {
        /*
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(EntranceTeleport __instance)
        {
            InteractTrigger ic = __instance.gameObject.GetComponent<InteractTrigger>();
            ic.hoverTip = ic.hoverTip.Replace("[Near activity detected!]", "[근처에서 활동이 감지되었습니다!]");
            ic.hoverTip = ic.hoverTip.Replace("Enter :", "들어가기 :");
        }
        */
    }
}
