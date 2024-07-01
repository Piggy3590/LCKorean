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
                if (dialogue.speakerText == "PILOT COMPUTER")
                {
                    dialogue.speakerText = "파일럿 컴퓨터";
                }

                switch (dialogue.bodyText)
                {
                    case "Warning! No response from crew, which has not returned. Emergency code activated.":
                        dialogue.bodyText = Plugin.allDead1;
                        break;
                    case "The autopilot will now attempt to fly to the closest safe spaceport. Your items have been lost.":
                        dialogue.bodyText = Plugin.allDead2;
                        break;

                    case "Alert! The autopilot is leaving due to dangerous conditions.":
                        dialogue.bodyText = Plugin.autoTakeoff1;
                        break;
                    case "The Company must minimize risk of damage to proprietary hardware. Goodbye!":
                        dialogue.bodyText = Plugin.autoTakeoff2;
                        break;

                    case "WARNING!!! The autopilot ship will leave at midnight. Please return quickly.":
                        dialogue.bodyText = Plugin.midnightWarning;
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
            try
            {
                TranslateScanNode(node);
                if (Plugin.translateModdedContent)
                {
                    TranslateModdedContent(node);
                }
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
                node.headerText = "납치 여우";
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
            else if (node.headerText == "Cash register")
            {
                node.headerText = "금전 등록기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Chemical jug")
            {
                node.headerText = "화학 용기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Clown horn")
            {
                node.headerText = "광대 나팔";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Large axle")
            {
                node.headerText = "대형 축";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Teeth")
            {
                node.headerText = "틀니";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Dust pan")
            {
                node.headerText = "쓰레받기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Egg beater")
            {
                node.headerText = "달걀 거품기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "V-type engine")
            {
                node.headerText = "V형 엔진";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Golden cup")
            {
                node.headerText = "황금 컵";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Fancy lamp")
            {
                node.headerText = "멋진 램프";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Painting")
            {
                node.headerText = "그림";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Plastic fish")
            {
                node.headerText = "플라스틱 물고기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Laser pointer")
            {
                node.headerText = "레이저 포인터";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Gold Bar")
            {
                node.headerText = "금 주괴";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Hairdryer")
            {
                node.headerText = "헤어 드라이기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Magnifying glass")
            {
                node.headerText = "돋보기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Tattered metal sheet")
            {
                node.headerText = "너덜너덜한 금속 판";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Cookie mold pan")
            {
                node.headerText = "쿠키 틀";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Coffee mug")
            {
                node.headerText = "커피 머그잔";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Perfume bottle")
            {
                node.headerText = "향수 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Old phone")
            {
                node.headerText = "구식 전화기";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Jar of pickles")
            {
                node.headerText = "피클 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Pill bottle")
            {
                node.headerText = "약 병";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Remote")
            {
                node.headerText = "리모컨";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Wedding ring")
            {
                node.headerText = "결혼 반지";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Robot Toy")
            {
                node.headerText = "로봇 장난감";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Rubber ducky")
            {
                node.headerText = "고무 오리";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Red soda")
            {
                node.headerText = "빨간색 소다";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Steering wheel")
            {
                node.headerText = "운전대";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Stop sign")
            {
                node.headerText = "정지 표지판";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Tea Kettle")
            {
                node.headerText = "찻주전자";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Toothpaste")
            {
                node.headerText = "치약";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Toy cube")
            {
                node.headerText = "장난감 큐브";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Bee hive")
            {
                node.headerText = "벌집";
                node.subText = node.subText.Replace("Value:", "가격:");
                node.subText = node.subText.Replace("VALUE", "가격");
            }
            else if (node.subText == "(Radar booster)")
            {
                node.subText = "(레이더 부스터)";
            }
            else if (node.headerText == "Yield sign")
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
            else if (node.headerText == "Homemade Flashbang")
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
            else if (node.headerText == "Flask")
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
            else if (node.headerText == "Whoopie cushion")
            {
                node.headerText = "방퀴 쿠션";
                node.subText = node.subText.Replace("Value:", "가격:");
            }
            else if (node.headerText == "Kitchen knife")
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

        static void TranslateModdedContent(ScanNodeProperties node)
        {
            //ImmersiveScraps
            ModdedTranslateScanNode(node, "Alcohol Flask", "알코올 플라스크");
            ModdedTranslateScanNode(node, "Anvil", "모루");
            ModdedTranslateScanNode(node, "Baseball bat", "야구 방망이");
            ModdedTranslateScanNode(node, "Beer can", "맥주 캔");
            ModdedTranslateScanNode(node, "Brick", "벽돌");
            ModdedTranslateScanNode(node, "Broken engine", "망가진 엔진");
            ModdedTranslateScanNode(node, "Bucket", "양동이");
            ModdedTranslateScanNode(node, "Can paint", "페인트 캔");
            ModdedTranslateScanNode(node, "Canteen", "수통");
            ModdedTranslateScanNode(node, "Car battery", "자동차 배터리");
            ModdedTranslateScanNode(node, "Clamp", "조임틀");
            ModdedTranslateScanNode(node, "Clock", "시계");
            ModdedTranslateScanNode(node, "Fancy Painting", "멋진 그림");
            ModdedTranslateScanNode(node, "Fan", "선풍기");
            ModdedTranslateScanNode(node, "Fireaxe", "소방 도끼");
            ModdedTranslateScanNode(node, "Fire extinguisher", "소화기");
            ModdedTranslateScanNode(node, "Fire hydrant", "소화전");
            ModdedTranslateScanNode(node, "Food can", "통조림");
            ModdedTranslateScanNode(node, "Gameboy", "게임보이");
            ModdedTranslateScanNode(node, "Garbage", "쓰레기");
            ModdedTranslateScanNode(node, "Hammer", "망치");
            ModdedTranslateScanNode(node, "Jerrycan", "기름통");
            ModdedTranslateScanNode(node, "Keyboard", "키보드");
            ModdedTranslateScanNode(node, "Lantern", "랜턴");
            ModdedTranslateScanNode(node, "Library lamp", "도서관 램프");
            ModdedTranslateScanNode(node, "Plant", "식물");
            ModdedTranslateScanNode(node, "Pliers", "플라이어");
            ModdedTranslateScanNode(node, "Plunger", "뚫어뻥");
            ModdedTranslateScanNode(node, "Retro Toy", "레트로 장난감");
            ModdedTranslateScanNode(node, "Screwdriver", "스크류 드라이버");
            ModdedTranslateScanNode(node, "Sink", "싱크대");
            ModdedTranslateScanNode(node, "Socket Wrench", "소켓 렌치");
            ModdedTranslateScanNode(node, "Squeaky toy", "고무 오리");
            ModdedTranslateScanNode(node, "Suitcase", "여행 가방");
            ModdedTranslateScanNode(node, "Toaster", "토스터기");
            ModdedTranslateScanNode(node, "Toolbox", "공구 상자");
            ModdedTranslateScanNode(node, "Top hat", "실크햇");
            ModdedTranslateScanNode(node, "Traffic cone", "라바콘");
            ModdedTranslateScanNode(node, "Vent", "환풍구");
            ModdedTranslateScanNode(node, "Watering Can", "물뿌리개");
            ModdedTranslateScanNode(node, "Wheel", "바퀴");
            ModdedTranslateScanNode(node, "Wine bottle", "와인 병");
            ModdedTranslateScanNode(node, "Wrench", "렌치");

            //Wesleys
            ModdedTranslateScanNode(node, "Amethyst Cluster", "자수정 군집");
            ModdedTranslateScanNode(node, "Syringe", "주사기");
            ModdedTranslateScanNode(node, "Syringe Gun", "주사기총");
            ModdedTranslateScanNode(node, "Corner Pipe", "코너 파이프");
            ModdedTranslateScanNode(node, "Small Pipe", "작은 파이프");
            ModdedTranslateScanNode(node, "Flow Pipe", "파이프");
            ModdedTranslateScanNode(node, "Brain Jar", "뇌가 담긴 병");
            ModdedTranslateScanNode(node, "Toy Nutcracker", "호두까기 인형 장난감");
            ModdedTranslateScanNode(node, "Test Tube", "시험관");
            ModdedTranslateScanNode(node, "Test Tube Rack", "시험관 랙");
            ModdedTranslateScanNode(node, "Nutcracker Eye", "호두까기 인형 눈");
            ModdedTranslateScanNode(node, "Blue Test Tube", "파란색 시험관");
            ModdedTranslateScanNode(node, "Yellow Test Tube", "노란색 시험관");
            ModdedTranslateScanNode(node, "Red Test Tube", "빨간색 시험관");
            ModdedTranslateScanNode(node, "Green Test Tube", "초록색 시험관");
            ModdedTranslateScanNode(node, "Crowbar", "쇠지렛대");
            ModdedTranslateScanNode(node, "Plzen", "플젠");
            ModdedTranslateScanNode(node, "Cup", "컵");
            ModdedTranslateScanNode(node, "Microwave", "전자레인지");
            ModdedTranslateScanNode(node, "bubblegun", "비눗방울 총");
            ModdedTranslateScanNode(node, "Broken P88", "망가진 P88");
            ModdedTranslateScanNode(node, "employee", "직원");
            ModdedTranslateScanNode(node, "Mine", "지뢰");
            ModdedTranslateScanNode(node, "Gravity Gun", "중력건");
            ModdedTranslateScanNode(node, "Redbull", "레드불");
            ModdedTranslateScanNode(node, "Diamond ore", "다이아몬드 광석");
            ModdedTranslateScanNode(node, "Stone", "돌");
            ModdedTranslateScanNode(node, "Diamond block", "다이아몬드 블록");
            ModdedTranslateScanNode(node, "Tau Cannon", "타우 캐논");
            ModdedTranslateScanNode(node, "Toothles", "투슬리스");
            ModdedTranslateScanNode(node, "Crossbow", "석궁");
            ModdedTranslateScanNode(node, "physgun", "피직스건");
            ModdedTranslateScanNode(node, "Ammo crate", "탄약 상자");
            ModdedTranslateScanNode(node, "Drink", "음료수");
            ModdedTranslateScanNode(node, "Radio", "라디오");
            ModdedTranslateScanNode(node, "Mouse", "마우스");
            ModdedTranslateScanNode(node, "Monitor", "모니터");
            ModdedTranslateScanNode(node, "Battery", "건전지");
            ModdedTranslateScanNode(node, "Cannon", "대포");
            ModdedTranslateScanNode(node, "Health Drink", "건강 음료");
            ModdedTranslateScanNode(node, "Chemical", "화학 약품");
            ModdedTranslateScanNode(node, "Disinfecting Alcohol", "소독용 알코올");
            ModdedTranslateScanNode(node, "Ampoule", "앰풀");
            ModdedTranslateScanNode(node, "Blood Pack", "혈액 팩");
            ModdedTranslateScanNode(node, "Flip Lighter", "라이터");
            ModdedTranslateScanNode(node, "Rubber Ball", "고무 공");
            ModdedTranslateScanNode(node, "Video Tape", "비디오 테이프");
            ModdedTranslateScanNode(node, "First Aid Kit", "구급 상자");
            ModdedTranslateScanNode(node, "Gold Medallion", "금메달");
            ModdedTranslateScanNode(node, "Steel Pipe", "금속 파이프");
            ModdedTranslateScanNode(node, "Axe", "도끼");
            ModdedTranslateScanNode(node, "Emergency Hammer", "비상용 망치");
            ModdedTranslateScanNode(node, "Katana", "카타나");
            ModdedTranslateScanNode(node, "Silver Medallion", "은메달");
            ModdedTranslateScanNode(node, "Pocket Radio", "휴대용 라디오");
            ModdedTranslateScanNode(node, "Teddy Plush", "곰 인형");
            ModdedTranslateScanNode(node, "LEDx Transilluminator", "LEDx 트랜스일루미네이터");
            ModdedTranslateScanNode(node, "Graphics Card", "그래픽 카드");
            ModdedTranslateScanNode(node, "Labs Access Keycard", "Labs 접근 키카드");
            ModdedTranslateScanNode(node, "Tetriz portable game console", "Tetriz 휴대용 게임기");
            ModdedTranslateScanNode(node, "Gas Analyzer", "가스 측정기");
            ModdedTranslateScanNode(node, "Bitcoin", "비트코인");
            ModdedTranslateScanNode(node, "Tank Battery", "탱크 배터리");
            ModdedTranslateScanNode(node, "Secure Container Kappa", "보안 컨테이너 카파");
            ModdedTranslateScanNode(node, "AI-2 Medkit", "AI-2 응급 치료 키트");
            ModdedTranslateScanNode(node, "Bronze Lion", "사자 동상");
            ModdedTranslateScanNode(node, "Raven Figurine", "까마귀 조각상");
            ModdedTranslateScanNode(node, "Golden Rooster Figurine", "황금 수탉 조각상");
            ModdedTranslateScanNode(node, "Red Rebel Ice Pick", "레드 레벨 아이스 픽");
            ModdedTranslateScanNode(node, "UVSR Taiga-1 survival machete", "UVSR Taiga-1 생존용 마체테");
            ModdedTranslateScanNode(node, "Experiment Log Hyper Acid", "Hyper Acid 실험 기록");
            ModdedTranslateScanNode(node, "Experiment Log Comedy Mask", "희극 가면 실험 기록");
            ModdedTranslateScanNode(node, "Experiment Log Cursed Coin", "저주받은 동전 실험 기록");
            ModdedTranslateScanNode(node, "Experiment Log BIO HXNV7", "바이오 HXNV7 실험 기록");
            ModdedTranslateScanNode(node, "Blue Folder", "파란색 폴더");
            ModdedTranslateScanNode(node, "Red Folder", "빨간색 폴더");
            ModdedTranslateScanNode(node, "Fire Extinguisher", "소화기");
            ModdedTranslateScanNode(node, "Coil", "코일");
            ModdedTranslateScanNode(node, "Typewriter", "타자기");
            ModdedTranslateScanNode(node, "Documents", "서류 더미");
            ModdedTranslateScanNode(node, "Stapler", "스테이플러");
            ModdedTranslateScanNode(node, "Old Computer", "구식 컴퓨터");
            ModdedTranslateScanNode(node, "Bronze Trophy", "브론즈 트로피");
            ModdedTranslateScanNode(node, "Banana", "바나나");
            ModdedTranslateScanNode(node, "Stun Baton", "스턴봉");
            ModdedTranslateScanNode(node, "BIO-HXNV7", "바이오-HXNV7");
            ModdedTranslateScanNode(node, "Recovered Secret Log", "복구된 비밀 일지");
            ModdedTranslateScanNode(node, "Experiment Log Golden Dagger", "황금 단검 실험 기록");
            ModdedTranslateScanNode(node, "Clam", "대합");
            ModdedTranslateScanNode(node, "Turtle Shell", "거북이 등딱지");
            ModdedTranslateScanNode(node, "Fish Bones", "생선 뼈");
            ModdedTranslateScanNode(node, "Horned Shell", "뿔 달린 껍질");
            ModdedTranslateScanNode(node, "Porcelain Teacup", "도자기 찻잔");
            ModdedTranslateScanNode(node, "Marble", "대리석");
            ModdedTranslateScanNode(node, "Porcelain Bottle", "도자기 병");
            ModdedTranslateScanNode(node, "Porcelain Perfume Bottle", "도자기 향수 병");
            ModdedTranslateScanNode(node, "Glowing Orb", "발광구");
            ModdedTranslateScanNode(node, "Golden Skull", "황금 해골");
            ModdedTranslateScanNode(node, "Map of Cosmocos", "코스모코스 지도");
            ModdedTranslateScanNode(node, "Wet Note 1", "젖은 노트 1");
            ModdedTranslateScanNode(node, "Wet Note 2", "젖은 노트 2");
            ModdedTranslateScanNode(node, "Wet Note 3", "젖은 노트 3");
            ModdedTranslateScanNode(node, "Wet Note 4", "젖은 노트 4");
            ModdedTranslateScanNode(node, "Cosmic Shard", "우주빛 파편");
            ModdedTranslateScanNode(node, "Cosmic Growth", "우주 생장물");
            ModdedTranslateScanNode(node, "Chunk of Celestial Brain", "천상의 두뇌 덩어리");
            ModdedTranslateScanNode(node, "Bucket of Shards", "파편이 든 양동이");
            ModdedTranslateScanNode(node, "Cosmic Flashlight", "우주빛 손전등");
            ModdedTranslateScanNode(node, "Forgotten Log 1", "잊혀진 일지 1");
            ModdedTranslateScanNode(node, "Forgotten Log 2", "잊혀진 일지 2");
            ModdedTranslateScanNode(node, "Forgotten Log 3", "잊혀진 일지 3");
            ModdedTranslateScanNode(node, "Glasses", "안경");
            ModdedTranslateScanNode(node, "Grown Petri Dish", "생장한 배양 접시");
            ModdedTranslateScanNode(node, "Petri Dish", "배양 접시");
            ModdedTranslateScanNode(node, "Cosmochad", "코스모채드");
            ModdedTranslateScanNode(node, "Dying Cosmic Flashlight", "죽어가는 우주빛 손전등");
            ModdedTranslateScanNode(node, "Dying Cosmic Growth", "죽어가는 우주 생장물");
            ModdedTranslateScanNode(node, "Blood Petri Dish", "혈액 배양 접시");
            ModdedTranslateScanNode(node, "Evil Cosmochad", "악마 코스모채드");
            ModdedTranslateScanNode(node, "Evil Cosmo", "악마 코스모");
            ModdedTranslateScanNode(node, "Lil Cosmo", "릴 코스모");
            ModdedTranslateScanNode(node, "Dying Grown Petri Dish", "죽어가는 생장물 배양 접시");
            ModdedTranslateScanNode(node, "Watching Petri Dish", "감시하는 배양 접시");
            ModdedTranslateScanNode(node, "Microscope", "현미경");
            ModdedTranslateScanNode(node, "Round Vile", "원통형 바일");
            ModdedTranslateScanNode(node, "Square Vile", "사각형 바일");
            ModdedTranslateScanNode(node, "Oval Vile", "타원형 바일");
            ModdedTranslateScanNode(node, "Harrington Log 1", "해링턴 일지 1");
            ModdedTranslateScanNode(node, "Harrington Log 2", "해링턴 일지 2");
            ModdedTranslateScanNode(node, "Harrington Log 3", "해링턴 일지 3");
            ModdedTranslateScanNode(node, "Harrington Log 4", "해링턴 일지 4");
            ModdedTranslateScanNode(node, "Jar of Growth", "생장물이 든 병");
            ModdedTranslateScanNode(node, "Tape Player Log 1", "테이프 플레이어 일지 1");
            ModdedTranslateScanNode(node, "Tape Player Log 2", "테이프 플레이어 일지 1");
            ModdedTranslateScanNode(node, "Tape Player Log 3", "테이프 플레이어 일지 1");
            ModdedTranslateScanNode(node, "Tape Player Log 4", "테이프 플레이어 일지 1");
        }

        static void ModdedTranslateScanNode(ScanNodeProperties node, string oldHeader, string newHeader)
        {
            if (node.headerText == oldHeader)
            {
                node.headerText = newHeader;
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
