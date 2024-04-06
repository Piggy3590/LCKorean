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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using System.Reflection;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(ref TerminalNodesList ___terminalNodes)
        {
            TranslateKeyword(___terminalNodes);
            TranslateNode(___terminalNodes);
        }

        [HarmonyPrefix]
        [HarmonyPatch("TextPostProcess")]
        private static void LoadNewNode_Prefix(Terminal __instance, TerminalNode node)
        {
            FieldInfo fieldInfo = typeof(Terminal).GetField("modifyingText", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(__instance, true);
            __instance.RunTerminalEvents(node);
            __instance.screenText.interactable = true;
            string text = "";
            if (node.clearPreviousText)
            {
                text = "\n\n\n" + node.displayText.ToString();
            }
            else
            {
                text = "\n\n" + __instance.screenText.text.ToString() + "\n\n" + node.displayText.ToString();
                int num = text.Length - 250;
                text = text.Substring(Mathf.Clamp(num, 0, text.Length)).ToString();
            }
            try
            {
                text = TextPostProcess(__instance, text, node);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("An error occured while post processing terminal text: {0}", ex));
            }
            __instance.screenText.text = text;
            __instance.currentText = __instance.screenText.text;
            __instance.textAdded = 0;
            if (node.playSyncedClip != -1)
            {
                __instance.PlayTerminalAudioServerRpc(node.playSyncedClip);
            }
            else if (node.playClip != null)
            {
                __instance.terminalAudio.PlayOneShot(node.playClip);
            }
            __instance.LoadTerminalImage(node);
            __instance.currentNode = node;
            return;
        }

        private static string TextPostProcess(Terminal __instance, string modifiedDisplayText, TerminalNode node)
        {
            int num = modifiedDisplayText.Split(new string[] { "[planetTime]" }, StringSplitOptions.None).Length - 1;
            if (num > 0)
            {
                Regex regex = new Regex(Regex.Escape("[planetTime]"));
                int num2 = 0;
                while (num2 < num && __instance.moonsCatalogueList.Length > num2)
                {
                    Debug.Log(string.Format("isDemo:{0} ; {1}", GameNetworkManager.Instance.isDemo, __instance.moonsCatalogueList[num2].lockedForDemo));
                    string text;
                    if (GameNetworkManager.Instance.isDemo && __instance.moonsCatalogueList[num2].lockedForDemo)
                    {
                        text = "(잠김)";
                    }
                    else if (__instance.moonsCatalogueList[num2].currentWeather == LevelWeatherType.None)
                    {
                        text = "";
                    }
                    else
                    {
                        Plugin.mls.LogInfo(__instance.moonsCatalogueList[num2].currentWeather.ToString());
                        if (__instance.moonsCatalogueList[num2].currentWeather.ToString().Contains("Rainy"))
                        {
                            text = "(우천)";
                        }else if (__instance.moonsCatalogueList[num2].currentWeather.ToString().Contains("Stormy"))
                        {
                            text = "(뇌우)";
                        }
                        else if (__instance.moonsCatalogueList[num2].currentWeather.ToString().Contains("Foggy"))
                        {
                            text = "(안개)";
                        }
                        else if (__instance.moonsCatalogueList[num2].currentWeather.ToString().Contains("Flooded"))
                        {
                            text = "(홍수)";
                        }
                        else if (__instance.moonsCatalogueList[num2].currentWeather.ToString().Contains("Eclipsed"))
                        {
                            text = "(일식)";
                        }
                        else
                        {
                            text = "";
                        }
                    }
                    modifiedDisplayText = regex.Replace(modifiedDisplayText, text, 1);
                    num2++;
                }
            }
            try
            {
                if (node.displayPlanetInfo != -1)
                {
                    string text;
                    if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather == LevelWeatherType.None)
                    {
                        text = "맑음";
                    }
                    else
                    {
                        if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower().Contains("Rainy"))
                        {
                            text = "(우천)";
                        }
                        else if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower().Contains("Stormy"))
                        {
                            text = "(뇌우)";
                        }
                        else if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower().Contains("Foggy"))
                        {
                            text = "(안개)";
                        }
                        else if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower().Contains("Flooded"))
                        {
                            text = "(홍수)";
                        }
                        else if (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower().Contains("Eclipsed"))
                        {
                            text = "(일식)";
                        }
                        else
                        {
                            text = "";
                        }
                    }
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentPlanetTime]", text);
                }
            }
            catch
            {
                Debug.Log(string.Format("Exception occured on terminal while setting node planet info; current node displayPlanetInfo:{0}", node.displayPlanetInfo));
            }
            if (modifiedDisplayText.Contains("[currentScannedEnemiesList]"))
            {
                if (__instance.scannedEnemyIDs == null || __instance.scannedEnemyIDs.Count <= 0)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentScannedEnemiesList]", "생명체에 대한 데이터가 수집되지 않습니다. 스캔이 필요합니다.");
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < __instance.scannedEnemyIDs.Count; i++)
                    {
                        Debug.Log(string.Format("scanID # {0}: {1}; {2}", i, __instance.scannedEnemyIDs[i], __instance.enemyFiles[__instance.scannedEnemyIDs[i]].creatureName));
                        Debug.Log(string.Format("scanID # {0}: {1}", i, __instance.scannedEnemyIDs[i]));
                        stringBuilder.Append("\n" + __instance.enemyFiles[__instance.scannedEnemyIDs[i]].creatureName);
                        if (__instance.newlyScannedEnemyIDs.Contains(__instance.scannedEnemyIDs[i]))
                        {
                            stringBuilder.Append(" (NEW)");
                        }
                    }
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentScannedEnemiesList]", stringBuilder.ToString());
                }
            }
            if (modifiedDisplayText.Contains("[buyableItemsList]"))
            {
                if (__instance.buyableItemsList == null || __instance.buyableItemsList.Length == 0)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[buyableItemsList]", "[재고가 없습니다!]");
                }
                else
                {
                    StringBuilder stringBuilder2 = new StringBuilder();
                    for (int j = 0; j < __instance.buyableItemsList.Length; j++)
                    {
                        if (GameNetworkManager.Instance.isDemo && __instance.buyableItemsList[j].lockedInDemo)
                        {
                            stringBuilder2.Append("\n* " + __instance.buyableItemsList[j].itemName + " (잠김)");
                        }
                        else
                        {
                            stringBuilder2.Append("\n* " + __instance.buyableItemsList[j].itemName + "  //  가격: $" + ((float)__instance.buyableItemsList[j].creditsWorth * ((float)__instance.itemSalesPercentages[j] / 100f)).ToString());
                        }
                        if (__instance.itemSalesPercentages[j] != 100)
                        {
                            stringBuilder2.Append(string.Format("   ({0}% 세일!)", 100 - __instance.itemSalesPercentages[j]));
                        }
                    }
                    modifiedDisplayText = modifiedDisplayText.Replace("[buyableItemsList]", stringBuilder2.ToString());
                }
            }
            if (modifiedDisplayText.Contains("[currentUnlockedLogsList]"))
            {
                if (__instance.unlockedStoryLogs == null || __instance.unlockedStoryLogs.Count <= 0)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentUnlockedLogsList]", "[모든 데이터가 손상되거나 덮어쓰기됨]");
                }
                else
                {
                    StringBuilder stringBuilder3 = new StringBuilder();
                    for (int k = 0; k < __instance.unlockedStoryLogs.Count; k++)
                    {
                        stringBuilder3.Append("\n" + __instance.logEntryFiles[__instance.unlockedStoryLogs[k]].creatureName);
                        if (__instance.newlyUnlockedStoryLogs.Contains(__instance.unlockedStoryLogs[k]))
                        {
                            stringBuilder3.Append(" (NEW)");
                        }
                    }
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentUnlockedLogsList]", stringBuilder3.ToString());
                }
            }
            if (modifiedDisplayText.Contains("[unlockablesSelectionList]"))
            {
                if (__instance.ShipDecorSelection == null || __instance.ShipDecorSelection.Count <= 0)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[unlockablesSelectionList]", "[사용 가능한 아이템이 없음]");
                }
                else
                {
                    StringBuilder stringBuilder4 = new StringBuilder();
                    for (int l = 0; l < __instance.ShipDecorSelection.Count; l++)
                    {
                        stringBuilder4.Append(string.Format("\n{0}  //  ${1}", __instance.ShipDecorSelection[l].creatureName, __instance.ShipDecorSelection[l].itemCost));
                    }
                    modifiedDisplayText = modifiedDisplayText.Replace("[unlockablesSelectionList]", stringBuilder4.ToString());
                }
            }
            if (modifiedDisplayText.Contains("[storedUnlockablesList]"))
            {
                StringBuilder stringBuilder5 = new StringBuilder();
                bool flag = false;
                for (int m = 0; m < StartOfRound.Instance.unlockablesList.unlockables.Count; m++)
                {
                    if (StartOfRound.Instance.unlockablesList.unlockables[m].inStorage)
                    {
                        flag = true;
                        stringBuilder5.Append("\n" + StartOfRound.Instance.unlockablesList.unlockables[m].unlockableName);
                    }
                }
                if (!flag)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[storedUnlockablesList]", "[보관된 아이템이 없습니다. B로 개체를 이동하는 동안 X를 눌러 보관합니다.]");
                }
                else
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[storedUnlockablesList]", stringBuilder5.ToString());
                }
            }
            if (modifiedDisplayText.Contains("[scanForItems]"))
            {
                System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 91);
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                GrabbableObject[] array = GameObject.FindObjectsOfType<GrabbableObject>();
                for (int n = 0; n < array.Length; n++)
                {
                    if (array[n].itemProperties.isScrap && !array[n].isInShipRoom && !array[n].isInElevator)
                    {
                        num5 += array[n].itemProperties.maxValue - array[n].itemProperties.minValue;
                        num4 += Mathf.Clamp(random.Next(array[n].itemProperties.minValue, array[n].itemProperties.maxValue), array[n].scrapValue - 6 * n, array[n].scrapValue + 9 * n);
                        num3++;
                    }
                }
                modifiedDisplayText = modifiedDisplayText.Replace("[scanForItems]", string.Format("There are {0} objects outside the ship, totalling at an approximate value of ${1}.", num3, num4));
            }
            if (__instance.numberOfItemsInDropship <= 0)
            {
                modifiedDisplayText = modifiedDisplayText.Replace("[numberOfItemsOnRoute]", "");
            }
            else
            {
                modifiedDisplayText = modifiedDisplayText.Replace("[numberOfItemsOnRoute]", string.Format("{0} purchased items on route.", __instance.numberOfItemsInDropship));
            }
            modifiedDisplayText = modifiedDisplayText.Replace("[currentDay]", DateTime.Now.DayOfWeek.ToString());
            modifiedDisplayText = modifiedDisplayText.Replace("[variableAmount]", __instance.playerDefinedAmount.ToString());
            modifiedDisplayText = modifiedDisplayText.Replace("[playerCredits]", "$" + __instance.groupCredits.ToString());
            FieldInfo fieldInfo = typeof(Terminal).GetField("totalCostOfItems", BindingFlags.NonPublic | BindingFlags.Instance);
            modifiedDisplayText = modifiedDisplayText.Replace("[totalCost]", "$" + fieldInfo.GetValue(__instance).ToString());
            modifiedDisplayText = modifiedDisplayText.Replace("[companyBuyingPercent]", string.Format("{0}%", Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)));
            if (__instance.displayingPersistentImage)
            {
                modifiedDisplayText = "\n\n\n\n\n\n\n\n\n\n\n\n\n\nn\n\n\n\n\n\n" + modifiedDisplayText;
            }
            return modifiedDisplayText;
        }

        static void TranslateNode(TerminalNodesList terminalNodes)
        {
            foreach (TerminalNode node in terminalNodes.specialNodes)
            {
                switch (node.name)
                {
                    case "Start":
                        node.displayText = "FORTUNE-9 OS에 오신 것을 환영합니다\r\n\t회사 제공\r\n\r\n행복한 [currentDay] 되세요.\r\n\r\n명령 목록을 보려면 \"Help\"를 입력하세요.\r\n\r\n\r\n\r\n\r\n";
                        break;
                    case "StartFirstTime":
                        node.displayText.Replace("Welcome to the FORTUNE-9 OS", "FORTUNE-9 OS에 오신 것을 환영합니다");
                        node.displayText.Replace("Courtesy of the Company", "회사 제공");
                        node.displayText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"Help\"를 입력하세요.");
                        break;
                    case "ParserError1":
                        node.displayText = "[이 단어에 제공되는 작업이 없습니다.]\r\n\r\n";
                        break;
                    case "ParserError2":
                        node.displayText = "[이 작업과 함께 제공된 개체가 없거나, 단어가 잘못 입력되었거나 존재하지 않습니다.]\r\n\r\n";
                        break;
                    case "ParserError3":
                        node.displayText = "[이 작업은 이 개체와 맞지 않습니다.]\r\n\r\n";
                        break;
                }
                switch (node.displayText)
                {
                    case "BG IG, A System-Act Ally\r\nCopyright (C) 2084-2108, Halden Electronics Inc.\r\nCourtesy of the Company.\r\n\r\n\r\n\r\nBios for FORTUNE-9 87.7/10MHZ SYSTEM\r\n\r\nCurrent date is Tue  3-7-2532\r\nCurrent time is 8:03:32.15\r\n\r\nPlease enter favorite animal: ":
                        node.displayText = "BG IG, 시스템 행동 연합\r\nCopyright (C) 2084-2108, Halden Electronics Inc.\r\n회사 제공.\r\n\r\n\r\n\r\nFORTUNE-9 전용 바이오스 87.7/10MHZ 시스템\r\n\r\n현재 날짜는 2532년 3월 7일 화요일입니다\n현재 시간은 8:03:32.15입니다.\n\n좋아하는 동물을 입력하세요: ";
                        break;
                    case "You could not afford these items!\r\nYour balance is [playerCredits]. Total cost of these items is [totalCost].\r\n\r\n":
                        node.displayText = "자금이 충분하지 않습니다!\n당신의 소지금은 [playerCredits]이지만 이 아이템의 총 가격은 [totalCost]입니다.\n\n";
                        break;
                    case "Unable to route the ship currently. It must be in orbit around a moon to route the autopilot.\r\nUse the main lever at the front desk to enter orbit.\r\n\r\n\r\n\r\n":
                        node.displayText = "함선의 항로를 지정할 수 없습니다. 항로를 지정하려면 이륙한 상태여야 합니다.\n궤도에 들어가려면 프런트 데스크에 있는 메인 레버를 사용하세요.\n\n\n\n";
                        break;
                    case "The delivery vehicle cannot hold more than 12 items\r\nat a time. Please pick up your items when they land!\r\n\r\n\r\n":
                        node.displayText = "수송선은 최대 12개의 아이템만을 적재할 수 있습니다\n착륙하면 아이템을 회수해주세요!\n\n\n";
                        break;
                    case "An error occured! Try again.\r\n\r\n":
                        node.displayText = "오류가 발생했습니다! 다시 시도하세요.\r\n\r\n";
                        break;
                    case "No data has been collected on this creature. \r\nA scan is required.\r\n\r\n":
                        node.displayText = "이 생명체에 대해 수집된 데이터가 없습니다. \r\n스캔이 필요합니다.\r\n\r\n";
                        break;
                    case "To purchase decorations, the ship cannot be landed.\r\n\r\n":
                        node.displayText = "가구를 구매하려면 함선이 완전히 이착륙할 때까지 기다리세요.\r\n\r\n";
                        break;
                    case "The autopilot ship is already orbiting this moon!":
                        node.displayText = "이미 이 위성의 궤도에 있습니다!";
                        break;
                    case "[DATA CORRUPTED OR OVERWRITTEN]\r\n\r\n":
                        node.displayText = "[데이터가 손상되거나 덮어쓰기됨]\r\n\r\n";
                        break;
                    case "This has already been unlocked for your ship!":
                        node.displayText = "이미 잠금이 해제된 아이템입니다!";
                        break;
                    case "The ship cannot be leaving or landing!\r\n\r\n":
                        node.displayText = "함선이 완전히 이착륙할 때까지 기다리세요!\r\n\r\n";
                        break;
                    case "This item is not in stock!\r\n\r\n":
                        node.displayText = "이 아이템은 재고가 없습니다!\r\n\r\n";
                        break;
                    case "Returned the item from storage!":
                        node.displayText = "아이템을 저장고에서 꺼냈습니다!";
                        break;
                    case "Entered broadcast code.\r\n":
                        node.displayText = "송출 코드를 입력했습니다.\r\n";
                        break;
                    case "Switched radar to player.\r\n\r\n":
                        node.displayText = "레이더를 플레이어로 전환했습니다.\r\n\r\n";
                        break;
                    case "Pinged radar booster.\r\n\r\n":
                        node.displayText = "레이더 부스터를 핑했습니다.\r\n\r\n";
                        break;
                    case "Sent transmission.\r\n\r\n":
                        node.displayText = "전송했습니다.\r\n\r\n";
                        break;
                    case "Flashed radar booster.\r\n\r\n":
                        node.displayText = "레이더 부스터의 섬광 효과를 사용했습니다.\r\n\r\n";
                        break;
                    case "You selected the Challenge Moon save file. You can't route to another moon during the challenge.":
                        node.displayText = "챌린지 위성 저장 파일을 선택했습니다. 챌린지 도중에는 다른 위성으로 이동할 수 없습니다.";
                        break;
                }
            }
        }


        static void TranslateKeyword(TerminalNodesList terminalNodes)
        {
            foreach (TerminalKeyword keyword in terminalNodes.allKeywords)
            {
                switch (keyword.word)
                {
                    case "buy":
                        foreach (CompatibleNoun noun in keyword.compatibleNouns)
                        {
                            switch (noun.result.name)
                            {
                                case "buyProFlashlight1":
                                    noun.result.displayText = "프로 손전등을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 프로 손전등을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    noun.result.terminalOptions[1].result.displayText = "주문을 취소했습니다.\r\n";
                                    break;
                                case "buyFlash":
                                    noun.result.displayText = "손전등을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 손전등을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyLockpickers":
                                    noun.result.displayText = "자물쇠 따개를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 자물쇠 따개를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyBoombox":
                                    noun.result.displayText = "붐박스를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 붐박스를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyExtensLadder":
                                    noun.result.displayText = "연장형 사다리를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 연장형 사다리를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyJetpack":
                                    noun.result.displayText = "제트팩을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 제트팩을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyRadarBooster":
                                    noun.result.displayText = "레이더 부스터를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 레이더 부스터를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyShovel":
                                    noun.result.displayText = "철제 삽을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 철제 삽을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buySpraypaint":
                                    noun.result.displayText = "스프레이 페인트를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 스프레이 페인트를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyStunGrenade":
                                    noun.result.displayText = "기절 수류탄을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 기절 수류탄을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyTZP":
                                    noun.result.displayText = "TZP-흡입제를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 TZP-흡입제를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyWalkieTalkie":
                                    noun.result.displayText = "무전기를 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 무전기를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "buyZapGun":
                                    noun.result.displayText = "잽건을 주문하려고 합니다. 수량: [variableAmount]. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 잽건을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\\n\\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\\n\\n\r\n";
                                    break;
                                case "CozyLightsBuy1":
                                    noun.result.displayText = "아늑한 조명을 주문하려고 합니다. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "아늑한 조명을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\r\n전등 스위치를 사용해 아늑한 조명을 활성화하세요.\r\n\r\n";
                                    break;
                                case "GreenSuitBuy1":
                                    noun.result.displayText = "초록색 슈트를 주문하려고 합니다. \r\n아이템의 총 가격: [totalCost].\r\n\r\nCONFIRM 또는 DENY를 입력해주세요.\r\n\r\n";
                                    noun.result.terminalOptions[0].result.displayText = "초록색 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\r\n\r\n";
                                    break;
                            }
                        }
                        break;
                    case "help":
                        keyword.specialKeywordResult.displayText = ">MOONS\n항로를 지정할 위성 목록을 봅니다.\n\n>STORE\n회사 상점의 유용한 아이템 목록을 봅니다.\n\n>BESTIARY\n기록된 생명체 목록을 봅니다.\n\n>STORAGE\n저장소에 있는 물체에 접근합니다.\n\n>OTHER\n기타 명령어를 봅니다.\n\n[numberOfItemsOnRoute]\n";
                        break;
                    case "moons":
                        keyword.specialKeywordResult.displayText = "위성 카탈로그에 오신 것을 환영합니다.\n함선의 경로를 지정하려면 ROUTE를 입력하세요.\n위성에 대해 알아보려면 INFO를 입력하세요.\n____________________________\n\n* 회사 건물   //   [companyBuyingPercent]에 매입 중.\n\n* 익스페리멘테이션 [planetTime]\n* 어슈어런스 [planetTime]\n* 보우 [planetTime]\n\n* 오펜스 [planetTime]\n* 머치 [planetTime]\n* 애더먼스 [planetTime]\n\n* 렌드 [planetTime]\n* 다인 [planetTime]\n* 타이탄 [planetTime]\n\n";
                        break;
                    case "store":
                        keyword.specialKeywordResult.displayText = "회사 상점에 오신 것을 환영합니다. \n아이템 이름 앞에 BUY와 INFO를 입력해보세요. \n숫자를 입력하여 도구를 여러 개 주문할 수 있습니다.\n____________________________\n\n[buyableItemsList]\n\n함선 강화:\n* 시끄러운 경적    //    가격: $100\n* 신호 해석기    //    가격: $255\n* 순간이동기    //    가격: $375\n* 역방향 순간이동기    //    가격: $425\n\n함선 장식 목록은 할당량별로 순환됩니다. 다음 주에 꼭 다시 확인해보세요:\n------------------------------\n[unlockablesSelectionList]\n\n";
                        break;
                    case "sigurd":
                        keyword.specialKeywordResult.displayText = "시구르드의 일지 기록\\n\\n일지를 읽으려면, 이름 앞에 \"VIEW\"를 입력하세요.\\n---------------------------------\\n\\n[currentUnlockedLogsList]\\n\\n\\n\r\n";
                        break;
                    case "bestiary":
                        keyword.specialKeywordResult.displayText = "생명체 도감\\n\\n생명체 파일에 접근하려면, 이름 뒤에 \"INFO\"를 입력하세요.\\n---------------------------------\\n\\n[currentScannedEnemiesList]\\n\\n\\n";
                        break;
                    case "pro flashlight":
                        keyword.word = "프로";
                        break;
                    case "flashlight":
                        keyword.word = "손전등";
                        break;
                    case "lockpicker":
                        keyword.word = "자물쇠";
                        break;
                    case "shovel":
                        keyword.word = "철제";
                        break;
                    case "jetpack":
                        keyword.word = "제트팩";
                        break;
                    case "boombox":
                        keyword.word = "붐박스";
                        break;
                    case "stun":
                        keyword.word = "기절";
                        break;
                    case "vow":
                        keyword.word = "보우";
                        break;
                    case "experimentation":
                        keyword.word = "익스페리멘테이션";
                        break;
                    case "assurance":
                        keyword.word = "어슈어런스";
                        break;
                    case "offense":
                        keyword.word = "오펜스";
                        break;
                    case "adamance":
                        keyword.word = "애더먼스";
                        break;
                    case "television":
                        keyword.word = "텔레비전";
                        break;
                    case "teleporter":
                        keyword.word = "순간이동기";
                        break;
                    case "rend":
                        keyword.word = "렌드";
                        break;
                    case "march":
                        keyword.word = "머치";
                        break;
                    case "dine":
                        keyword.word = "다인";
                        break;
                    case "titan":
                        keyword.word = "타이탄";
                        break;
                    case "artifice":
                        keyword.word = "아터피스";
                        break;
                    case "embrion":
                        keyword.word = "엠브리언";
                        break;
                    case "company":
                        keyword.word = "회사";
                        break;
                    case "walkie-talkie":
                        keyword.word = "무전기";
                        break;
                    case "spray paint":
                        keyword.word = "스프레이";
                        break;

                    //적
                    case "brackens":
                        keyword.word = "브래컨";
                        break;
                    case "forest keeper":
                        keyword.word = "숲지기";
                        break;
                    case "earth leviathan":
                        keyword.word = "육지";
                        break;
                    case "lasso":
                        keyword.word = "올가미";
                        break;
                    case "spore lizards":
                        keyword.word = "포자";
                        break;
                    case "snare fleas":
                        keyword.word = "올무";
                        break;
                    case "eyeless dogs":
                        keyword.word = "눈없는";
                        break;
                    case "hoarding bugs":
                        keyword.word = "비축";
                        break;
                    case "bunker spiders":
                        keyword.word = "벙커";
                        break;
                    case "hygroderes":
                        keyword.word = "하이그로디어";
                        break;
                    case "coil-heads":
                        keyword.word = "코일";
                        break;
                    case "manticoils":
                        keyword.word = "만티코일";
                        break;
                    case "baboon hawks":
                        keyword.word = "개코매";
                        break;
                    case "nutcracker":
                        keyword.word = "호두까기";
                        break;
                    case "old birds":
                        keyword.word = "올드";
                        break;
                    case "butler":
                        keyword.word = "집사";
                        break;
                    case "circuit bees":
                        keyword.word = "회로";
                        break;
                    case "locusts":
                        keyword.word = "배회";
                        break;
                    case "thumpers":
                        keyword.word = "덤퍼";
                        break;
                    case "jester":
                        keyword.word = "광대";
                        break;


                    case "green suit":
                        keyword.word = "초록색";
                        break;
                    case "hazard suit":
                        keyword.word = "방호복";
                        break;
                    case "pajama suit":
                        keyword.word = "파자마";
                        break;
                    case "cozy lights":
                        keyword.word = "아늑한";
                        break;
                    case "signal":
                        keyword.word = "신호";
                        break;
                    case "toilet":
                        keyword.word = "변기";
                        break;
                    case "record":
                        keyword.word = "레코드";
                        break;
                    case "shower":
                        keyword.word = "샤워";
                        break;
                    case "table":
                        keyword.word = "테이블";
                        break;
                    case "romantic table":
                        keyword.word = "로맨틱한";
                        break;
                    case "file cabinet":
                        keyword.word = "파일";
                        break;
                    case "cupboard":
                        keyword.word = "수납장";
                        break;
                    case "bunkbeds":
                        keyword.word = "벙커침대";
                        break;

                    case "first":
                        keyword.word = "첫번째";
                        break;
                    case "smells":
                        keyword.word = "냄새";
                        break;
                    case "swing":
                        keyword.word = "상황의";
                        break;
                    case "shady":
                        keyword.word = "수상한";
                        break;
                    case "sound":
                        keyword.word = "너머의";
                        break;
                    case "goodbye":
                        keyword.word = "작별";
                        break;
                    case "screams":
                        keyword.word = "비명";
                        break;
                    case "golden":
                        keyword.word = "황금빛";
                        break;
                    case "idea":
                        keyword.word = "아이디어";
                        break;
                    case "nonsense":
                        keyword.word = "헛소리";
                        break;
                    case "hiding":
                        keyword.word = "숨어";
                        break;
                    case "desmond":
                        keyword.word = "데스몬드";
                        break;
                    case "real":
                        keyword.word = "진짜";
                        break;


                    case "zap gun":
                        keyword.word = "잽건";
                        break;
                    case "loud horn":
                        keyword.word = "시끄러운";
                        break;
                    case "extension ladder":
                        keyword.word = "연장형";
                        break;
                    case "inverse teleporter":
                        keyword.word = "역방향";
                        break;
                    case "jackolantern":
                        keyword.word = "잭오랜턴";
                        break;
                    case "radar":
                        keyword.word = "레이더";
                        break;
                    case "welcome mat":
                        keyword.word = "웰컴";
                        break;
                    case "goldfish":
                        keyword.word = "금붕어";
                        break;
                    case "plushie pajama man":
                        keyword.word = "인형";
                        break;
                    case "purple suit":
                        keyword.word = "보라색 슈트";
                        break;
                    case "purple":
                        keyword.word = "보라색";
                        break;
                    case "bee":
                        keyword.word = "꿀벌";
                        break;
                    case "bunny":
                        keyword.word = "토끼";
                        break;
                    case "disco":
                        keyword.word = "디스코";
                        break;
                }
            }
        }
    }
}
