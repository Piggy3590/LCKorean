using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        public static bool vehicleChecked;
        private static FieldInfo modifyingTextField = AccessTools.Field(typeof(Terminal), "modifyingText");
        private static FieldInfo totalCostField = AccessTools.Field(typeof(Terminal), "totalCostOfItems");

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

        /*
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
        */

        private static void TranslateTerminalScreen(string key, TMP_InputField screenText, ref string ___currentText)
        {
            //string translatedText = TranslationManager.ReplaceArrayText(screenText.text, "Terminal", key);
            string translatedText = TranslationManager.ReplaceArrayTextAll(screenText.text, "Terminal");
            translatedText = TranslationManager.ReplaceArrayTextAll(screenText.text, "Planets");
            screenText.text = translatedText;
            ___currentText = translatedText;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch("LoadNewNode")]
        public static bool LoadNewNode_Prefix(Terminal __instance, TerminalNode node)
        {
            modifyingTextField.SetValue(__instance, true);
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

            return false;
        }
        
        private static string TextPostProcess(Terminal __instance, string modifiedDisplayText, TerminalNode node)
        {
            int num = modifiedDisplayText.Split("[planetTime]", StringSplitOptions.None).Length - 1;
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
					    text = TranslationManager.GetArrayTranslation("Terminal", "Locked");
				    }
				    else if (__instance.moonsCatalogueList[num2].currentWeather == LevelWeatherType.None)
				    {
					    text = "";
				    }
				    else
				    {
                        text = "(" +
                               $"{TranslationManager.GetArrayTranslation("Planets", __instance.moonsCatalogueList[num2].currentWeather.ToString())}" +
                               ")";
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
                        text = TranslationManager.GetArrayTranslation("Planets", "mild weather");
				    }
				    else
				    {
					    text = TranslationManager.GetArrayTranslation("Planets", StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString())
                               ?? "";
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[currentPlanetTime]", text);
			    }
		    }
		    catch
		    {
			    Debug.Log(string.Format("Exception occured on terminal while setting node planet info; current node displayPlanetInfo:{0}", node.displayPlanetInfo));
		    }
		    if (modifiedDisplayText.Contains("[warranty]"))
		    {
			    if (__instance.hasWarrantyTicket)
			    {
                    //TranslationManager.GetArrayTranslation(
				    modifiedDisplayText = modifiedDisplayText.Replace("[warranty]", 
                        TranslationManager.GetArrayTranslation("Terminal", "FreeWarranty", 0, false, "[warranty]"));
			    }
			    else
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[warranty]", "");
			    }
		    }
		    if (modifiedDisplayText.Contains("[currentScannedEnemiesList]"))
		    {
			    if (__instance.scannedEnemyIDs == null || __instance.scannedEnemyIDs.Count <= 0)
                {
                    modifiedDisplayText = modifiedDisplayText.Replace("[currentScannedEnemiesList]",
                        TranslationManager.GetArrayTranslation("Terminal",
                            "NoCreatureData",
                            0,
                            false,
                            "No data collected on wildlife. Scans are required."));
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
						    stringBuilder.Append($" ({
                                TranslationManager.GetArrayTranslation("Terminal","NEW")})");
					    }
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[currentScannedEnemiesList]", stringBuilder.ToString());
                    //TranslationManager.GetArrayTranslation
			    }
		    }
		    if (modifiedDisplayText.Contains("[buyableItemsList]"))
		    {
			    if (__instance.buyableItemsList == null || __instance.buyableItemsList.Length == 0)
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[buyableItemsList]", TranslationManager.GetArrayTranslation("Terminal", "NoStock"));
			    }
			    else
			    {
				    StringBuilder stringBuilder2 = new StringBuilder();
				    for (int j = 0; j < __instance.buyableItemsList.Length; j++)
				    {
					    stringBuilder2.Append("\n* " + TranslationManager.GetArrayTranslation("Item", __instance.buyableItemsList[j].itemName) + "  //  " + 
                                              TranslationManager.GetArrayTranslation("Terminal", "Price")
                                              + ": $" + ((float)__instance.buyableItemsList[j].creditsWorth * ((float)__instance.itemSalesPercentages[j] / 100f)).ToString());
					    if (__instance.itemSalesPercentages[j] != 100)
					    {
						    stringBuilder2.Append(string.Format("   ({0}% " + TranslationManager.GetArrayTranslation("Terminal", "OFF!") + ")", 100 - __instance.itemSalesPercentages[j]));
					    }
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[buyableItemsList]", stringBuilder2.ToString());
			    }
		    }
		    if (modifiedDisplayText.Contains("[buyableVehiclesList]"))
		    {
			    if (__instance.buyableVehicles == null || __instance.buyableVehicles.Length == 0)
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[buyableVehiclesList]",
                        TranslationManager.GetArrayTranslation("Terminal", "NoStock"));
			    }
			    else
			    {
				    StringBuilder stringBuilder3 = new StringBuilder();
				    for (int k = 0; k < __instance.buyableVehicles.Length; k++)
				    {
					    stringBuilder3.Append("\n* " + __instance.buyableVehicles[k].vehicleDisplayName + "  //  " + TranslationManager.GetArrayTranslation("Terminal", "Price") + ": $" + ((float)__instance.buyableVehicles[k].creditsWorth * ((float)__instance.itemSalesPercentages[k + __instance.buyableItemsList.Length] / 100f)).ToString());
					    if (__instance.itemSalesPercentages[k + __instance.buyableItemsList.Length] != 100)
					    {
						    stringBuilder3.Append(string.Format("   ({0}% " + TranslationManager.GetArrayTranslation("Terminal", "OFF!") + ")", 100 - __instance.itemSalesPercentages[k + __instance.buyableItemsList.Length]));
					    }
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[buyableVehiclesList]", stringBuilder3.ToString());
			    }
		    }
		    if (modifiedDisplayText.Contains("[currentUnlockedLogsList]"))
		    {
			    if (__instance.unlockedStoryLogs == null || __instance.unlockedStoryLogs.Count <= 0)
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[currentUnlockedLogsList]", TranslationManager.GetArrayTranslation("Terminal", "DataCorrupted"));
			    }
			    else
			    {
				    StringBuilder stringBuilder4 = new StringBuilder();
				    for (int l = 0; l < __instance.unlockedStoryLogs.Count; l++)
				    {
					    stringBuilder4.Append("\n" + __instance.logEntryFiles[__instance.unlockedStoryLogs[l]].creatureName);
					    if (__instance.newlyUnlockedStoryLogs.Contains(__instance.unlockedStoryLogs[l]))
					    {
						    stringBuilder4.Append(" (" + TranslationManager.GetArrayTranslation("Terminal", "NEW") + ")");
					    }
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[currentUnlockedLogsList]", stringBuilder4.ToString());
			    }
		    }
		    if (modifiedDisplayText.Contains("[unlockablesSelectionList]"))
		    {
			    if (__instance.ShipDecorSelection == null || __instance.ShipDecorSelection.Count <= 0)
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[unlockablesSelectionList]", TranslationManager.GetArrayTranslation("Terminal", "NoItems"));
			    }
			    else
			    {
				    StringBuilder stringBuilder5 = new StringBuilder();
				    for (int m = 0; m < __instance.ShipDecorSelection.Count; m++)
				    {
					    stringBuilder5.Append(string.Format("\n{0}  //  ${1}", __instance.ShipDecorSelection[m].creatureName, __instance.ShipDecorSelection[m].itemCost));
				    }
				    modifiedDisplayText = modifiedDisplayText.Replace("[unlockablesSelectionList]", stringBuilder5.ToString());
			    }
		    }
		    if (modifiedDisplayText.Contains("[storedUnlockablesList]"))
		    {
			    StringBuilder stringBuilder6 = new StringBuilder();
			    bool flag = false;
			    for (int n = 0; n < StartOfRound.Instance.unlockablesList.unlockables.Count; n++)
			    {
				    if (StartOfRound.Instance.unlockablesList.unlockables[n].inStorage)
				    {
					    flag = true;
					    stringBuilder6.Append("\n" + StartOfRound.Instance.unlockablesList.unlockables[n].unlockableName);
				    }
			    }
			    if (!flag)
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[storedUnlockablesList]", TranslationManager.GetArrayTranslation("Terminal", "NoStored"));
			    }
			    else
			    {
				    modifiedDisplayText = modifiedDisplayText.Replace("[storedUnlockablesList]", stringBuilder6.ToString());
			    }
		    }
            if (modifiedDisplayText.Contains("[scanForItems]"))
            {
                Random random = new Random(StartOfRound.Instance.randomMapSeed + 91);
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                GrabbableObject[] array = Object.FindObjectsOfType<GrabbableObject>();
                for (int num6 = 0; num6 < array.Length; num6++)
                {
                    if (array[num6].itemProperties.isScrap && !array[num6].isInShipRoom && !array[num6].isInElevator)
                    {
                        num5 += array[num6].itemProperties.maxValue - array[num6].itemProperties.minValue;
                        num4 += Mathf.Clamp(random.Next(array[num6].itemProperties.minValue, array[num6].itemProperties.maxValue), array[num6].scrapValue - 6 * num6, array[num6].scrapValue + 9 * num6);
                        num3++;
                    }
                }
                modifiedDisplayText = modifiedDisplayText.Replace("[scanForItems]", string.Format(TranslationManager.GetArrayTranslation("Terminal", "ScanForItems"), num3, num4));
            }
		    if (__instance.numberOfItemsInDropship <= 0)
		    {
			    modifiedDisplayText = modifiedDisplayText.Replace("[numberOfItemsOnRoute]", "");
		    }
		    else
		    {
			    modifiedDisplayText = modifiedDisplayText.Replace("[numberOfItemsOnRoute]", string.Format(TranslationManager.GetArrayTranslation("Terminal", "BoughtItemsInRoute"), __instance.numberOfItemsInDropship));
		    }
            var culture = CultureInfo.GetCultureInfo("ko-KR");
            modifiedDisplayText = modifiedDisplayText.Replace(
                "[currentDay]",
                DateTime.Now.ToString("dddd", culture));
            
		    modifiedDisplayText = modifiedDisplayText.Replace("[variableAmount]", __instance.playerDefinedAmount.ToString());
		    modifiedDisplayText = modifiedDisplayText.Replace("[playerCredits]", "$" + __instance.groupCredits.ToString());
		    modifiedDisplayText = modifiedDisplayText.Replace("[totalCost]", "$" + totalCostField.GetValue(__instance).ToString());
		    modifiedDisplayText = modifiedDisplayText.Replace("[companyBuyingPercent]", string.Format("{0}%", Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)));
		    if (__instance.displayingPersistentImage)
		    {
			    modifiedDisplayText = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" + modifiedDisplayText;
		    }
		    return modifiedDisplayText;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(Terminal __instance, ref TMP_InputField ___screenText, ref string ___currentText, ref int ___numberOfItemsInDropship)
        {
            if (!vehicleChecked)
            {
                if (GameNetworkManager.Instance.gameVersionNum >= 55)
                {
                    TranslateVehicle(__instance);
                }
                
                foreach (UnlockableItem unlockableItem in StartOfRound.Instance.unlockablesList.unlockables)
                {
                    TranslateUnlockable(unlockableItem);
                }
                vehicleChecked = true;
            }
            if (___screenText.text.Contains("numberOfItemsOnRoute2"))
            {
                if (___numberOfItemsInDropship != 0)
                {
                    ___screenText.text = ___screenText.text.Replace("[numberOfItemsOnRoute2]", 
                        TranslationManager.GetArrayTranslation("Terminal", "InRoute") + ___numberOfItemsInDropship + 
                        TranslationManager.GetArrayTranslation("Terminal", "ItemBought"));
                    ___currentText = ___screenText.text;
                }else
                {
                    ___screenText.text = ___screenText.text.Replace("[numberOfItemsOnRoute2]", "");
                    ___currentText = ___screenText.text;
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
                ___screenText.text = $"\n\n{TranslationManager.GetArrayTranslation("Terminal", "ScanForItems")}\n\n";
                ___currentText = ___screenText.text;
            }
        }
        
        static void TranslateNode(Terminal instance)
        {
            foreach (TerminalNode node in instance.terminalNodes.specialNodes)
            {
                /*
                Plugin.mls.LogInfo(
                    $"{node.displayText}, {TranslationManager.GetArrayTranslation("Terminal", node.name, 0, false, node.displayText)}");
                */
                node.displayText = TranslationManager.GetArrayTranslation("Terminal", node.name, 0, false, node.displayText);
            }
        }

        static void TranslateUnlockable(UnlockableItem unlockableItem)
        {
            if (unlockableItem.shopSelectionNode == null)
                return;

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
                    terminalNodes.displayText = "순간이동기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요.\n\n";
                    terminalNodes.creatureName = "순간이동기";
                    break;
                case "Television":
                    unlockableItem.unlockableName = "텔레비전";
                    terminalNodes.displayText = "텔레비전을 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요.\n\n";
                    terminalNodes.creatureName = "텔레비전";
                    break;
                case "Toilet":
                    unlockableItem.unlockableName = "변기";
                    terminalNodes.displayText = "변기를 주문하려고 합니다. \n아이템의 총 가격: [totalCost].\n\n" + Plugin.confirmString.ToUpper() + " 또는 " + Plugin.denyString.ToUpper() + "을(를) 입력하세요.\n\n";
                    terminalNodes.creatureName = "변기";
                    break;
                case "Shower":
                    unlockableItem.unlockableName = "샤워 부스";
                    terminalNodes.displayText = "샤워 부스를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "샤워 부스";
                    break;
                case "Record player":
                    unlockableItem.unlockableName = "레코드 플레이어";
                    terminalNodes.displayText = "레코드 플레이어를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "레코드";
                    break;
                case "Table":
                    unlockableItem.unlockableName = "테이블";
                    terminalNodes.displayText = "테이블을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "테이블";
                    break;
                case "Romantic table":
                    unlockableItem.unlockableName = "로맨틱한 테이블";
                    terminalNodes.displayText = "로맨틱한 테이블을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "로맨틱한 테이블";
                    break;
                case "Sofa chair":
                    unlockableItem.unlockableName = "소파 의자";
                    terminalNodes.displayText = "소파 의자를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "소파 의자";
                    break;
                case "Microwave":
                    unlockableItem.unlockableName = "전자레인지";
                    terminalNodes.displayText = "전자레인지를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "전자레인지";
                    break;
                case "Fridge":
                    unlockableItem.unlockableName = "냉장고";
                    terminalNodes.displayText = "냉장고를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "냉장고";
                    break;
                case "Signal translator":
                    unlockableItem.unlockableName = "신호 해석기";
                    terminalNodes.displayText = "신호 해석기를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "신호 해석기";
                    break;
                case "Loud horn":
                    unlockableItem.unlockableName = "시끄러운 경적";
                    terminalNodes.displayText = "시끄러운 경적을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "시끄러운 경적";
                    break;
                case "Inverse Teleporter":
                    unlockableItem.unlockableName = "역방향 순간이동기";
                    terminalNodes.displayText = "역방향 순간이동기를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "역방향 순간이동기";
                    break;
                case "JackOLantern":
                    unlockableItem.unlockableName = "잭오랜턴";
                    terminalNodes.displayText = "잭오랜턴을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "잭오랜턴";
                    break;
                case "Welcome mat":
                    unlockableItem.unlockableName = "웰컴 매트";
                    terminalNodes.displayText = "웰컴 매트를 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "웰컴 매트";
                    break;
                case "Goldfish":
                    unlockableItem.unlockableName = "금붕어";
                    terminalNodes.displayText = "금붕어 어항을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "금붕어";
                    break;
                case "Plushie pajama man":
                    unlockableItem.unlockableName = "인형 파자마 맨";
                    terminalNodes.displayText = "인형 파자마 맨을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "인형 파자마 맨";
                    break;
                case "Disco Ball":
                    unlockableItem.unlockableName = "디스코 볼";
                    terminalNodes.displayText = "디스코 볼을 주문하려고 합니다. \\n아이템의 총 가격: [totalCost].\\n\\n\" + Plugin.confirmString.ToUpper() + \" 또는 \" + Plugin.denyString.ToUpper() + \"을(를) 입력하세요.\\n\\n\n\n";
                    terminalNodes.creatureName = "디스코 볼";
                    break;
            }
        }


        static void TranslateKeyword(TerminalNodesList terminalNodes, List<TerminalNode> ___enemyFiles)
        {
            foreach (TerminalNode node in ___enemyFiles)
            {
                node.creatureName = TranslationManager.GetArrayTranslation("Terminal", node.name);
                node.displayText = TranslationManager.GetArrayTranslation("Terminal", node.name, 1);
            }
            foreach (TerminalKeyword keyword in terminalNodes.allKeywords)
            {
                if (keyword.name.Contains("LogFile"))
                {
                    keyword.word = TranslationManager.GetArrayTranslation("Sigurd", keyword.name, 0, false, keyword.word);
                    foreach (CompatibleNoun comNouns in keyword.defaultVerb.compatibleNouns)
                    {
                        comNouns.result.creatureName = TranslationManager.GetArrayTranslation("Sigurd", comNouns.noun.name,
                            1, false, comNouns.result.creatureName);
                        comNouns.result.displayText = TranslationManager.GetArrayTranslation("Sigurd", comNouns.noun.name,
                            2, false, comNouns.result.displayText);
                    }
                }

                switch (keyword.word)
                {
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
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 13-Kast", "13 카스트로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 57 Asteroid-13", "57 아스테로이드-13로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 44 Atlantica", "44 아틀란티카로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 42 Cosmocos", "42 코스모코스로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 48 Desolation", "48 디솔레이션으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 154 Etern", "154 이턴으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 25 Fission-C", "25 피션-C로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 36 Gloom", "36 글룸으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 147 Gratar", "147 그라타로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 46 Infernis", "46 인퍼니스로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 84 Junic", "84 주닉으로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 134 Oldred", "134 올드레드로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 94 Polarus", "94 폴라러스로 이동합니다.");
                                noun.result.terminalOptions[1].result.displayText = noun.result.terminalOptions[1].result.displayText.Replace("Routing autopilot to 76 Acidir", "76 어시디어로 이동합니다.");

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
