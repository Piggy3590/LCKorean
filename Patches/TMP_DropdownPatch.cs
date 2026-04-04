using HarmonyLib;
using TMPro;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(TMP_Dropdown))]
    internal class TMP_DropdownPatch
    {
        
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void Awake_Postfix(TMP_Dropdown __instance)
        {
            if (__instance.options[0].text == "Sort: near")
            {
                __instance.options[0].text = "정렬: 가까운 서버";
                __instance.options[1].text = "정렬: 먼 서버";
                __instance.options[2].text = "정렬: 전 세계";
            }else if (__instance.options[0].text == "Use monitor (V-Sync)")
            {
                __instance.options[0].text = "모니터 사용 (수직 동기화)";
                __instance.options[1].text = "제한 없음";
            }else if (__instance.options[0].text == "Fullscreen")
            {
                __instance.options[0].text = "전체 화면";
                __instance.options[1].text = "테두리 없는 창";
                __instance.options[2].text = "최대화된 창";
                __instance.options[3].text = "창 모드";
            }else if (__instance.options[0].text == "Inside")
            {
                __instance.options[0].text = "내부 적";
                __instance.options[1].text = "외부 적";
                __instance.options[2].text = "주간 적";
            }else if (__instance.options[0].text == "Moderate")
            {
                __instance.options[0].text = "중간";
                __instance.options[1].text = "약함";
                __instance.options[2].text = "꺼짐";
            }else if (__instance.options[0].text == "Ultra")
            {
                __instance.options[0].text = "매우 높음";
                __instance.options[1].text = "높음 (기본값)";
                __instance.options[2].text = "중간";
                __instance.options[3].text = "낮음";
            }else if (__instance.options[1].text == "Performance")
            {
                __instance.options[0].text = "기본값";
                __instance.options[1].text = "성능 우선";
                __instance.options[2].text = "고성능";
                __instance.options[3].text = "레트로";
            }
        }
    }
}
