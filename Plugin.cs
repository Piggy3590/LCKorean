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
        private const string modVersion = "1.0.21";

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

        public static bool translatePlanet;
        public static bool patchFont;
        public static bool thumperTranslation;
        public static bool toKG;
        public static bool artificePronounce = false;

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
            translatePlanet = (bool)base.Config.Bind<bool>("번역", "행성 내부 이름 번역", false, "기본값은 false입니다.\n코드에서 사용되는 행성의 내부 이름을 한글화합니다. 게임 플레이에서 달라지는 부분은 없지만, true로 하면 모드 인테리어의 구성을 변경할 때 행성 명을 한글로 입력해야 합니다. false로 두면 그대로 영어로 입력하시면 됩니다.").Value;
            thumperTranslation = (bool)base.Config.Bind<bool>("번역", "Thumper 번역", false, "기본값은 false입니다.\ntrue로 설정하면 \"Thumper\"를 썸퍼로 번역합니다. false로 설정하면 덤퍼로 설정됩니다.").Value;
            toKG = (bool)base.Config.Bind<bool>("번역", "KG 변환", true, "기본값은 true입니다.\ntrue로 설정하면 무게 수치를 kg으로 변환합니다.").Value;
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
