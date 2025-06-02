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
using System.Collections.Specialized;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        public static bool vehicleChecked;

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(Terminal __instance, ref TerminalNodesList ___terminalNodes, ref List<TerminalNode> ___enemyFiles,
            ref TMP_InputField ___screenText, ref string ___currentText)
        {
            Plugin.mls.LogInfo("클라이언트 버전: " + GameNetworkManager.Instance.gameVersionNum);
            TranslateKeyword(___terminalNodes, ___enemyFiles);
            TranslateNode(__instance);

        }

        static void TranslateVehicle(Terminal instance)
        {
            foreach (BuyableVehicle buyableVehicle in instance.buyableVehicles)
            {
                if (buyableVehicle.vehicleDisplayName == "Cruiser")
                {
                    buyableVehicle.vehicleDisplayName = "크루저";
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("BeginUsingTerminal")]
        private static void BeginUsingTerminal_Postfix()
        {
            if (StartOfRound.Instance.localPlayerUsingController)
            {
                HUDManager.Instance.ChangeControlTip(0, "터미널 나가기 : [Start]", true);
            }
            else
            {
                HUDManager.Instance.ChangeControlTip(0, "터미널 나가기 : [TAB]", true);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(Terminal __instance, ref TMP_InputField ___screenText, ref string ___currentText, ref int ___numberOfItemsInDropship)
        {
            if (!vehicleChecked)
            {
                if (GameNetworkManager.Instance.gameVersionNum >= 55)
                {
                    Plugin.mls.LogInfo("클라이언트 버전이 55 이상입니다");
                    TranslateVehicle(__instance);
                }else
                {
                    Plugin.mls.LogInfo("클라이언트 버전이 55 이하입니다");
                }
                foreach (UnlockableItem unlockableItem in StartOfRound.Instance.unlockablesList.unlockables)
                {
                    TranslateUnlockable(unlockableItem);
                }
                vehicleChecked = true;
            }
            if (Plugin.fullyKoreanMoons)
            {
                ___screenText.text = ___screenText.text.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"도움말\"을 입력하세요.");
                ___currentText = ___currentText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"도움말\"을 입력하세요.");
            }
            else
            {
                ___screenText.text = ___screenText.text.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"Help\"를 입력하세요.");
                ___currentText = ___currentText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"Help\"를 입력하세요.");
            }
            //Startnode
            ___screenText.text = ___screenText.text.Replace("Welcome to the FORTUNE-9 OS", "FORTUNE-9 OS에 오신 것을 환영합니다");
            ___currentText = ___currentText.Replace("Welcome to the FORTUNE-9 OS", "FORTUNE-9 OS에 오신 것을 환영합니다");

            ___screenText.text = ___screenText.text.Replace("Courtesy of the Company", "회사 제공");
            ___currentText = ___currentText.Replace("Courtesy of the Company", "회사 제공");

            ___screenText.text = ___screenText.text.Replace("Monday.", "월요일 되세요.");
            ___currentText = ___currentText.Replace("Monday.", "월요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Tuesday.", "화요일 되세요.");
            ___currentText = ___currentText.Replace("Tuesday.", "화요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Wednesday.", "수요일 되세요.");
            ___currentText = ___currentText.Replace("Wednesday.", "수요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Thursday.", "목요일 되세요.");
            ___currentText = ___currentText.Replace("Thursday.", "목요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Friday.", "금요일 되세요.");
            ___currentText = ___currentText.Replace("Friday.", "금요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Saturday.", "토요일 되세요.");
            ___currentText = ___currentText.Replace("Saturday.", "토요일 되세요.");
            ___screenText.text = ___screenText.text.Replace("Sunday.", "일요일 되세요.");
            ___currentText = ___currentText.Replace("Sunday.", "일요일 되세요.");
                    
            ___screenText.text = ___screenText.text.Replace("Happy ", "좋은 ");
            ___currentText = ___currentText.Replace("Happy ", "좋은 ");
            //___screenText.text = ___screenText.text.Replace("Happy [currentDay].", "좋은 [currentDay] 되세요.");

            //LLL 바닐라 행성
            ___screenText.text = ___screenText.text.Replace("Experimentation", "익스페리멘테이션");
            ___currentText = ___currentText.Replace("Experimentation", "익스페리멘테이션");

            ___screenText.text = ___screenText.text.Replace("Assurance", "어슈어런스");
            ___currentText = ___currentText.Replace("Assurance", "어슈어런스");

            ___screenText.text = ___screenText.text.Replace("Vow", "보우");
            ___currentText = ___currentText.Replace("Vow", "보우");

            ___screenText.text = ___screenText.text.Replace("March", "머치");
            ___currentText = ___currentText.Replace("March", "머치");

            ___screenText.text = ___screenText.text.Replace("Offense", "오펜스");
            ___currentText = ___currentText.Replace("Offense", "오펜스");

            ___screenText.text = ___screenText.text.Replace("Adamance", "애더먼스");
            ___currentText = ___currentText.Replace("Adamance", "애더먼스");

            ___screenText.text = ___screenText.text.Replace("Rend", "렌드");
            ___currentText = ___currentText.Replace("Rend", "렌드");

            ___screenText.text = ___screenText.text.Replace("Dine", "다인");
            ___currentText = ___currentText.Replace("Dine", "다인");

            ___screenText.text = ___screenText.text.Replace("Titan", "타이탄");
            ___currentText = ___currentText.Replace("Titan", "타이탄");

            ___screenText.text = ___screenText.text.Replace("Artifice", "아터피스");
            ___currentText = ___currentText.Replace("Artifice", "아터피스");

            ___screenText.text = ___screenText.text.Replace("Embrion", "엠브리언");
            ___currentText = ___currentText.Replace("Embrion", "엠브리언");

            //LLL 모드 행성
            if (Plugin.translateModdedContent)
            {
                ___screenText.text = ___screenText.text.Replace("Asteroid", "아스테로이드");
                ___currentText = ___currentText.Replace("Asteroid", "아스테로이드");

                ___screenText.text = ___screenText.text.Replace("Atlantica", "아틀란티카");
                ___currentText = ___currentText.Replace("Atlantica", "아틀란티카");

                ___screenText.text = ___screenText.text.Replace("Cosmocos", "코스모코스");
                ___currentText = ___currentText.Replace("Cosmocos", "코스모코스");

                ___screenText.text = ___screenText.text.Replace("Desolation", "디솔레이션");
                ___currentText = ___currentText.Replace("Desolation", "디솔레이션");

                ___screenText.text = ___screenText.text.Replace("Etern", "이턴");
                ___currentText = ___currentText.Replace("Etern", "이턴");

                ___screenText.text = ___screenText.text.Replace("Fission", "피션");
                ___currentText = ___currentText.Replace("Fission", "피션");

                ___screenText.text = ___screenText.text.Replace("Gloom", "글룸");
                ___currentText = ___currentText.Replace("Gloom", "글룸");

                ___screenText.text = ___screenText.text.Replace("Gratar", "그라타");
                ___currentText = ___currentText.Replace("Gratar", "그라타");

                ___screenText.text = ___screenText.text.Replace("Infernis", "인퍼니스");
                ___currentText = ___currentText.Replace("Infernis", "인퍼니스");

                ___screenText.text = ___screenText.text.Replace("Junic", "주닉");
                ___currentText = ___currentText.Replace("Junic", "주닉");

                ___screenText.text = ___screenText.text.Replace("Oldred", "올드레드");
                ___currentText = ___currentText.Replace("Oldred", "올드레드");

                ___screenText.text = ___screenText.text.Replace("Polarus", "폴라러스");
                ___currentText = ___currentText.Replace("Polarus", "폴라러스");

                ___screenText.text = ___screenText.text.Replace("Acidir", "어시디어");
                ___currentText = ___currentText.Replace("Acidir", "어시디어");
            }


            //날씨
            ___screenText.text = ___screenText.text.Replace("mild weather", "맑음");
            ___currentText = ___currentText.Replace("mild weather", "맑음");

            ___screenText.text = ___screenText.text.Replace("Rainy", "우천");
            ___screenText.text = ___screenText.text.Replace("rainy", "우천");
            ___currentText = ___currentText.Replace("Rainy", "우천");
            ___currentText = ___currentText.Replace("rainy", "우천");

            ___screenText.text = ___screenText.text.Replace("Foggy", "안개");
            ___screenText.text = ___screenText.text.Replace("foggy", "안개");
            ___currentText = ___currentText.Replace("Foggy", "안개");
            ___currentText = ___currentText.Replace("foggy", "안개");

            ___screenText.text = ___screenText.text.Replace("Flooded", "홍수");
            ___screenText.text = ___screenText.text.Replace("flooded", "홍수");
            ___currentText = ___currentText.Replace("Flooded", "홍수");
            ___currentText = ___currentText.Replace("flooded", "홍수");

            ___screenText.text = ___screenText.text.Replace("Stormy", "뇌우");
            ___screenText.text = ___screenText.text.Replace("stormy", "뇌우");
            ___currentText = ___currentText.Replace("Stormy", "뇌우");
            ___currentText = ___currentText.Replace("stormy", "뇌우");

            ___screenText.text = ___screenText.text.Replace("Eclipsed", "일식");
            ___screenText.text = ___screenText.text.Replace("eclipsed", "일식");
            ___currentText = ___currentText.Replace("Eclipsed", "일식");
            ___currentText = ___currentText.Replace("eclipsed", "일식");

            ___screenText.text = ___screenText.text.Replace("OFF!", "세일!");
            ___currentText = ___currentText.Replace("OFF!", "세일!");

            ___screenText.text = ___screenText.text.Replace("(NEW)", "(신규)");
            ___currentText = ___currentText.Replace("(NEW)", "(신규)");

            ___screenText.text = ___screenText.text.Replace("Price:", "가격:");
            ___currentText = ___currentText.Replace("Price:", "가격:");

            ___screenText.text = ___screenText.text.Replace("[No items available]", "[사용 가능한 아이템이 없음]");
            ___currentText = ___currentText.Replace("[No items available]", "[사용 가능한 아이템이 없음]");

            ___screenText.text = ___screenText.text.Replace("[No items stored. While moving an object with B, press X to store it.]", "[보관된 아이템이 없습니다. B를 사용하여 개체를 이동하는 동안 X를 눌러 보관합니다.]");
            ___currentText = ___currentText.Replace("[No items stored. While moving an object with B, press X to store it.]", "[보관된 아이템이 없습니다. B를 사용하여 개체를 이동하는 동안 X를 눌러 보관합니다.]");

            ___screenText.text = ___screenText.text.Replace("[ALL DATA HAS BEEN CORRUPTED OR OVERWRITTEN]", "[모든 데이터가 손상되거나 덮어쓰기되었습니다]");
            ___currentText = ___currentText.Replace("[ALL DATA HAS BEEN CORRUPTED OR OVERWRITTEN]", "[모든 데이터가 손상되거나 덮어쓰기되었습니다]");


            if (___screenText.text.Contains("numberOfItemsOnRoute2"))
            {
                if (___numberOfItemsInDropship != 0)
                {
                    ___screenText.text = ___screenText.text.Replace("[numberOfItemsOnRoute2]", "항로에서 " + ___numberOfItemsInDropship + "개의 아이템을 구매했습니다.");
                    ___currentText = ___currentText.Replace("[numberOfItemsOnRoute2]", "항로에서 " + ___numberOfItemsInDropship + "개의 아이템을 구매했습니다.");
                }else
                {
                    ___screenText.text = ___screenText.text.Replace("[numberOfItemsOnRoute2]", "");
                    ___currentText = ___currentText.Replace("[numberOfItemsOnRoute2]", "");
                }
            }

            if (___screenText.text.Contains("objects outside the ship, totalling at an approximate value"))
            {
                System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 91);
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
                for (int num5 = 0; num5 < array.Length; num5++)
                {
                    if (array[num5].itemProperties.isScrap && !array[num5].isInShipRoom && !array[num5].isInElevator)
                    {
                        num4 += array[num5].itemProperties.maxValue - array[num5].itemProperties.minValue;
                        num3 += Mathf.Clamp(random.Next(array[num5].itemProperties.minValue, array[num5].itemProperties.maxValue), array[num5].scrapValue - 6 * num5, array[num5].scrapValue + 9 * num5);
                        num2++;
                    }
                }
                ___screenText.text = $"\n\n함선 외부에 {num2}개의 물체가 있으며, 총 ${num3}의 가치를 가지고 있습니다.\n\n";
                ___currentText = $"\n\n함선 외부에 {num2}개의 물체가 있으며, 총 ${num3}의 가치를 가지고 있습니다.\n\n";
            }
        }

        static void TranslateTerminal(TMP_InputField screenText, string currentText, string oldValue, string newVaule)
        {
            screenText.text = screenText.text.Replace(oldValue, newVaule);
            currentText = currentText.Replace(oldValue, newVaule);
        }

        private static string NewTextPostProcess(Terminal __instance, string modifiedDisplayText, TerminalNode node)
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
                            stringBuilder.Append(" (신규)");
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
                            stringBuilder3.Append(" (신규)");
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

        static void TranslateNode(Terminal instance)
        {
            foreach (TerminalNode node in instance.terminalNodes.specialNodes)
            {
                switch (node.name)
                {
                    case "Start":
                        node.displayText.Replace("Welcome to the FORTUNE-9 OS", "FORTUNE-9 OS에 오신 것을 환영합니다");
                        node.displayText.Replace("Courtesy of the Company", "회사 제공");
                        node.displayText.Replace("Happy [currentDay].", "좋은 [currentDay] 되세요.");
                        if (Plugin.fullyKoreanMoons)
                        {
                            node.displayText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"도움말\"을 입력하세요.");
                        }
                        else
                        {
                            node.displayText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"Help\"를 입력하세요.");
                        }
                        break;
                    case "StartFirstTime":
                        node.displayText.Replace("Welcome to the FORTUNE-9 OS", "FORTUNE-9 OS에 오신 것을 환영합니다");
                        node.displayText.Replace("Courtesy of the Company", "회사 제공");
                        if (Plugin.fullyKoreanMoons)
                        {
                            node.displayText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"도움말\"을 입력하세요.");
                        }else
                        {
                            node.displayText.Replace("Type \"Help\" for a list of commands.", "명령 목록을 보려면 \"Help\"를 입력하세요.");
                        }
                        break;
                    case "ParserError1":
                        node.displayText = "[이 단어에 제공되는 작업이 없습니다.]\n\n";
                        break;
                    case "ParserError2":
                        node.displayText = "[이 작업과 함께 제공된 개체가 없거나, 단어가 잘못 입력되었거나 존재하지 않습니다.]\n\n";
                        break;
                    case "ParserError3":
                        node.displayText = "[이 작업은 이 개체와 맞지 않습니다.]\n\n";
                        break;

                    case "CannotAfford":
                        node.displayText = "자금이 충분하지 않습니다!\n당신의 소지금은 [playerCredits]이지만 이 아이템의 총 가격은 [totalCost]입니다.\n\n";
                        break;
                    case "CantBuyVehicleYet":
                        node.displayText = "수송선이 현재 물품을 배송하고 있습니다. 차량을 구매하려면 수송선이 비어 있어야 합니다.";
                        break;
                    case "BusyDeliveringVehicle":
                        node.displayText = "수송선이 컴퍼니 크루저를 수송 중입니다. 차량이 도착할 때까지 아이템을 구매할 수 없습니다.\n";
                        break;
                }

                if (node.displayText.Contains("[DATA CORRUPTED OR OVERWRITTEN]"))
                {
                    node.displayText = node.displayText.Replace("[DATA CORRUPTED OR OVERWRITTEN]", "[데이터가 손상되거나 덮어쓰기됨]");
                }

                switch (node.displayText)
                {
                    case "BG IG, A System-Act Ally\nCopyright (C) 2084-2108, Halden Electronics Inc.\nCourtesy of the Company.\n\n\n\nBios for FORTUNE-9 87.7/10MHZ SYSTEM\n\nCurrent date is Tue  3-7-2532\nCurrent time is 8:03:32.15\n\nPlease enter favorite animal: ":
                        node.displayText = "BG IG, 시스템 행동 연합\nCopyright (C) 2084-2108, Halden Electronics Inc.\n회사 제공.\n\n\n\nFORTUNE-9 전용 바이오스 87.7/10MHZ 시스템\n\n현재 날짜는 2532년 3월 7일 화요일입니다\n현재 시간은 8:03:32.15입니다.\n\n좋아하는 동물을 입력하세요: ";
                        break;
                    case "You could not afford these items!\nYour balance is [playerCredits]. Total cost of these items is [totalCost].\n\n":
                        node.displayText = "자금이 충분하지 않습니다!\n당신의 소지금은 [playerCredits]이지만 이 아이템의 총 가격은 [totalCost]입니다.\n\n";
                        break;
                    case "Unable to route the ship currently. It must be in orbit around a moon to route the autopilot.\nUse the main lever at the front desk to enter orbit.\n\n\n\n":
                        node.displayText = "함선의 항로를 지정할 수 없습니다. 항로를 지정하려면 이륙한 상태여야 합니다.\n궤도에 들어가려면 프런트 데스크에 있는 메인 레버를 사용하세요.\n\n\n\n";
                        break;
                    case "The delivery vehicle cannot hold more than 12 items\nat a time. Please pick up your items when they land!\n\n\n":
                        node.displayText = "수송선은 최대 12개의 아이템만을 적재할 수 있습니다\n착륙하면 아이템을 회수해주세요!\n\n\n";
                        break;
                    case "An error occured! Try again.\n\n":
                        node.displayText = "오류가 발생했습니다! 다시 시도하세요.\n\n";
                        break;
                    case "No data has been collected on this creature. \nA scan is required.\n\n":
                        node.displayText = "이 생명체에 대해 수집된 데이터가 없습니다. \n스캔이 필요합니다.\n\n";
                        break;
                    case "To purchase decorations, the ship cannot be landed.\n\n":
                        node.displayText = "가구를 구매하려면 함선이 완전히 이착륙할 때까지 기다리세요.\n\n";
                        break;
                    case "The autopilot ship is already orbiting this moon!":
                        node.displayText = "이미 이 위성의 궤도에 있습니다!";
                        break;
                    case "This has already been unlocked for your ship!":
                        node.displayText = "이미 잠금이 해제된 아이템입니다!";
                        break;
                    case "The ship cannot be leaving or landing!\n\n":
                        node.displayText = "함선이 완전히 이착륙할 때까지 기다리세요!\n\n";
                        break;
                    case "This item is not in stock!\n\n":
                        node.displayText = "이 아이템은 재고가 없습니다!\n\n";
                        break;
                    case "Returned the item from storage!":
                        node.displayText = "아이템을 저장고에서 꺼냈습니다!\n\n";
                        break;
                    case "Entered broadcast code.\n\n":
                        node.displayText = "송출 코드를 입력했습니다.\n\n";
                        break;
                    case "Switched radar to player.\n\n":
                        node.displayText = "레이더를 플레이어로 전환했습니다.\n\n";
                        break;
                    case "Pinged radar booster.\n\n":
                        node.displayText = "레이더 부스터를 핑했습니다.\n\n";
                        break;
                    case "Sent transmission.\n\n":
                        node.displayText = "전송했습니다.\n\n";
                        break;
                    case "Flashed radar booster.\n\n":
                        node.displayText = "레이더 부스터의 섬광 효과를 사용했습니다.\n\n";
                        break;
                    case "You selected the Challenge Moon save file. You can't route to another moon during the challenge.":
                        node.displayText = "챌린지 위성 저장 파일을 선택했습니다. 챌린지 도중에는 다른 위성으로 이동할 수 없습니다.";
                        break;
                    case "For the safety of your crew, the Company only allows one Cruiser to be in operation at any given time, but a Cruiser has been detected.\n\n":
                        node.displayText = "팀원의 안전을 위해 회사는 한 번에 한 대의 크루저만 운행할 수 있도록 허용하고 있지만, 현재 구역에서 크루저가 감지되었습니다.\n\n";
                        break;
                    case "The delivery pod is currently filled with items en route; you cannot purchase a vehicle until it is empty.":
                        node.displayText = "수송선이 현재 물품을 배송하고 있습니다. 차량을 구매하려면 수송선이 비어 있어야 합니다.";
                        break;
                    case "The delivery pod is busy transporting your Company Cruiser. You cannot purchase items until it arrives.\n":
                        node.displayText = "수송선이 컴퍼니 크루저를 수송 중입니다. 차량이 도착할 때까지 아이템을 구매할 수 없습니다.\n";
                        break;
                }
            }
        }

        static void TranslateUnlockable(UnlockableItem unlockableItem)
        {
            TerminalNode terminalNodes = unlockableItem.shopSelectionNode;
            switch (unlockableItem.unlockableName)
            {
                case "Bunkbeds":
                    unlockableItem.unlockableName = "벙커침대";
                    terminalNodes.displayText = "벙커침대\n\n";
                    terminalNodes.creatureName = "벙커침대";
                    break;
                case "File Cabinet":
                    unlockableItem.unlockableName = "파일 캐비닛";
                    terminalNodes.displayText = "파일 캐비닛\n\n";
                    terminalNodes.creatureName = "파일 캐비닛";
                    break;
                case "Cupboard":
                    unlockableItem.unlockableName = "수납장";
                    terminalNodes.displayText = "수납장\n\n";
                    terminalNodes.creatureName = "수납장";
                    break;
                case "Teleporter":
                    unlockableItem.unlockableName = "순간이동기";
                    terminalNodes.displayText = "순간이동기\n\n";
                    terminalNodes.creatureName = "순간이동기";
                    break;
                case "Television":
                    unlockableItem.unlockableName = "텔레비전";
                    terminalNodes.displayText = "텔레비전\n\n";
                    terminalNodes.creatureName = "텔레비전";
                    break;
                case "Toilet":
                    unlockableItem.unlockableName = "변기";
                    terminalNodes.displayText = "변기\n\n";
                    terminalNodes.creatureName = "변기";
                    break;
                case "Shower":
                    unlockableItem.unlockableName = "샤워 부스";
                    terminalNodes.displayText = "샤워 부스\n\n";
                    terminalNodes.creatureName = "샤워 부스";
                    break;
                case "Record player":
                    unlockableItem.unlockableName = "레코드 플레이어";
                    terminalNodes.displayText = "레코드 플레이어\n\n";
                    terminalNodes.creatureName = "레코드";
                    break;
                case "Table":
                    unlockableItem.unlockableName = "테이블";
                    terminalNodes.displayText = "테이블\n\n";
                    terminalNodes.creatureName = "테이블";
                    break;
                case "Romantic table":
                    unlockableItem.unlockableName = "로맨틱한 테이블";
                    terminalNodes.displayText = "로맨틱한 테이블\n\n";
                    terminalNodes.creatureName = "로맨틱한 테이블";
                    break;
                case "Sofa chair":
                    unlockableItem.unlockableName = "소파 의자";
                    terminalNodes.displayText = "소파 의자\n\n";
                    terminalNodes.creatureName = "소파 의자";
                    break;
                case "Microwave":
                    unlockableItem.unlockableName = "전자레인지";
                    terminalNodes.displayText = "전자레인지\n\n";
                    terminalNodes.creatureName = "전자레인지";
                    break;
                case "Fridge":
                    unlockableItem.unlockableName = "냉장고";
                    terminalNodes.displayText = "냉장고\n\n";
                    terminalNodes.creatureName = "냉장고";
                    break;
                case "Signal translator":
                    unlockableItem.unlockableName = "신호 해석기";
                    terminalNodes.displayText = "신호 해석기\n\n";
                    terminalNodes.creatureName = "신호 해석기";
                    break;
                case "Loud horn":
                    unlockableItem.unlockableName = "시끄러운 경적";
                    terminalNodes.displayText = "시끄러운 경적\n\n";
                    terminalNodes.creatureName = "시끄러운 경적";
                    break;
                case "Inverse Teleporter":
                    unlockableItem.unlockableName = "역방향 순간이동기";
                    terminalNodes.displayText = "역방향 순간이동기\n\n";
                    terminalNodes.creatureName = "역방향 순간이동기";
                    break;
                case "JackOLantern":
                    unlockableItem.unlockableName = "잭오랜턴";
                    terminalNodes.displayText = "잭오랜턴\n\n";
                    terminalNodes.creatureName = "잭오랜턴";
                    break;
                case "Welcome mat":
                    unlockableItem.unlockableName = "웰컴 매트";
                    terminalNodes.displayText = "웰컴 매트\n\n";
                    terminalNodes.creatureName = "웰컴 매트";
                    break;
                case "Goldfish":
                    unlockableItem.unlockableName = "금붕어";
                    terminalNodes.displayText = "금붕어\n\n";
                    terminalNodes.creatureName = "금붕어";
                    break;
                case "Plushie pajama man":
                    unlockableItem.unlockableName = "인형 파자마 맨";
                    terminalNodes.displayText = "인형 파자마 맨\n\n";
                    terminalNodes.creatureName = "인형 파자마 맨";
                    break;
                case "Disco Ball":
                    unlockableItem.unlockableName = "디스코 볼";
                    terminalNodes.displayText = "디스코 볼\n\n";
                    terminalNodes.creatureName = "디스코 볼";
                    break;
            }
        }


        static void TranslateKeyword(TerminalNodesList terminalNodes, List<TerminalNode> ___enemyFiles)
        {
            foreach (TerminalNode node in ___enemyFiles)
            {
                switch (node.name)
                {
                    case "SnarefleaFile":
                        node.displayText = "올무 벼룩\n\n시거드의 위험도: 30%\n\n학명: Dolus-scolopendra \n순각강에 속하는 매우 큰 절지동물입니다. 그들은 몸에서 은폐된 장소로 이동하는 데 사용하는 실크를 만듭니다. 외골격은 다소 약해서 오래 떨어지면 죽을 수 있습니다. 올무 벼룩은 독을 생산하지도 않고 강하게 물지도 못합니다. 대신 큰 먹잇감을 조여 질식시키는 능력으로 이러한 약점을 보완합니다.\n\n올무 벼룩은 어둡고 따뜻한 곳에서 잘 자랍니다. 저온에서는 살아남지 못하며 일반적으로 야외와 햇빛을 피합니다. 밖으로 데리고 나가거나 때려눕히세요!; 걔네 내장으로 좋은 밀크쉐이크를 만들 수 있을 것 같아,,,\n\n";
                        node.creatureName = "올무 벼룩";
                        break;
                    case "BrackenFile":
                        node.displayText = "브래컨 -- 일명 플라워 맨!\n\n그건 플라워 맨이었어! 변명하지 마! 난 걔 시체라도 찾고 싶었다고! 겁쟁이 같은 녀석들!\n\n학명: Rapax-Folium\n브래컨의 생물학적 분류군에 대한 논쟁이 있습니다. 브래컨은 붉은 사탕무에 가까운 피부색과 질감을 가진 이족보행형 척추동물입니다. 브래컨(고사리)이라는 이름은 위쪽 척추에서 솟아난 부위가 잎처럼 생겼기 때문에 붙여진 이름입니다. 이 부위는 적을 위협하기 위한 용도로 추정되지만, 브래컨의 개체 수가 적으며 발견하기도 어렵기 때문에 구체적인 행동에 대해서는 알려진 바가 많지 않습니다.\n\n우리는 그것을 접한 생물 전문가들의 이야기를 통해 약간의 정보를 알고 있습니다. 그는 매우 높은 지능을 가진 고독한 사냥꾼입니다. 브래컨의 행동은 냉담해 보일 수 있으며, 이유 없는 행동에도 높은 공격성을 보이나 직접 마주치면 재빨리 도망갑니다. 하지만 브래컨은 궁지에 몰리거나 오랫동안 지켜보면 적대감을 드러내는 것으로 알려져 있습니다. 따라서 브래컨을 주의 깊게 살펴보되 오래 쳐다보지 않는 것이 좋습니다. 생포되거나 죽은 상태의 표본을 채집한 사례는 없습니다. 다른 대형 동물과 달리 시체가 빠르게 분해되는 것으로 알려져 있습니다.\n\n";
                        node.creatureName = "브래컨";
                        break;
                    case "ThumperFile":
                        if (Plugin.thumperTranslation)
                        {
                            node.displayText = "썸퍼\n\n시거드의 위험도: 90%\n\n학명: Pistris-saevus\n해프(Halves) 또는 썸퍼는 연골어강에 속하는 매우 공격적인 육식성 어종입니다. 골격이 연골로 이루어져 있어 신축성있고 고무처럼 부드러운 신체를 가지고 있습니다. Halves라는 이름은 부화한 알의 껍질을 벗어나기 위해 뒷다리를 뜯어먹어야 하기 때문에 붙혀진 이름입니다. 팔, 즉 앞다리는 매우 강해서 먹잇감을 짓밟을 때에도 사용합니다. 직선 지형에서 빠른 속도를 낼 수 있으며, 보통 먹이사슬의 최상위에 있는 끈질긴 사냥꾼입니다.\n\n가장 큰 약점은 지능이 낮고 청각이 전혀 없다는 것입니다. 모퉁이를 돌 때 속도가 느리고 먹이를 쉽게 추적할 수 없기 때문에 덤퍼와 마주치면 시야에서 벗어나는 것이 최선의 생존 수단입니다.\n\n이 종의 빠르고 불안정한 진화 때문에 일부 학자들은 썸퍼가 엉겅퀴 성운 주변 행성에서 종 분화율이 높은 돌연변이의 증가를 보여주는 사례 중 하나라고 이론을 세웠습니다.\n\n";
                            node.creatureName = "썸퍼";
                        }
                        else
                        {
                            node.displayText = "덤퍼\n\n시거드의 위험도: 90%\n\n학명: Pistris-saevus\n해프(Halves) 또는 덤퍼는 연골어강에 속하는 매우 공격적인 육식성 어종입니다. 골격이 연골로 이루어져 있어 신축성있고 고무처럼 부드러운 신체를 가지고 있습니다. Halves라는 이름은 부화한 알의 껍질을 벗어나기 위해 뒷다리를 뜯어먹어야 하기 때문에 붙혀진 이름입니다. 팔, 즉 앞다리는 매우 강해서 먹잇감을 짓밟을 때에도 사용합니다. 직선 지형에서 빠른 속도를 낼 수 있으며, 보통 먹이사슬의 최상위에 있는 끈질긴 사냥꾼입니다.\n\n가장 큰 약점은 지능이 낮고 청각이 전혀 없다는 것입니다. 모퉁이를 돌 때 속도가 느리고 먹이를 쉽게 추적할 수 없기 때문에 덤퍼와 마주치면 시야에서 벗어나는 것이 최선의 생존 수단입니다.\n\n이 종의 빠르고 불안정한 진화 때문에 일부 학자들은 덤퍼가 엉겅퀴 성운 주변 행성에서 종 분화율이 높은 돌연변이의 증가를 보여주는 사례 중 하나라고 이론을 세웠습니다.\n\n";
                            node.creatureName = "덤퍼";
                        }
                        break;
                    case "EyelessDogFile":
                        node.displayText = "눈없는 개\n\n학명: Leo caecus\nSaeptivus강에 속하는 대형 포유류입니다. 사교적이며 크게 무리지어 사냥합니다. 알아들을 수 있는 소리와 큰 입 때문에 \"숨쉬는 사자\"라고도 불립니다. 눈없는 개는 지구력이 뛰어난 사냥꾼으로 퇴화된 시각을 청각으로 보완하려고 합니다. 종종 동족의 소리를 먹이로 착각해 무리 내에서 싸움에 뛰어든다는 속설이 있습니다.\n\n눈 없는 개의 행동은 다른 무리형 생물과 달리 광범위한 거리를 커버하기 위해 멀리 흩어지는 경향이 있습니다. 눈없는 개가 먹이를 발견하면 주변에 있는 다른 개들에게 경고를 보내기 위해 울부짖고, 이 개들도 경보를 울려 일종의 연쇄 반응이 일어나기도 합니다. 눈 없는 개는 무리를 지어 다니면 위험할 수 있습니다. 그러나 그들은 성급하게 판단하는 면도 있어 먹이의 위치를 추적하던 도중, 부정확한 판단으로 먹이를 놓치는 경우도 있습니다.\n\n";
                        node.creatureName = "눈없는 개";
                        break;
                    case "HoardingBugFile":
                        node.displayText = "비축 벌레\n\n시거드의 위험도: 0%\n\n학명: Linepithema-crassus\n비축 벌레(벌목)는 사회성이 강한 대형 곤충입니다. 혼자 생활하는 경우가 많지만, 같은 종의 개체들과 둥지를 공유하기도 합니다. 평균 키는 3피트 정도이며 둥근 모양의 몸체를 가지고 있습니다. 가벼운 체액과 혈액, 외골격 덕에 막으로 된 날개를 사용하여 비행할 수도 있습니다. 또한 몸통도 약간 투명합니다.\n\n비축 벌레라는 이름은 영역적인 성향 때문에 붙여진 이름입니다. 일단 한 장소를 둥지로 선택하면 주변의 물건을 이용해 둥지를 장식하고 둥지의 일부처럼 보호합니다. 비축 벌레는 혼자서는 큰 둥지에 있을 때만큼 위험하지 않습니다. 그러나 비축 벌레는 혼자 남겨두면 상당히 중립적이며 크게 위험하지 않습니다. ㅇ-우리는 멍청한 포옹 벌레를 사랑해.!! - 이ㅓ것은 불굴의 시거드의 메모다\n\n";
                        node.creatureName = "비축 벌레";
                        break;
                    case "HygrodereFile":
                        node.displayText = "하이그로디어\n\n시거드의 위험도: 0%, 만약 너가 달팽이보다 빠르기만 하다면야!\n\n학명: Hygrodere\n원생동물 문에 속하는 진핵 생물로, 수백만 마리가 번식합니다. 이 작은 유기체는 놀라운 번식 속도로 수백만 마리까지 증식할 수 있습니다. 하이그로디어는 분열하는 경우가 거의 없으며, 대신 크고 끈적끈적한 덩어리를 형성하여 많은 공간을 차지할 수 있고 다루기 위험하므로 대형 도구나 미끼를 사용하여 이동시켜야 합니다.\n\n하이그로디어는 열과 산소에 끌리기 때문에 어디에서나 열과 산소를 감지할 수 있습니다. 자신의 체질로 전환할 수 없는 유기물은 거의 없습니다. 이들을 독살할 수 있는 물질은 아직 발견되지 않았습니다. 끊임없이 스스로를 교체하기 때문에 수십만 년 동안 살아있을 수 있습니다. 하이그로디어는 기어오르는 데 어려움을 겪으니 궁지에 몰리면 높은 물체를 찾아 그 위에 올라서세요. 걔네는 음악 취향이 참 뛰어나. 어쩌다 보니 한 개체와 친구가 되기도 했는데, 내 음악 덕분인 것 같아.\n\n";
                        node.creatureName = "하이그로디어";
                        break;
                    case "ForestKeeperFile":
                        node.displayText = "숲지기\n\n시거드의 위험도: 50%\n\n학명: Satyrid-proceritas\nRapax-Folium과 공통 분류을 공유한 것으로 알려진 이 거대 괴수들은 주로 서식하는 생물군계에서 이름을 본따 숲지기라고 불립니다. 그들의 몸의 앞뒤에는 눈을 모방한 듯한 무늬가 있는데, 이러한 특징은 민첩하지 않은 숲지기의 특성 때문에 어린 개체들에게 매우 유용합니다. 숲지기의 피부는 독특하고 밀도가 높은 물질로 구성되며 그들의 일생 동안 점점 단단해지며, 몸 전체에 있는 커다란 가시와 돌기는 늙은 개체일수록 길고 많이 형성됩니다.\n\n숲지기는 5~6세의 인간 아이와 비슷하게 호기심이 많습니다. 흥미로워 보이는 것은 무엇이든 집어먹으려 합니다. 그들은 실제로 무언가를 먹을 필요가 없으며, 광합성과 유사한 과정으로 에너지를 얻는 것으로 알려져 있습니다. 그럼에도 불구하고, 이러한 행동 양상 때문에 숲지기를 관찰하는 것은 굉장한 위험이 따릅니다. 먼 거리도 볼 수 있기 때문에 낮은 자세를 유지하고 엄폐물을 활용하는 것이 권장됩니다. 좁은 공간에는 들어가지 못하며 일반적으로 파괴적인 성향이 아니므로 대피소나 돌출부에 가까이 피신하는 것이 좋습니다.\n\n";
                        node.creatureName = "숲지기";
                        break;
                    case "CoilHeadFile":
                        node.displayText = "코일 헤드\n\n시거드의 위험도: 80%\n\n학명: Vir colligerus\n비르 콜리게루스, 또는 구어체로 코일-헤드라고 명명된 이 생물은 위험하고 예측불허의 특성으로 인해 알려진 것이 많지 않습니다. 해부되거나 무력화될 때에는 연소되는 것으로 알려져 있으며, 위험할 정도로 높은 수준의 방사성 입자를 운반합니다. 여러 이유로 인해, 입증되지는 않았지만 생체 병기로 만들어졌을 가능성이 높습니다.\n\n코일-헤드의 시각적 외형은 스프링으로 머리가 연결된 피투성이 마네킹 같은 외형을 가지고 있습니다. 코일-헤드의 주된 행동 양상은 누군가 쳐다보면 정지하는 것입니다. 그러나 이것은 엄격한 규칙은 아닌 것 같습니다. 또한 시끄럽거나 밝은 빛에 노출될 경우, 긴 리셋 모드에 들어가는 것으로 보이기도 합니다. \n그냥 계속 쳐다보거나 기절 수류탄을 사용하도록! - 시구르드\n\n";
                        node.creatureName = "코일 헤드";
                        break;
                    case "LassoManFile":
                        node.displayText = "올가미 인간\n\n시거드의 위험도: 30%이긴 한데 좀 무섭네\n\n학명: \n\n\n";
                        node.creatureName = "올가미 인간";
                        break;
                    case "EarthLeviathanFile":
                        node.displayText = "육지 레비아탄\n\n시거드의 위험도: 2% 왜냐면 함선 카메라에서 숨을 수 없거든!!!\n\n학명: Hemibdella-gigantis\n피시콜리데아과에 속하는 경건한 이름의 육지 레비아탄은 엉겅퀴 성운 주변에서 발견되는 가장 거대한 무척추동물 중 하나입니다. 아직 포획된 개체는 전무하기 때문에 이들의 생태에 대해 알려진 바는 많지 않습니다. \n\n그들은 포식자처럼 행동하는 것으로 보입니다. 육지 레비아탄이 남긴 엄청난 땅굴의 규모로 보아 지하 40m까지 파고들 수 있는 것으로 추측됩니다. 그들은 아주 미세한 진동도 감지할 수 있기 때문에 근처에 있을 때 가만히 있는 것이 좋다고 하지만, 이는 잘못된 상식입니다. 땅을 파는 듯한 소리가 들리면 빠르게 발걸음을 되돌려야 합니다.\n\n";
                        node.creatureName = "육지 레비아탄";
                        break;
                    case "JesterFile":
                        node.displayText = "광대\n\n시거드의 위험도: 90% 저 녀석이 날뛰기 전에 튀어!! 저 녀석에게서 숨을 곳은 없어. 그냥 밖으로 도망가.\n\n학명: 미친 놈\n망할 과학적 기록 따윈 없어! 행운을 빌어, 너도 우리만큼이나 잘 알잖아. 우린 저걸 광대라고 불러\n\n";
                        node.creatureName = "광대";
                        break;
                    case "PufferFile":
                        node.displayText = "포자 도마뱀\n\n시거드의 위험도: 잘 모르겠어 아마도 5% 난 단지 이 통통한 다리가 싫을 뿐이야\n\n학명: Lacerta-glomerorum\n구어체로 Puffer 또는 포자 도마뱀이라고 불리는 (악어과에 속한) Lacerta-glomerorum은 가장 거대하고 무거운 파충류 중 하나입니다. 큰 입을 갖고 있지만 초식성이며 강하게 물지 않습니다. 꼬리에 있는 구근은 곰팡이 종인 Lycoperdon perlatum의 성장을 유인하고 촉진하는 화학 물질을 분비하는 것으로 알려져 있으며, 이 물질을 흔들어 포자를 방출하는 방어기제를 가지고 있어 독특한 상호 공생 관계의 예시라고 할 수 있습니다.\n\n포자 도마뱀은 매우 소심한 기질을 가지고 있어 싸움을 피하는 경향이 있습니다. 자신의 위협이 효과적이지 않다고 판단되면 공격을 시도할 수 있으므로 포자 도마뱀을 구석에 몰아넣거나 쫓아다니지 않는 것이 좋습니다. 포자 도마뱀은 수백 년 전에 적어도 부분적으로 가축화되었다는 역사적 기록이 있지만, 이러한 노력은 꼬리를 약용으로 채취하려는 계획에 의해 중단되었습니다.\n\n";
                        node.creatureName = "포자 도마뱀";
                        break;
                    case "BunkerSpiderFile":
                        node.displayText = "벙커 거미\n\n시거드의 위험도: 20%\n\n학명: Theraphosa-ficedula\n벙커 거미는 테라포사 속에 속하는 거미로, 엉겅퀴 성운에서 발견되는 거미류 중 가장 크고 지금까지 발견된 거미류 중 두 번째로 큰 거미입니다. 벙커 거미는 보트가 엉겅퀴 성운을 여행한 후 약 수백 년에 걸쳐 대형 포유류를 잡아먹도록 진화한 것으로 추정됩니다. (참조: 희미해져 가는 성운 주변의 종 다양성 증가에 대한 추측)\n\n벙커 거미는 실크를 생산하여 선택한 둥지 주변에 깔고 먹이가 걸려 넘어지기를 기다립니다. 벙커 거미는 벽이나 먹잇감이 들어올 수 있는 출입구 위에서 기다리는 모습을 볼 수 있습니다. 벙커 거미를 '준비되지 않은' 상태로 발견하면 방어 반응으로 멈출 수 있습니다. 이 경우에는 그냥 내버려 두는 것이 가장 좋습니다. 벙커 거미가 공격적으로 반응하면 일반적인 도구로 싸우지 않는 것이 가장 좋습니다. 거미는 거미줄을 사용하여 느린 움직임을 보완하므로 주변을 잘 살펴보세요. 거미줄은 무딘 도구로도 쉽게 부술 수 있습니다.\n\n벙커 거미는 생태계에 큰 도움이 되지 않으면서도 특히 인간과 도시 탐험가에게 큰 위험을 초래할 수 있습니다. 이에 따라 벙커 거미가 서식하는 여러 주에서 비공식적으로 사살 명령이 합의되었으며, 2497년 10월 6일부로 ITDA의 승인을 받았습니다.\n\n";
                        node.creatureName = "벙커 거미";
                        break;
                    case "ManticoilFile":
                        node.displayText = "만티코일\n\n시거드의 위험도: 0%\n\n학명: Quadrupes-manta\n만티코일은 코비과에 속하는 텃새입니다. 초기 후손에 비해 몸집이 상당히 크고, 날개 길이가 55~64인치에 이릅니다. 가장 뚜렷한 특징은 네 개의 날개를 가지고 있다는 것입니다. 뒷날개는 저속에서 안정적으로 비행하는 데 주로 사용되며, 앞쪽 두 날개가 대부분의 양력을 만들어냅니다. 둥근 몸통은 눈에 띄는 노란색이지만 주 깃털(뒷깃)을 따라 검은색 윤곽선이나 줄무늬가 있습니다.\n\n만티코일은 주로 작은 곤충을 먹지만 작은 설치류도 잡아먹을 수 있습니다. 매우 지능적이고 사교적입니다. 광견병, 루벤클로리아, 피트 바이러스를 전염시킬 수 있지만 인간에게는 위협이 거의 되지 않으며 일반적으로 소극적인 기질을 가지고 있습니다.\n\n";
                        node.creatureName = "만티코일";
                        break;
                    case "CircuitBeeFile":
                        node.displayText = "회로 벌\n\n시거드의 위험도: 90%\n\n학명: Crabro-coruscus\n붉은벌이라고도 알려진 회로 벌은 꿀벌의 후손인 Apis 속의 진사회성 비행곤충입니다. 그들은 수북한 털과 붉은색 몸체와 두 쌍의 날개로 쉽게 알아볼 수 있습니다. 그들의 조상과 마찬가지로, 그들은 지능적인 사회적 꿀벌 행동, 대규모 군체 크기, 꿀을 저장하는 데 사용하는 밀랍 둥지 건설 및 수분에 있어서 중요한 역할로 잘 알려져 있습니다. 나무 등 높은 곳을 선택해 벌집을 만드는 경우가 많은 꿀벌과 달리, 붉은벌은 땅에 벌집을 만듭니다.\n\n붉은벌은 매우 방어적입니다. 여왕벌과 수벌을 제외하고 모든 벌이 벌집에서 벗어나 수 미터 이내에 접근하는 모든 생물을 공격합니다. 이러한 대담한 행동은 정전기라는 벌의 가장 특징적인 특성 덕분에 가능합니다. 벌은 공기와의 마찰을 일으킵니다. 또한 벌집 안에서 두 쌍의 날개를 서로 문지르며 마찰을 일으키고, 벌집 안에서도 서로를 문지르며 마찰을 일으킵니다. 당황하거나 화를 낼 때 더 강한 전기장을 생성하기 때문에 꿀벌에 비해 더 많은 전기장을 생성할 수 있는 이유는 아직 연구 중에 있습니다. 이 능력은 물 주변에서 특히 위험합니다.\n\n\n\n그것과는 거리를 유지하는 것이 좋습니다. 벌집을 도난당하면 붉은벌 떼는 모든 생물을 공격하는 맹공격에 들어갑니다. 이 파괴적인 벌떼의 행동은 벌집을 찾거나 완전히 지칠 때까지 지속되며, 몇 시간에서 며칠이 걸릴 수 있습니다. 작은 설치류, 곤충, 심지어 일부 대형 포유류의 사체 뒤에 벌집을 남기는 것으로 알려져 있으며, 드물게는 화재를 일으키기도 합니다. 이 강력한 꿀벌의 생태계에 대한 이점과 단점에 대해 많은 논쟁이 있습니다. BEEbated! - 불굴의 시구르드\n\n";
                        node.creatureName = "회로 벌";
                        break;
                    case "LocustFile":
                        node.displayText = "배회 메뚜기\n\n시구르드의 위험도: 0%\n\n학명: Anacridium-vega\n\n배회 메뚜기로 알려진 메뚜기의 일종입니다. 뛰어오르거나 날아다니는 경향이 있는 일부 종과 달리, 배회 메뚜기는 땅에 잘 붙어있지 않으며 숫자가 적을 때에도 서로 가까이 붙어 있습니다. 포식자가 방해하면 빠르게 흩어지지만 빛에 매우 끌립니다.\n\n";
                        node.creatureName = "배회 메뚜기";
                        break;
                    case "BaboonHawkFile":
                        node.displayText = "개코 매\n\n시구르드의 위험도: 75%\n\n학명: Papio-volturius\n개코 매는 긴꼬리원숭이과에 속하는 영장류입니다. 굽은 등을 가지고 있지만 평균적으로 8피트까지 서 있을 수 있습니다. 머리는 뼈로 이루어져 있고, 새처럼 생긴 부리와 긴 뿔을 꼬챙이처럼 사용하여 먹이를 잡아먹습니다. 뿔은 두개골의 나머지 부분처럼 뼈가 아닌 케라틴로 이루어져 있으며 신경이나 혈관이 없습니다. 따라서 개코 매는 강한 힘에 의해 뿔이 부러졌다가 같은 계절에 완전히 다시 자라는 경우가 많습니다. 개코 매의 이름은 부분적으로 그 큰 몸무게를 지탱할 수 없는 큰 날개 때문에 붙여진 이름이며, 대신 위협과 비바람으로부터 보호하는 데 사용됩니다.\n\n지금까지 관찰된 가장 큰 개코 매 무리는 18마리의 개코 매로 구성되었습니다. 그들은 영토를 느슨하게 갖고 있으며, 그들의 행동의 대부분은 위협과 과시에서 비롯됩니다. 그들은 자신의 영역을 표시하기 위해 화려하거나 다채로운 물건을 사용하기도 합니다. 고독한 정찰병인 개코 매는 일반적으로 소심하며 자극받지 않는 한 공격하지 않습니다. 개체 수가 많으면 위험할 수 있으므로 다른 사람과 함께 다녀서 자신이 위험해 보이게 만드는 것이 공격을 예방하는 가장 좋은 방법입니다. 그들은 더 작은 포유류를 선호하지만, 필요할 때에는 자신의 무리와 함께 눈 없는 개와 같이 크기가 두 배나 되는 동물도 공격하기도 합니다. 쟤네가 ㄴㅐ 피클 가져갔어\n\n";
                        node.creatureName = "개코 매";
                        break;
                    case "NutcrackerFile":
                        node.displayText = "호두까기 인형\n\n집을 지키는 수문장입니다.\n\n지칠 줄 모르는 하나의 눈으로 움직임을 감지하며, 마지막으로 감지한 생물의 움직임 여부를 기억합니다.";
                        node.creatureName = "호두까기 인형";
                        break;
                    case "RadMechFile":
                        node.displayText = "올드 버드\n\n 올드 버드는 인간형 디자인을 가진 자율적이고 공격적인 전쟁 병기입니다. 높이 19피트, 폭 11피트 크기의 이 로봇의 가장 큰 특징은 머리 부분에 위치한 10만 루멘의 빛을 발하는 스포트라이트와 음향 대포라고 불리는 가슴에 달린 장거리 음향 장치입니다. 어깨에는 먼 거리까지 소리를 내보내는 데 사용되는 스피커가 추가로 달려 있습니다. 올드 버드의 왼팔에는 집게가 있고, 오른팔에는 로켓 추진 수류탄을 발사하고 근거리에서 매우 뜨거운 불꽃으로 불을 붙일 수 있는 노즐이 있습니다. 올드 버드는 대량 생산된 최초의 궤도 외 병기 중 하나입니다.\n\n누가 올드 버드를 개발했는지에 대한 주제는 2143년 12월 18일, 50여 마리의 올드 버드가 앵글렌 수도를 침공한 기록이 처음 등장한 이후 격렬한 논쟁의 주제로 사용되었습니다. 이는 앵글렌 제국이 몰락한 주요 원인 중 하나로 꼽힙니다. 일반적으로 받아들여지는 이론은 2100년대 내내 앵글렌과 부에모흐 군대 사이의 긴장을 고려한 것이지만, 이후 수 세기 동안 입증된 것은 없습니다. 올드 버드의 디자인은 금속의 담금질까지 그 기원을 감추기 위한 것으로 보입니다. 그것은 \"걸어 다니는 랜섬 레터\"라고 불립니다. \n\n 올드 버드의 다리는 로켓처럼 작동하여 먼 거리를 이동하고 목표물을 효율적으로 찾을 수 있습니다. 하지만 이 기능이 존재하는 가장 유력한 근거는 그들이 궤도에 진입하고 착륙하도록 돕는 것입니다. 올드 버드가 사용하는 재료와 연료는 2130년경의 우주 여객선과 비슷합니다.\n\n올드 버드는 목표 행성에 착륙하여 영원히 머물러 있습니다. 한 번의 여행에 충분한 연료를 가지고 있는 경우가 많지만, 프로그램에서 '이주'를 선택할 수 있음을 시사하는 행동은 발견되지 않았습니다. 하지만 수백 년의 휴면기를 거쳐 새로운 행성으로 자율적으로 이동하는 올드 버드에 대한 검증되지 않은 설명이 존재합니다. \n\n올드 버드는 역사적으로 영국군이 부여한 암호명인 \"A16-L31\"을 따서 \"Al(알)\"로 불렸습니다. 그러나 2384년, 포스트 펑크 프로젝트 언더 레모라가 발표한 '올드 버드'라는 곡이 컬트 히트곡으로 발표되고, 불과 3년 후 거리 예술가 랜드 아이리가 동명의 유명한 작품에서 오토톤이 행성 사이를 비행하고 착륙하는 모습을 기러기 떼와 비슷한 배열로 묘사하면서 현대 문화에서 가장 상징적이고 영향력 있는 이미지로 여겨지며 현대적인 별명으로 굳혀지게 됩니다.\n\n 2356년, 5-엠브리언은 함선을 이용한 여행이나 정착 목적으로는 부적합한 것으로 분류되었습니다. 목격자들에 따르면 올드 버드들이 \"지평선을 따라 늘어서 있다\"고 묘사되었습니다. 오늘날에는 작동하지 않는 것처럼 보이지만, 많은 수가 여전히 수면 모드에 있을 가능성이 있으므로 행성은 그 위험한 상태를 유지할 가능성이 높습니다. 이 작은 위성은 올드 버드의 비행 및 착륙 능력을 시험하는 장소로 사용되었을 가능성이 높습니다.\n\n";
                        node.creatureName = "올드 버드";
                        break;
                    case "ButlerFile":
                        node.displayText = "집사\n\n시구르드의 위험도: 70%\n\n이 녀석들과 관련된 파일이 하나도 없어서 내가 직접 쓰고 있다. 저택에서 계속 이 구역질나는 놈들을 발견하고 있는데, 우리에게 대화도 하지 않고 관심도 주지 않고 있다. 생긴 것도 뭔 바람 빠진 풍선처럼 생겼고. 그리고 리처드보다 심한 썩은 고기 냄새가 나. 그리고 몸 안에서 뭔가 파리가 윙윙거리는 소리를 들었던 것 같아. 그리고 제스는 눈구멍에서 뭔가가 기어 나왔다가 다시 들어가는 걸 봤다고 했어. \n\n이 녀석들을 어떻게 해야 할 지 잘 모르겠어\n\n얘넨 그냥 돌아다니면서 바닥이나 청소하고 다니고 있어. 그래서 저택이 이렇게 깨끗한 거겠지? 일단 걔네들은 자기 할 일만 하니까. 근데 얘네 내가 안 볼 때 자꾸 나를 째려보는 것 같은데\n\n추가: 그 녀석이 칼을 들고 리치를 쫓아왔어. 우린 못 봤지만 걔가 방에 들어가자마자 그 집사가 다시 빗자루를 꺼내 아무 일도 없었다는 듯이 바닥을 청소하고 있었다고 했어. 이제부터 우리 모두 같이 붙어다녀야 해. 이 망할 집사 녀석들! 이제 리치 좀 봐야겠어 아마 슈트에 오줌 지렸을 거야\n\n";
                        node.creatureName = "집사";
                        break;
                    case "MaskHornetsFile":
                        node.displayText = "위장 말벌\n\n위장 말벌";
                        node.creatureName = "위장 말벌";
                        break;
                    case "TulipSnakeFile":
                        node.displayText = "튤립 뱀\n\n시거드의 위험도: 1%\n\n학명: Draco tulipa\n튤립 뱀 또는 튤립 도마뱀은 날아다니는 도마뱀의 일종으로, 긴 팔과 날개로 쉽게 구별할 수 있으며, 같은 속의 다른 종(Draco)보다 크기가 두 배나 크며 비정상적으로 밝은 색상과 패턴을 자랑합니다. 튤립 뱀이라는 이름은 목 아래와 머리 뒤의 덮개가 큰 꽃잎을 닮았기 때문에 지어졌습니다. (다른 특이한 특징으로는 한 쌍의 눈과 깊게 갈라진 꼬리가 있습니다).\n\n튤립 뱀의 행동은 완고하고 겁이 없습니다. 실제로 튤립 뱀은 포식자를 피해 도망치는 일이 거의 없습니다. 하지만 아이러니하게도 많은 생물학자들은 튤립 뱀의 꽃무늬가 위협을 표시하기보다는 위장술에 더 도움이 된다는 이론을 세우기도 했습니다. 또한 새와 비슷한 정교한 구애 의식에서도 꽃은 중요한 역할을 합니다. \n\n독특하게도 튤립뱀은 짝짓기를 할 때 무겁고 큰 물체를 공중으로 들어올리는 모습을 종종 관찰할 수 있는데, 잠재적 짝에게 깊은 인상을 주기 위해 자신의 두 배 이상 큰 바위나 식물을 들어 올릴 수 있습니다. 수컷은 보호심과 질투심이 강하고 광분한 나머지 마음에 드는 물체를 두고 줄다리기 싸움을 벌일 수 있으며, 여기에는 다른 생물도 포함될 수 있습니다.\n\n";
                        node.creatureName = "튤립 뱀";
                        break;
                    case "ClaySurgeonFile":
                        node.displayText = "이발사\n\n이발사에 관한 꿈을 꿨어\n어느 날 그가 가발을 만들어 줬어\n왜냐면 가위가 너무 커서\n\n";
                        node.creatureName = "이발사";
                        break;
                    case "VainShroudFile":
                        node.displayText = "은폐 수풀\n\n시거드의 위험도: 이건 생명체가 아ㄴl야.\n\n학명: Phlebodium ruber\n흔히 \"Vains\"라고도 불리는 은폐 수풀은 미나리아재비과의 뿌리줄기 양치식물의 일종입니다. 이상적인 조건에서 은폐 수풀의 잎은 평균 6피트 11인치, 너비는 30~50센티미터에 이르지만 높이는 8피트까지 자랄 수 있습니다. 뿌리는 비정상적으로 크고 약간 야아아아아아아악간 가시가 많습니다. \n\n공격적인 번식력과 놀라운 크기로 인해 공간을 많이 차지하여 시야를 방해하고 제거하기도 어렵습니다. 건강한 식물을 밀어내기도 하며 영양가도 거의 없고, 불이 매우 잘 붙습니다. 은폐 수풀과 납치범 여우 사이에는 강한 상관관계가 관찰되었으며, 이러한 이유로 은폐 수풀이 많은 수로 번식하게 되면 위험하다는 결론이 도출되었습니다. 이러한 점과 여러 생태계와 농업에 미치는 해로운 영향 때문에 은폐 수풀은 2147년부터 ITDA에 의해 유해 잡초로 분류되었습니다.\n\n\n\n은폐 수풀의 원산지는 알려지지 않았으며, 2143년 식물 연구자 알렉스 커트가 CoRoT-7b에서 처음 발견한 후 불과 몇 주 후에 같은 행성계 반대편에서 발견되었습니다. 은폐 수풀은 섭취 시 가벼운 환각 작용을 하기 때문에 처음에 여러 행성으로 퍼졌다는 설이 있지만, 그 역사는 베일에 싸여 있습니다.\n\n<b>일어나... 회ㅅㅏ가 수송선에 씨앗을 넣었고, 그들은 ㅇ우리가 이 시뻘건 풀떼기들을 싫어한다는 것을 알고 있기 때문에 우리에게 제초제를 팔고 있어</b> 아 볼드체 ㅈㅅ\n\n";
                        node.creatureName = "은폐 수풀";
                        break;
                    case "BushWolfFile":
                        node.displayText = "납치범 여우\n\n시거드의 위험도: 80% 좋아 아마 쟨 학ㅇ교에서 수상함으론 A+ 학점 맞을듯\n\n학명: Vulpes raptor\n개과에 속하는 대형 포유류인 납치범 여우는 독특한 생물학적 특징을 가지고 있습니다. 그 중 두 가지는 수평 턱과 독특한 두개골 모양으로 인한 강력한 치악력과 몸 길이의 두 배까지 닿을 수 있는 가늘고 긴 개구리처럼 생긴 혀입니다. 그들은 고독한 사냥꾼이며 평생의 짝입니다.\n\n납치범 여우는 매우 길고 덥수룩한 목과 꼬리를 가지고 있으며 붉은색을 띠고 있어 은폐 수풀과 잘 어우러진다고 여겨지며, 숨어서 먹이를 기다리는 모습을 자주 볼 수 있습니다. 납치범 여우와 은폐 수풀은 두 침입종 사이의 가장 악명 높고 독특한 상호주의적 관계 중 하나를 형성하고 있습니다. 이 두 종은 대형 우주선을 타고 행성계 곳곳으로 퍼져나가고 있습니다. 일부 자연 보호 구역을 제외하고, ITDA는 생태계에 미치는 영향 때문에 납치범 여우와 은폐 수풀에 대한 제거 명령을 발령했습니다. \n\n납치범 여우는 공격적이면서도 소심하고 영역이 뚜렷한 행동 때문에 이름이 붙여졌으며, 주로 눈에 띄지 않는 곳에서 먹잇감을 잡아먹는 것을 선호합니다. 보통은 몸을 낮추고 사냥감이 나타나기를 기다립니다. 하지만 불안해지면 길고 끈적한 혀를 이용해 먼 거리에서 먹이를 낚아채 필요한 만큼 멀리 끌고 갈 수 있습니다. 주변에 은폐 수풀이 있다면 무기를 소지하고 다른 사람과 멀리 떨어지지 않는 것이 좋습니다. 혀는 매우 부드럽고 연약하기 때문에 반격을 가하면 공격을 중단합니다. 납치범 여우로 인한 연간 인명 피해는 적은 편이며, 대부분의 피해자는 보호자가 없는 어린이나 혼자 하이킹을 하는 성인입니다.\n\n\n";
                        node.creatureName = "납치범 여우";
                        break;
                    case "ManeaterFile":
                        node.displayText = "맨이터\n\n시거드의 위험도: 1000%\n\n약간 아들 같아\n\n학명: Periplaneta clamorus\n\n맨이터는 집바퀴속으로 알려진 바퀴벌레속과 연관되어 있습니다. 맨이터는 1951년 레푸스 소행성대에 있는 여러 구리 광산 중 하나인 카스웰라 광산 노동자들이 처음 붙인 별명입니다. 카스웰라 광산은 전쟁이 끝난 후로 쇠퇴하기 시작하여 12년 동안 운영되다가 곧 문을 닫을 예정이었는데, 맨이터에 대한 소문이 퍼지기 훨씬 전부터 노동 조건이 악화되어 있었기 때문에 당시 상부나 언론에서는 이를 심각하게 받아들이지 않았습니다. 하지만 그해 6월 23일, 카스웰라 광산 최하층에서 장기가 적출된 시신 한 구가 구출되었습니다. 나중에 시신은 선임 광선 작업자였던 브렛 골덴트로 확인되었으며, 부검 결과 장비 결함으로 인한 사고로 사망한 것으로 기록되었습니다. 하지만 이 이야기는 센세이션을 일으켰고, 6년 후 사이먼 골덴트가 이끄는 DEEP57 동굴 탐험대가 아버지의 죽음에 대한 미스터리를 풀기 위해 처음 기록할 때까지 맨이터는 유명한 신화로 떠올랐습니다. 현재 가장 큰 troglobite(동굴에 서식하는 종)로 알려진 이 생물은 과학계에 큰 반향을 일으켰습니다. 이 정도 크기의 생물이 지하 동굴에 갇혀 사는 것은 불가능하다고 여겨졌습니다. 이 신화를 반증하고자 했던 사이먼 골덴트는 자신의 발견을 “충격적”이라고 표현했습니다.\n\nPeriplaneta clamorus는 주로 식물 찌꺼기와 동물 사체를 먹고 사는 것으로 추정됩니다. 이들은 둘 다 지하 동굴로 흘러드는 물에 의해 운반될 수 있습니다. 지하에 사는 동안 큰 포식자에 대한 본능적인 두려움이 발달하지 않은 것으로 보이며, 애벌레일 때는 인간의 존재에 매료되거나 무관심할 수 있습니다. 그러나 유충 상태에서는 시끄러운 소음 혹은 피, 상처를 보면 두려움에 떨며 반응하기도 하며, 일부 연구자들은 이 애벌레가 외로움을 타는 징후를 보이는 것을 관찰했습니다.\n\n불행하거나 두려움을 느끼는 것은 Periplaneta clamorus의 성장을 가속시키는 것으로 보입니다. 그들의 변태 과정은 매우 갑작스럽고 관찰하기 위험한 현상입니다. 유충이 변태에 가까워지고 있다는 가장 명확한 신호는 아래턱에서 산성의 거품 액체를 배출하는 것으로, 이 물질은 촉매제로 여겨지며 변태가 시작되면 체외로 대량 배출됩니다.\n\n성체는 매우 빠르게 움직일 수 있으며 큰턱을 높은 주파수로 움직이며 큰 소음을 낼 수 있습니다. Periplaneta clamorus는 사람의 접근을 기다리며 시야를 피하는 것처럼 보이는 것으로 알려졌기 때문에, 그것들과 마주쳤을 때는 천천히 뒤로 물러나 거리를 유지하는 것이 좋습니다. 다리를 뒤로 젖히면서 갑자기 멈추고 숨을 내쉬는 것은 성체가 추격에 나선다는 신호입니다. 신의 가호가[ 있기를\n\n\n";
                        node.creatureName = "맨이터";
                        break;
                    case "SapsuckerFile":
                        node.displayText = "거대 딱따구리\n\n시거드의 위험도: 10% 아니 80% 저거 누가 쓴 거야\n\n학명: Sphyrapicus cursus \n\n거대 딱따구리는 Sphyrapicus속의 \"Sapsuckers\"라고도 불리는 딱따구리입니다. 이름에서 알 수 있듯이, 이 종은 단연코 가장 큰 종입니다. 수컷은 평균 5피트이고, 암컷은 7피트에서 9피트 사이로 다양합니다. 이들은 날 수 없습니다. 하지만 깃털에 가려진 퇴화한 날개 구조를 가지고 있습니다. 몸은 갈색 또는 검은색이며, 얼굴에는 흰색과 빨간색 무늬가 있습니다. 시력이 매우 예리하며, 거의 360도의 시야 범위를 가지고 있습니다. 완전히 성장한 성체의 몸은 그들이 주로 서식하는 숲 행성의 모든 종 중에서 가장 운동 능력이 뛰어난 종 중 하나입니다.\n\n이들의 식단은 곤충과 나무 수액으로 구성되며, 작은 설치류도 포함됩니다. 거대 딱따구리는 평생 짝을 지어 살며, 성체 수컷은 주로 사냥과 먹이 찾기를 담당하고 암컷은 땅에 있는 둥지를 지키는 역할을 합니다. 그들은 기회주의적인 본성 때문에 맹렬하게 보호하며, 만약 그들의 영역을 침범하는 것이 있다면 조각내어 새끼에게 먹이려고 시도할 것입니다. 인간에게도 위험할 수 있으므로 그들의 영역에 들어가지 않는 것이 좋습니다. \n\n이들은 일부 숲과 생태계에 파괴적인 영향을 미쳤습니다. 다른 종들만큼 빠르게 번식하지는 않지만, 유목민처럼 매우 빠르게 매우 먼 거리를 이동하기 때문에 개체수를 추적하기는 어렵습니다. \n\n리차드도 사실 거대 딱따구리야\n";
                        node.creatureName = "거대 딱따구리";
                        break;
                }
            }
            foreach (TerminalKeyword keyword in terminalNodes.allKeywords)
            {
                if (Plugin.translateModdedContent)
                {
                    switch (keyword.word)
                    {
                        case "asteroid13":
                            keyword.word = "아스테로이드13";
                            break;
                        case "atlantica":
                            keyword.word = "아틀란티카";
                            break;
                        case "cosmocos":
                            keyword.word = "코스모코스";
                            break;
                        case "desolation":
                            keyword.word = "디솔레이션";
                            break;
                        case "etern":
                            keyword.word = "이턴";
                            break;
                        case "fissionc":
                            keyword.word = "피션";
                            break;
                        case "gloom":
                            keyword.word = "글룸";
                            break;
                        case "gratar":
                            keyword.word = "그라타";
                            break;
                        case "infernis":
                            keyword.word = "인퍼니스";
                            break;
                        case "junic":
                            keyword.word = "주닉";
                            break;
                        case "oldred":
                            keyword.word = "올드레드";
                            break;
                        case "polarus":
                            keyword.word = "폴라러스";
                            break;
                        case "acidir":
                            keyword.word = "어시디어";
                            break;
                    }
                }
                switch (keyword.word)
                {
                    case "first":
                        keyword.word = "첫번째";
                        foreach (CompatibleNoun noun in keyword.defaultVerb.compatibleNouns)
                        {
                            switch (noun.noun.name)
                            {
                                case "LogFile1Keyword":
                                    noun.noun.word = "첫번째";
                                    noun.result.creatureName = "첫번째 일지 - 8월 22일";
                                    noun.result.displayText = "날짜: 1968년 8월 22일\r\n안녕. 난 지금 제 정신을 유지하기 위해 이 일지를 쓰고 있어. 이 낡아빠진 컴퓨터로는 가장 기본적인 일도 할 수가 없어서 데스몬드에게 일지 기능을 추가해달라 했거든. 지금까지 내가 아는 건 뭐든지 도감에 메모를 적고 있어. 내 동생이 일기를 쓰라고해서 할 수 있는 데까지는 하고있어! 이 일지는 깨끗하게 삭제하지 않는 한 아마도 몇 년 동안 남아 있을 테고 역사적 기록이 될 수 있으니, 전문적인 방식으로 이 글을 쓰고 있는거야. 데스몬드가 말한대로ㅀ\n\n나중에 당신이 이 글을 읽고 있다면 아마 나랑 다 른 크루 출신이겠지. 여긴 이직률이 어마어마한데, 아마 이 직업이 워낙 개판이고 다들 빠르게 죽어나가서 그런 것 같아! 경험이 좀 쌓이면 어느정도 도움을 줄 수 있을 것 같아. 일지 종료.\n\n아, 우리 이름은 시거드(나), 리처드, 데스몬드, 제스야.";
                                    break;
                                case "LogFile2Keyword":
                                    noun.noun.word = "냄새";
                                    noun.result.creatureName = "냄새 나! - 8월 24일";
                                    noun.result.displayText = "맙소사, 이 슈트 망할 미라마냥 날 쥐어짜고 있어! 난 다 큰 성인인라고, 다리 사이에 공간 좀 줘! 나 죽겠다, 죽겠어! 아빠가 기뻐할 거야. 나 드디어 일자리를 구했어. 에라이! 여기 사람들 다 냄새 나. 특히 리치. 나 쟤 호수에 던져버릴 거야. 이게 뭘로 만들어졌는지는 내 알 바 아냐. 그리고 카메라 임무 하는거 재밌네. 오\n날짜 쓰는 걸 까먹었네 1968년 8월 24일이야. 이 키보드에는 ㅇㄴㅁ어ㅑㅣㄻㄴㅄ..ㄹ/ㅎ;...\n\n\n오늘 우리는 후라이팬 두 개와 커다란 나사를 발견했다. 딱히 쓸모가 없다. 회사는 이딴 걸 어디에 쓰려고 하는 거지\n\n";
                                    break;
                                case "LogFile3Keyword":
                                    noun.noun.word = "상황의";
                                    noun.result.creatureName = "상황의 변화 - 8월 27일";
                                    noun.result.displayText = "우리는 지난 며칠 동안 \"상황의 변화\"에 빠져 있었다. 리치가 계속해서 했던 말인데, 걔한테선 썩은 참치 통조림 냄새가 난다고. 내가 ㄱㅏ본 여름 캠프 중 최악이다. 날짜는 1968년 8월 27일.\\n\\n우리는 우리 중 한명이 모든 움직이는 것들을 때려잡을 수 있는 삽을 가지고 있는지 확인하고, 다른 한명을 항상 \"카메라 임무\"에 투입해서 그들이 글자와 숫자로 이 큰 보안문을 열 수 있도록 한다. 마법사 데스몬드에게 어떻게 작동하는지 물어봐봐. 내 생각에 데스몬드는 그냥 문에 있는 코드를 입력하는 것 같다.\\n. 그게 다야.\\n\\n우리는 오늘 일부 상품을 70% 가치로 회사에 판매했다. 카운터 뒤에서 나는 빌어먹을 정신병 걸릴 듯한 소리를 들으니 오싹해졌 다. 다들 조금도. ??쓰지 않는다.?\\n신경쓰여! 내 손전등이 제대로 안들어오고, 빛만 어두워졌다고.\\n\\n";
                                    break;
                                case "LogFile4Keyword":
                                    noun.noun.word = "수상한";
                                    noun.result.creatureName = "수상한 - 8월 31일";
                                    noun.result.displayText = "날짜는 1968년 8월 31일. 나는 다시 카메라 임무를 받았고 어쨌든 이 위성 위를 걷는 것이 싫다. 아하 쟤들 꼴 좀 보라지, 막 비가 내리기 시작했다!!! 난 그냥 앉아만 있다. 개미들이나 쓸 만한 이 작은 침대에서는 잠을 잘 수 없어\n\n\n수 없이 생각해봤다. 이 일이 수상하다고 생각한다. 이 글을 읽고 있는 당신도 아마 나와 마찬가지로 여기까지 왔을 것이다. 보수도 괜찮고 계약은 한 계절밖에 되지 않아. 그들은 이상한 목소리로 전화로 \"평가 시험\"을 통과시켰고, 당신은 나머지 팀원들과 셔틀에서 계약에 서명했을 거야. 하지만 당신은 그 동안 아무에게도 말을 하지 않았을 테지. 셔틀이 자동 조종됐었어. 내 생각엔 전화에서 들린 목소리가 가짜였던 것 같다. 최악의 꿈을 꿨다. 그냥 집으로 돌아가고 싶다. 하지만 집에 기어가서 울면서 아빠 현관문을 긁지는 않을 것이다. 그건 시거드답지 않다고!\\n\\n";
                                    break;
                                case "LogFile5Keyword":
                                    noun.noun.word = "뒤에서";
                                    noun.result.creatureName = "뒤에서 난 소리 - 9월 4일";
                                    noun.result.displayText = "날짜: 1968년 9월 4일\n\n오늘 아침은 쓸모없는 쓰레기나 팔러 회사로 일찍 갔다. 정산 비율은 120%였는데, 데스몬드는 그게 희귀한 일이라고 했고 놓치고 싶지 않다고 했다. 그리고 그게 멍청한 주식인 것 마냥 쳐다보고 있고는 했다.\n\n여전히 여긴 개판인게, 리치에게서는 여전히 똥 냄새가 나거든. 잠도 거의 못 잘 정도야. 회사 건물 벽 뒤에서 엄청 끔찍한 소리를 들었는데, 그 소리는 마치 우리 어머니가 씨앗과 향신료를 절구공이에 담아 으깰 때 나는 것 같이, 울부짖는 붉은 얼굴들이 마구 뒤섞여 콘크리트에 휩쓸려 가는 소리 같았다,. 아직도 그 소리가 들린다. 악몽이야. 아무도 저 소리를 듣지 못했어. 쟤넨 뭘 해야 할지도 몰라. 제스는 내가 \"향수병\"에 걸린 거라고 생각하고 있어. 그게 아니라 이 싸구려 슈트에 병이 날 정도로 질린 거다\n\n";
                                    break;
                                case "LogFile6Keyword":
                                    noun.noun.word = "작별";
                                    noun.result.creatureName = "작별 - 9월 7일";
                                    noun.result.displayText = "우린 리치를 버리고 왔다. 방 하나만 확인하면 일과가 끝인데, 문을 열 수가 없었다. 그리고 뒤를 보니 리치는 거기에 없었다. 쭈글쭈글한 피부를 가진 망할 이족보행 꽃대가리 인간이었어! 그게 걔 목을 꺾었어. 난 무언가 꺾이는 소리를 들었지만, 걔는 거기에 없었어. 리치의 시체라도 찾고 싶었는데, 다들 그저 겁쟁이들이야! 그들의 얼굴은 멍청한 듯이 텅 비어 있었고, 그들의 멍청한 대가리는 움직이지 않았다. 마찬가지로 그들은 나를 떠날 것이었다. 우리 모두는 리치를 싫어했지만 이런 걸 원하지 않았다. 이건 이럴 가치가 없어. 그냥 이럴 가치가 없다고. 우리는 아무 가치도 없겠지만 팔 수 있는 가위 한 개, 우표가 가득 담긴 상자 한 개, 코드 묶음을 얻었다. 이럴 ㄱㅏ치도 없는 일인데, 회사가 원하는 게 대체 뭘까?\n\n\n마지막으로 리치를 본 날짜는 1968년 9월 7일이다.\n\n";
                                    break;
                                case "LogFile7Keyword":
                                    noun.noun.word = "비명";
                                    noun.result.creatureName = "비명 - 9월 13일";
                                    noun.result.displayText = "날짜: 1968년 9월 13일\n\n무슨 이유에서인지 사고 신고를 위해 회사 전화번호로 전화를 걸어야 했던 사람은 바로 나였다. 다른 사람들은 너무 겁을 먹어서 하지 못했다. 집에서 본 전화 인터뷰랑 바보 같은 훈련용 영상들에서 나왔던 가짜 목소리와 똑같았다. 하지만 내 생각에 그것은 내가 했던 말을 들었던 것 같았다, 왜냐하면 그것은 그 가족과 연락해서 대체자를 찾는다는 말과 다른 뭔가를 말했기 때문이다. 말이 정말 빨랐다.\n\n\n회사 건물 벽에 가면 무전기에서 항상 비명소리를 들을 수 있다는 것을 배웠다. 그들은 내 말을 믿지 않았지만, 글쎄 지금은 믿을걸. 내가 몇 주 전에 들었을 때처럼 들렸거든. 그들은 그만두고 싶어해. 나는 겁쟁이가 아니라고 말했다. 이 직업을 얻었은 지 얼마 안 됐거든\n\n";
                                    break;
                                case "LogFile8Keyword":
                                    noun.noun.word = "황금빛";
                                    noun.result.creatureName = "황금빛 행성 - 8월 ??일";
                                    noun.result.displayText = "무전기로 목소리를 들려주었는데 비명소리의 일부 같았다. 그는 황금빛 행성이 실제로 존재했다고 말했고, 전설이 아니라고 말했다. 그리고 그것이 단지 유성에 부딪힌 것이 아니라고 말했다. 그는 행성이 \"짐승\"에 의해 삼켜졌고, 소화되고 있다고 말했다. 그 짐승이 뭐냐고 물었더니 모른다고 하더군!. 그는 그것이 행성을 먹어치웠고 그들은 모든 것을 잊어버렸다고 말했다. \\n\\n말을 멈추도록 할 수는 없었다. 하지만 나는 그가 큰 벽의 반대편에 있으니 꺼내줄 수 있다고 말했다. 나는 그가 건물 안에 있다고 말했고, 그때 그는 몹시 놀라기 시작했다/. 한마디도 알아들을 수 없었고, \"껍질을 뱉어낸다\"고 뭔가 말한 것 같다. 그래서 그냥 껐다. 정말 엉망이었다\\n\\n제스는 황금빛 행성은 단지 이야기일 뿐이라고 했다. 안다고 말했다. 나도 바보가 아니라고. 글쎄, 그녀는 내가 그만둬야 한다고 말했고, 내가 그만두면 제스도 그만둔다고 한다. 그래서 그 사람으 여전히 하고있다.\\n\\n";
                                    break;
                                case "LogFile9Keyword":
                                    noun.noun.word = "아이디어";
                                    noun.result.creatureName = "아이디어 - 9월 19일";
                                    noun.result.displayText = "날짜: 1968년 9월 19일\n지난 주말에 데스몬드와 제스가 깨어난 것 같다. 우리는 며칠 안에 네 번째 팀원을 데려올 예정이다. 우리는 더 이상 어떤 위험도 감수하지 않고, 그만큼 많이 얻지도 않지만 더 안전하다고 느꼈고, 더 편하게 잘 수 있게 됐어. 근데 아직도 힘들어. 아직도 똥 냄새가 난다. 아직도 리치 냄새가 나 걔는 그 정도로 냄새가 심했거든\n\n\n난 전화에서 들리던 목소리가 가짜 같다고 얘기하고 있었다. 그러다 데스몬드가 이상한 아이디어를 떠올렸다. 자기는 마법사니까 내가 회사 번호로 전화한 걸로 그 목소리가 어디에서 나오는지 알아낼 수 있겠다고 생각했어. 왜 그러려는지는 모르겠지만, 진지해 보였어. 요즘 그는 터미널을 많이 사용하고 있어.\n\n";
                                    break;
                                case "LogFile10Keyword":
                                    noun.noun.word = "헛소리";
                                    noun.result.creatureName = "헛소리 - 9월 27일";
                                    noun.result.displayText = "1968년 9월 27일\\n데스몬드는 나보고 우리가 알아낸 내용을 ㄱl록하고 \"헛소리를 최소화\"하길 원하는 것 같아,  그래서 나는 내 헛소리들을 아주 많이 집어넣기로 했다. 엿이나 먹으라지. 새로 온 팀원 이름은 루카스야. 그는 항상 혼란스러워하고 겁에 질려 있는데, 아기가 따로 없어. 적어도 괴물같은 냄새는 안 나네.\\n9월인데도 날씨가 너무 좋아서 이 슈트가 잘 어울리지 않아\\n음 일단 데스몬드가 \"전화를 추적\"해서 회사 건물에서 우리에게 전화하는 \"척\"하는 사람들이 어디에 있는지 알게 됐어. 그들은 태양계 너머에 있어!! 왜 그렇게 멀리 있는 거지?\\n\\n데스몬드는 모른다고 하지만 제 생각은.. 무전기에서 들었던 목소리처럼 정말 회사 건물에 큰 괴물이 있는 거면 어떡해? 그걸 가둬놓고 길들이기 위해 먹이를 주는 것일지도 몰라. 난 그냥 바보같은 일을 하고 싶었을 뿐이라고!!!!!!";
                                    break;
                                case "LogFile11Keyword":
                                    noun.noun.word = "숨어";
                                    noun.result.creatureName = "숨어 있는 것 - 9월 30일";
                                    noun.result.displayText = "1968년 9월 27일\n\n\n난 무언가가 회사 건물의 거대한 콘크리트 벽을 뚫고 나오는 꿈을 자주 꾸고 있어. 그게 어떻게 생겼는지는 모르겠는데 그냥 엄청 크고 빠른 것 같아. 우리는 전화 속 목소리가 진짜인지 알아낼 방법을 찾을 수가 없어. 데스몬드는 저게 너무 멀리 있다고 말하더라. 나는 그에게 자동 조종 장치를 직접 조종할 수 있는지 물어봤는데 걔가 나보고 미쳤다고 하고 우릴 죽일 수도 있다고 했어. 그래, 난 미쳤어 데스몬드.\n\n그는 우리가 좌표로 가려면 개인 함선을 타야 할 것이라고 말했고, 거기에 실제로 무엇이 있을지는 누가 알겠냐고 말했어. 만약에.. 그냥 아무것도 아니었다면 어떨까?\n나는 우리가 어떻게 여기까지 왔는지 기억이 안 나. 계약을 맺은 건물로 날아갔던 것처럼 작은 일들만 기억나. 하지만 나는 함선에 어떻게 탔는지도, 심지어 아빠에게 작별 인사를 했던 기억조차 없어. 꿈 속에서는 회사가 그 안에 갇혀 있는 게 아니라 그냥 숨어 있는 것 같아. 내가 집에 돌아갈 수 있을지 잘 모르겠어.\n\n";
                                    break;
                                case "LogFile12Keyword":
                                    noun.noun.word = "진짜";
                                    noun.result.creatureName = "진짜 직업 - 10월 1일";
                                    noun.result.displayText = "날짜 : 1968년 10월 1일\\n과자와 레모네이드 팩이 또 동났다. 데스몬드는 기절 수류탄이 \"더 효과적\"이라고 생각하고 있어. 내 물건 주문을 다음 날로 미루면 난 조종 레버에 응가 싸놓고 기절 수류탄에 대해 어떻게 생각하는지 물어볼 거야. 데스몬드 이거 읽지 마\\n\\n\\n내가 마법사가 아니라는 건 알지만 오늘도 컴퓨터 담당인 것 같아. 내가 보우에 나갈 때마다 죽은 것처럼 창백해진다고 해서 스크린 업무를 시켰어. 내가 약해졌다고 생각하나 봐. 어쨌든 루카스는 겁이 많아. 걔가 미치지 않게 잘 지켜봐. 난 밤에도 내 집 뒷마당인 것 처럼 산책할 수 있어!! 그냥 강 건너편 언덕에서 리치를 봐서 그래. 그게 3일 전이야. 난 무섭지 않아. 그냥 밖이 추울 뿐이야. 사실 그냥 걔네는 내가 매일 스크린 업무를 하길 바라는 것 같은데 그냥 다들 핑계를 대고 있는 것 같아\\n\\n아빠가 그립다. 나는 아빠가 타이탄에 머물지 않기를 바란다. 사람들은 2년 뒤엔 예전 같을 거라고 말했다. 제스는 곧 전쟁이 일어날 거라고 했고 모두가 그걸 기다리고 있다고 말했다. 우리가 물건을 팔러 갈 때마다 회사 건물이 안에 용광로가 있는 것처럼 요란하게 흔들린다. 그들은 그만두는 것을 너무 두려워한다. 우리는 할당량을 맞추기 위해 잠도 거의 못 자고, 상황은 매번 더 힘들어진다. 마치 바늘 구멍을 통과하려고 쥐어짜내지는 기분이다. 다시 돌아갈 수 있었으면 좋겠다. 그냥 매일 용돈만 받더라도 아빠 밑에서 일하는 게 더 나았다. 나는 아빠가 폭포를 보기 위해 우리를 마을 밖으로 데려가서 우리가 오래된 나무 계단을 걸어 올라갔던 때가 좋았다. 난 그냥 진짜 직업을 원했을 뿐인데\\n\\n";
                                    break;
                                case "LogFile13Keyword":
                                    noun.noun.word = "데스몬드";
                                    noun.result.creatureName = "데스몬드 - 10월 15일";
                                    noun.result.displayText = "데스몬드. 1968년 10월 3일. 이 일지가 발견되면 시스템째로 삭제될 것 같아 이것을 숨기기 위해 암호화하고 있습니다. 모두 위장이었습니다. 이 모든 것이 단순히 거래하는 것처럼 보이지만, 우리의 진짜 임무는 공포를 계속 먹여 키우는 것이었습니다. 배부름이 끝나고 그것의 배고픔이 어느 것으로도 채워지지 않을 때까지 얼마나 걸릴 지는 아무도 모릅니다. 어쩌면 이 모든 황량한 위성들과 관련이 있을지도 모릅니다. 이 글을 읽는 사람에게, 부담스럽게 해서 죄송합니다. 제발 밤낮으로 좋은 시간 되길 바랍니다. 우리가 할 일이 또 무엇이 있겠습니까";
                                    break;
                                case "LogFile14Keyword":
                                    noun.noun.word = "팀의";
                                    noun.result.creatureName = "팀의 시너지 - 10월 10일";
                                    noun.result.displayText = "1968년 10월 10일\n\n\n글 쓸 기분이 아니었다. 시간도 없었고. 하지만 어젯밤, 맹세코 아빠 목소리를 들었다. 충전기 아래에 있는 그 통풍구에서 말이야. 내가 미친 건 아닌 것 같아. 그래, 그래, 다들 그렇게 말하지! 나도 알아! 그래도 미치지 않으려고 이 일지를 쓰는 거잖아, 그치? 그러니까 써볼게. 아빠가 자기 지금 어디 있는 거냐고 물었어. 숨을 제대로 못 쉬는 것처럼 깊게 숨을 쉬더니, 마치 엔진 안으로 끌려가는 것 같았어. 겁먹은 것처럼 들렸어. 그리고 목이 마르다고 했어.\n\n그런 아빠 목소리는 처음이었다. 아마 내가 잠을 제ㄷㅐ로 못 자서 그럴 수도. 젠장 이제 어케 하지. 그냥 그걸 열어서 들여다봐야 하나.\n\n아마 내가 기분이 안 좋아서 그런 걸지도 몰라. 우리가 훔쳤던 그 아기 새를 팔아버려서. 그 날 보우에서 항상 우리를 째려보던 날개 없는 거대한 놈한테서 훔친 거. 그 녀석은 엄청 빨랐고, 우리는 바로 도망쳐야 했어. 루카스 그 멍청이는 그 이후로 이까지 덜덜 떨고 있잖아. 다시는 그런 짓 안 할 거야. 걔네가 나한테 그 아기 새를 책상 위에 올려두게 했거든. 그땐 그냥 해야 하나 싶었는데, 이제 와서 왜 나한테 시킨 건지 알겠어... 걔네가 나보다 더 죄책감 느껴서 그런 거야. 겁쟁이들. 그래서 내가 데스몬드 면전에다 말했어. \"니가 알아서 해.\" 라고.\n\n라고 하니 멍청한 표정으로 \"뭔 죄책감?\" 이러더라. 아기 새 알 팔아버린 거 말이야!그리고, 믿ㄱl지도 않게 \"그게 아침에 먹는 계란이랑 뭐가 달라?\" 라고 했어. 그래서 내가 말했지. 걔가 깨고 나와서 우리를 봤다고, 그걸 내가 어떻게 먹어! 그건 살아 있었다고! 라고 하니, 걔가 한 말 좀 들어봐. \"그게 폐품을 나를 수 있나?우리가 할당량을 채울 수 있게 해주나?우리 팀의 효율성과 생산성을 높이고 시너지를 향상시켜 줄 수 있나?어쩌고 저쩌고 어쩌고저쩌고어쩌고저쩌고어ㅉ까어ㅉ꺼ㅗ어저고 ㅓㄱ쇼ㅕㅇㅇㄴ\" 그 뚱뚱한 입술이 회사 전화기 음성처럼 푸드덕거리며 움직이는데, 말 같지도 않은 소리를 계속 늘어놓는 거야. 그러더니 결국 만족스럽게 깊은 숨을 쉬더니, 마치 이긴 것처럼 돌아가서 일하더라. 죽여버릴거야 진짜!! 데스몬드, 왜 널 책상 위에 올려두지 않았을까, 리치한테 했던 것처럼. 그럼 너도 팀의 시너지가 뭔지 알겠지.\n\n나는 그 통풍구 안에 포도주스를 조금 부었어. 나 진짜 바보 같지. 왜 했는지도 모르겠어. 많이 넣은 것도 아니고. 그냥... 해도 괜찮을 것 같았어.\n";
                                    break;
                            }
                        }
                        break;
                    case "buy":
                        foreach (CompatibleNoun noun in keyword.compatibleNouns)
                        {
                            switch (noun.result.name)
                            {
                                //아이템 구매
                                case "buyProFlashlight1":
                                    noun.result.displayText = "프로 손전등을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";

                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 프로 손전등을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    noun.result.terminalOptions[0].noun.word = Plugin.confirmString;
                                    noun.result.terminalOptions[1].result.displayText = "주문을 취소했습니다.\n\n";
                                    noun.result.terminalOptions[1].noun.word = Plugin.denyString;
                                    break;
                                case "buyFlash":
                                    noun.result.displayText = "손전등을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 손전등을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyLockpickers":
                                    noun.result.displayText = "자물쇠 따개를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 자물쇠 따개를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyBoombox":
                                    noun.result.displayText = "붐박스를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 붐박스를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyExtensLadder":
                                    noun.result.displayText = "연장형 사다리를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 연장형 사다리를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyJetpack":
                                    noun.result.displayText = "제트팩을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 제트팩을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyRadarBooster":
                                    noun.result.displayText = "레이더 부스터를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 레이더 부스터를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyShovel":
                                    noun.result.displayText = "철제 삽을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 철제 삽을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buySpraypaint":
                                    noun.result.displayText = "스프레이 페인트를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 스프레이 페인트를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyStunGrenade":
                                    noun.result.displayText = "기절 수류탄을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 기절 수류탄을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyTZP":
                                    noun.result.displayText = "TZP-흡입제를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 TZP-흡입제를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyWalkieTalkie":
                                    noun.result.displayText = "무전기를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 무전기를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyZapGun":
                                    noun.result.displayText = "잽건을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 잽건을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyWeedkiller":
                                    noun.result.displayText = "제초제를 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 제초제를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;
                                case "buyBeltBag":
                                    noun.result.displayText = "전술 벨트 배낭을 주문하려고 합니다. 수량: [variableAmount]. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "[variableAmount]개의 전술 벨트 배낭을 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n우리의 계약자는 작업 중에도 빠른 무료 배송 혜택을 누릴 수 있습니다! 구매한 모든 상품은 1시간마다 대략적인 위치에 도착합니다.\n\n\n";
                                    break;

                                //컴퍼니 크루저, 트럭 구매
                                case "buyCruiser":
                                    noun.result.displayText = "컴퍼니 크루저를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "컴퍼니 크루저를 주문했습니다. 당신의 현재 소지금은 [playerCredits]입니다.\n\n당사는 이 제품의 품질을 매우 확신하며, 보증이 제공됩니다! 크루저를 분실하거나 파손한 경우, 한 번 무료로 교체할 수 있습니다. 차량을 운반하는 동안에는 아이템을 구매할 수 없습니다.\n\n";
                                    break;

                                    //가구 구매
                                case "CozyLightsBuy1":
                                    noun.result.creatureName = "아늑한 조명";
                                    noun.result.displayText = "아늑한 조명을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "아늑한 조명을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n전등 스위치를 사용해 아늑한 조명을 활성화하세요.\n\n";
                                    break;
                                case "GreenSuitBuy1":
                                    noun.result.creatureName = "초록색 슈트";
                                    noun.result.displayText = "초록색 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "초록색 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "HazardSuitBuy1":
                                    noun.result.creatureName = "방호복 슈트";
                                    noun.result.displayText = "방호복 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "방호복 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "LoudHornBuy1":
                                    noun.result.creatureName = "시끄러운 경적";
                                    noun.result.displayText = "시끄러운 경적을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "시끄러운 경적을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n코드를 길게 당겨 시끄러운 경적을 활성화합니다.\n\n";
                                    break;
                                case "PajamaSuitBuy1":
                                    noun.result.creatureName = "파자마 슈트";
                                    noun.result.displayText = "파자마 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "파자마 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "PurpleSuitBuy1":
                                    noun.result.creatureName = "보라색 슈트";
                                    noun.result.displayText = "보라색 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "보라색 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "RomTableBuy1":
                                    noun.result.creatureName = "로맨틱한 테이블";
                                    noun.result.displayText = "로맨틱한 테이블을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "로맨틱한 테이블을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "SofaChairBuy":
                                    noun.result.creatureName = "소파 의자";
                                    noun.result.displayText = "소파 의자를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "소파 의자를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "MicrowaveBuy":
                                    noun.result.creatureName = "전자레인지";
                                    noun.result.displayText = "전자레인지를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "전자레인지를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "FridgeBuy":
                                    noun.result.creatureName = "냉장고";
                                    noun.result.displayText = "냉장고를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "냉장고를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "ElectricChairBuy":
                                    noun.result.creatureName = "전기 의자";
                                    noun.result.displayText = "전기 의자를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "전기 의자를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "DoghouseBuy":
                                    noun.result.creatureName = "개집";
                                    noun.result.displayText = "개집을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "개집을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "ClassicPaintingBuy":
                                    noun.result.creatureName = "고전적인 그림";
                                    noun.result.displayText = "고전적인 그림을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "고전적인 그림을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n";
                                    break;
                                case "ShowerBuy1":
                                    noun.result.creatureName = "샤워 부스";
                                    noun.result.displayText = "샤워 부스를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "샤워 부스를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "TableBuy1":
                                    noun.result.creatureName = "테이블";
                                    noun.result.displayText = "테이블을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "테이블을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "TeleporterBuy1":
                                    noun.result.creatureName = "순간이동기";
                                    noun.result.displayText = "순간이동기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "순간이동기를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n버튼을 눌러 순간이동기를 활성화합니다. 현재 함선의 레이더에 모니터링 중인 사람을 순간이동시킵니다. 순간이동기를 통해 보유한 아이템은 보관할 수 없습니다. 재충전하는 데 약 10초가 걸립니다.\n\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "InverseTeleporterBuy":
                                    noun.result.creatureName = "역방향 순간이동기";
                                    noun.result.displayText = "역방향 순간이동기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "역방향 순간이동기를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n버튼을 누르고 역방향 순간이동기가 활성화되는 동안 위에 올라가세요.\n\n";
                                    break;
                                case "TelevisionBuy1":
                                    noun.result.creatureName = "텔레비전";
                                    noun.result.displayText = "텔레비전을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "텔레비전을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "ToiletBuy1":
                                    noun.result.creatureName = "변기";
                                    noun.result.displayText = "변기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "변기를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "WelcomeMatBuy":
                                    noun.result.creatureName = "웰컴 매트";
                                    noun.result.displayText = "웰컴 매트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "웰컴 매트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "SignalTranslatorBuy":
                                    noun.result.creatureName = "신호 해석기";
                                    noun.result.displayText = "신호 해석기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "신호 해석기를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n\n신호 해석기는 'transmit' 명령 뒤에 10글자 미만의 메시지를 입력해서 사용할 수 있습니다.\n\n";
                                    break;
                                case "FishBowlBuy":
                                    noun.result.creatureName = "금붕어";
                                    noun.result.displayText = "금붕어 어항을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "금붕어 어항을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "DiscoBallBuy":
                                    noun.result.creatureName = "디스코 볼";
                                    noun.result.displayText = "디스코 볼을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "디스코 볼을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n전등 스위치를 사용해 디스코를 시작합니다.\n\n";
                                    break;
                                case "RecordPlayerBuy":
                                    noun.result.creatureName = "레코드 플레이어";
                                    noun.result.displayText = "레코드 플레이어를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "레코드 플레이어를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "JackOLanternBuy":
                                    noun.result.creatureName = "잭오랜턴";
                                    noun.result.displayText = "잭오랜턴을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "잭오랜턴을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                                case "BunnySuitBuy":
                                    noun.result.creatureName = "토끼 슈트";
                                    noun.result.displayText = "토끼 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "토끼 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "BeeSuitBuy":
                                    noun.result.creatureName = "꿀벌 슈트";
                                    noun.result.displayText = "꿀벌 슈트를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "꿀벌 슈트를 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n\n";
                                    break;
                                case "PlushiePajamaManBuy":
                                    noun.result.creatureName = "인형 파자마 맨";
                                    noun.result.displayText = "인형 파자마 맨을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                                    noun.result.terminalOptions[0].result.displayText = "인형 파자마 맨을 주문했습니다! 당신의 현재 소지금은 [playerCredits]입니다.\n함선 안의 물체를 재배치하려면 [B]를 누르세요. 배치를 확정하려면 [V]를 누르세요.\n\n";
                                    break;
                            }
                        }
                        break;
                    case "info":
                        foreach (CompatibleNoun noun in keyword.compatibleNouns)
                        {
                            switch (noun.result.name)
                            {
                                case "FlashlightInfo":
                                    noun.result.displayText = "\n길어진 배터리 수명과 더욱 밝아진 전구 덕분에 동료들이 더 이상 어둠 속에 헤매지 않아도 됩니다!\n\n";
                                    break;
                                case "MapperInfo":
                                    noun.result.displayText = "\n빛 감지 및 거리 측정 기능을 사용하여 주변 환경을 한눈에 볼 수 있는 가장 진보된 지도 장치입니다.\n\n";
                                    break;
                                case "BBbulbInfo":
                                    noun.result.displayText = "\n가성비 좋은 광원입니다. 심지어 방수까지 됩니다!\n\n";
                                    break;
                                case "ShovelInfo":
                                    noun.result.displayText = "\n자기 방어를 위해!\n\n";
                                    break;
                                case "ZapGunInfo":
                                    noun.result.displayText = "\n80,000 볼트 이상을 보낼 수 있는 가장 특화된 자기 보호 장비!\n\n최대한 오래 표적을 유지하려면 총을 좌우로 당겨 광선을 직선으로 유지해야 합니다. 전류가 흐르는 동안에만 기절시킬 수 있습니다.\n\n";
                                    break;
                                case "AutopickerInfo":
                                    noun.result.displayText = "\n자물쇠 따개는 업무 효율을 높일 수 있는 무한한 잠재력을 열어줍니다. 독점 AI 소프트웨어로 구동되며, 잠긴 문을 열 수 있도록 도와줍니다.\n\n";
                                    break;
                                case "WalkieTalkieInfo":
                                    noun.result.displayText = "\n연락을 유지하는 데 유용합니다! 무전기가 인벤토리에 있으면 다른 플레이어의 목소리를 들을 수 있습니다. 음성을 전송하려면 손에 든 상태로 버튼을 눌러야 합니다.\n\n";
                                    break;
                                case "SurvivalKitInfo":
                                    noun.result.displayText = "\n생존 키트에는 이러한 필수품이 편리한 패키지에 포함되어 있습니다! :\n\n* 손전등 x4\n* 무전기 x4\n* 철제 삽 x2";
                                    break;
                                case "BoomboxInfo":
                                    noun.result.displayText = "\n이 흥겨운 음악은 팀원들의 사기를 복돋우는 데 아주 좋을 것입니다!\n\n";
                                    break;
                                case "JetpackInfo":
                                    noun.result.displayText = "\n이 장비를 이용해 어디든 갈 수 있습니다! 책임감 있게 사용하세요!\n\n";
                                    break;
                                case "TZPChemicalInfo":
                                    noun.result.displayText = "\n이 안전하고 합법적인 약을 투여하면 업무 수행에 큰 도움이 됩니다! 무거운 물건을 들고 먼 거리를 이동하는 능력이 누구에게도 뒤쳐지지 않을 것입니다! 경고: TZP 가스는 장시간 노출 시 뇌에 영향을 미칠 수 있습니다. 용기와 함께 제공된 사용 설명서를 따르십시오.\n나눠주는 걸 잊지 마세요!\n\n";
                                    break;
                                case "ExtensionLadderInfo":
                                    noun.result.displayText = "\n연장형 사다리는 9미터까지 올라갈 수 있습니다! 이 사다리를 이용해 절벽을 오르고 별을 향해 손을 뻗어 보세요! 배터리를 절약하기 위해 연장형 사다리는 18초 후에 자동으로 수납됩니다.\n\n";
                                    break;
                                case "RadarBoosterInfo":
                                    noun.result.displayText = "\n레이더 부스터는 다양한 용도로 사용할 수 있습니다!\n\n레이더 부스터의 이름 앞에 \"SWITCH\" 명령어를 입력하여 메인 모니터에서 레이더 부스터를 확인합니다. 활성화되어 있어야만 합니다.\n\n레이더 부스터의 이름 앞에 \"PING\" 명령어를 입력하여 장치에서 특수한 소리를 재생합니다.\n\n";
                                    break;
                                case "WeedKillerInfo":
                                    noun.result.displayText = "성가신 잡초를 처리하세요! 잡초의 뿌리를 바라보고 방아쇠를 반복해서 누르기만 하면 됩니다!";
                                    break;
                                case "BeltBagInfo":
                                    noun.result.displayText = "우리 회사의 전술 벨트 배낭은 모든 것을 가지고 다니고 싶은 분들을 위한 필수품입니다! 한 번에 15개의 유틸리티 아이템을 넣을 수 있습니다. 다시는 쓸모없다고 느낄 일이 없을 겁니다!\n\n[Q]를 눌러 가방에 아이템을 보관한 다음, 클릭하여 내용물을 확인하고 아이템을 드래그하면 됩니다. 가방을 허리에 착용한 상태에서는 다른 플레이어도 배낭을 사용할 수 있습니다.\n\n";
                                    break;

                                case "CruiserInfo":
                                    noun.result.displayText = "컴퍼니 크루저는 필요한 만큼 많은 물품, 심지어 동료 직원까지 운반할 수 있는 배달 트럭입니다! 컴퍼니 크루저를 구매하시면 무상 보증이 제공됩니다. 당사는 내구성과 유용성에 대한 자신감으로 가득하기 때문이죠!\n\n지침서가 함께 제공되므로 사용법을 꼭 읽어보시길 바랍니다.";
                                    break;

                                case "LoudHornInfo":
                                    noun.result.displayText = "\n무전기 없이도 모든 팀원과 원거리에서 소통할 수 있습니다! 경적은 어디서나 들을 수 있습니다. 하지만 무슨 뜻일까요? 그건 당신에게 달려 있습니다!\n\n";
                                    break;
                                case "TeleporterInfo":
                                    noun.result.displayText = "버튼을 눌러 순간이동기를 활성화합니다. 현재 함선의 레이더에 모니터링 중인 사람을 순간이동시킵니다. 순간이동기를 통해 보유한 아이템은 보관할 수 없습니다. 재충전하는 데 약 10초가 걸립니다.\n\n";
                                    break;
                                case "InverseTeleporterInfo":
                                    noun.result.displayText = "\n역방향 순간이동기는 순간이동기의 변형으로, 함선 외부의 임의 위치로 순간이동합니다. 모든 아이템은 이동 전에 순간이동기 위에 떨어집니다. 역방향 순간이동기는 모든 플레이어가 한 번에 사용할 수 있으며 재사용 대기시간은 3.5분입니다.\n\n면책 조항: 역방향 순간이동기는 사용자를 밖으로만 이동시킬 수 있고 안으로 이동시킬 수 없으며, 갇힐 수 있습니다. 회사는 양자 얽힘 및 불운으로 인한 부상이나 머리와 팔다리의 교체에 대해 책임을 지지 않습니다.\n\n";
                                    break;
                                case "SignalTranslatorInfo":
                                    noun.result.displayText = "\n'transmit' 명령을 사용하여 모든 팀원에게 메시지를 전송합니다. 메시지는 10자 이내여야 합니다.\n\n";
                                    break;

                                //Planets
                                case "CompanyBuildingInfo":
                                    noun.result.displayText = "회사가 귀하의 상품을 [companyBuyingPercent]에 매입하고 있습니다. \n\n업무 중에 수집한 귀중한 폐품을 여기로 가져가세요. 판매 비율은 매시간 업데이트되며 며칠에 걸쳐 변동됩니다. \n\n";
                                    break;

                                case "ExpInfo":
                                    noun.result.displayText = "41-익스페리멘테이션\n\n----------------------\n\n조건: 건조하며, 거주 가능성이 낮습니다. 산업 인공물로 인해 악화되었습니다.\n\n역사: 거대 기체 행성인 빅 그린과 가까이 있기 때문에 꽤 오랫동안 발견되지 않았습니다. 그러나 비밀리에 사용되어 온 것으로 보입니다.\n\n동물군: 알 수 없음\n\n";
                                    break;
                                case "AssInfo":
                                    noun.result.displayText = "220-어슈어런스\n----------------------\n\n조건: 쌍둥이 달인 41-익스페리멘테이션과 비슷하지만, 훨씬 더 울퉁불퉁하고 풍화된 지형이 특징입니다. \n\n역사: 220-어슈어런스는 최근에 발견되었으며, 41-익스페리멘테이션 이전에 발견되었습니다.\n\n동물군: 알 수 없음\n\n";
                                    break;
                                case "VowInfo":
                                    noun.result.displayText = "56-보우\n----------------------\n\n조건: 습함.\n\n역사: 보우는 여러 대륙에 걸쳐 많은 식민지가 존재했던 것으로 보이지만, 현재는 생명의 흔적을 찾아볼 수 없어 수수께끼로 남아있습니다.\n\n동물군: 다양하고 식물이 풍부함.\n\n";
                                    break;

                                case "MarchInfo":
                                    noun.result.displayText = "\n61-머치\n----------------------\n\n조건: 지속적으로 비가 내립니다. 지형도 더욱 광활합니다.\n\n역사: 이 달은 쌍둥이 달인 보우 때문에 눈에 띄지 않습니다.\n\n동물군: 알 수 없음\n\n";
                                    break;
                                case "OffenseInfo":
                                    noun.result.displayText = "\n21-오펜스\n----------------------\n\n조건: 사촌격 위성인 어슈어런스에서 분리된 것으로 알려진 오펜스는 어슈어런스와 비슷한 들쭉날쭉하고 건조한 환경을 갖추고 있지만 생태계에서 차이를 보입니다.\n\n역사: 21-오펜스는 소행성 위성으로 분류되며 수백 년 이상 자체적으로 존재하지 않은 것으로 보입니다. 이곳의 산업 인공물들은 손상을 입었으며, 21-오펜스가 분리되기 훨씬 전에 만들어진 것으로 추정됩니다.\n\n동물군: 경쟁력 있고 강화된 생태계 때문에 공격적인 생명체가 많습니다. 21-오펜스를 방문하는 여행객들은 이곳이 심장이 약한 사람들을 위한 곳이 아니라는 것을 알아야 합니다.\n\n";
                                    break;
                                case "AdamanceInfo":
                                    noun.result.displayText = "\n20-애더먼스\n----------------------\n\n조건: 침식과 지리적 힘으로 인해 깊은 계곡과 산으로 이루어졌습니다.\n\n역사: 보우, 머치, 애더먼스는 노 서비스 궤도를 돌고 있으며, 그 중 애더먼스는 구어체로 \"그린 위치\"라고 불리는 것들 중 가장 크고 오래되었습니다.\n\n동물군: 애더먼스에는 작은 크기의 잡식성 동물로 구성된 활기차고 다양한 생태계의 본거지입니다.\n\n";
                                    break;

                                case "RendInfo":
                                    noun.result.displayText = "\n85-렌드\n----------------------\n\n조건: 백색 왜성 주위를 공전하고 있어 혹독하고 추운 환경을 조성하고 있습니다. 지속적인 눈보라 때문에 가시성이 좋지 않습니다.\n\n역사: 몇몇 유명한 여행자들이 이곳에서 실종되어 명성을 얻었지만, 행성의 환경 때문에 시신을 찾을 가능성은 거의 없어 보입니다.\n\n동물군: 이곳에 다세포 생명체가 존재할 가능성은 거의 없습니다.\n\n";
                                    break;
                                case "DineInfo":
                                    noun.result.displayText = "\n7-다인\n----------------------\n\n조건: 백색 왜성 주위를 공전하고 있어 혹독하고 추운 환경을 조성하고 있습니다. 지속적인 눈보라 때문에 가시성이 좋지 않습니다.\n\n역사: 몇몇 유명한 여행자들이 이곳에서 실종되어 명성을 얻었지만, 행성의 환경 때문에 시신을 찾을 가능성은 거의 없어 보입니다.\n\n동물군: 이곳에 다세포 생명체가 존재할 가능성은 거의 없습니다.\n\n";
                                    break;
                                case "TitanInfo":
                                    noun.result.displayText = "\n8-타이탄\n----------------------\n\n조건: 얼어붙었으며 평평한 지형을 갖추고 있습니다.\n\n역사: 이 위성은 자원을 얻기 위해 채굴된 것으로 보입니다. 거대한 산업 단지 안에서 길을 잃기 쉽습니다. 곳곳에 입구가 많이 흩어져 있습니다.\n\n동물군: 광대한 터널 네트워크에 위험한 생명체들이 서식한다는 소문이 있습니다.\n\n";
                                    break;

                                case "ArtificeInfo":
                                    noun.result.displayText = "\n68-아터피스\n----------------------\n\n조건: 한때 활기 넘치는 위성이었으나 현재는 버려진 시설들만이 남아 있습니다. \n\n역사: 최소 200년 전으로 거슬러 올라가는 무기와 기밀 기술이 지표면의 다양한 유적지에서 발견되었습니다.\n\n동물군: 아직 작동 중인 기계가 방치되어 있다는 소문이 있습니다.\n\n";
                                    break;
                                case "EmbrionInfo":
                                    noun.result.displayText = "\n5-엠브리언\n----------------------\n\n조건: 황량하고 평평하며 공기가 통하지 않는 엠브리언의 표면층은 거의 전부가 자수정 석영으로 이루어져 있는데, 이는 엠브리언이 가장 가까운 별과의 거리와 그로 인한 지열 활동 덕분에 가능한 일입니다. 엠브리언의 표면은 매우 뜨겁습니다.\n\n조건: 엠브리언은 불과 지난 세기에 발견되었습니다. 공식적으로 발견되기 훨씬 전부터 올드 버드의 시험장으로 사용되었던 것으로 보입니다. 이러한 위험성 때문에 엠브리언의 상태는 규탄됨으로 표시되어 있습니다. 엠브리언의 표면 아래 광대한 동굴 네트워크에 다양한 지열 생태계가 번성하고 있을 것이라는 학설이 있습니다.\n\n동물군: 엠브리언에는 생물학적 생명체가 존재하지 않습니다.\n\n\n";
                                    break;

                            }
                        }
                        break;
                    case "route":
                        foreach (CompatibleNoun noun in keyword.compatibleNouns)
                        {
                            noun.result.terminalOptions[0].result.displayText = noun.result.terminalOptions[0].result.displayText.Replace("You have cancelled the order.", "주문을 취소했습니다.");
                            if (noun.result.displayText.Contains("The Company is buying"))
                            {
                                noun.result.displayText = noun.result.displayText.Replace("The Company is buying at ", "현재 회사가 물품을 ");
                                noun.result.displayText = noun.result.displayText.Replace(".\n\nDo you want to route the autopilot to the Company building?", "에 매입하고 있습니다.\n\n회사 건물로 이동할까요?");

                                noun.result.displayText = noun.result.displayText.Replace(". \n\nDo you want to route the autopilot to the Company building?", "에 매입하고 있습니다.\n\n회사 건물로 이동할까요?");

                                noun.result.displayText = noun.result.displayText.Replace(". \r\n\r\nDo you want to route the autopilot to the Company building?", "에 매입하고 있습니다.\r\n\r\n회사 건물로 이동할까요?");

                                noun.result.displayText = noun.result.displayText.Replace("Please CONFIRM or DENY.", Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요.");

                                noun.result.displayText = "현재 회사가 물품을 [companyBuyingPercent]에 매입하고 있습니다. \n\n회사 건물로 이동할까요?\n\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n\n";
                            }
                            if (noun.result.displayText.Contains("The cost to route to"))
                            {
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 41-Experimentation is", "41-익스페리멘테이션의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 220-Assurance is", "220-어슈어런스의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 56-Vow is", "56-보우의 이동 비용은");

                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 21-Offense is", "21-오펜스의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 61-March is", "61-머치의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 20-Adamance is", "20-애더먼스의 이동 비용은");

                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 85-Rend is", "85-렌드의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 7-Dine is", "7-다인의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 8-Titan is", "8-타이탄의 이동 비용은");

                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 5-Embrion is", "5-엠브리언의 이동 비용은");
                                noun.result.displayText = noun.result.displayText.Replace("The cost to route to 68-Artifice is", "68-아터피스의 이동 비용은");

                                //모드행성

                                if (Plugin.translateModdedContent)
                                {
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 13-Kast is", "13 카스트의 이동 비용은"); 
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 57 Asteroid-13 is", "57 아스테로이드-13의 이동 비용은"); 
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 44 Atlantica is", "44 아틀란티카의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 42 Cosmocos is", "42 코스모코스의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 48 Desolation is", "48 디솔레이션의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 154 Etern is", "154 이턴의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 25 Fission-C is", "25 피션-C의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 36 Gloom is", "36 글룸의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 147 Gratar is", "147 그라타의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 46 Infernis is", "46 인퍼니스의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 84 Junic is", "84 주닉의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 134 Oldred is", "134 올드레드의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 94 Polarus is", "94 폴라러스의 이동 비용은");
                                    noun.result.displayText = noun.result.displayText.Replace("The cost to route to 76 Acidir is", "76 어시디어의 이동 비용은");
                                }

                                noun.result.displayText = noun.result.displayText.Replace(". It is \ncurrently", "입니다.\n 이 위성의 현재 날씨는");
                                noun.result.displayText = noun.result.displayText.Replace(". It is currently", "입니다.\n 이 위성의 현재 날씨는");
                                noun.result.displayText = noun.result.displayText.Replace(" on this moon.", "입니다.");
                                noun.result.displayText = noun.result.displayText.Replace("Please CONFIRM or DENY.", Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요.");

                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to the Company building.", "회사 건물로 이동합니다.");

                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 41-Experimentation.", "41-익스페리멘테이션으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 220-Assurance.", "220-어슈어런스로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 56-Vow.", "56-보우로 이동합니다.");

                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 21-Offense.", "21-오펜스로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 61-March.", "61-머치로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 20-Adamance.", "20-애더먼스로 이동합니다.");

                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 85-Rend.", "85-렌드로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 7-Dine.", "7-다인으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 8-Titan.", "8-타이탄으로 이동합니다.");

                                //모드행성
                                if (Plugin.translateModdedContent)
                                {
                                    if (Plugin.translateModdedContent)
                                    {
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 13-Kast", "13 카스트로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 57 Asteroid-13", "57 아스테로이드-13로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 44 Atlantica", "44 아틀란티카로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 42 Cosmocos", "42 코스모코스로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 48 Desolation", "48 디솔레이션으로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 154 Etern", "154 이턴으로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 25 Fission-C", "25 피션-C로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 36 Gloom", "36 글룸로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 147 Gratar", "147 그라타로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 46 Infernis", "46 인퍼니스로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 84 Junic", "84 주닉으로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 134 Oldred", "134 올드레드로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 94 Polarus", "94 폴라러스로 이동합니다.");
                                        noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 76 Acidir", "76 어시디어로 이동합니다.");
                                    }
                                }

                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Your new balance is ", "당신의 현재 소지금은 ");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace(".\n\nGood luck.", "입니다.\n\n행운을 빕니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace(".\n\nPlease enjoy your flight.", "입니다.\n\n편안한 비행 되세요.");
                                //noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Please enjoy your flight.", "편안한 비행 되세요.");
                            }
                        }
                        break;
                    case "help":
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("To see the list of moons the autopilot can route to.", "항로를 지정할 위성 목록을 봅니다.");
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("To see the company store's selection of useful items.", "회사 상점의 유용한 아이템 목록을 봅니다.");
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("To see the list of wildlife on record.", "기록된 생명체 목록을 봅니다.");
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("To access objects placed into storage.", "저장고에 있는 물체에 접근합니다.");
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("To see the list of other commands", "기타 명령어 목록을 봅니다.");
                        
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "도움말";
                            keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace(">MOONS", ">위성");
                            keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace(">STORE", ">상점");
                            keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace(">BESTIARY", ">도감");
                            keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace(">STORAGE", ">저장고");
                            keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace(">OTHER", ">기타");
                        }
                        break;
                    case "storage":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "저장고";
                        }
                        
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("While moving furniture with [B], you can press [X] to send it to storage. You can call it back from storage here.", "[B]로 가구를 이동시킬 수 있고, [X]를 눌러 저장고에 보관할 수 있습니다. 보관한 가구는 저장고에서 다시 꺼낼 수 있습니다.");
                        keyword.specialKeywordResult.displayText = keyword.specialKeywordResult.displayText.Replace("These are the items in storage:", "보관 중인 물품 목록:");
                        break;
                    case "moons":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "위성";
                        }
                        if (GameNetworkManager.Instance.gameVersionNum <= 49)
                        {
                            keyword.specialKeywordResult.displayText = "위성 카탈로그에 오신 것을 환영합니다.\n함선의 경로를 지정하려면 ROUTE를 입력하세요.\n위성에 대해 알아보려면 INFO를 입력하세요.\n____________________________\n\n* 회사 건물   //   [companyBuyingPercent]에 매입 중.\n\n* 익스페리멘테이션 [planetTime]\n* 어슈어런스 [planetTime]\n* 보우 [planetTime]\n\n* 오펜스 [planetTime]\n* 머치 [planetTime]\n\n* 렌드 [planetTime]\n* 다인 [planetTime]\n* 타이탄 [planetTime]\n\n";
                        } else
                        {
                            keyword.specialKeywordResult.displayText = "위성 카탈로그에 오신 것을 환영합니다.\n함선의 경로를 지정하려면 ROUTE를 입력하세요.\n위성에 대해 알아보려면 INFO를 입력하세요.\n____________________________\n\n* 회사 건물   //   [companyBuyingPercent]에 매입 중.\n\n* 익스페리멘테이션 [planetTime]\n* 어슈어런스 [planetTime]\n* 보우 [planetTime]\n\n* 오펜스 [planetTime]\n* 머치 [planetTime]\n* 애더먼스 [planetTime]\n\n* 렌드 [planetTime]\n* 다인 [planetTime]\n* 타이탄 [planetTime]\n\n";
                        }
                        break;
                    case "store":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "상점";
                        }
                        if (GameNetworkManager.Instance.gameVersionNum >= 55)
                        {
                            keyword.specialKeywordResult.displayText = "회사 상점에 오신 것을 환영합니다. \n아이템 이름 앞에 BUY와 INFO를 입력해보세요. \n숫자를 입력하여 도구를 여러 개 주문할 수 있습니다.\n____________________________\n\n[buyableItemsList]\n[buyableVehiclesList]\n\n함선 강화:\n* 시끄러운 경적    //    가격: $100\n* 신호 해석기    //    가격: $255\n* 순간이동기    //    가격: $375\n* 역방향 순간이동기    //    가격: $425\n\n함선 장식 목록은 할당량별로 순환됩니다. 다음 주에 꼭 다시 확인해보세요:\n------------------------------\n[unlockablesSelectionList]\n\n";
                        }else
                        {
                            keyword.specialKeywordResult.displayText = "회사 상점에 오신 것을 환영합니다. \n아이템 이름 앞에 BUY와 INFO를 입력해보세요. \n숫자를 입력하여 도구를 여러 개 주문할 수 있습니다.\n____________________________\n\n[buyableItemsList]\n\n함선 강화:\n* 시끄러운 경적    //    가격: $100\n* 신호 해석기    //    가격: $255\n* 순간이동기    //    가격: $375\n* 역방향 순간이동기    //    가격: $425\n\n함선 장식 목록은 할당량별로 순환됩니다. 다음 주에 꼭 다시 확인해보세요:\n------------------------------\n[unlockablesSelectionList]\n\n";
                        }
                        break;
                    case "other":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "기타";
                            keyword.specialKeywordResult.displayText = "기타 명령어:\n\n>VIEW MONITOR\n메인 모니터의 지도 카메라를 켜고 끕니다.\n\n>SWITCH [플레이어 이름]\n메인 모니터에서 볼 플레이어를 전환합니다.\n\n>PING [레이더 부스터 이름]\n레이더 부스터에 소음을 재생합니다.\n\n>TRANSMIT [메세지]\n신호 해석기로 메세지를 전송합니다\n\n>SCAN\n현재 행성에 남아 있는 아이템 개수를 스캔합니다.\n\n\n";
                        }else
                        {
                            keyword.specialKeywordResult.displayText = "기타 명령어:\n\n>VIEW MONITOR\n메인 모니터의 지도 카메라를 켜고 끕니다.\n\n>SWITCH [플레이어 이름]\n메인 모니터에서 볼 플레이어를 전환합니다.\n\n>PING [레이더 부스터 이름]\n레이더 부스터에 소음을 재생합니다.\n\n>TRANSMIT [메세지]\n신호 해석기로 메세지를 전송합니다\n\n>SCAN\n현재 행성에 남아 있는 아이템 개수를 스캔합니다.\n\n\n";
                        }
                        break;
                        /*
                    case "view":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "확인";
                        }
                        foreach (CompatibleNoun noun in keyword.compatibleNouns)
                        {
                            if (noun.noun.word == "monitor")
                            {
                                noun.noun.word = "모니터";
                            }
                        }
                        break;
                    case "switch":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "전환";
                        }
                        break;
                    case "scan":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "스캔";
                        }
                        break;
                        */
                    case "sigurd":
                        keyword.specialKeywordResult.displayText = "시거드의 일지 기록\n\n일지를 읽으려면, 이름 앞에 \"확인\"를 입력하세요.\n---------------------------------\n\n[currentUnlockedLogsList]\n\n\n\n";
                        break;
                    case "bestiary":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "도감";
                        }
                        keyword.specialKeywordResult.displayText = "생명체 도감\n\n생명체 파일에 접근하려면, 이름 뒤에 \"INFO\"를 입력하세요.\n---------------------------------\n\n[currentScannedEnemiesList]\n\n\n";
                        break;
                    case "eject":
                        if (Plugin.fullyKoreanMoons)
                        {
                            keyword.word = "사출";
                        }
                        keyword.specialKeywordResult.displayText = "당신을 포함한 모든 팀원을 사출시키시겠습니까? 이륙한 상태여야 합니다.\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요." + "\n\n";
                        keyword.specialKeywordResult.terminalOptions[0].result.displayText = "사출을 시작합니다.\n\n";
                        keyword.specialKeywordResult.terminalOptions[1].result.displayText = "사출 절차를 취소했습니다.\n\n";
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


                    case "cruiser":
                        keyword.word = "크루저";
                        break;
                    case "weed killer":
                        keyword.word = "제초제";
                        break;
                    case "belt bag":
                        keyword.word = "벨트";
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
                        keyword.word = "개코";
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
                    case "tulip":
                        keyword.word = "튤립";
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
                    case "barber":
                        keyword.word = "이발사";
                        break;
                    case "vain":
                        keyword.word = "은폐";
                        break;
                    case "kidnapper":
                        keyword.word = "납치범";
                        break;
                    case "maneater":
                        keyword.word = "맨이터";
                        break;
                    case "giant":
                        keyword.word = "거대";
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
                    case "sofa":
                        keyword.word = "소파";
                        break;
                    case "microwave":
                        keyword.word = "전자레인지";
                        break;
                    case "fridge":
                        keyword.word = "냉장고";
                        break;
                    case "electric":
                        keyword.word = "전기";
                        break;
                    case "classic":
                        keyword.word = "고전적인";
                        break;
                    case "dog house":
                        keyword.word = "개집";
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
