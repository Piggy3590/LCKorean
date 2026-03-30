using HarmonyLib;
using TMPro;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdateProfitQuotaCurrentTime")]
        private static void UpdateProfitQuotaCurrentTime_Postfix(ref int ___daysUntilDeadline, ref float ___timeUntilDeadline, ref float ___totalTime,
            ref int ___hoursUntilDeadline, ref float ___lengthOfHours, ref int ___numberOfHours, ref int ___quotaFulfilled, ref int ___profitQuota)
        {
            if (StartOfRound.Instance.isChallengeFile)
            {
                StartOfRound.Instance.deadlineMonitorText.text =
                    TranslationManager.GetArrayTranslation("Ingame", "CH_DeadlineMonitor") + "\n" +
                    TranslationManager.GetArrayTranslation("Ingame", "CH_DeadlineMonitor", 1) + "\n" +
                    TranslationManager.GetArrayTranslation("Ingame", "CH_DeadlineMonitor", 2);
                //StartOfRound.Instance.deadlineMonitorText.text = "가능한  한\n많은  수익을\n얻으세요";
                //StartOfRound.Instance.profitQuotaMonitorText.text = "환영합니다!\n이곳은:\n" + GameNetworkManager.Instance.GetNameForWeekNumber();
                
                StartOfRound.Instance.deadlineMonitorText.text =
                    TranslationManager.GetArrayTranslation("Ingame", "CH_QuotaMonitor") + "\n" +
                    TranslationManager.GetArrayTranslation("Ingame", "CH_QuotaMonitor", 1) + "\n" +
                    GameNetworkManager.Instance.GetNameForWeekNumber();
            }else
            {
                StartOfRound.Instance.deadlineMonitorText.text = TranslationManager.ReplaceArrayText(
                    StartOfRound.Instance.deadlineMonitorText.text, "Ingame", "Days");
                StartOfRound.Instance.deadlineMonitorText.text = TranslationManager.ReplaceArrayText(
                    StartOfRound.Instance.deadlineMonitorText.text, "Ingame", "DEADLINE");
                if (___timeUntilDeadline <= 0f)
                {
                    StartOfRound.Instance.deadlineMonitorText.text = TranslationManager.ReplaceArrayText(
                        StartOfRound.Instance.deadlineMonitorText.text, "Ingame", "Now");
                }
                else
                {
                    StartOfRound.Instance.deadlineMonitorText.text = TranslationManager.ReplaceArrayText(
                        StartOfRound.Instance.deadlineMonitorText.text, "Ingame", "CL_DeadlineMonitor");
                }
                StartOfRound.Instance.profitQuotaMonitorText.text = $"{TranslationManager.GetArrayTranslation("Ingame",
                    "QuotaMonitor")}:\n${___quotaFulfilled} / ${___profitQuota}";
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix()
        {
            TextMeshProUGUI t = HUDManager.Instance.clockNumber;
            t.text = TranslationManager.ReplaceArrayText(
                t.text, "Ingame", "AM");
            t.text = TranslationManager.ReplaceArrayText(
                t.text, "Ingame", "PM");
        }
        [HarmonyPostfix]
        [HarmonyPatch("VoteShipToLeaveEarly")]
        private static void VoteShipToLeaveEarly_Postfix(ref DialogueSegment[] ___shipLeavingEarlyDialogue)
        {
            ___shipLeavingEarlyDialogue[0].bodyText = TranslationManager.ReplaceArrayText(
                ___shipLeavingEarlyDialogue[0].bodyText, "Ingame", "AM");
            ___shipLeavingEarlyDialogue[0].bodyText = TranslationManager.ReplaceArrayText(
                ___shipLeavingEarlyDialogue[0].bodyText, "Ingame", "AM");
        }
    }
}
