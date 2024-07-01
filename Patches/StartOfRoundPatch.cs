using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using Newtonsoft.Json.Linq;
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
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace LCKorean.Patches
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
                TranslateItem();
                if (Plugin.translateModdedContent)
                {
                    TranslateModdedItem();
                }
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("아이템 이름을 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }

            try
            {
                TranslateDialogue();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("컴퓨터 파일럿 대사를 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }

            try
            {
                TranslatePlanet();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("행성 목록을 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }

            try
            {
                //TranslateUnlockableList();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("함선 강화 및 장식 목록을 번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }
            //ConvertImage();

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

        [HarmonyPostfix]
        [HarmonyPatch("FirePlayersAfterDeadlineClientRpc")]
        private static void FirePlayersAfterDeadlineClientRpc_Postfix()
        {
            HUDManager.Instance.EndOfRunStatsText.text = HUDManager.Instance.EndOfRunStatsText.text.Replace("Days on the job", "근무일수");
            HUDManager.Instance.EndOfRunStatsText.text = HUDManager.Instance.EndOfRunStatsText.text.Replace("Scrap value collected", "수집한 폐품의 가치");
            HUDManager.Instance.EndOfRunStatsText.text = HUDManager.Instance.EndOfRunStatsText.text.Replace("Deaths", "사망 횟수");
            HUDManager.Instance.EndOfRunStatsText.text = HUDManager.Instance.EndOfRunStatsText.text.Replace("Steps taken", "걸음 수");
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
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Weather", "날씨");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Where the Company resides", "회사가 소재하는 지역입니다");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Rainy", "우천");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Stormy", "뇌우");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Foggy", "안개");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Flooded", "홍수");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Eclipsed", "일식");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("Orbiting", "공전 중");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("71 Gordion", "71 고르디온");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("41 Experimentation", "41 익스페리멘테이션");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("220 Assurance", "220 어슈어런스");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("56 Vow", "56 보우")
                ;
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("21 Offense", "21 오펜스");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("61 March", "61 머치");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("20 Adamance", "20 애더먼스");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("85 Rend", "85 렌드");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("7 Dine", "7 다인");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("8 Titan", "8 타이탄");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("68 Artifice", "68 아티피스");

            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("5 Embrion", "5 엠브리언");
            ___screenLevelDescription.text = ___screenLevelDescription.text.Replace("44 Liquidation", "44 리퀴데이션");
        }

        [HarmonyPostfix]
        [HarmonyPatch("WritePlayerNotes")]
        public static void WritePlayerNotes_Postfix(ref EndOfGameStats ___gameStats, ref PlayerControllerB[] ___allPlayerScripts, ref int ___connectedPlayersAmount,
            ref bool ___localPlayerWasMostProfitableThisRound)
        {
            for (int i = 0 ; i < ___allPlayerScripts.Length ; i++)
            {
                for (int j = 0; j < ___gameStats.allPlayerStats[i].playerNotes.Count; j++)
                {
                    ___gameStats.allPlayerStats[i].playerNotes[j] = ___gameStats.allPlayerStats[i].playerNotes[j].Replace("The laziest employee.", "가장 게으른 직원.");
                    ___gameStats.allPlayerStats[i].playerNotes[j] = ___gameStats.allPlayerStats[i].playerNotes[j].Replace("The most paranoid employee.", "가장 피해망상이 심한 직원.");
                    ___gameStats.allPlayerStats[i].playerNotes[j] = ___gameStats.allPlayerStats[i].playerNotes[j].Replace("Sustained the most injuries.", "가장 많은 부상을 입음.");
                    ___gameStats.allPlayerStats[i].playerNotes[j] = ___gameStats.allPlayerStats[i].playerNotes[j].Replace("Most profitable", "가장 수익성이 높음");
                }
            }
        }

        static void ConvertImage()
        {
            foreach (Texture2D texture in TextureReplacer.Textures)
            {
                if (texture.name == "StopSignTex")
                {
                    CopyTexture2D(texture, Plugin.stopSignTex);
                }
                else if (texture.name == "YieldSignTex")
                {
                    CopyTexture2D(texture, Plugin.yieldSignTex);
                }
                else if (texture.name == "StickyNoteTex")
                {
                    CopyTexture2D(texture, Plugin.StickyNoteTex);
                }

                else if (texture.name == "artificeHint")
                {
                    CopyTexture2D(texture, Plugin.artificeHint);
                }
                else if (texture.name == "endgameAllPlayersDead")
                {
                    CopyTexture2D(texture, Plugin.endgameAllPlayersDead);
                }
                else if (texture.name == "endgameStatsBoxes")
                {
                    CopyTexture2D(texture, Plugin.endgameStatsBoxes);
                }
                else if (texture.name == "endgameStatsDeceased")
                {
                    CopyTexture2D(texture, Plugin.endgameStatsDeceased);
                }
                else if (texture.name == "endgameStatsMissing")
                {
                    CopyTexture2D(texture, Plugin.endgameStatsMissing);
                }
                else if (texture.name == "FlashbangBottleTexture")
                {
                    CopyTexture2D(texture, Plugin.flashbangBottleTexture);
                }

                else if (texture.name == "manual1")
                {
                    CopyTexture2D(texture, Plugin.manual1);
                }
                else if (texture.name == "manual2v2")
                {
                    CopyTexture2D(texture, Plugin.manual2v2);
                }
                else if (texture.name == "manual3v2")
                {
                    CopyTexture2D(texture, Plugin.manual3v2);
                }
                else if (texture.name == "manual4v2")
                {
                    CopyTexture2D(texture, Plugin.manual4v2);
                }

                else if (texture.name == "PlayerLevelStickers")
                {
                    CopyTexture2D(texture, Plugin.playerLevelStickers);
                }
                else if (texture.name == "PlayerLevelStickers 1")
                {
                    CopyTexture2D(texture, Plugin.playerLevelStickers1);
                }

                else if (texture.name == "posters")
                {
                    CopyTexture2D(texture, Plugin.posters);
                }
                else if (texture.name == "TipsPoster2")
                {
                    CopyTexture2D(texture, Plugin.TipsPoster2);
                }
                else if (texture.name == "powerBoxTextures")
                {
                    CopyTexture2D(texture, Plugin.powerBoxTextures);
                }

                else if (texture.name == "RedUIPanelGlitchBWarningRadiation")
                {
                    CopyTexture2D(texture, Plugin.RedUIPanelGlitchBWarningRadiation);
                }
                else if (texture.name == "RedUIPanelGlitchBWarningRadiationB")
                {
                    CopyTexture2D(texture, Plugin.RedUIPanelGlitchBWarningRadiationB);
                }
                else if (texture.name == "RedUIPanelGlitchBWarningRadiationC")
                {
                    CopyTexture2D(texture, Plugin.RedUIPanelGlitchBWarningRadiationC);
                }
                else if (texture.name == "RedUIPanelGlitchBWarningRadiationD")
                {
                    CopyTexture2D(texture, Plugin.RedUIPanelGlitchBWarningRadiationD);
                }
            }
        }

        static void CopyTexture2D(Texture2D originalTexture, Texture2D source)
        {
            RenderTexture renderTexture = new RenderTexture(originalTexture.width, originalTexture.height, 0);
            Graphics.Blit(Plugin.RedUIPanelGlitchBWarningRadiationD, renderTexture);
            Texture2D combinedTexture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            originalTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            originalTexture.Apply();
        }

        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            RenderTexture active = RenderTexture.active;
            Graphics.Blit(source, temporary);
            RenderTexture.active = temporary;
            Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }

        static void TranslatePlanet()
        {
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

                if (Plugin.translatePlanet)
                {
                    if (level.PlanetName == "71 Gordion")
                    {
                        level.PlanetName = "71 고르디온";
                    }else if (level.PlanetName == "41 Experimentation")
                    {
                        level.PlanetName = "41 익스페리멘테이션";
                    }
                    else if (level.PlanetName == "220 Assurance")
                    {
                        level.PlanetName = "220 어슈어런스";
                    }
                    else if (level.PlanetName == "56 Vow")
                    {
                        level.PlanetName = "56 보우";
                    }
                    else if (level.PlanetName == "61 March")
                    {
                        level.PlanetName = "61 머치";
                    }
                    else if (level.PlanetName == "21 Offense")
                    {
                        level.PlanetName = "21 오펜스";
                    }
                    else if (level.PlanetName == "85 Rend")
                    {
                        level.PlanetName = "85 렌드";
                    }
                    else if (level.PlanetName == "7 Dine")
                    {
                        level.PlanetName = "7 다인";
                    }
                    else if (level.PlanetName == "8 Titan")
                    {
                        level.PlanetName = "8 타이탄";
                    }
                    
                    else if (level.PlanetName == "20 Adamance")
                    {
                        level.PlanetName = "20 애더먼스";
                    }
                    else if (level.PlanetName == "68 Artifice")
                    {
                        level.PlanetName = "68 아터피스";
                    }
                    else if (level.PlanetName == "5 Embrion")
                    {
                        level.PlanetName = "5 엠브리언";
                    }
                    else if (level.PlanetName == "44 Liquidation")
                    {
                        level.PlanetName = "44 리퀴데이션";
                    }
                }
            }
        }
        static void TranslateDialogue()
        {
            Plugin.mls.LogInfo("Translating Dialogues");
            foreach (DialogueSegment dialogue in StartOfRound.Instance.openingDoorDialogue)
            {
                switch (dialogue.speakerText)
                {
                    case "PILOT COMPUTER":
                        dialogue.speakerText = "파일럿 컴퓨터";
                        break;
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
                        dialogue.bodyText = "우리 회사는 독점 하드웨어에 대한 손상 위험을 최소화해야 합니다. 안녕히 계세요!\r\n";
                        break;
                }
            }
        }
        
        static void TranslateUnlockableList()
        {
            Plugin.mls.LogInfo("Translating Unlockable List");
            foreach (UnlockableItem unlockableItem in StartOfRound.Instance.unlockablesList.unlockables)
            {
                switch (unlockableItem.unlockableName)
                {
                    case "Orange suit":
                        unlockableItem.unlockableName = "주황색 슈트";
                        break;
                    case "Green suit":
                        unlockableItem.unlockableName = "초록색 슈트";
                        break;
                    case "Hazard suit":
                        unlockableItem.unlockableName = "방호복 슈트";
                        break;
                    case "Pajama suit":
                        unlockableItem.unlockableName = "파자마 슈트";
                        break;
                    case "Cozy lights":
                        unlockableItem.unlockableName = "아늑한 조명";
                        break;
                    case "Teleporter":
                        unlockableItem.unlockableName = "순간이동기";
                        break;
                    case "Television":
                        unlockableItem.unlockableName = "텔레비전";
                        break;
                    case "Cupboard":
                        unlockableItem.unlockableName = "수납장";
                        break;
                    case "File Cabinet":
                        unlockableItem.unlockableName = "파일 캐비닛";
                        break;
                    case "Toilet":
                        unlockableItem.unlockableName = "변기";
                        break;
                    case "Shower":
                        unlockableItem.unlockableName = "샤워 부스";
                        break;
                    case "Light switch":
                        unlockableItem.unlockableName = "전등 스위치";
                        break;
                    case "Record player":
                        unlockableItem.unlockableName = "레코드 플레이어";
                        break;
                    case "Table":
                        unlockableItem.unlockableName = "테이블";
                        break;
                    case "Romantic table":
                        unlockableItem.unlockableName = "로맨틱한 테이블";
                        break;
                    case "Bunkbeds":
                        unlockableItem.unlockableName = "벙커침대";
                        break;
                    case "Terminal":
                        unlockableItem.unlockableName = "터미널";
                        break;
                    case "Signal translator":
                        unlockableItem.unlockableName = "신호 해석기";
                        break;
                    case "Loud horn":
                        unlockableItem.unlockableName = "시끄러운 경적";
                        break;
                    case "Inverse Teleporter":
                        unlockableItem.unlockableName = "역방향 순간이동기";
                        break;
                    case "JackOLantern":
                        unlockableItem.unlockableName = "잭오랜턴";
                        break;
                    case "Welcome mat":
                        unlockableItem.unlockableName = "웰컴 매트";
                        break;
                    case "Goldfish":
                        unlockableItem.unlockableName = "금붕어";
                        break;
                    case "Plushie pajama man":
                        unlockableItem.unlockableName = "인형 파자마 맨";
                        break;
                    case "Purple Suit":
                        unlockableItem.unlockableName = "보라색 슈트";
                        break;
                    case "Bee Suit":
                        unlockableItem.unlockableName = "꿀벌 슈트";
                        break;
                    case "Bunny Suit":
                        unlockableItem.unlockableName = "토끼 슈트";
                        break;
                    case "Disco Ball":
                        unlockableItem.unlockableName = "디스코 볼";
                        break;
                }
            }
        }

        static void TranslateItem()
        {
            Plugin.mls.LogInfo("Translating Items");
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.itemName)
                {
                    case "Boombox":
                        item.itemName = "붐박스";
                        item.toolTips[0] = "음악 전환하기 : [RMB]";
                        break;
                    case "Flashlight":
                        item.itemName = "손전등";
                        item.toolTips[0] = "전등 전환하기 : [RMB]";
                        break;
                    case "Jetpack":
                        item.itemName = "제트팩";
                        item.toolTips[0] = "제트팩 사용하기 : [RMB]";
                        break;
                    case "Key":
                        item.itemName = "열쇠";
                        item.toolTips[0] = "열쇠 사용하기 : [RMB]";
                        break;
                    case "Lockpicker":
                        item.itemName = "자물쇠 따개";
                        item.toolTips[0] = "문에 설치하기 : [RMB]";
                        break;
                    case "Apparatus":
                        item.itemName = "장치";
                        break;
                    case "Pro-flashlight":
                        item.itemName = "프로 손전등";
                        item.toolTips[0] = "전등 전환하기 : [RMB]";
                        break;
                    case "Shovel":
                        item.itemName = "철제 삽";
                        item.toolTips[0] = "삽 휘두르기: [RMB]";
                        break;
                    case "Stun grenade":
                        item.itemName = "기절 수류탄";
                        item.toolTips[0] = "수류탄 사용하기 : [RMB]";
                        break;
                    case "Extension ladder":
                        item.itemName = "연장형 사다리";
                        item.toolTips[0] = "사다리 꺼내기 : [RMB]";
                        break;
                    case "TZP-Inhalant":
                        item.itemName = "TZP-흡입제";
                        item.toolTips[0] = "TZP 흡입하기 : [RMB]";
                        break;
                    case "Walkie-talkie":
                        item.itemName = "무전기";
                        item.toolTips[0] = "전원 버튼 : [Q]";
                        item.toolTips[1] = "목소리 송신하기 : [RMB]";
                        break;
                    case "Zap gun":
                        item.itemName = "잽건";
                        item.toolTips[0] = "위협 감지하기 : [RMB]";
                        break;
                    case "Magic 7 ball":
                        item.itemName = "마법의 7번 공";
                        break;
                    case "Airhorn":
                        item.itemName = "에어혼";
                        item.toolTips[0] = "에어혼 사용하기 : [RMB]";
                        break;
                    case "Bell":
                        item.itemName = "종";
                        break;
                    case "Big bolt":
                        item.itemName = "큰 나사";
                        break;
                    case "Bottles":
                        item.itemName = "병 묶음";
                        break;
                    case "Brush":
                        item.itemName = "빗";
                        break;
                    case "Candy":
                        item.itemName = "사탕";
                        break;
                    case "Cash register":
                        item.itemName = "금전 등록기";
                        item.toolTips[0] = "금전 등록기 사용하기 : [RMB]";
                        break;
                    case "Chemical jug":
                        item.itemName = "화학 용기";
                        break;
                    case "Clown horn":
                        item.itemName = "광대 나팔";
                        item.toolTips[0] = "광대 나팔 사용하기 : [RMB]";
                        break;
                    case "Large axle":
                        item.itemName = "대형 축";
                        break;
                    case "Teeth":
                        item.itemName = "틀니";
                        break;
                    case "Dust pan":
                        item.itemName = "쓰레받기";
                        break;
                    case "Egg beater":
                        item.itemName = "달걀 거품기";
                        break;
                    case "V-type engine":
                        item.itemName = "V형 엔진";
                        break;
                    case "Golden cup":
                        item.itemName = "황금 컵";
                        break;
                    case "Fancy lamp":
                        item.itemName = "멋진 램프";
                        break;
                    case "Painting":
                        item.itemName = "그림";
                        break;
                    case "Plastic fish":
                        item.itemName = "플라스틱 물고기";
                        break;
                    case "Laser pointer":
                        item.itemName = "레이저 포인터";
                        item.toolTips[0] = "레이저 전환하기 : [RMB]";
                        break;
                    case "Gold bar":
                        item.itemName = "금 주괴";
                        break;
                    case "Hairdryer":
                        item.itemName = "헤어 드라이기";
                        item.toolTips[0] = "헤어 드라이기 사용하기 : [RMB]";
                        break;
                    case "Magnifying glass":
                        item.itemName = "돋보기";
                        break;
                    case "Metal sheet":
                        item.itemName = "금속 판";
                        break;
                    case "Cookie mold pan":
                        item.itemName = "쿠키 틀";
                        break;
                    case "Mug":
                        item.itemName = "머그잔";
                        break;
                    case "Perfume bottle":
                        item.itemName = "향수 병";
                        break;
                    case "Old phone":
                        item.itemName = "구식 전화기";
                        break;
                    case "Jar of pickles":
                        item.itemName = "피클 병";
                        break;
                    case "Pill bottle":
                        item.itemName = "약 병";
                        break;
                    case "Remote":
                        item.itemName = "리모컨";
                        item.toolTips[0] = "리모컨 사용하기 : [RMB]";
                        break;
                    case "Ring":
                        item.itemName = "반지";
                        break;
                    case "Toy robot":
                        item.itemName = "장난감 로봇";
                        break;
                    case "Rubber Ducky":
                        item.itemName = "고무 오리";
                        break;
                    case "Red soda":
                        item.itemName = "빨간색 소다";
                        break;
                    case "Steering wheel":
                        item.itemName = "운전대";
                        break;
                    case "Stop sign":
                        item.itemName = "정지 표지판";
                        item.toolTips[0] = "표지판 사용하기 : [RMB]";
                        break;
                    case "Tea kettle":
                        item.itemName = "찻주전자";
                        break;
                    case "Toothpaste":
                        item.itemName = "치약";
                        break;
                    case "Toy cube":
                        item.itemName = "장난감 큐브";
                        break;
                    case "Hive":
                        item.itemName = "벌집";
                        break;
                    case "Radar-booster":
                        item.itemName = "레이더 부스터";
                        item.toolTips[0] = "부스터 켜기 : [RMB]";
                        break;
                    case "Yield sign":
                        item.itemName = "양보 표지판";
                        item.toolTips[0] = "표지판 사용하기 : [RMB]";
                        break;
                    case "Shotgun":
                        item.itemName = "산탄총";
                        item.toolTips[0] = "격발 : [RMB]";
                        item.toolTips[1] = "재장전 : [E]";
                        item.toolTips[2] = "안전 모드 해제 : [Q]";
                        break;
                    case "Ammo":
                        item.itemName = "탄약";
                        break;
                    case "Spray paint":
                        item.itemName = "스프레이 페인트";
                        item.toolTips[0] = "스프레이 뿌리기 : [RMB]";
                        item.toolTips[1] = "캔 흔들기 : [Q]";
                        break;
                    case "Homemade flashbang":
                        item.itemName = "사제 섬광탄";
                        item.toolTips[0] = "사제 섬광탄 사용하기 : [RMB]";
                        break;
                    case "Gift":
                        item.itemName = "선물";
                        item.toolTips[0] = "선물 열기 : [RMB]";
                        break;
                    case "Flask":
                        item.itemName = "플라스크";
                        break;
                    case "Tragedy":
                        item.itemName = "비극";
                        item.toolTips[0] = "가면 쓰기 : [RMB]";
                        break;
                    case "Comedy":
                        item.itemName = "희극";
                        item.toolTips[0] = "가면 쓰기 : [RMB]";
                        break;
                    case "Whoopie cushion":
                        item.itemName = "방귀 쿠션";
                        break;
                    case "Kitchen knife":
                        item.itemName = "식칼";
                        item.toolTips[0] = "찌르기 : [RMB]";
                        break;
                    case "Easter egg":
                        item.itemName = "부활절 달걀";
                        break;
                    case "Weed killer":
                        item.itemName = "제초제";
                        item.toolTips[0] = "뿌리기 : [RMB]";
                        break;
                }
            }
        }


        static void TranslateModdedItem()
        {
            Plugin.mls.LogInfo("Translating Items");
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.itemName)
                {
                    case "Alcohol Flask":
                        item.itemName = "알코올 플라스크";
                        break;
                    case "Anvil":
                        item.itemName = "모루";
                        break;
                    case "Baseball bat":
                        item.itemName = "야구 방망이";
                        break;
                    case "Beer can":
                        item.itemName = "맥주 캔";
                        break;
                    case "Brick":
                        item.itemName = "벽돌";
                        break;
                    case "Broken engine":
                        item.itemName = "망가진 엔진";
                        break;
                    case "Bucket":
                        item.itemName = "양동이";
                        break;
                    case "Can paint":
                        item.itemName = "페인트 캔";
                        break;
                    case "Canteen":
                        item.itemName = "수통";
                        break;
                    case "Car battery":
                        item.itemName = "자동차 배터리";
                        break;
                    case "Clamp":
                        item.itemName = "조임틀";
                        break;
                    case "Clock":
                        item.itemName = "시계";
                        break;
                    case "Fancy Painting":
                        item.itemName = "멋진 그림";
                        break;
                    case "Fan":
                        item.itemName = "선풍기";
                        break;
                    case "Fireaxe":
                        item.itemName = "소방 도끼";
                        break;
                    case "Fire extinguisher":
                        item.itemName = "소화기";
                        break;
                    case "Fire hydrant":
                        item.itemName = "소화전";
                        break;
                    case "Food can":
                        item.itemName = "통조림";
                        break;
                    case "Gameboy":
                        item.itemName = "게임보이";
                        break;
                    case "Garbage":
                        item.itemName = "쓰레기";
                        break;
                    case "Hammer":
                        item.itemName = "망치";
                        break;
                    case "Jerrycan":
                        item.itemName = "기름통";
                        break;
                    case "Keyboard":
                        item.itemName = "키보드";
                        break;
                    case "Lantern":
                        item.itemName = "랜턴";
                        break;
                    case "Library lamp":
                        item.itemName = "도서관 램프";
                        break;
                    case "Plant":
                        item.itemName = "식물";
                        break;
                    case "Pliers":
                        item.itemName = "플라이어";
                        break;
                    case "Plunger":
                        item.itemName = "뚫어뻥";
                        break;
                    case "Retro Toy":
                        item.itemName = "레트로 장난감";
                        break;
                    case "Screwdriver":
                        item.itemName = "스크류 드라이버";
                        break;
                    case "Sink":
                        item.itemName = "싱크대";
                        break;
                    case "Socket Wrench":
                        item.itemName = "소켓 렌치";
                        break;
                    case "Squeaky toy":
                        item.itemName = "고무 오리";
                        break;
                    case "Suitcase":
                        item.itemName = "여행 가방";
                        break;
                    case "Toaster":
                        item.itemName = "토스터기";
                        break;
                    case "Toolbox":
                        item.itemName = "공구 상자";
                        break;
                    case "Top hat":
                        item.itemName = "실크햇";
                        break;
                    case "Traffic cone":
                        item.itemName = "라바콘";
                        break;
                    case "Vent":
                        item.itemName = "환풍구";
                        break;
                    case "Watering Can":
                        item.itemName = "물뿌리개";
                        break;
                    case "Wheel":
                        item.itemName = "바퀴";
                        break;
                    case "Wine bottle":
                        item.itemName = "와인 병";
                        break;
                    case "Wrench":
                        item.itemName = "렌치";
                        break;


                    case "Syringe":
                        item.itemName = "주사기";
                        break;
                    case "Syringe Gun":
                        item.itemName = "주사기총";
                        break;
                    case "Corner Pipe":
                        item.itemName = "코너 파이프";
                        break;
                    case "Small Pipe":
                        item.itemName = "작은 파이프";
                        break;
                    case "Flow Pipe":
                        item.itemName = "파이프";
                        break;
                    case "Brain Jar":
                        item.itemName = "뇌가 담긴 병";
                        break;
                    case "Toy Nutcracker":
                        item.itemName = "호두까기 인형 장난감";
                        break;
                    case "Test Tube":
                        item.itemName = "시험관";
                        break;
                    case "Test Tube Rack":
                        item.itemName = "시험관 랙";
                        break;
                    case "Nutcracker Eye":
                        item.itemName = "호두까기 인형 눈";
                        break;
                    case "Blue Test Tube":
                        item.itemName = "파란색 시험관";
                        break;
                    case "Yellow Test Tube":
                        item.itemName = "노란색 시험관";
                        break;
                    case "Red Test Tube":
                        item.itemName = "빨간색 시험관";
                        break;
                    case "Green Test Tube":
                        item.itemName = "초록색 시험관";
                        break;
                    case "Crowbar":
                        item.itemName = "쇠지렛대";
                        break;
                    case "Plzen":
                        item.itemName = "플젠";
                        break;
                    case "Cup":
                        item.itemName = "컵";
                        break;
                    case "Microwave":
                        item.itemName = "전자레인지";
                        break;
                    case "bubblegun":
                        item.itemName = "비눗방울 총";
                        break;
                    case "Broken P88":
                        item.itemName = "망가진 P88";
                        break;
                    case "employee":
                        item.itemName = "직원";
                        break;
                    case "Mine":
                        item.itemName = "지뢰";
                        break;
                    case "Toothles":
                        item.itemName = "투슬리스";
                        break;
                    case "Crossbow":
                        item.itemName = "석궁";
                        break;
                    case "physgun":
                        item.itemName = "피직스건";
                        break;
                    case "Ammo crate":
                        item.itemName = "탄약 상자";
                        break;
                    case "Drink":
                        item.itemName = "음료수";
                        break;
                    case "Radio":
                        item.itemName = "라디오";
                        break;
                    case "Mouse":
                        item.itemName = "마우스";
                        break;
                    case "Monitor":
                        item.itemName = "모니터";
                        break;
                    case "Battery":
                        item.itemName = "건전지";
                        break;
                    case "Cannon":
                        item.itemName = "대포";
                        break;
                    case "Health Drink":
                        item.itemName = "건강 음료";
                        break;
                    case "Chemical":
                        item.itemName = "화학 약품";
                        break;
                    case "Disinfecting Alcohol":
                        item.itemName = "소독용 알코올";
                        break;
                    case "Ampoule":
                        item.itemName = "앰풀";
                        break;
                    case "Blood Pack":
                        item.itemName = "혈액 팩";
                        break;
                    case "Flip Lighter":
                        item.itemName = "라이터";
                        break;
                    case "Rubber Ball":
                        item.itemName = "고무 공";
                        break;
                    case "Video Tape":
                        item.itemName = "비디오 테이프";
                        break;
                    case "First Aid Kit":
                        item.itemName = "구급 상자";
                        break;
                    case "Gold Medallion":
                        item.itemName = "금메달";
                        break;
                    case "Steel Pipe":
                        item.itemName = "금속 파이프";
                        break;
                    case "Axe":
                        item.itemName = "도끼";
                        break;
                    case "Emergency Hammer":
                        item.itemName = "비상용 망치";
                        break;
                    case "Katana":
                        item.itemName = "카타나";
                        break;
                    case "Silver Medallion":
                        item.itemName = "은메달";
                        break;
                    case "Pocket Radio":
                        item.itemName = "휴대용 라디오";
                        break;
                    case "Teddy Plush":
                        item.itemName = "곰 인형";
                        break;
                    case "Experiment Log Hyper Acid":
                        item.itemName = "Hyper Acid 실험 기록";
                        break;
                    case "Experiment Log Comedy Mask":
                        item.itemName = "희극 가면 실험 기록";
                        break;
                    case "Experiment Log Cursed Coin":
                        item.itemName = "저주받은 동전 실험 기록";
                        break;
                    case "Experiment Log BIO HXNV7":
                        item.itemName = "바이오 HXNV7 실험 기록";
                        break;
                    case "Blue Folder":
                        item.itemName = "파란색 폴더";
                        break;
                    case "Red Folder":
                        item.itemName = "빨간색 폴더";
                        break;
                    case "Fire Extinguisher":
                        item.itemName = "소화기";
                        break;
                    case "Coil":
                        item.itemName = "코일";
                        break;
                    case "Typewriter":
                        item.itemName = "타자기";
                        break;
                    case "Documents":
                        item.itemName = "서류 더미";
                        break;
                    case "Stapler":
                        item.itemName = "스테이플러";
                        break;
                    case "Old Computer":
                        item.itemName = "구식 컴퓨터";
                        break;
                    case "Bronze Trophy":
                        item.itemName = "브론즈 트로피";
                        break;
                    case "Banana":
                        item.itemName = "바나나";
                        break;
                    case "Stun Baton":
                        item.itemName = "스턴봉";
                        break;
                    case "BIO-HXNV7":
                        item.itemName = "바이오-HXNV7";
                        break;
                    case "Recovered Secret Log":
                        item.itemName = "복구된 비밀 일지";
                        break;
                    case "Experiment Log Golden Dagger":
                        item.itemName = "황금 단검 실험 기록";
                        break;
                    case "Clam":
                        item.itemName = "대합";
                        break;
                    case "Turtle Shell":
                        item.itemName = "거북이 등딱지";
                        break;
                    case "Fish Bones":
                        item.itemName = "생선 뼈";
                        break;
                    case "Horned Shell":
                        item.itemName = "뿔 달린 껍질";
                        break;
                    case "Porcelain Teacup":
                        item.itemName = "도자기 찻잔";
                        break;
                    case "Marble":
                        item.itemName = "대리석";
                        break;
                    case "Porcelain Bottle":
                        item.itemName = "도자기 병";
                        break;
                    case "Porcelain Perfume Bottle":
                        item.itemName = "도자기 향수 병";
                        break;
                    case "Glowing Orb":
                        item.itemName = "발광구";
                        break;
                    case "Golden Skull":
                        item.itemName = "황금 해골";
                        break;
                    case "Map of Cosmocos":
                        item.itemName = "코스모코스 지도";
                        break;
                    case "Wet Note 1":
                        item.itemName = "젖은 노트 1";
                        break;
                    case "Wet Note 2":
                        item.itemName = "젖은 노트 2";
                        break;
                    case "Wet Note 3":
                        item.itemName = "젖은 노트 3";
                        break;
                    case "Wet Note 4":
                        item.itemName = "젖은 노트 4";
                        break;
                    case "Cosmic Shard":
                        item.itemName = "우주빛 파편";
                        break;
                    case "Cosmic Growth":
                        item.itemName = "우주 생장물";
                        break;
                    case "Chunk of Celestial Brain":
                        item.itemName = "천상의 두뇌 덩어리";
                        break;
                    case "Bucket of Shards":
                        item.itemName = "파편이 든 양동이";
                        break;
                    case "Cosmic Flashlight":
                        item.itemName = "우주빛 손전등";
                        break;
                    case "Forgotten Log 1":
                        item.itemName = "잊혀진 일지 1";
                        break;
                    case "Forgotten Log 2":
                        item.itemName = "잊혀진 일지 2";
                        break;
                    case "Forgotten Log 3":
                        item.itemName = "잊혀진 일지 3";
                        break;
                    case "Glasses":
                        item.itemName = "안경";
                        break;
                    case "Grown Petri Dish":
                        item.itemName = "생장한 배양 접시";
                        break;
                    case "Petri Dish":
                        item.itemName = "배양 접시";
                        break;
                    case "Cosmochad":
                        item.itemName = "코스모채드";
                        break;
                    case "Dying Cosmic Flashlight":
                        item.itemName = "죽어가는 우주빛 손전등";
                        break;
                    case "Dying Cosmic Growth":
                        item.itemName = "죽어가는 우주 생장물";
                        break;
                    case "Blood Petri Dish":
                        item.itemName = "혈액 배양 접시";
                        break;
                    case "Evil Cosmochad":
                        item.itemName = "악마 코스모채드";
                        break;
                    case "Evil Cosmo":
                        item.itemName = "악마 코스모";
                        break;
                    case "Lil Cosmo":
                        item.itemName = "릴 코스모";
                        break;
                    case "Dying Grown Petri Dish":
                        item.itemName = "죽어가는 생장물 배양 접시";
                        break;
                    case "Watching Petri Dish":
                        item.itemName = "감시하는 배양 접시";
                        break;
                    case "Microscope":
                        item.itemName = "현미경";
                        break;
                    case "Round Vile":
                        item.itemName = "원통형 바일";
                        break;
                    case "Square Vile":
                        item.itemName = "사각형 바일";
                        break;
                    case "Oval Vile":
                        item.itemName = "타원형 바일";
                        break;
                    case "Harrington Log 1":
                        item.itemName = "해링턴 일지 1";
                        break;
                    case "Harrington Log 2":
                        item.itemName = "해링턴 일지 2";
                        break;
                    case "Harrington Log 3":
                        item.itemName = "해링턴 일지 3";
                        break;
                    case "Harrington Log 4":
                        item.itemName = "해링턴 일지 4";
                        break;
                    case "Jar of Growth":
                        item.itemName = "생장물이 든 병";
                        break;
                    case "Tape Player Log 1":
                        item.itemName = "테이프 플레이어 일지 1";
                        break;
                    case "Tape Player Log 2":
                        item.itemName = "테이프 플레이어 일지 1";
                        break;
                    case "Tape Player Log 3":
                        item.itemName = "테이프 플레이어 일지 1";
                        break;
                    case "Tape Player Log 4":
                        item.itemName = "테이프 플레이어 일지 1";
                        break;
                }
            }
        }
    }
}
