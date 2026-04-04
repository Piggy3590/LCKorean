using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        public static string cursorTip;

        [HarmonyPostfix]
        [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
        private static void SetHoverTipAndCurrentInteractTrigger_Postfix(ref TextMeshProUGUI ___cursorTip)
        {
            string originalText = ___cursorTip.text;
            if (originalText != cursorTip)
            {
                cursorTip = TranslationManager.ReplaceArrayTextAll( originalText, "CursorTip");
                 ___cursorTip.text = cursorTip;
            }

            /*
            ___cursorTip.text = ___cursorTip.text.Replace("Inventory full!", "인벤토리 가득 참!");
            ___cursorTip.text = ___cursorTip.text.Replace("(Cannot hold until ship has landed)", "(함선이 착륙하기 전까지 집을 수 없음)");
            ___cursorTip.text = ___cursorTip.text.Replace("[Hands full]", "[양 손 사용 중]");
            ___cursorTip.text = ___cursorTip.text.Replace("Grab", "줍기");
            ___cursorTip.text = ___cursorTip.text.Replace("Locked", "잠김");

            ___cursorTip.text = ___cursorTip.text.Replace("Picking lock", "자물쇠 따는 중");
            ___cursorTip.text = ___cursorTip.text.Replace(" sec.", "초 남음.");

            ___cursorTip.text = ___cursorTip.text.Replace("Use door", "문 사용하기");
            */
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpawnDeadBody")]
        private static void SpawnDeadBody_Postfix()
        {
            DeadBodyInfo[] deadBodyInfo = GameObject.FindObjectsOfType<DeadBodyInfo>();
            foreach (DeadBodyInfo info in deadBodyInfo)
            {
                ScanNodeProperties componentInChildren = info.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (!componentInChildren.headerText.Contains(TranslationManager.GetArrayTranslation("DeathReasons", "PlayerBody")))
                {
                    componentInChildren.headerText = componentInChildren.headerText.Replace("Body of ", "");
                    componentInChildren.headerText += TranslationManager.GetArrayTranslation("DeathReasons", "PlayerBody");
                }
                string[] parts = componentInChildren.subText.Split(':');
                string left  = parts[0].Trim();
                string right = parts[1].Trim();

                componentInChildren.subText = TranslationManager.ReplaceArrayText(componentInChildren.subText, "DeathReasons", left);
                componentInChildren.subText = TranslationManager.ReplaceArrayText(componentInChildren.subText, "DeathReasons", right);
            }
        }
    }
}
