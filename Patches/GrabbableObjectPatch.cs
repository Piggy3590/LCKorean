using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
            /*
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(GrabbableObject __instance, ref Item ___itemProperties)
        {
            if (___itemProperties.itemName == "clipboard")
            {
                ___itemProperties.itemName = "클립보드";
                ___itemProperties.toolTips[0] = "자세히 보기: [Z]";
                ___itemProperties.toolTips[1] = "페이지 넘기기 : [Q/E]";
            }
            else if (___itemProperties.itemName == "Sticky note")
            {
                ___itemProperties.itemName = "스티커 메모";
                ___itemProperties.toolTips[0] = "자세히 보기: [Z]";
            }
        }
            */
    }
}
