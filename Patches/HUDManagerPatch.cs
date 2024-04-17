using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
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

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(HUDManager __instance, ref TextMeshProUGUI ___weightCounter, ref Animator ___weightCounterAnimator,
            ref TextMeshProUGUI ___holdButtonToEndGameEarlyText)
        {
            /*
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                __instance.globalNotificationAnimator.SetTrigger("TriggerNotif");
                __instance.globalNotificationText.text = "새로운 생명체 어쩌구 전송했음";
                __instance.UIAudio.PlayOneShot(__instance.globalNotificationSFX);
            }
            */

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
                if (dialogue.speakerText == "PILOT COMPUTER")
                {
                    dialogue.speakerText = "파일럿 컴퓨터";
                }

                switch (dialogue.bodyText)
                {
                    case "Warning! No response from crew, which has not returned. Emergency code activated.":
                        dialogue.bodyText = "경고! 모든 팀원이 응답하지 않으며 함선에 돌아오지 않았습니다. 긴급 코드가 활성화되었습니다.";
                        break;
                    case "The autopilot will now attempt to fly to the closest safe spaceport. Your items have been lost.":
                        dialogue.bodyText = "가까운 기지로 이동합니다. 모든 폐품을 분실했습니다.";
                        break;

                    case "Alert! The autopilot is leaving due to dangerous conditions.":
                        dialogue.bodyText = "경고! 위험한 상황으로 인해 함선이 이륙하고 있습니다.";
                        break;
                    case "The Company must minimize risk of damage to proprietary hardware. Goodbye!":
                        dialogue.bodyText = "우리 회사는 독점 하드웨어에 대한 손상 위험을 최소화해야 합니다. 안녕히 계세요!";
                        break;

                    case "WARNING!!! The autopilot ship will leave at midnight. Please return quickly.":
                        dialogue.bodyText = "경고!!! 함선이 자정에 이륙합니다. 빠르게 복귀하세요.";
                        break;
                }

                dialogue.bodyText = dialogue.bodyText.Replace("WARNING! Please return by ", "경고! ");
                dialogue.bodyText = dialogue.bodyText.Replace("AM. A vote has been cast, and the autopilot ship will leave early.", "오전 까지 돌아오세요. 함선이 일찍 출발하기로 투표가 완료되었습니다.");
                dialogue.bodyText = dialogue.bodyText.Replace("PM. A vote has been cast, and the autopilot ship will leave early.", "오후 까지 돌아오세요. 함선이 일찍 출발하기로 투표가 완료되었습니다.");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplayTip")]
        private static void DisplayTip_Postfix(TextMeshProUGUI ___tipsPanelHeader, TextMeshProUGUI ___tipsPanelBody)
        {
            switch (___tipsPanelHeader.text)
            {
                case "To read the manual:":
                    ___tipsPanelHeader.text = "지침서 읽기";
                    ___tipsPanelBody.text = "Z를 눌러 자세히 봅니다. Q와 E를 눌러 페이지를 넘깁니다.";
                    break;
                case "TIP:":
                    ___tipsPanelHeader.text = "팁:";
                    if (___tipsPanelBody.text == "Use the ship computer terminal to access secure doors.")
                    {
                        ___tipsPanelBody.text = "함선 내 컴퓨터 터미널을 사용하여 보안 문에 접근하세요.";
                    }else
                    {
                        ___tipsPanelBody.text = "잠긴 문을 효율적으로 지나가려면 함선 터미널에서 <u>자물쇠 따개</u>를 주문하세요.";
                    }
                    break;
                case "Welcome!":
                    ___tipsPanelHeader.text = "환영합니다!";
                    ___tipsPanelBody.text = "우클릭을 눌러 함선 내부의 물체를 스캔하고 정보를 확인할 수 있습니다.";
                    break;
                case "Got scrap!":
                    ___tipsPanelHeader.text = "폐품을 얻었습니다!";
                    ___tipsPanelBody.text = "판매하려면 터미널을 이용해 함선을 회사 건물로 이동시키세요.";
                    break;
                case "Items missed!":
                    ___tipsPanelHeader.text = "아이템을 놓쳤습니다!";
                    ___tipsPanelBody.text = "수송선이 구매하신 상품과 함께 돌아갔습니다. 비용은 환불되지 않습니다.";
                    break;
                case "Item stored!":
                    ___tipsPanelHeader.text = "아이템을 보관했습니다!";
                    ___tipsPanelBody.text = "보관된 물품은 터미널에서 'STORAGE' 명령을 사용하여 확인할 수 있습니다.";
                    break;
                case "HALT!":
                    ___tipsPanelHeader.text = "기다리세요!";
                    ___tipsPanelBody.text = "할당량을 충족시키기 위한 날이 0일 남았습니다. 터미널을 이용하여 회사로 이동한 후 아이템을 판매하세요.";
                    break;
                case "Weather alert!":
                    ___tipsPanelHeader.text = "날씨 경고!";
                    ___tipsPanelBody.text = "당신은 현재 일식이 일어난 위성에 착륙했습니다. 주의하세요!";
                    break;
                case "???":
                    ___tipsPanelBody.text = "입구가 막힌 것 같습니다.";
                    break;
                case "ALERT!":
                    ___tipsPanelHeader.text = "경고!";
                    ___tipsPanelBody.text = "당장 모든 금속성 아이템을 떨어뜨리세요! 정전기가 감지되었습니다. 당신은 몇 초 뒤에 사망할 것입니다.";
                    break;
                case "Welcome back!":
                    ___tipsPanelHeader.text = "돌아오신 것을 환영합니다!";
                    ___tipsPanelBody.text = "이전에 구입한 도구에 대한 보상을 받았습니다. 받으려면 다시 구매하셔야 합니다.";
                    break;
                case "Tip":
                    ___tipsPanelHeader.text = "팁";
                    ___tipsPanelBody.text = "함선의 물체를 재배치하려면 B를 누르세요. 취소하려면 E를 누르세요.";
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
        [HarmonyPostfix, HarmonyPatch("ChangeControlTip")]
        private static void ChangeControlTip_Postfix(int toolTipNumber, string changeTo, bool clearAllOther, ref TextMeshProUGUI[] ___controlTipLines)
        {
            foreach (TextMeshProUGUI text in ___controlTipLines)
            {
                text.text = text.text.Replace("Pull pin", "핀 뽑기");
                text.text = text.text.Replace("Use grenade", "수류탄 사용하기");
                text.text = text.text.Replace("Toss egg", "달걀 던지기");
                text.text = text.text.Replace("Turn safety off", "안전 모드 해제");
                text.text = text.text.Replace("Turn safety on", "안전 모드 설정");
                text.text = text.text.Replace("Quit terminal", "터미널 나가기");
            }
        }
        [HarmonyPrefix, HarmonyPatch("ChangeControlTip")]
        private static void ChangeControlTip_Prefix(int toolTipNumber, string changeTo, bool clearAllOther = false)
        {
            Plugin.mls.LogInfo("Changing Tooltips");
            changeTo = changeTo.Replace("Pull pin", "핀 뽑기");
            changeTo = changeTo.Replace("Use grenade", "수류탄 사용하기");
            changeTo = changeTo.Replace("Toss egg", "달걀 던지기");
            changeTo = changeTo.Replace("Turn safety off", "안전 모드 해제");
            changeTo = changeTo.Replace("Turn safety on", "안전 모드 설정");
            changeTo = changeTo.Replace("Quit terminal", "터미널 나가기");
        }

        [HarmonyPrefix, HarmonyPatch("ChangeControlTipMultiple")]
        private static void ChangeControlTipMultiple_Prefix(string[] allLines, bool holdingItem, Item itemProperties, ref TextMeshProUGUI[] ___controlTipLines)
        {
            if (allLines != null)
            {
                for (int i = 0; i < allLines.Length; i++)
                {
                    string thisLine = allLines[i];
                    thisLine = thisLine.Replace("Pull pin", "핀 뽑기");
                    thisLine = thisLine.Replace("Use grenade", "수류탄 사용하기");
                    thisLine = thisLine.Replace("Toss egg", "달걀 던지기");
                    thisLine = thisLine.Replace("Turn safety off", "안전 모드 해제");
                    thisLine = thisLine.Replace("Turn safety on", "안전 모드 설정");
                    thisLine = thisLine.Replace("Quit terminal", "터미널 나가기");
                    allLines[i] = thisLine;
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch("DisplayDaysLeft")]
        private static void DisplayDaysLeft_Postfix(ref TextMeshProUGUI ___profitQuotaDaysLeftText, ref TextMeshProUGUI ___profitQuotaDaysLeftText2)
        {
            ___profitQuotaDaysLeftText.text = ___profitQuotaDaysLeftText.text.Replace(" Day Left", "일 남았습니다");
            ___profitQuotaDaysLeftText.text = ___profitQuotaDaysLeftText.text.Replace(" Days Left", "일 남았습니다");
            ___profitQuotaDaysLeftText2.text = ___profitQuotaDaysLeftText2.text.Replace(" Day Left", "일 남았습니다");
            ___profitQuotaDaysLeftText2.text = ___profitQuotaDaysLeftText2.text.Replace(" Days Left", "일 남았습니다");
        }

        [HarmonyPostfix, HarmonyPatch("ChangeControlTipMultiple")]
        private static void ChangeControlTipMultiple_Postfix(string[] allLines, bool holdingItem, Item itemProperties, ref TextMeshProUGUI[] ___controlTipLines)
        {
            if (holdingItem && allLines != null)
            {
                ___controlTipLines[0].text = itemProperties.itemName + " 떨어뜨리기 : [G]";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void Update_Prefix(ref TextMeshProUGUI ___planetInfoHeaderText, ref TextMeshProUGUI ___globalNotificationText, ref TextMeshProUGUI ___loadingText)
        {
            if (___loadingText.text == "Waiting for crew...")
            {
                ___loadingText.text = "팀원 기다리는 중...";
            }

            if (___planetInfoHeaderText.text.Contains("CELESTIAL BODY:"))
            {
                ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("CELESTIAL BODY:", "천체:");
                if (___planetInfoHeaderText.text.Contains("Experimentation"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Experimentation", "익스페리멘테이션");
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Where the Company resides", "회사가 소재하는 지역입니다");
                }
                else if (___planetInfoHeaderText.text.Contains("Assurance"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Assurance", "어슈어런스");
                }
                else if (___planetInfoHeaderText.text.Contains("Offense"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Offense", "오펜스");
                }
                else if (___planetInfoHeaderText.text.Contains("Adamance"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Adamance", "애더먼스");
                }
                else if (___planetInfoHeaderText.text.Contains("Rend"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Rend", "렌드");
                }
                else if (___planetInfoHeaderText.text.Contains("Dine"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Dine", "다인");
                }
                else if (___planetInfoHeaderText.text.Contains("March"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("March", "머치");
                }
                else if (___planetInfoHeaderText.text.Contains("Vow"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Vow", "보우");
                }
                else if (___planetInfoHeaderText.text.Contains("Titan"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Titan", "타이탄");
                }
                else if (___planetInfoHeaderText.text.Contains("Artifice"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Artifice", "아터피스");
                }
                else if (___planetInfoHeaderText.text.Contains("Embrion"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Embrion", "엠브리언");
                }
                else if (___planetInfoHeaderText.text.Contains("Gordion"))
                {
                    ___planetInfoHeaderText.text = ___planetInfoHeaderText.text.Replace("Gordion", "고르디온");
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
            try
            {
                TranslateScanNode(node);
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("스캔 노드를 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }
        }

        static void TranslateScanNode(ScanNodeProperties node)
        {
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


            if (node.headerText == "Landmine")
            {
                node.headerText = "지뢰";
            }
            else if (node.headerText == "Turret")
            {
                node.headerText = "터렛";
            }


            if (node.headerText == "Coil")
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

            if (node.headerText == "Key")
            {
                node.headerText = "열쇠";
                node.subText = "가격: $3";
            }
            else if (node.headerText == "Apparatice")
            {
                node.headerText = "장치";
                node.subText = "가격: ???";
            }
            else if (node.headerText == "Magic 7 ball")
            {
                node.headerText = "마법의 7번 공";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Airhorn")
            {
                node.headerText = "에어혼";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Brass bell")
            {
                node.headerText = "황동 종";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Big bolt")
            {
                node.headerText = "큰 나사";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Bottles")
            {
                node.headerText = "병 묶음";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Hair brush")
            {
                node.headerText = "빗";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Candy")
            {
                node.headerText = "사탕";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Cash register"))
            {
                node.headerText = "금전 등록기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Chemical jug"))
            {
                node.headerText = "화학 용기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Clown horn"))
            {
                node.headerText = "광대 나팔";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Large axle"))
            {
                node.headerText = "대형 축";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Teeth")
            {
                node.headerText = "틀니";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Dust pan"))
            {
                node.headerText = "쓰레받기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Egg beater"))
            {
                node.headerText = "달걀 거품기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("V-type engine"))
            {
                node.headerText = "V형 엔진";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Golden cup"))
            {
                node.headerText = "황금 컵";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Fancy lamp"))
            {
                node.headerText = "멋진 램프";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Painting"))
            {
                node.headerText = "그림";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Plastic fish"))
            {
                node.headerText = "플라스틱 물고기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Laser pointer"))
            {
                node.headerText = "레이저 포인터";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Gold Bar"))
            {
                node.headerText = "금 주괴";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Hairdryer"))
            {
                node.headerText = "헤어 드라이기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Magnifying glass"))
            {
                node.headerText = "돋보기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Tattered metal sheet"))
            {
                node.headerText = "너덜너덜한 금속 판";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Cookie mold pan"))
            {
                node.headerText = "쿠키 틀";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Coffee mug"))
            {
                node.headerText = "커피 머그잔";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Perfume bottle"))
            {
                node.headerText = "향수 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Old phone")
            {
                node.headerText = "구식 전화기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Jar of pickles"))
            {
                node.headerText = "피클 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Pill bottle"))
            {
                node.headerText = "약 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Remote")
            {
                node.headerText = "리모컨";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Wedding ring"))
            {
                node.headerText = "결혼 반지";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Robot Toy"))
            {
                node.headerText = "로봇 장난감";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Rubber ducky"))
            {
                node.headerText = "고무 오리";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Red soda"))
            {
                node.headerText = "빨간색 소다";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Steering wheel"))
            {
                node.headerText = "운전대";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Stop sign"))
            {
                node.headerText = "정지 표지판";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Tea Kettle"))
            {
                node.headerText = "찻주전자";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Toothpaste"))
            {
                node.headerText = "치약";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Toy cube"))
            {
                node.headerText = "장난감 큐브";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Bee hive"))
            {
                node.headerText = "벌집";
                node.subText = node.subText.Replace("Value:", "가격:");
                node.subText = node.subText.Replace("VALUE", "가격");
            }
            else if (node.subText == "(Radar booster)")
            {
                node.subText = "(레이더 부스터)";
            }
            else if (node.headerText.Contains("Yield sign"))
            {
                node.headerText = "양보 표지판";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Shotgun")
            {
                node.headerText = "더블-배럴";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Double-barrel")
            {
                node.headerText = "더블-배럴";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Shotgun shell")
            {
                node.headerText = "산탄총 탄약";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Homemade Flashbang"))
            {
                node.headerText = "사제 섬광탄";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Gift")
            {
                node.headerText = "선물";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Gift box")
            {
                node.headerText = "선물 상자";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Flask"))
            {
                node.headerText = "플라스크";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Tragedy")
            {
                node.headerText = "비극";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Comedy")
            {
                node.headerText = "희극";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Whoopie cushion"))
            {
                node.headerText = "방퀴 쿠션";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText.Contains("Kitchen knife"))
            {
                node.headerText = "식칼";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Easter egg")
            {
                node.headerText = "부활절 달걀";
                node.subText = node.subText.Replace("Value:", "가격:");
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
