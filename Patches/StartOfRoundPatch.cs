using GameNetcodeStuff;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine.UI;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Postfix()
        {
            try
            {
                TranslatePlanet();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("행성 리스트를 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SceneManager_OnLoad")]
        private static void SceneManager_OnLoad_Postfix()
        {
            if (HUDManager.Instance.loadingText.text == "LOADING WORLD...")
            {
                HUDManager.Instance.loadingText.text = "세계 불러오는 중...";
            }
        }
        
        /*
        [HarmonyPostfix]
        [HarmonyPatch("SetDiscordStatusDetails")]
        private static void SetDiscordStatusDetails_Postfix()
        {
            if (DiscordController.Instance == null)
            {
                return;
            }

            if (GameNetworkManager.Instance.disableSteam)
            {
                return;
            }

            DiscordController.Instance.status_Details =
                DiscordController.Instance.status_Details.Replace("Getting fired", "해고당하는 중");
            DiscordController.Instance.status_Details =
                DiscordController.Instance.status_Details.Replace("In orbit (Waiting for crew)", "공전 중 (팀원 기다리는 중)");
            if (StartOfRound.Instance.inShipPhase)
            {
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName + "을(를) 공전하는 중";
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Gordion", "고르디온");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Experimentation", "익스페리멘테이션");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Assuarance", "어슈어런스");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Vow", "보우");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Offense", "오펜스");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("March", "머치");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Adamance", "애더먼스");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Rend", "렌드");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Dine", "다인");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Titan", "타이탄");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Embrion", "엠브리온");
                DiscordController.Instance.status_Details =
                    StartOfRound.Instance.currentLevel.PlanetName.Replace("Artifice", "아터피스");
            }

            DiscordController.Instance.status_smallText =
                DiscordController.Instance.status_smallText.Replace("Deceased", "사망함");
            DiscordController.Instance.status_smallText =
                DiscordController.Instance.status_smallText.Replace("In orbit", "공전 중");

            if (RoundManager.Instance != null && StartOfRound.Instance.inShipPhase)
            {
                float num = (float)StartOfRound.Instance.GetValueOfAllScrap(true, false) /
                    (float)TimeOfDay.Instance.profitQuota * 100f;
                DiscordController.Instance.status_State = string.Format("할당량의 {0}% 달성 | {1}일 남음", (int)num,
                    TimeOfDay.Instance.daysUntilDeadline);
            }
        }
        */

        [HarmonyPostfix]
        [HarmonyPatch("FirePlayersAfterDeadlineClientRpc")]
        private static void FirePlayersAfterDeadlineClientRpc_Postfix()
        {
            TextMeshProUGUI t = HUDManager.Instance.EndOfRunStatsText;
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", "Days on the job");
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", "Scrap value collected");
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", "Deaths");
            t.text = TranslationManager.ReplaceArrayText(t.text, "HUD", "Steps taken");
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetMapScreenInfoToCurrentLevel")]
        private static void SetMapScreenInfoToCurrentLevel_Postfix(ref TextMeshProUGUI ___screenLevelDescription)
        {
            TranslatePlanet();
            foreach (SelectableLevel level in StartOfRound.Instance.levels)
            {
                if (level.riskLevel == "Safe")
                {
                    level.riskLevel = "안전";
                }
            }

            string text;
            if (StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.None)
            {
                string weatherText = TranslationManager.GetArrayTranslation("Planets", "Weather");
                string weather = TranslationManager.GetArrayTranslation("Planets",
                    StartOfRound.Instance.currentLevel.currentWeather.ToString());
                text = $"{weatherText}: {weather}";
            }
            else
            {
                text = "";
            }

            string levelDescription = StartOfRound.Instance.currentLevel.LevelDescription;
            if (StartOfRound.Instance.isChallengeFile)
            {
                StartOfRound.Instance.screenLevelDescription.text = string.Concat(new string[]
                {
                    TranslationManager.GetArrayTranslation("Planets", "Orbiting") + ": ",
                    GameNetworkManager.Instance.GetNameForWeekNumber(-1),
                    "\n",
                    TranslationManager.GetArrayTranslation("Planets", levelDescription, 0, true),
                    "\n",
                    text
                });
            }
            else
            {
                StartOfRound.Instance.screenLevelDescription.text = string.Concat(new string[]
                {
                    "공전 중: ",
                    TranslationManager.GetArrayTranslation("Planets", StartOfRound.Instance.currentLevel.PlanetName, 0, true),
                    "\n",
                    TranslationManager.GetArrayTranslation("Planets", levelDescription, 0, true),
                    "\n",
                    text
                });
            }
            //___screenLevelDescription.text = TranslationManager.GetArrayTranslation("Planets", ___screenLevelDescription.text, 0, true);

            /*
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Weather", "날씨");

            ___screenLevelDescription.text =
                ___screenLevelDescription.text.Replace("Where the Company resides", "회사가 소재하는 지역입니다");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Rainy", "우천");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Stormy", "뇌우");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Foggy", "안개");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Flooded", "홍수");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Eclipsed", "일식");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Orbiting", "공전 중");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("71 Gordion", "71 고르디온");
            

            ___screenLevelDescription.text =
                ___screenLevelDescription.text.Replace("41 Experimentation", "41 익스페리멘테이션");
                */
        }

        bool TryGetValueAfterColon(string text, string removalKey, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(text))
                return false;

            int colonIndex = text.IndexOf(':');
            if (colonIndex < 0)
                return false;

            string key = text.Substring(0, colonIndex).Trim();
            if (key != removalKey)
                return false;

            value = text.Substring(colonIndex + 1).Trim();
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("WritePlayerNotes")]
        public static void WritePlayerNotes_Postfix(ref EndOfGameStats ___gameStats,
            ref PlayerControllerB[] ___allPlayerScripts, ref int ___connectedPlayersAmount,
            ref bool ___localPlayerWasMostProfitableThisRound)
        {
            for (int i = 0; i < ___allPlayerScripts.Length; i++)
            {
                for (int j = 0; j < ___gameStats.allPlayerStats[i].playerNotes.Count; j++)
                {
                    string t = ___gameStats.allPlayerStats[i].playerNotes[j];
                    t = TranslationManager.GetArrayTranslation("HUD", t);
                    ___gameStats.allPlayerStats[i].playerNotes[j] = t;
                }
            }
        }

        static void TranslatePlanet()
        {
            /*
            Plugin.mls.LogInfo("Translating Planets");
            foreach (SelectableLevel level in StartOfRound.Instance.levels)
            {
                level.LevelDescription = level.LevelDescription.Replace("POPULATION: Abandoned", "인구: 버려짐");
                level.LevelDescription = level.LevelDescription.Replace("POPULATION: None", "인구: 없음");
                level.LevelDescription = level.LevelDescription.Replace("POPULATION: Unknown", "인구: 알 수 없음");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: A landscape of deep valleys and mountains.",
                    "조건: 깊은 계곡과 산이 어우러졌습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Waning forests. Abandoned facilities littered across the landscape.",
                    "조건: 숲이 점점 사라지고 있습니다. 버려진 시설이 곳곳에 흩어져 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Jagged and weathered terrain.",
                    "조건: 들쭉날쭉하고 풍화되었습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: No land masses. Continual storms.",
                    "조건: 육지가 없습니다. 지속적으로 폭풍이 일어납니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Frozen, rocky. Its planet orbits a white dwarf star.",
                    "조건: 얼어붙은 바위로 이루어졌으며, 백색 왜성 주위를 공전하고 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Desolate, made of amethyst.",
                    "조건: 황폐합니다. 자수정으로 이루어졌습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Arid. Thick haze, worsened by industrial artifacts.",
                    "조건: 건조하며, 거주 가능성이 낮습니다. 산업 인공물로 인해 악화되었습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Previously mined for its rich industrial resources, Liquidation is now largely an ocean moon.",
                    "조건: 풍부한 산업 자원으로 채굴되던 리퀴데이션은 이제는 대부분 해양 위성으로 남아 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Expansive. Constant rain.",
                    "조건: 방대합니다. 지속적으로 비가 내립니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Frozen, rocky. This moon was mined for resources. It's easy to get lost here.",
                    "조건: 얼어붙은 바위로 이루어졌습니다. 이 달은 자원을 얻기 위해 채굴되었습니다. 이곳에서는 길을 잃기 쉽습니다.");

                level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Humid. Rough terrain. Teeming with plant-life.",
                    "조건: 습함. 거친 지형. 식물이 많습니다.");

                //
                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Home to a lively, diverse ecosystem of smaller-sized omnivores.",
                    "동물군: 활기차고 다양한 생태계의 본거지이며, 작은 크기의 잡식성 동물로 구성되어 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Rumored active machinery left behind.",
                    "동물군: 아직 작동 중인 기계가 방치되어 있다는 소문이 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Ecosystem supports territorial behaviour.",
                    "동물군: 영역 행동이 빈번합니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Unknown",
                    "동물군: 알 수 없음");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Unlikely for complex life to exist",
                    "동물군: 이곳에 다세포 생명체가 존재할 가능성은 거의 없습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Embrion is devoid of biological life.",
                    "동물군: 엠브리언에는 생물학적 생명체가 존재하지 않습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Dominated by a few species.",
                    "동물군: 몇몇 종이 지배하고 있습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Diverse",
                    "동물군: 다양함");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: A competitive ecosystem supports aggressive lifeforms.",
                    "동물군: 경쟁적인 생태계 때문에 공격적인 생명체가 많습니다.");

                level.LevelDescription = level.LevelDescription.Replace("FAUNA: Dangerous entities have been rumored to take residence in the vast network of tunnels.",
                    "동물군: 위험한 존재들이 광대한 터널 네트워크에 거주한다는 소문이 있습니다.");

                //모드행성
                if (Plugin.translateModdedContent)
                {
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Left/Deceased", "인구: 떠남/사망함");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Gone", "인구: 떠남");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Transformed", "인구: 변형됨");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Drowned", "인구: 익사함");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Killed", "인구: 사망함");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Lost", "인구: 잃음");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Missing", "인구: 실종됨");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Sacrificed/Deceased", "인구: 희생됨/사망함");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: Several tribes detected, no human life in designated area.", "인구: 여러 부족이 발견되었으며, 지정된 지역에 인명 피해는 없습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("POPULATION: ???", "인구: ???");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Cold, hard rocky terrain, no surface plant life detected.",
                        "조건: 차갑고 딱딱한 암석 지형으로 이루어졌습니다. 표면에는 어떠한 식물도 감지되지 않습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A dead ecosystem.",
                        "동물군: 이곳의 생태계는 죽어가고 있음.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Cold wet terrain, supports aquatic plant life.",
                        "조건: 차갑고 습한 지형으로 이루어졌습니다. 수생 식물의 생활을 지원합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A healthy ecosystem, fauna supports neutral behaviour.",
                        "동물군: 정상적인 생태계를 가지고 있습니다. 동물군이 중립적인 행동을 지원합니다.");

                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: ???",
                        "동물군: ???");
                    level.LevelDescription = level.LevelDescription.Replace("UNKNOWN ANOMALY DETECTED",
                        "알 수 없는 이상 현상이 감지됩니다");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Dead, desolate landscape, no healthy flora present.",
                        "조건: 황량하며, 정상적인 식물이 존재하지 않습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A dying ecosystem, remaining fauna display agressive behaviour.",
                        "동물군: 이곳의 생태계는 죽어가고 있으며, 동물들은 공격적인 행동을 보입니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Warm, Dry sandy terrain, supports plant life such as cactaceae and rarely polypodiophyta.",
                        "조건: 따뜻하고 건조한 모래 지형으로 이루어졌습니다. 선인장과 같은 식물의 생활을 지원하며 드물게 다발성 식물도 있습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A thriving ecosystem, creatures support agressive behaviour.",
                        "동물군: 번성하는 생태계로, 생명체들이 공격적인 행동을 지원합니다.");

                    level.LevelDescription = level.LevelDescription.Replace("Hot dry toxic terrain, no surviving flora.",
                        "조건: 뜨겁고 건조한 독성 지형으로 이루어졌습니다. 이곳에서 살아남은 식물군은 없습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("An irradiated ecosystem.",
                        "동물군: 방사선이 산출되는 생태계를 가지고 있습니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Cold moist terrain, roofed trees present.",
                        "조건: 차갑고 습한 지형으로 이루어졌습니다. 지붕처럼 덮힌 나무가 존재합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("A flowering ecosystem, agressive behaviour present.",
                        "동물군: 꽃이 만발한 생태계를 가지고 있습니다. 공격적인 행동이 존재합니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Warm, Dry dirt terrain, supports plant life such as Fabaceae.",
                        "조건: 따뜻하고 건조한 흙 지형으로, 콩과 같은 식물의 생활을 지원합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A thriving Avian ecosystem, creatures support agressive behaviour.",
                        "동물군: 조류 생태계가 번성하며, 공격적인 행동이 잦습니다.");

                    level.LevelDescription = level.LevelDescription.Replace("Extremely hot, Dry dusty terrain, Rarely supports living plant life due to recent volcanic activity.",
                        "조건: 매우 덥고 건조한 먼지가 많은 지형으로, 최근 화산 활동으로 인해 식물 생육이 거의 이루어지지 않았습니다.");
                    level.LevelDescription = level.LevelDescription.Replace("A desperate ecosystem, remaining creatures support agressive behaviour.",
                        "동물군: 절망적인 생태계로, 이곳에 남은 생물들은 공격적인 행동이 잦습니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Warm, very lush terrain, supports plenty of flora, always sunny.",
                        "조건: 따뜻하고 매우 울창한 지형으로, 많은 식물이 서식하며 항상 화창합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A very healthy ecosystem, fauna exhibits neutral behaviour.",
                        "동물군: 매우 건강한 생태계로, 동물들은 중립적인 행동을 보입니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Hot thin terrain, no surviving flora, high pressure within outer crust.",
                        "조건: 덥고 얇은 지형으로 이루어졌으며, 살아남은 식물군은 없습니다. 외부 지각 내부에 높은 압력이 존재합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A desperate ecosystem, remaining fauna display agressive behaviour.",
                        "동물군: 절망적인 생태계로, 이곳에 남은 생물들은 공격적인 행동이 잦습니다");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Cold, wet snowy terrain, supports plant life.",
                        "조건: 춥고 습한 눈 덮인 지형으로 이루어졌습니다. 식물의 생활을 지원합니다.");
                    level.LevelDescription = level.LevelDescription.Replace("FAUNA: A balanced ecosystem, creatures support very agressive behaviour.",
                        "동물군: 균형 잡힌 생태계로, 생명체들은 매우 공격적으로 행동합니다.");

                    level.LevelDescription = level.LevelDescription.Replace("CONDITIONS: Warm lush terrain, supports pleny of flora, acidic pools present.",
                        "조건: 따뜻하고 무성한 지형으로 이루어졌으며, 많은 식물을 지원하고 산성 웅덩이가 존재합니다.");
                }
            }
                */
        }
    }
}
