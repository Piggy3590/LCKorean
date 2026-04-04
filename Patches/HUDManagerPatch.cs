using HarmonyLib;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        static readonly FieldInfo forceChangeTextCoroutineField = AccessTools.Field(typeof(HUDManager), "forceChangeTextCoroutine");
        static readonly MethodInfo forceChangeTextMethod = AccessTools.Method(typeof(HUDManager), "ForceChangeText");
        
        //private static List<ScanNodeProperties> scanNodes = new List<ScanNodeProperties>();
        private string amPM;
        private string newLine;

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix(ref PlayerLevel[] ___playerLevels)
        {
            foreach (PlayerLevel level in ___playerLevels)
            {
                level.levelName = TranslationManager.GetArrayTranslation("PlayerLevel",level.levelName);
            }

            HUDManager.Instance.planetRiskLevelText.transform.localPosition = new Vector3(336.5525f, 274.1743f, -74);
        }

        static void TranslateAdvertText(HUDManager __instance)
        {
            TextMeshProUGUI advertTopText = __instance.advertTopText;
            TextMeshProUGUI bottomText = __instance.advertBottomText;
            if (!advertTopText.text.Contains("<size=35>"))
            {
                advertTopText.text = TranslationManager.ReplaceArrayText(advertTopText.text, "Item", advertTopText.text);
                advertTopText.text = "<size=35>" + advertTopText.text;
            }
            if (!bottomText.text.Contains("<size=35>"))
            {
                bottomText.text = "<size=35>" + bottomText.text;
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "OFF");
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "AVAILABLE NOW");
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "CURES CANCER");
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "NO WAY");
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "LIMITED TIME ONLY");
                bottomText.text = TranslationManager.ReplaceArrayText(bottomText.text, "HUD", "GET YOURS TODAY");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(HUDManager __instance, ref TextMeshProUGUI ___weightCounter, ref Animator ___weightCounterAnimator,
            ref TextMeshProUGUI ___holdButtonToEndGameEarlyText, ref TextMeshProUGUI ___holdButtonToEndGameEarlyVotesText)
        {
            if (GameNetworkManager.Instance.gameVersionNum >= 70)
            {
                TranslateAdvertText(__instance);
            }
            ___holdButtonToEndGameEarlyText.text = TranslationManager.ReplaceArrayTextAll(___holdButtonToEndGameEarlyText.text, "HUD");
            
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

        [HarmonyPrefix]
        [HarmonyPatch("DisplaySpectatorTip")]
        private static void DisplaySpectatorTip_Postfix(string tipText, ref TextMeshProUGUI ___spectatorTipText)
        {
            string text = TranslationManager.GetArrayTranslation("HUD", "TipMouse");
            if (StartOfRound.Instance.localPlayerUsingController)
            {
                text = TranslationManager.GetArrayTranslation("HUD", "TipController");
            }
            ___spectatorTipText.text = text;
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplayStatusEffect")]
        private static void DisplayStatusEffect_Postfix(string statusEffect, ref TextMeshProUGUI ___statusEffectText)
        {
            if (statusEffect == "VISIBILITY LOW!\n\nSteam leak detected in area")
            {
                ___statusEffectText.text = $"{
                    TranslationManager.GetArrayTranslation( "HUD", "SteamLeakDetected")}" +
                                           $"\n\n" +
                                           $"{TranslationManager.GetArrayTranslation( "HUD", "SteamLeakDetected", 1)}" +
                                           $"\n" +
                                           $"{TranslationManager.GetArrayTranslation( "HUD", "SteamLeakDetected", 2)}";
            }else if (statusEffect == "Oxygen critically low!")
            {
                ___statusEffectText.text = TranslationManager.ReplaceArrayText(___statusEffectText.text, "HUD", "OxygenLow");
            }else if (statusEffect == "HEALTH RISK!")
            {
                ___statusEffectText.text = TranslationManager.GetArrayTranslation("HUD", "HealthRisk") + "\n"
                    + TranslationManager.GetArrayTranslation("HUD", "HealthRisk", 1) + "\n"
                + TranslationManager.GetArrayTranslation("HUD", "HealthRisk", 2);
            }else if (statusEffect.Contains("HIGH FEVER DETECTED!") && statusEffect.Contains("REACHING"))
            {
                ___statusEffectText.text = 
                    ___statusEffectText.text.Replace("HIGH FEVER DETECTED!", TranslationManager.GetArrayTranslation("HUD", "HighFever"));
                ___statusEffectText.text = 
                    ___statusEffectText.text.Replace("REACHING", TranslationManager.GetArrayTranslation("HUD", "HighFever", 1));
            }else if (statusEffect.Contains("HIGH FEVER DETECTED!!!"))
            {
                ___statusEffectText.text =
                    ___statusEffectText.text
                        = TranslationManager.GetArrayTranslation("HUD", "HighFever2") + "\n\n"
                        + TranslationManager.GetArrayTranslation("HUD", "HighFever2", 1) + "\n\n"
                        + TranslationManager.GetArrayTranslation("HUD", "HighFever2", 2);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetSpectatingTextToPlayer")]
        private static void SetSpectatingTextToPlayer_Postfix(ref TextMeshProUGUI ___spectatingPlayerText)
        {
            ___spectatingPlayerText.text = TranslationManager.ReplaceArrayText(___spectatingPlayerText.text, "HUD", "Spectating");
        }

        [HarmonyPostfix]
        [HarmonyPatch("ApplyPenalty")]
        private static void ApplyPenalty_Postfix(ref EndOfGameStatUIElements ___statsUIElements)
        {
            TMP_Text t = ___statsUIElements.penaltyAddition;
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", " casualties");
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", " bodies recovered");
        }

        [HarmonyPrefix]
        [HarmonyPatch("ReadDialogue")]
        private static void ReadDialogue_Prefix(DialogueSegment[] dialogueArray)
        {
            foreach (DialogueSegment dialogue in dialogueArray)
            {
                dialogue.speakerText = TranslationManager.GetArrayTranslation("Dialogue", dialogue.speakerText);

                float timeToLeaveEarly = TimeOfDay.Instance.shipLeaveAutomaticallyTime;
                
                if (dialogue.bodyText == "AM. A vote has been cast")
                {
                    dialogue.bodyText = TranslationManager.GetArrayTranslation("Dialogue", "ReturnAM")
                        .Replace("{time}", GetClockTimeFormatted(timeToLeaveEarly, TimeOfDay.Instance.numberOfHours, false));
                }
                else if (dialogue.bodyText == "PM. A vote has been cast")
                {
                    dialogue.bodyText = TranslationManager.GetArrayTranslation("Dialogue", "ReturnPM")
                        .Replace("{time}", GetClockTimeFormatted(timeToLeaveEarly, TimeOfDay.Instance.numberOfHours, false));
                }
                else
                {
                    dialogue.bodyText = TranslationManager.GetArrayTranslation("Dialogue", dialogue.bodyText);
                }
            }
        }
        
        public static string GetClockTimeFormatted(float timeNormalized, float numberOfHours, bool createNewLine = true)
        {
            int totalMinutes = (int)(timeNormalized * (60f * numberOfHours)) + 360;
            int hour = (int)Mathf.Floor((float)(totalMinutes / 60));

            string newLine = createNewLine ? "\n" : " ";

            if (hour >= 24)
            {
                return "오전" + newLine + "12:00";
            }

            string ampm;
            if (hour < 12)
                ampm = "오전";
            else
                ampm = "오후";

            if (hour > 12)
                hour %= 12;

            int minute = totalMinutes % 60;

            string time = string.Format("{0:00}:{1:00}", hour, minute).TrimStart('0');

            return ampm + newLine + time;
        }

        [HarmonyPostfix]
        [HarmonyPatch("DisplayTip")]
        private static void DisplayTip_Postfix(TextMeshProUGUI ___tipsPanelHeader, TextMeshProUGUI ___tipsPanelBody)
        {
            switch (___tipsPanelHeader.text)
            {
                case "To read the manual:":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "ReadManual");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "ReadManual", 1);
                    break;
                case "TIP:":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "TipTerminal");
                    if (___tipsPanelBody.text == "Use the ship computer terminal to access secure doors.")
                    {
                        ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "TipTerminal", 1);
                    }else
                    {
                        ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "TipLockedDoor", 1);
                    }
                    break;
                case "Welcome!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "Welcome");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "Welcome", 1);
                    break;
                case "Got scrap!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "GotScrap");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "GotScrap", 1);
                    break;
                case "Items missed!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "ItemMissed");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "ItemMissed", 1);
                    break;
                case "Item stored!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "ItemStored");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "ItemStored", 1);
                    break;
                case "HALT!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "DeadlineWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "DeadlineWarning", 1);
                    break;
                case "Weather alert!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "EclipseWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "EclipseWarning", 1);
                    break;
                case "???":
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "MissingEntrance", 1);
                    break;
                case "ALERT!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "LightningWarning");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "LightningWarning", 1);
                    break;
                case "Welcome back!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "WelcomeBack");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "WelcomeBack", 1);
                    break;
                case "Tip":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "TipShipObject");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "TipShipObject", 1);
                    break;
                case "Equipped to utility belt!":
                    ___tipsPanelHeader.text = TranslationManager.GetArrayTranslation("Tip", "UtilityBelt");
                    ___tipsPanelBody.text = TranslationManager.GetArrayTranslation("Tip", "UtilityBelt", 1);
                    break;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("DisplayNewScrapFound")]
        private static void DisplayNewScrapFound_Postfix()
        {
            ScrapCollectionBox(HUDManager.Instance.ScrapItemBoxes[0]);
            ScrapCollectionBox(HUDManager.Instance.ScrapItemBoxes[1]);
            ScrapCollectionBox(HUDManager.Instance.ScrapItemBoxes[2]);
        }

        private static void ScrapCollectionBox(ScrapItemHUDDisplay scrapBox)
        {
            scrapBox.headerText.text = TranslationManager.ReplaceArrayText(scrapBox.headerText.text, "Item",
                scrapBox.headerText.text.Replace(" collected!", ""));
            scrapBox.headerText.text = TranslationManager.ReplaceArrayText(scrapBox.headerText.text, "HUD", "collected");
            scrapBox.valueText.text = TranslationManager.ReplaceArrayText(scrapBox.valueText.text, "HUD", "Value");
        }
        
        [HarmonyPostfix, HarmonyPatch("DisplayCreditsEarning")]
        private static void DisplayCreditsEarning_Postfix(ref TextMeshProUGUI ___moneyRewardsTotalText)
        {
            ___moneyRewardsTotalText.text = TranslationManager.ReplaceArrayText(___moneyRewardsTotalText.text, "HUD", "TOTAL");
        }

        [HarmonyPostfix, HarmonyPatch("DisplayDaysLeft")]
        private static void DisplayDaysLeft_Postfix(ref TextMeshProUGUI ___profitQuotaDaysLeftText, ref TextMeshProUGUI ___profitQuotaDaysLeftText2)
        {
            string t = TranslationManager.ReplaceArrayText(___profitQuotaDaysLeftText.text, "HUD", 
                Regex.Replace(___profitQuotaDaysLeftText.text, @"\d", ""));
            ___profitQuotaDaysLeftText.text = t;
            ___profitQuotaDaysLeftText2.text = t;
        }

        [HarmonyPostfix, HarmonyPatch("ChangeControlTip")]
        private static void ChangeControlTip_Postfix(HUDManager __instance, ref TextMeshProUGUI[] ___controlTipLines, int toolTipNumber, string changeTo, bool clearAllOther = false)
        {
            string actionKey = changeTo.Split(':')[0].Trim();
            string translatedString = actionKey;

            translatedString = changeTo.Replace(
                actionKey,
                TranslationManager.GetArrayTranslation("ControlTip", actionKey, 0, true)
            );

            ___controlTipLines[toolTipNumber].text = translatedString;

            //Plugin.mls.LogInfo($"key-{actionKey}, line-{___controlTipLines[toolTipNumber].text}");


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
                ___controlTipLines[0].text = TranslationManager.GetArrayTranslation("Item", itemProperties.itemName) + " 떨어뜨리기 : [G]";

                int maxLines = Mathf.Min(allLines.Length, ___controlTipLines.Length);

                for (int i = 0; i < maxLines; i++)
                {
                    if (allLines.Length > i && !string.IsNullOrEmpty(allLines[i]))
                    {
                        string originalLine = allLines[i];
                        string actionKey = originalLine.Split(':')[0].Trim();

                        //Plugin.mls.LogInfo($"{i}, {actionKey}-");

                        ___controlTipLines[i + 1].text = ___controlTipLines[i + 1].text.Replace(
                            actionKey,
                            TranslationManager.GetArrayTranslation("ControlTip", actionKey, 0, true)
                        );
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void Update_Prefix(ref TextMeshProUGUI ___planetInfoHeaderText, ref TextMeshProUGUI ___globalNotificationText, ref TextMeshProUGUI ___loadingText, ref TextMeshProUGUI[] ___controlTipLines)
        {
            /*
            ___controlTipLines[0].text = ___controlTipLines[0].text.Replace("Gas pedal", TranslationManager.GetStringTranslation("ControlTip", ___controlTipLines[0].text));
            ___controlTipLines[1].text = ___controlTipLines[1].text.Replace("Brake pedal", TranslationManager.GetStringTranslation("ControlTip", ___controlTipLines[1].text));
            ___controlTipLines[2].text = ___controlTipLines[2].text.Replace("Boost", TranslationManager.GetStringTranslation("ControlTip", ___controlTipLines[2].text));
            */
            if (___loadingText.text == "Waiting for crew...")
            {
                ___loadingText.text = TranslationManager.GetArrayTranslation("HUD", "WaitingForCrew");
            }

            string t = ___planetInfoHeaderText.text;
            if (t.Contains("CELESTIAL BODY:"))
            {
                t = TranslationManager.ReplaceArrayText(t, "Planets", "CELESTIAL BODY");

                t = TranslationManager.ReplaceArrayText(t, "Planets", "Experimentation");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Where the Company resides");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Assurance");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Offense");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Adamance");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Rend");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Dine");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "March");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Vow");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Titan");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Artifice");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Embrion");
                t = TranslationManager.ReplaceArrayText(t, "Planets", "Gordion");
                ___planetInfoHeaderText.text = t;
            }

            if (___globalNotificationText.text == "New creature data sent to terminal!")
            {
                ___globalNotificationText.text =
                    TranslationManager.GetArrayTranslation("HUD", "NewCreature");
            }
            else if (___globalNotificationText.text.Contains("Found journal entry:"))
            {
                string translatedText = TranslationManager.GetArrayTranslation("HUD", "FoundJournal");
                ___globalNotificationText.text = ___globalNotificationText.text.Replace("Found journal entry:", translatedText);
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch("AttemptScanNode")]
        private static void AttemptScanNode_Prefix(ScanNodeProperties node)
        {
            if (node == null)
                return;
            string headerText = TranslationManager.GetArrayTranslation("ScanNode", node.headerText);
            string subText = node.subText;
            if (node.subText.Contains("Value: ") || node.subText.Contains("value: "))
            {
                subText = subText.Replace("Value: ", TranslationManager.GetArrayTranslation("ScanNode", "valueCheck", 1));
                subText = subText.Replace("value: ", TranslationManager.GetArrayTranslation("ScanNode", "valueCheck", 1));
            }
            else
            {
                subText = TranslationManager.GetArrayTranslation("ScanNode", node.headerText, 1);
            }
            node.headerText = headerText;
            if (headerText != subText)
                node.subText = subText;
        }
        
        [HarmonyPostfix, HarmonyPatch("DisplayNewDeadline")]
        static void DisplayNewDeadline_Postfix(ref TextMeshProUGUI ___reachedProfitQuotaBonusText)
        {
            ___reachedProfitQuotaBonusText.text = TranslationManager.ReplaceArrayText(___reachedProfitQuotaBonusText.text, "HUD",
                "Overtime bonus");
        }

        /*
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
        */
    }
}
