using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using LCKorean.Patches;
using UnityEngine;
using System.IO;
using TMPro;
using BepInEx.Configuration;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Texture_Replacer;
using UnityEngine.Video;

namespace LCKorean
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Piggy.LCKorean";
        private const string modName = "LCKorean";
        private const string modVersion = "1.1.3";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        public static ManualLogSource mls;
        public static AssetBundle Bundle;

        public static TMP_FontAsset font3270_HUDIngame;
        public static TMP_FontAsset font3270_HUDIngame_Variant;
        public static TMP_FontAsset font3270_HUDIngameB;
        public static TMP_FontAsset font3270_Regular_SDF;
        public static TMP_FontAsset font3270_b;
        public static TMP_FontAsset font3270_DialogueText;
        public static TMP_FontAsset fontEdunline;

        public static Texture2D artificeHint;
        public static Texture2D endgameAllPlayersDead;
        public static Texture2D endgameStatsBoxes;
        public static Texture2D endgameStatsDeceased;
        public static Texture2D endgameStatsMissing;
        public static Texture2D flashbangBottleTexture;

        public static Texture2D manual1;
        public static Texture2D manual2v2;
        public static Texture2D manual3v2;
        public static Texture2D manual4v2;

        public static Texture2D playerLevelStickers;
        public static Texture2D playerLevelStickers1;
        public static Texture2D posters;
        public static Texture2D TipsPoster2;
        public static Texture2D powerBoxTextures;

        public static Texture2D RedUIPanelGlitchBWarningRadiation;
        public static Texture2D RedUIPanelGlitchBWarningRadiationB;
        public static Texture2D RedUIPanelGlitchBWarningRadiationC;
        public static Texture2D RedUIPanelGlitchBWarningRadiationD;

        public static Texture2D StickyNoteTex;
        public static Texture2D yieldSignTex;
        public static Texture2D stopSignTex;

        public static VideoClip snareKorean;

        public static bool fullyKoreanMoons;
        public static string confirmString;
        public static string denyString;

        public static bool translateModdedContent;
        public static bool translatePlanet;
        public static bool patchFont;
        public static bool thumperTranslation;
        public static bool toKG;
        public static bool artificePronounce = false;
        
        //Translation
        public static string deathText;
        public static string quotaReached;
        public static string firedText;
        public static string sellText;
        public static string injuryText;
        public static string systemOnlineText;

        public static string allDead1;
        public static string allDead2;

        public static string autoTakeoff1;
        public static string autoTakeoff2;

        public static string midnightWarning;

        public static string PluginDirectory;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Plugin.PluginDirectory = base.Info.Location;

            LoadAssets();
            TextureReplacer.Setup();

            patchFont = (bool)base.Config.Bind<bool>("폰트", "폰트 변경", true, "기본값은 true입니다.\nFontPatcher 등 외부 폰트 모드를 사용하려면 이 값을 true로 설정하세요. 이 값을 false로 설정한다면 바닐라 폰트로 적용되며, 한글이 깨져보일 수도 있습니다.").Value;

            fullyKoreanMoons = (bool)base.Config.Bind<bool>("접근성", "터미널 카탈로그 한글 입력", false, "기본값은 false입니다.\n위성 카탈로그 \"MOONS\"나 상점 카탈로그 \"STORE\"같은 키워드를 \"위성\" 및 \"상점\"으로 변경합니다.\n(help => 도움말, moons => 위성, store => 상점, bestiary => 도감, other => 기타, eject => 사출, sigurd는 그대로입니다)").Value;
            confirmString = (string)base.Config.Bind<string>("접근성", "확정 키워드", "confirm", "기본값은 confirm입니다.\n컨펌 노드 (Confirm)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;
            denyString = (string)base.Config.Bind<string>("접근성", "취소 키워드", "deny", "기본값은 deny입니다.\n컨펌 취소 노드 (Deny)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;

            translateModdedContent = (bool)base.Config.Bind<bool>("번역", "모드 번역", true, "기본값은 true입니다.\n다른 모드의 여러 컨텐츠(아이템, 행성)를 한글로 번역합니다.\n\n지원 모드:\nImmersiveScrap(XuXiaolan), ").Value;
            translatePlanet = (bool)base.Config.Bind<bool>("번역", "행성 내부 이름 번역", false, "기본값은 false입니다.\n코드에서 사용되는 행성의 내부 이름을 한글화합니다. 게임 플레이에서 달라지는 부분은 없지만, true로 하면 모드 인테리어의 구성을 변경할 때 행성 명을 한글로 입력해야 합니다. false로 두면 그대로 영어로 입력하시면 됩니다.").Value;
            thumperTranslation = (bool)base.Config.Bind<bool>("번역", "Thumper 번역", false, "기본값은 false입니다.\ntrue로 설정하면 \"Thumper\"를 썸퍼로 번역합니다. false로 설정하면 덤퍼로 설정됩니다.").Value;
            toKG = (bool)base.Config.Bind<bool>("번역", "KG 변환", true, "기본값은 true입니다.\ntrue로 설정하면 무게 수치를 kg으로 변환합니다.").Value;
            
            
            deathText = (string)base.Config.Bind<string>("번역", "사망 텍스트", "[생명 신호: 오프라인]", "기본값은 《[생명 신호: 오프라인]》 입니다.\n사망 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            quotaReached = (string)base.Config.Bind<string>("번역", "할당량 달성 텍스트", "할당량을 달성했습니다!", "기본값은 《할당량을 달성했습니다!》 입니다.\n할당량 달성 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            firedText = (string)base.Config.Bind<string>("번역", "해고 텍스트", "해고당했습니다.", "기본값은 《해고당했습니다.》 입니다.\n해고 시 출력되는 텍스트를 수정합니다.").Value;
            sellText = (string)base.Config.Bind<string>("번역", "판매 텍스트", "급여를 받았습니다!", "기본값은 《급여를 받았습니다!》 입니다.\n아이템 판매 시 패널에 출력되는 텍스트를 수정합니다.").Value;
            injuryText = (string)base.Config.Bind<string>("번역", "치명적인 부상 텍스트", "치명적인 부상", "기본값은 《치명적인 부상》 입니다.\n치명적인 부상 발생 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            systemOnlineText = (string)base.Config.Bind<string>("번역", "시스템 온라인 텍스트", "시스템 온라인", "기본값은 《시스템 온라인》 입니다.\n로비 접속 시 화면에 출력되는 텍스트를 수정합니다.").Value;

            allDead1 = (string)base.Config.Bind<string>("파일럿 컴퓨터 대사 번역", "전원 사망 1", "경고! 모든 팀원이 응답하지 않으며 함선에 돌아오지 않았습니다. 긴급 코드가 활성화되었습니다.", "기본값은\n《경고! 모든 팀원이 응답하지 않으며 함선에 돌아오지 않았습니다. 긴급 코드가 활성화되었습니다.》\n입니다.\n전원 사망 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            allDead2 = (string)base.Config.Bind<string>("파일럿 컴퓨터 대사 번역", "전원 사망 2", "가까운 기지로 이동합니다. 모든 폐품을 분실했습니다.", "기본값은\n《가까운 기지로 이동합니다. 모든 폐품을 분실했습니다.》\n입니다.\n전원 사망 시 화면에 출력되는 텍스트를 수정합니다.").Value;

            autoTakeoff1 = (string)base.Config.Bind<string>("파일럿 컴퓨터 대사 번역", "자동 이륙1", "경고! 위험한 상황으로 인해 함선이 이륙하고 있습니다.", "기본값은\n《경고! 위험한 상황으로 인해 함선이 이륙하고 있습니다.》\n입니다.\n자동 이륙 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            autoTakeoff2 = (string)base.Config.Bind<string>("파일럿 컴퓨터 대사 번역", "자동 이륙2", "회사는 독점 하드웨어에 대한 손상 위험을 최소화해야 합니다. 안녕히 계세요!", "기본값은\n《회사는 독점 하드웨어에 대한 손상 위험을 최소화해야 합니다. 안녕히 계세요!》\n입니다.\n자동 이륙 시 화면에 출력되는 텍스트를 수정합니다.").Value;
            
            midnightWarning = (string)base.Config.Bind<string>("파일럿 컴퓨터 대사 번역", "자정 경고", "경고!!! 함선이 자정에 이륙합니다. 빠르게 복귀하세요.", "기본값은\n《경고!!! 함선이 자정에 이륙합니다. 빠르게 복귀하세요.》\n입니다.\n자정 경고 시 화면에 출력되는 텍스트를 수정합니다.").Value;


            //artificePronounce = (bool)base.Config.Bind<bool>("번역", "아 \"티\" 피스", false, "기본값은 false입니다.\ntrue로 설정하면 Artifice를 아\"터\"피스 대신 아\"티\"피스로 번역합니다.").Value;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LC Korean is loaded");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadAssets()
        {
            try
            {
                Plugin.Bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Plugin.PluginDirectory), "lckorean"));
            }
            catch (Exception ex)
            {
                Plugin.mls.LogError("Couldn't load asset bundle: " + ex.Message);
                return;
            }
            try
            {
                font3270_HUDIngame = Plugin.Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngame.asset");
                font3270_HUDIngame_Variant = Plugin.Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngame - Variant.asset");
                font3270_HUDIngameB = Plugin.Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngameB.asset");
                font3270_Regular_SDF = Plugin.Bundle.LoadAsset<TMP_FontAsset>("3270-Regular SDF.asset");
                font3270_b = Plugin.Bundle.LoadAsset<TMP_FontAsset>("b.asset");
                font3270_DialogueText = Plugin.Bundle.LoadAsset<TMP_FontAsset>("DialogueText.asset");

                fontEdunline = Plugin.Bundle.LoadAsset<TMP_FontAsset>("edunline SDF.asset");

                stopSignTex = Plugin.Bundle.LoadAsset<Texture2D>("StopSignTex.png");
                yieldSignTex = Plugin.Bundle.LoadAsset<Texture2D>("YieldSignTex.png");
                StickyNoteTex = Plugin.Bundle.LoadAsset<Texture2D>("StickyNoteTex.png");

                artificeHint = Plugin.Bundle.LoadAsset<Texture2D>("artificeHint.png");
                endgameAllPlayersDead = Plugin.Bundle.LoadAsset<Texture2D>("endgameAllPlayersDead.png");
                endgameStatsBoxes = Plugin.Bundle.LoadAsset<Texture2D>("endgameStatsBoxes.png");
                endgameStatsDeceased = Plugin.Bundle.LoadAsset<Texture2D>("endgameStatsDeceased.png");
                endgameStatsMissing = Plugin.Bundle.LoadAsset<Texture2D>("endgameStatsMissing.png");
                flashbangBottleTexture = Plugin.Bundle.LoadAsset<Texture2D>("FlashbangBottleTexture.png");

                manual1 = Plugin.Bundle.LoadAsset<Texture2D>("manual1.png");
                manual2v2 = Plugin.Bundle.LoadAsset<Texture2D>("manual2v2.png");
                manual3v2 = Plugin.Bundle.LoadAsset<Texture2D>("manual3v2.png");
                manual4v2 = Plugin.Bundle.LoadAsset<Texture2D>("manual4v2.png");

                playerLevelStickers = Plugin.Bundle.LoadAsset<Texture2D>("PlayerLevelStickers.png");
                playerLevelStickers1 = Plugin.Bundle.LoadAsset<Texture2D>("PlayerLevelStickers 1.png");
                posters = Plugin.Bundle.LoadAsset<Texture2D>("posters.png");
                TipsPoster2 = Plugin.Bundle.LoadAsset<Texture2D>("TipsPoster2.png");
                powerBoxTextures = Plugin.Bundle.LoadAsset<Texture2D>("powerBoxTextures.png");

                RedUIPanelGlitchBWarningRadiation = Plugin.Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiation.png");
                RedUIPanelGlitchBWarningRadiationB = Plugin.Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationB.png");
                RedUIPanelGlitchBWarningRadiationC = Plugin.Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationC.png");
                RedUIPanelGlitchBWarningRadiationD = Plugin.Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationD.png");

                snareKorean = Plugin.Bundle.LoadAsset<VideoClip>("SnareFleaTipChannel2.m4v");

                base.Logger.LogInfo("Successfully loaded assets!");
            }
            catch (Exception ex2)
            {
                base.Logger.LogError("Couldn't load assets: " + ex2.Message);
            }
        }
    }
}
