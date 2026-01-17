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
using System.Collections;

namespace LCKorean
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Piggy.LCKorean";
        private const string modName = "LCKorean";
        private const string modVersion = "2.000";
        private static string modVerType = "b";

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
        public static TMP_FontAsset fontAds;

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
        public static string TranslationFilePath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            PluginDirectory = base.Info.Location;
            TranslationFilePath = base.Config.ConfigFilePath.Replace("Piggy.LCKorean.cfg", "") + "\\LCKR_Translation";

            LoadAssets();
            TextureReplacer.Setup();
            TranslationManager.Setup();

            patchFont = (bool)base.Config.Bind<bool>("폰트", "폰트 변경", true, "기본값은 true입니다.\nFontPatcher 등 외부 폰트 모드를 사용하려면 이 값을 false로 설정하세요. false로 설정하면 본 모드에서 폰트를 변경하지 않습니다.").Value;

            fullyKoreanMoons = (bool)base.Config.Bind<bool>("접근성", "터미널 카탈로그 한글 입력", false, "기본값은 false입니다.\n위성 카탈로그 \"MOONS\"나 상점 카탈로그 \"STORE\"같은 키워드를 \"위성\", \"상점\"으로 변경합니다.\n(help => 도움말, moons => 위성, store => 상점, bestiary => 도감, other => 기타, eject => 사출, sigurd는 그대로입니다.)").Value;
            confirmString = (string)base.Config.Bind<string>("접근성", "확정 키워드", "confirm", "기본값은 confirm입니다.\n컨펌 노드 (Confirm)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;
            denyString = (string)base.Config.Bind<string>("접근성", "취소 키워드", "deny", "기본값은 deny입니다.\n디나이 노드 (Deny)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;

            translateModdedContent = (bool)base.Config.Bind<bool>("번역", "모드 번역", true, "기본값은 true입니다.\n다른 모드의 여러 컨텐츠(아이템, 행성)를 한글로 번역합니다.\n\n지원 모드:\nImmersiveScrap(XuXiaolan), ").Value;
            thumperTranslation = (bool)base.Config.Bind<bool>("번역", "Thumper 번역", true, "기본값은 true입니다.\ntrue로 설정하면 \"Thumper\"를 썸퍼로 번역합니다. false로 설정하면 덤퍼로 설정됩니다.").Value;
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

            translatePlanet = (bool)base.Config.Bind<bool>("개발자 기능", "행성 내부명(이름 아님!) 번역", false, "기본값은 false입니다.\n이 설정이 어떤 역할을 하는지 잘 모른다면 항상 비활성화 상태로 설정하세요! 타 모드와의 구성이 꼬여버릴 수 있습니다.\n코드에서 사용되는 행성의 내부 이름을 한글화합니다. 플레이에서 달라지는 부분은 없지만 true로 하면 모드 내부 맵의 구성을 변경할 때 행성 이름을 한글로 입력해야 합니다. false면 행성 이름을 영어로 입력하시면 됩니다.").Value;
            
            //artificePronounce = (bool)base.Config.Bind<bool>("번역", "아 \"티\" 피스", false, "기본값은 false입니다.\ntrue로 설정하면 Artifice를 아\"터\"피스 대신 아\"티\"피스로 번역합니다.").Value;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LC Korean is loaded");
            mls.LogInfo("base.Config.ConfigFilePath: " + base.Config.ConfigFilePath);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            /*
            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(CoroutineManager));
            harmony.PatchAll(typeof(EntranceTeleportPatch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(IngamePlayerSettingsPatch));
            harmony.PatchAll(typeof(InteractTriggerPatch));
            harmony.PatchAll(typeof(FontLoader));
            harmony.PatchAll(typeof(ManualCameraRendererPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(PreInitSceneScriptPatch));
            harmony.PatchAll(typeof(QuickMenuManagerPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(SaveFileUISlotPatch));
            harmony.PatchAll(typeof(ShipBuildModeManagerPatch));
            harmony.PatchAll(typeof(ShipTeleporterPatch));
            harmony.PatchAll(typeof(ShotgunItemPatch));
            harmony.PatchAll(typeof(StartMatchLeverPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(StunGrenadeItemPatch));
            harmony.PatchAll(typeof(TerminalPatch));
            harmony.PatchAll(typeof(TextMeshProUGUIPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(TMP_DropdownPatch));
            harmony.PatchAll(typeof(TVScriptPatch));
            harmony.PatchAll(typeof(UnlockableSuitPatch));
            harmony.PatchAll(typeof(VehicleControllerPatch));
            */
        }

        private void LoadAssets()
        {
            try
            {
                Bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(PluginDirectory), "lckorean"));
            }
            catch (Exception ex)
            {
                mls.LogError("Couldn't load asset bundle: " + ex.Message);
                return;
            }
            try
            {
                font3270_HUDIngame = Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngame.asset");
                font3270_HUDIngame_Variant = Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngame - Variant.asset");
                font3270_HUDIngameB = Bundle.LoadAsset<TMP_FontAsset>("3270-HUDIngameB.asset");
                font3270_Regular_SDF = Bundle.LoadAsset<TMP_FontAsset>("3270-Regular SDF.asset");
                font3270_b = Bundle.LoadAsset<TMP_FontAsset>("b.asset");
                font3270_DialogueText = Bundle.LoadAsset<TMP_FontAsset>("DialogueText.asset");

                fontEdunline = Bundle.LoadAsset<TMP_FontAsset>("edunline SDF.asset");
                fontAds = Bundle.LoadAsset<TMP_FontAsset>("HakgyoansimMalgeunnalB SDF.asset");

                stopSignTex = Bundle.LoadAsset<Texture2D>("StopSignTex.png");
                yieldSignTex = Bundle.LoadAsset<Texture2D>("YieldSignTex.png");
                StickyNoteTex = Bundle.LoadAsset<Texture2D>("StickyNoteTex.png");

                artificeHint = Bundle.LoadAsset<Texture2D>("artificeHint.png");
                endgameAllPlayersDead = Bundle.LoadAsset<Texture2D>("endgameAllPlayersDead.png");
                endgameStatsBoxes = Bundle.LoadAsset<Texture2D>("endgameStatsBoxes.png");
                endgameStatsDeceased = Bundle.LoadAsset<Texture2D>("endgameStatsDeceased.png");
                endgameStatsMissing = Bundle.LoadAsset<Texture2D>("endgameStatsMissing.png");
                flashbangBottleTexture = Bundle.LoadAsset<Texture2D>("FlashbangBottleTexture.png");

                manual1 = Bundle.LoadAsset<Texture2D>("manual1.png");
                manual2v2 = Bundle.LoadAsset<Texture2D>("manual2v2.png");
                manual3v2 = Bundle.LoadAsset<Texture2D>("manual3v2.png");
                manual4v2 = Bundle.LoadAsset<Texture2D>("manual4v2.png");

                playerLevelStickers = Bundle.LoadAsset<Texture2D>("PlayerLevelStickers.png");
                playerLevelStickers1 = Bundle.LoadAsset<Texture2D>("PlayerLevelStickers 1.png");
                posters = Bundle.LoadAsset<Texture2D>("posters.png");
                TipsPoster2 = Bundle.LoadAsset<Texture2D>("TipsPoster2.png");
                powerBoxTextures = Bundle.LoadAsset<Texture2D>("powerBoxTextures.png");

                RedUIPanelGlitchBWarningRadiation = Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiation.png");
                RedUIPanelGlitchBWarningRadiationB = Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationB.png");
                RedUIPanelGlitchBWarningRadiationC = Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationC.png");
                RedUIPanelGlitchBWarningRadiationD = Bundle.LoadAsset<Texture2D>("RedUIPanelGlitchBWarningRadiationD.png");

                snareKorean = Bundle.LoadAsset<VideoClip>("SnareFleaTipChannel2.m4v");

                base.Logger.LogInfo("Successfully loaded assets!");
            }
            catch (Exception ex2)
            {
                base.Logger.LogError("Couldn't load assets: " + ex2.Message);
            }
        }

        private void ReplaceImageFile()
    {
        // 현재 실행 중인 어셈블리의 경로 가져오기
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string modDirectory = Path.GetDirectoryName(assemblyLocation);

        // 덮어쓸 이미지 파일의 경로 (DLL 파일이 있는 디렉터리와 같은 이름)
        string imageName = "example.png"; // 실제 이미지 파일 이름으로 변경
        string modImagePath = Path.Combine(modDirectory, imageName);

        if (!File.Exists(modImagePath))
        {
            Logger.LogError($"이미지 파일이 {modImagePath}에 없습니다.");
            return;
        }

        // 게임 내 이미지 파일 경로 (경로는 게임에 따라 달라질 수 있음)
        string gameImagePath = Path.Combine("게임_이미지_파일_경로", imageName); // 실제 게임 내 경로로 변경

        if (!File.Exists(gameImagePath))
        {
            Logger.LogError($"게임 내 이미지 파일이 {gameImagePath}에 없습니다.");
            return;
        }

        try
        {
            // 이미지 파일 덮어쓰기
            File.Copy(modImagePath, gameImagePath, overwrite: true);
            Logger.LogInfo($"이미지 파일 {imageName}이 {gameImagePath}에 성공적으로 덮어쓰기되었습니다.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"이미지 파일 덮어쓰기 중 오류 발생: {ex.Message}");
        }
    }
    }
}
