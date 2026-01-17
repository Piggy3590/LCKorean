using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        static readonly FieldInfo forceChangeTextCoroutineField = AccessTools.Field(typeof(HUDManager), "forceChangeTextCoroutine");
        static readonly MethodInfo forceChangeTextMethod = AccessTools.Method(typeof(HUDManager), "ForceChangeText");


        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(ref PlayerLevel[] ___playerLevels)
        {
            foreach (PlayerLevel level in ___playerLevels)
            {
                switch (level.levelName)
                {
                    case "Intern":
                        level.levelName = "인턴";
                        break;
                    case "Part-timer":
                        level.levelName = "아르바이트";
                        break;
                    case "Employee":
                        level.levelName = "사원";
                        break;
                    case "Leader":
                        level.levelName = "팀장";
                        break;
                    case "Boss":
                        level.levelName = "사장";
                        break;
                }
            }
        }

        static void TranslateAdvertText(HUDManager __instance)
        {
            if (!__instance.advertTopText.text.Contains("<size=35>"))
            {
                __instance.advertTopText.text = "<size=35>" + __instance.advertTopText.text;
            }
            if (!__instance.advertBottomText.text.Contains("<size=35>"))
            {
                __instance.advertBottomText.text = "<size=35>" + __instance.advertBottomText.text;
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("OFF", "세일");
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("AVAILABLE NOW", "지금 구매하세요");
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("CURES CANCER", "암도 치료합니다");
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("NO WAY", "엄청난 기능");
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("LIMITED TIME ONLY", "한정판");
                __instance.advertBottomText.text = __instance.advertBottomText.text.Replace("GET YOURS TODAY", "지금 바로 주문하세요");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(HUDManager __instance, ref TextMeshProUGUI ___weightCounter, ref Animator ___weightCounterAnimator,
            ref TextMeshProUGUI ___holdButtonToEndGameEarlyText)
        {
            if (GameNetworkManager.Instance.gameVersionNum >= 70)
            {
                TranslateAdvertText(__instance);
            }
            ___holdButtonToEndGameEarlyText.text = ___holdButtonToEndGameEarlyText.text.Replace("Tell autopilot ship to leave early", "함선에게 일찍 떠나라고 지시하기\n");
            ___holdButtonToEndGameEarlyText.text = ___holdButtonToEndGameEarlyText.text.Replace("Voted for ship to leave early", "함선 조기 출발에 투표했습니다");
            ___holdButtonToEndGameEarlyText.text = ___holdButtonToEndGameEarlyText.text.Replace("Hold", "길게 누르기");
            
            if (!GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                if (Plugin.toKG)
                {
                    float num2 = Mathf.RoundToInt(Mathf.Clamp(GameNetworkManager.Instance.localPlayerController.carryWeight - 1f, 0f, 100f) * 105f / 2.2f);
                    ___weightCounter.text = $"{num2}kg";
                    ___weightCounterAnimator.SetFloat("weight", num2 / 130f);
                }else
                {
                    float num2 = Mathf.RoundToInt(Mathf.Clamp(GameNetworkManager.Instance.localPlayerController.carryWeight - 1f, 0f, 100f) * 105f);
                    ___weightCounter.text = $"{num2} lb";
                    ___weightCounterAnimator.SetFloat("weight", num2 / 130f);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplaySpectatorTip")]
        private static void DisplaySpectatorTip_Postfix(ref TextMeshProUGUI ___spectatorTipText)
        {
            ___spectatorTipText.text = ___spectatorTipText.text.Replace("TIP!: Hold [R-Trigger] to vote for the autopilot ship to leave early.", "팁!: [R-Trigger]을 길게 눌러 함선 조기 출발에 투표할 수 있습니다.");
            ___spectatorTipText.text = ___spectatorTipText.text.Replace("TIP!: Hold [RMB] to vote for the autopilot ship to leave early.", "팁!: [RMB]을 길게 눌러 함선 조기 출발에 투표할 수 있습니다.");
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplayStatusEffect")]
        private static void DisplayStatusEffect_Postfix(string statusEffect, ref TextMeshProUGUI ___statusEffectText)
        {
            if (statusEffect == "VISIBILITY LOW!\n\nSteam leak detected in area")
            {
                ___statusEffectText.text = "가시성이 낮습니다!\n\n해당 구역에서\n증기 누출이 감지되었습니다";
            }else if (statusEffect == "Oxygen critically low!")
            {
                ___statusEffectText.text = "산소가 부족합니다!";
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetSpectatingTextToPlayer")]
        private static void SetSpectatingTextToPlayer_Postfix(ref TextMeshProUGUI ___spectatingPlayerText)
        {
            ___spectatingPlayerText.text = ___spectatingPlayerText.text.Replace("Spectating", "관전 중");
        }

        [HarmonyPostfix]
        [HarmonyPatch("ApplyPenalty")]
        private static void ApplyPenalty_Postfix(ref EndOfGameStatUIElements ___statsUIElements)
        {
            ___statsUIElements.penaltyAddition.text = ___statsUIElements.penaltyAddition.text.Replace(" casualties", "명의 사상자 발생");
            ___statsUIElements.penaltyAddition.text = ___statsUIElements.penaltyAddition.text.Replace(" bodies recovered", "구의 시체 회수됨");
        }

        [HarmonyPrefix]
        [HarmonyPatch("ReadDialogue")]
        private static void ReadDialogue_Prefix(DialogueSegment[] dialogueArray)
        {
            foreach (DialogueSegment dialogue in dialogueArray)
            {
                dialogue.speakerText = TranslationManager.GetStringTranslation("Dialogue", dialogue.speakerText);

                if (dialogue.bodyText == "AM. A vote has been cast")
                {
                    dialogue.bodyText = TranslationManager.GetStringTranslation("Dialogue", "ReturnAM")
                        .Replace("{time}", HUDManager.Instance.SetClock(TimeOfDay.Instance.shipLeaveAutomaticallyTime, (float)TimeOfDay.Instance.numberOfHours, false));
                }
                else if (dialogue.bodyText == "PM. A vote has been cast")
                {
                    dialogue.bodyText = TranslationManager.GetStringTranslation("Dialogue", "ReturnPM")
                        .Replace("{time}", HUDManager.Instance.SetClock(TimeOfDay.Instance.shipLeaveAutomaticallyTime, (float)TimeOfDay.Instance.numberOfHours, false));
                }
                else
                {
                    dialogue.bodyText = TranslationManager.GetStringTranslation("Dialogue", dialogue.bodyText);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplayTip")]
        private static void DisplayTip_Postfix(TextMeshProUGUI ___tipsPanelHeader, TextMeshProUGUI ___tipsPanelBody)
        {
            switch (___tipsPanelHeader.text)
            {
                case "To read the manual:":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "ReadManual");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "ReadManual");
                    break;
                case "TIP:":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "TipTerminal");
                    if (___tipsPanelBody.text == "Use the ship computer terminal to access secure doors.")
                    {
                        ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "TipTerminal");
                    }else
                    {
                        ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "TipLockedDoor");
                    }
                    break;
                case "Welcome!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "Welcome");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "Welcome");
                    break;
                case "Got scrap!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "GotScrap");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "GotScrap");
                    break;
                case "Items missed!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "ItemMissed");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "ItemMissed");
                    break;
                case "Item stored!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "ItemStored");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "ItemStored");
                    break;
                case "HALT!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "DeadlineWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "DeadlineWarning");
                    break;
                case "Weather alert!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "EclipseWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "EclipseWarning");
                    break;
                case "???":
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "MissingEntrance");
                    break;
                case "ALERT!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "LightningWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "LightningWarning");
                    break;
                case "Welcome back!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "WelcomeBack");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "WelcomeBack");
                    break;
                case "Tip":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", 0, "TipShipObject");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", 1, "TipShipObject");
                    break;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("DisplayNewScrapFound")]
        private static void DisplayNewScrapFound_Postfix()
        {
            HUDManager.Instance.ScrapItemBoxes[0].headerText.text = HUDManager.Instance.ScrapItemBoxes[0].headerText.text.Replace("collected", "수집함");
            HUDManager.Instance.ScrapItemBoxes[0].valueText.text = HUDManager.Instance.ScrapItemBoxes[0].valueText.text.Replace("Value", "가격");

            HUDManager.Instance.ScrapItemBoxes[1].headerText.text = HUDManager.Instance.ScrapItemBoxes[1].headerText.text.Replace("collected", "수집함");
            HUDManager.Instance.ScrapItemBoxes[1].valueText.text = HUDManager.Instance.ScrapItemBoxes[1].valueText.text.Replace("Value", "가격");

            HUDManager.Instance.ScrapItemBoxes[2].headerText.text = HUDManager.Instance.ScrapItemBoxes[2].headerText.text.Replace("collected", "수집함");
            HUDManager.Instance.ScrapItemBoxes[2].valueText.text = HUDManager.Instance.ScrapItemBoxes[2].valueText.text.Replace("Value", "가격");
        }
        [HarmonyPostfix, HarmonyPatch("DisplayCreditsEarning")]
        private static void DisplayCreditsEarning_Postfix(ref TextMeshProUGUI ___moneyRewardsTotalText)
        {
            ___moneyRewardsTotalText.text = ___moneyRewardsTotalText.text.Replace("TOTAL", "합계");
        }

        [HarmonyPostfix, HarmonyPatch("DisplayDaysLeft")]
        private static void DisplayDaysLeft_Postfix(ref TextMeshProUGUI ___profitQuotaDaysLeftText, ref TextMeshProUGUI ___profitQuotaDaysLeftText2)
        {
            ___profitQuotaDaysLeftText.text = ___profitQuotaDaysLeftText.text.Replace(" Day Left", "일 남았습니다");
            ___profitQuotaDaysLeftText.text = ___profitQuotaDaysLeftText.text.Replace(" Days Left", "일 남았습니다");
            ___profitQuotaDaysLeftText2.text = ___profitQuotaDaysLeftText2.text.Replace(" Day Left", "일 남았습니다");
            ___profitQuotaDaysLeftText2.text = ___profitQuotaDaysLeftText2.text.Replace(" Days Left", "일 남았습니다");
        }

        [HarmonyPostfix, HarmonyPatch("ChangeControlTip")]
        private static void ChangeControlTip_Postfix(HUDManager __instance, ref TextMeshProUGUI[] ___controlTipLines, int toolTipNumber, string changeTo, bool clearAllOther = false)
        {
            string actionKey = changeTo.Split(':')[0].Trim();
            string translatedString = actionKey;

            translatedString = changeTo.Replace(
                actionKey,
                TranslationManager.GetStringTranslation("ControlTip", actionKey, true)
            );

            ___controlTipLines[toolTipNumber].text = translatedString;

            Plugin.mls.LogInfo($"key-{actionKey}, line-{___controlTipLines[toolTipNumber].text}");


            Coroutine coroutine = (Coroutine)forceChangeTextCoroutineField.GetValue(__instance);

            if (coroutine != null)
            {
                __instance.StopCoroutine(coroutine);
            }

            IEnumerator enumerator = (IEnumerator)forceChangeTextMethod.Invoke(
                __instance,
                new object[]
                {
            ___controlTipLines[toolTipNumber],
            translatedString
                });

            Coroutine newCoroutine = __instance.StartCoroutine(enumerator);

            forceChangeTextCoroutineField.SetValue(__instance, newCoroutine);
        }

        [HarmonyPostfix, HarmonyPatch("ChangeControlTipMultiple")]
        private static void ChangeControlTipMultiple_Postfix(ref TextMeshProUGUI[] ___controlTipLines, string[] allLines, bool holdingItem, Item itemProperties)
        {
            if (holdingItem && allLines != null)
            {
                if (itemProperties.itemName == "Maneater")
                {
                    ___controlTipLines[0].text = "맨이터 떨어뜨리기 : [G]";
                    ___controlTipLines[1].text = ___controlTipLines[1].text.Replace("Rock", "흔들기");
                    ___controlTipLines[1].text = ___controlTipLines[1].text.Replace("Hold", "길게 누르기");
                }
                else
                {
                    ___controlTipLines[0].text = TranslationManager.GetStringTranslation("Item", itemProperties.itemName) + " 떨어뜨리기 : [G]";

                    int maxLines = Mathf.Min(allLines.Length, ___controlTipLines.Length);

                    for (int i = 0; i < maxLines; i++)
                    {
                        if (allLines.Length > i && !string.IsNullOrEmpty(allLines[i]))
                        {
                            string originalLine = allLines[i];
                            string actionKey = originalLine.Split(':')[0].Trim();

                            Plugin.mls.LogInfo($"{i}, {actionKey}-");

                            ___controlTipLines[i + 1].text = ___controlTipLines[i + 1].text.Replace(
                                actionKey,
                                TranslationManager.GetStringTranslation("ControlTip", actionKey, true)
                            );
                        }
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void Update_Prefix(ref TextMeshProUGUI ___planetInfoHeaderText, ref TextMeshProUGUI ___globalNotificationText, ref TextMeshProUGUI ___loadingText, ref TextMeshProUGUI[] ___controlTipLines)
        {
            ___controlTipLines[0].text = ___controlTipLines[0].text.Replace("Gas pedal", "가속 페달");
            ___controlTipLines[1].text = ___controlTipLines[1].text.Replace("Brake pedal", "브레이크 페달");
            ___controlTipLines[2].text = ___controlTipLines[2].text.Replace("Boost", "부스터");
            if (___loadingText.text == "Waiting for crew...")
            {
                ___loadingText.text = "팀원 기다리는 중...";
            }

            if (___planetInfoHeaderText.text.Contains("CELESTIAL BODY:"))
            {
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("CELESTIAL BODY:", "천체:");

                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Experimentation", "익스페리멘테이션");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Where the Company resides", "회사가 소재하는 지역입니다");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Assurance", "어슈어런스");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Offense", "오펜스");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Adamance", "애더먼스");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Rend", "렌드");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Dine", "다인");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("March", "머치");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Vow", "보우");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Titan", "타이탄");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Artifice", "아터피스");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Embrion", "엠브리언");
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Gordion", "고르디온");

                if (Plugin.translateModdedContent)
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Asteroid", "아스테로이드");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Atlantica", "아틀란티카");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Cosmocos", "코스모코스");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Desolation", "디솔레이션");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Etern", "이턴");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Fission", "피션");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Gloom", "글룸");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Gratar", "그라타");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Infernis", "인퍼니스");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Junic", "주닉");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Oldred", "올드레드");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Polarus", "폴라러스");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Acidir", "어시디어");
                }
            }

            if (___globalNotificationText.text == "New creature data sent to terminal!")
            {
                ___globalNotificationText.text = "새로운 생명체 데이터가 터미널에 전송되었습니다!";
            }
            else if (___globalNotificationText.text.Contains("Found journal entry:"))
            {
                ___globalNotificationText.text = ___globalNotificationText.text.Replace("Found journal entry:", "일지를 찾았습니다:");
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch("AttemptScanNode")]
        private static void AttemptScanNode_Prefix(ScanNodeProperties node)
        {
            TranslateScanNode(node);
            if (Plugin.translateModdedContent)
            {
                TranslateModdedContent(node);
            }
        }

        static void TranslateScanNode(ScanNodeProperties node)
        {
            if (node == null)
                return;
            if (node.headerText == "Baboon hawk")
            {
                node.headerText = "개코 매";
            }
            else if (node.headerText == "Hygrodere")
            {
                node.headerText = "하이그로디어";
            }
            else if (node.headerText == "Mask Hornets")
            {
                node.headerText = "위장 말벌";
            }
            else if (node.headerText == "Butler")
            {
                node.headerText = "집사";
            }
            else if (node.headerText == "Snare flea")
            {
                node.headerText = "올무 벼룩";
            }
            else if (node.headerText.Contains("Thumper") || node.headerText.Contains("Halves"))
            {
                if (Plugin.thumperTranslation)
                {
                    node.headerText = "썸퍼";
                }
                else
                {
                    node.headerText = "덤퍼";
                }
            }
            else if (node.headerText == "Roaming locusts")
            {
                node.headerText = "배회 메뚜기";
            }
            else if (node.headerText == "Manticoil")
            {
                node.headerText = "만티코일";
            }
            else if (node.headerText == "Bracken")
            {
                node.headerText = "브래컨";
            }
            else if (node.headerText == "Forest Giant")
            {
                node.headerText = "숲 거인";
            }
            else if (node.headerText == "Hoarding bug")
            {
                node.headerText = "비축 벌레";
            }
            else if (node.headerText == "Jester")
            {
                node.headerText = "광대";
            }
            else if (node.headerText == "Lasso")
            {
                node.headerText = "올가미 인간";
            }
            else if (node.headerText == "Eyeless dog")
            {
                node.headerText = "눈없는 개";
            }
            else if (node.headerText == "Nutcracker")
            {
                node.headerText = "호두까기 인형";
            }
            else if (node.headerText == "Spore lizard")
            {
                node.headerText = "포자 도마뱀";
            }
            else if (node.headerText == "Old Bird")
            {
                node.headerText = "올드 버드";
            }
            else if (node.headerText == "Circuit bees")
            {
                node.headerText = "회로 벌";
            }
            else if (node.headerText == "Bunker spider")
            {
                node.headerText = "벙커 거미";
            }
            else if (node.headerText == "Earth Leviathan")
            {
                node.headerText = "육지 레비아탄";
            }
            else if (node.headerText == "Coil-head")
            {
                node.headerText = "코일 헤드";
            }
            else if (node.headerText == "Tulip Snake")
            {
                node.headerText = "튤립 뱀";
            }
            else if (node.headerText == "Barber")
            {
                node.headerText = "이발사";
            }
            else if (node.headerText == "Vain Shroud")
            {
                node.headerText = "은폐 수풀";
            }
            else if (node.headerText == "Kidnapper Fox")
            {
                node.headerText = "납치범 여우";
            }else if (node.headerText == "Maneater")
            {
                node.headerText = "맨이터";
            }else if (node.headerText == "Giant Sapsucker")
            {
                node.headerText = "거대 딱따구리";
            }


            else if (node.headerText == "Landmine")
            {
                node.headerText = "지뢰";
            }
            else if (node.headerText == "Turret")
            {
                node.headerText = "터렛";
            }


            else if (node.headerText == "Coil")
            {
                node.headerText = "코일";
                node.subText = "배터리를 충전합니다";
            }
            else if (node.headerText == "Terminal")
            {
                node.headerText = "터미널";
                node.subText = "손전등을 구매하고 행성을 여행하세요!";
            }
            else if (node.headerText == "Brake lever")
            {
                node.headerText = "브레이크 레버";
                node.subText = "함선을 착륙시킵니다.";
            }
            else if (node.headerText == "Training manual")
            {
                node.headerText = "교육용 지침서";
                node.subText = "유용한 정보들!";
            }
            else if (node.headerText == "Door controls")
            {
                node.headerText = "문 제어기";
                node.subText = "영원히 닫을 수는 없습니다";
            }
            else if (node.headerText == "Door control")
            {
                node.headerText = "문 제어기";
                node.subText = "영원히 닫을 수는 없습니다";
            }
            else if (node.headerText == "Quota")
            {
                node.headerText = "할당량";
                node.subText = "폐품을 현금으로 판매하세요.";
            }
            else if (node.headerText == "Ship")
            {
                node.headerText = "함선";
                node.subText = "당신의 기지";
            }
            else if (node.headerText == "Main entrance")
            {
                node.headerText = "정문";
            }
            else if (node.headerText == "Data chip")
            {
                node.headerText = "데이터 칩";
            }
            else if (node.headerText == "Key")
            {
                node.headerText = "열쇠";
                node.subText = "가격: $3";
            }
            else if (node.headerText == "Apparatice")
            {
                node.headerText = "장치";
                node.subText = "가격: ???";
            }

            if (node.subText == "(Radar booster)")
            {
                node.subText = "(레이더 부스터)";
            }
            ItemScanNode(node, "Magic 7 ball", "마법의 7번 공");
            ItemScanNode(node, "Airhorn", "에어혼");
            ItemScanNode(node, "Brass bell", "황동 종");
            ItemScanNode(node, "Big bolt", "큰 나사");
            ItemScanNode(node, "Bottles", "병 묶음");
            ItemScanNode(node, "Hair brush", "빗");
            ItemScanNode(node, "Candy", "사탕");
            ItemScanNode(node, "Cash register", "금전 등록기");
            ItemScanNode(node, "Chemical jug", "화학 용기");
            ItemScanNode(node, "Clown horn", "광대 나팔");
            ItemScanNode(node, "Large axle", "대형 축");
            ItemScanNode(node, "Teeth", "틀니");
            ItemScanNode(node, "Dust pan", "쓰레받기");
            ItemScanNode(node, "Egg beater", "달걀 거품기");
            ItemScanNode(node, "V-type engine", "V형 엔진");
            ItemScanNode(node, "Golden cup", "황금 컵");
            ItemScanNode(node, "Fancy lamp", "멋진 램프");
            ItemScanNode(node, "Painting", "그림");
            ItemScanNode(node, "Plastic fish", "플라스틱 물고기");
            ItemScanNode(node, "Laser pointer", "레이저 포인터");
            ItemScanNode(node, "Gold Bar", "금 주괴");
            ItemScanNode(node, "Hairdryer", "헤어 드라이기");
            ItemScanNode(node, "Magnifying glass", "돋보기");
            ItemScanNode(node, "Tattered metal sheet", "너덜너덜한 금속 판");
            ItemScanNode(node, "Cookie mold pan", "쿠키 틀");
            ItemScanNode(node, "Coffee mug", "커피 머그잔");
            ItemScanNode(node, "Perfume bottle", "향수 병");
            ItemScanNode(node, "Old phone", "구식 전화기");
            ItemScanNode(node, "Jar of pickles", "피클 병");
            ItemScanNode(node, "Pill bottle", "약 병");
            ItemScanNode(node, "Remote", "리모컨");
            ItemScanNode(node, "Wedding ring", "결혼 반지");
            ItemScanNode(node, "Robot Toy", "로봇 장난감");
            ItemScanNode(node, "Rubber ducky", "고무 오리");
            ItemScanNode(node, "Red soda", "빨간색 소다");
            ItemScanNode(node, "Steering wheel", "운전대");
            ItemScanNode(node, "Stop sign", "정지 표지판");
            ItemScanNode(node, "Tea Kettle", "찻주전자");
            ItemScanNode(node, "Toothpaste", "치약");
            ItemScanNode(node, "Toy cube", "장난감 큐브");
            ItemScanNode(node, "Bee hive", "벌집");
            ItemScanNode(node, "Yield sign", "양보 표지판");
            ItemScanNode(node, "Shotgun", "산탄총");
            ItemScanNode(node, "Double-barrel", "더블 배럴");
            ItemScanNode(node, "Shotgun shell", "산탄총 탄약");
            ItemScanNode(node, "Homemade Flashbang", "사제 섬광탄");
            ItemScanNode(node, "Gift", "선물");
            ItemScanNode(node, "Gift box", "선물 상자");
            ItemScanNode(node, "Flask", "플라스크");
            ItemScanNode(node, "Tragedy", "비극");
            ItemScanNode(node, "Comedy", "희극");
            ItemScanNode(node, "Whoopie cushion", "방퀴 쿠션");
            ItemScanNode(node, "Kitchen knife", "식칼");
            ItemScanNode(node, "Easter egg", "부활절 달걀");
            ItemScanNode(node, "Soccer ball", "축구공");
            ItemScanNode(node, "Control pad", "조작 패드");
            ItemScanNode(node, "Garbage lid", "쓰레기통 뚜껑");
            ItemScanNode(node, "Plastic cup", "플라스틱 컵");
            ItemScanNode(node, "Toilet paper", "화장실 휴지");
            ItemScanNode(node, "Toy train", "장난감 기차");
            ItemScanNode(node, "Clock", "시계");
            ItemScanNode(node, "Egg", "알");
        }

        static void TranslateModdedContent(ScanNodeProperties node)
        {
            if (node == null)
                return;
            //ImmersiveScraps
            ItemScanNode(node, "Alcohol Flask", "알코올 플라스크");
            ItemScanNode(node, "Anvil", "모루");
            ItemScanNode(node, "Baseball bat", "야구 방망이");
            ItemScanNode(node, "Beer can", "맥주 캔");
            ItemScanNode(node, "Brick", "벽돌");
            ItemScanNode(node, "Broken engine", "망가진 엔진");
            ItemScanNode(node, "Bucket", "양동이");
            ItemScanNode(node, "Can paint", "페인트 캔");
            ItemScanNode(node, "Canteen", "수통");
            ItemScanNode(node, "Car battery", "자동차 배터리");
            ItemScanNode(node, "Clamp", "조임틀");
            ItemScanNode(node, "Fancy Painting", "멋진 그림");
            ItemScanNode(node, "Fan", "선풍기");
            ItemScanNode(node, "Fireaxe", "소방 도끼");
            ItemScanNode(node, "Fire extinguisher", "소화기");
            ItemScanNode(node, "Fire hydrant", "소화전");
            ItemScanNode(node, "Food can", "통조림");
            ItemScanNode(node, "Gameboy", "게임보이");
            ItemScanNode(node, "Garbage", "쓰레기");
            ItemScanNode(node, "Hammer", "망치");
            ItemScanNode(node, "Jerrycan", "기름통");
            ItemScanNode(node, "Keyboard", "키보드");
            ItemScanNode(node, "Lantern", "랜턴");
            ItemScanNode(node, "Library lamp", "도서관 램프");
            ItemScanNode(node, "Plant", "식물");
            ItemScanNode(node, "Pliers", "플라이어");
            ItemScanNode(node, "Plunger", "뚫어뻥");
            ItemScanNode(node, "Retro Toy", "레트로 장난감");
            ItemScanNode(node, "Screwdriver", "스크류 드라이버");
            ItemScanNode(node, "Sink", "싱크대");
            ItemScanNode(node, "Socket Wrench", "소켓 렌치");
            ItemScanNode(node, "Squeaky toy", "고무 오리");
            ItemScanNode(node, "Suitcase", "여행 가방");
            ItemScanNode(node, "Toaster", "토스터기");
            ItemScanNode(node, "Toolbox", "공구 상자");
            ItemScanNode(node, "Top hat", "실크햇");
            ItemScanNode(node, "Traffic cone", "라바콘");
            ItemScanNode(node, "Vent", "환풍구");
            ItemScanNode(node, "Watering Can", "물뿌리개");
            ItemScanNode(node, "Wheel", "바퀴");
            ItemScanNode(node, "Wine bottle", "와인 병");
            ItemScanNode(node, "Wrench", "렌치");

            //Wesleys
            ItemScanNode(node, "Amethyst Cluster", "자수정 군집");
            ItemScanNode(node, "Syringe", "주사기");
            ItemScanNode(node, "Syringe Gun", "주사기총");
            ItemScanNode(node, "Corner Pipe", "코너 파이프");
            ItemScanNode(node, "Small Pipe", "작은 파이프");
            ItemScanNode(node, "Flow Pipe", "파이프");
            ItemScanNode(node, "Brain Jar", "뇌가 담긴 병");
            ItemScanNode(node, "Toy Nutcracker", "호두까기 인형 장난감");
            ItemScanNode(node, "Test Tube", "시험관");
            ItemScanNode(node, "Test Tube Rack", "시험관 랙");
            ItemScanNode(node, "Nutcracker Eye", "호두까기 인형 눈");
            ItemScanNode(node, "Blue Test Tube", "파란색 시험관");
            ItemScanNode(node, "Yellow Test Tube", "노란색 시험관");
            ItemScanNode(node, "Red Test Tube", "빨간색 시험관");
            ItemScanNode(node, "Green Test Tube", "초록색 시험관");
            ItemScanNode(node, "Crowbar", "쇠지렛대");
            ItemScanNode(node, "Plzen", "플젠");
            ItemScanNode(node, "Cup", "컵");
            ItemScanNode(node, "Microwave", "전자레인지");
            ItemScanNode(node, "Experiment Log Hyper Acid", "Hyper Acid 실험 기록");
            ItemScanNode(node, "Experiment Log Comedy Mask", "희극 가면 실험 기록");
            ItemScanNode(node, "Experiment Log Cursed Coin", "저주받은 동전 실험 기록");
            ItemScanNode(node, "Experiment Log BIO HXNV7", "바이오 HXNV7 실험 기록");
            ItemScanNode(node, "Blue Folder", "파란색 폴더");
            ItemScanNode(node, "Red Folder", "빨간색 폴더");
            ItemScanNode(node, "Fire Extinguisher", "소화기");
            ItemScanNode(node, "Coil", "코일");
            ItemScanNode(node, "Typewriter", "타자기");
            ItemScanNode(node, "Documents", "서류 더미");
            ItemScanNode(node, "Stapler", "스테이플러");
            ItemScanNode(node, "Old Computer", "구식 컴퓨터");
            ItemScanNode(node, "Bronze Trophy", "브론즈 트로피");
            ItemScanNode(node, "Banana", "바나나");
            ItemScanNode(node, "Stun Baton", "스턴봉");
            ItemScanNode(node, "BIO-HXNV7", "바이오-HXNV7");
            ItemScanNode(node, "Recovered Secret Log", "복구된 비밀 일지");
            ItemScanNode(node, "Experiment Log Golden Dagger", "황금 단검 실험 기록");
            ItemScanNode(node, "Clam", "대합");
            ItemScanNode(node, "Turtle Shell", "거북이 등딱지");
            ItemScanNode(node, "Fish Bones", "생선 뼈");
            ItemScanNode(node, "Horned Shell", "뿔 달린 껍질");
            ItemScanNode(node, "Porcelain Teacup", "도자기 찻잔");
            ItemScanNode(node, "Marble", "대리석");
            ItemScanNode(node, "Porcelain Bottle", "도자기 병");
            ItemScanNode(node, "Porcelain Perfume Bottle", "도자기 향수 병");
            ItemScanNode(node, "Glowing Orb", "발광구");
            ItemScanNode(node, "Golden Skull", "황금 해골");
            ItemScanNode(node, "Map of Cosmocos", "코스모코스 지도");
            ItemScanNode(node, "Wet Note 1", "젖은 노트 1");
            ItemScanNode(node, "Wet Note 2", "젖은 노트 2");
            ItemScanNode(node, "Wet Note 3", "젖은 노트 3");
            ItemScanNode(node, "Wet Note 4", "젖은 노트 4");
            ItemScanNode(node, "Cosmic Shard", "우주빛 파편");
            ItemScanNode(node, "Cosmic Growth", "우주 생장물");
            ItemScanNode(node, "Chunk of Celestial Brain", "천상의 두뇌 덩어리");
            ItemScanNode(node, "Bucket of Shards", "파편이 든 양동이");
            ItemScanNode(node, "Cosmic Flashlight", "우주빛 손전등");
            ItemScanNode(node, "Forgotten Log 1", "잊혀진 일지 1");
            ItemScanNode(node, "Forgotten Log 2", "잊혀진 일지 2");
            ItemScanNode(node, "Forgotten Log 3", "잊혀진 일지 3");
            ItemScanNode(node, "Glasses", "안경");
            ItemScanNode(node, "Grown Petri Dish", "생장한 배양 접시");
            ItemScanNode(node, "Petri Dish", "배양 접시");
            ItemScanNode(node, "Cosmochad", "코스모채드");
            ItemScanNode(node, "Dying Cosmic Flashlight", "죽어가는 우주빛 손전등");
            ItemScanNode(node, "Dying Cosmic Growth", "죽어가는 우주 생장물");
            ItemScanNode(node, "Blood Petri Dish", "혈액 배양 접시");
            ItemScanNode(node, "Evil Cosmochad", "악마 코스모채드");
            ItemScanNode(node, "Evil Cosmo", "악마 코스모");
            ItemScanNode(node, "Lil Cosmo", "릴 코스모");
            ItemScanNode(node, "Dying Grown Petri Dish", "죽어가는 생장물 배양 접시");
            ItemScanNode(node, "Watching Petri Dish", "감시하는 배양 접시");
            ItemScanNode(node, "Microscope", "현미경");
            ItemScanNode(node, "Round Vile", "원통형 바일");
            ItemScanNode(node, "Square Vile", "사각형 바일");
            ItemScanNode(node, "Oval Vile", "타원형 바일");
            ItemScanNode(node, "Harrington Log 1", "해링턴 일지 1");
            ItemScanNode(node, "Harrington Log 2", "해링턴 일지 2");
            ItemScanNode(node, "Harrington Log 3", "해링턴 일지 3");
            ItemScanNode(node, "Harrington Log 4", "해링턴 일지 4");
            ItemScanNode(node, "Jar of Growth", "생장물이 든 병");
            ItemScanNode(node, "Tape Player Log 1", "테이프 플레이어 일지 1");
            ItemScanNode(node, "Tape Player Log 2", "테이프 플레이어 일지 1");
            ItemScanNode(node, "Tape Player Log 3", "테이프 플레이어 일지 1");
            ItemScanNode(node, "Tape Player Log 4", "테이프 플레이어 일지 1");

            ItemScanNode(node, "Shopping Cart", "쇼핑 카트");
        }

        static void ItemScanNode(ScanNodeProperties node, string oldHeader, string newHeader)
        {
            if (node.headerText == oldHeader)
            {
                node.headerText = newHeader;
                node.subText = node.subText.Replace("Value:", "가격:");
            }else
            {
                return;
            }
        }

        [HarmonyPostfix, HarmonyPatch("DisplayNewDeadline")]
        static void DisplayNewDeadline_Postfix(ref TextMeshProUGUI ___reachedProfitQuotaBonusText)
        {
            ___reachedProfitQuotaBonusText.text = ___reachedProfitQuotaBonusText.text.Replace("Overtime bonus", "초과 근무 보너스");
        }

        [HarmonyPostfix, HarmonyPatch("OnEnable")]
        static void PostfixOnEnable()
        {
            HUDManager.Instance.chatTextField.onSubmit.AddListener(OnSubmitChat);
        }

        [HarmonyPostfix, HarmonyPatch("OnDisable")]
        static void PrefixOnDisable()
        {
            HUDManager.Instance.chatTextField.onSubmit.RemoveListener(OnSubmitChat);
        }

        static void OnSubmitChat(string chatString)
        {
            var localPlayer = GameNetworkManager.Instance.localPlayerController;
            if (!string.IsNullOrEmpty(chatString) && chatString.Length < 50)
            {
                HUDManager.Instance.AddTextToChatOnServer(chatString, (int)localPlayer.playerClientId);
            }
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                if (StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled && Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, StartOfRound.Instance.allPlayerScripts[i].transform.position) > 24.4f && (!GameNetworkManager.Instance.localPlayerController.holdingWalkieTalkie || !StartOfRound.Instance.allPlayerScripts[i].holdingWalkieTalkie))
                {
                    HUDManager.Instance.playerCouldRecieveTextChatAnimator.SetTrigger("ping");
                    break;
                }
            }
            localPlayer.isTypingChat = false;
            HUDManager.Instance.chatTextField.text = "";
            EventSystem.current.SetSelectedGameObject(null);
            HUDManager.Instance.PingHUDElement(HUDManager.Instance.Chat);
            HUDManager.Instance.typingIndicator.enabled = false;
        }

        [HarmonyPrefix, HarmonyPatch("SubmitChat_performed")]
        static void PrefixSubmitChat_performed(
            ref bool __runOriginal
        )
        {
            __runOriginal = false;
        }
    }
}
