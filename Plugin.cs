using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;
using System.IO;
using TMPro;
using System.Reflection;
using UnityEngine.Video;
using LCKR.Patches;

namespace LCKR
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Piggy.LCKR";
        private const string modName = "LCKR";
        public const string modVersion = "2.0.5";
        public static string modVerType = "a";

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

        public static VideoClip snareKorean;

        public static bool fullyKoreanMoons;
        public static string confirmString;
        public static string denyString;

        public static GameObject resetPanel;

        public static bool patchFont;
        public static bool toKG;
        public static bool showVersion;
        
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
        public static string DefTranslationFilePath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            PluginDirectory = Info.Location;
            TranslationFilePath = Config.ConfigFilePath.Replace("Piggy.LCKR.cfg", "") + "\\LCKR_Translation";
            DefTranslationFilePath =  Config.ConfigFilePath.Replace("Piggy.LCKR.cfg", "") + "LCKR_Translation\\Default";

            LoadAssets();
            TextureReplacer.Setup();
            TranslationManager.Setup();

            patchFont = (bool)base.Config.Bind<bool>("폰트", "폰트 변경", true, "기본값은 true입니다.\nFontPatcher 등 외부 폰트 모드를 사용하려면 이 값을 false로 설정하세요. false로 설정하면 본 모드에서 폰트를 변경하지 않습니다.").Value;

            showVersion = (bool)base.Config.Bind<bool>("일반", "버전 표시", true, "기본값은 true입니다.\ntrue로 설정하면 메인 화면에 모드의 버전을 표시합니다..").Value;

            fullyKoreanMoons = (bool)base.Config.Bind<bool>("접근성", "단말기 카탈로그 한글 입력", false, "기본값은 false입니다.\n위성 카탈로그 \"MOONS\"나 상점 카탈로그 \"STORE\"같은 키워드를 \"위성\", \"상점\"으로 변경합니다.\n(help => 도움말, moons => 위성, store => 상점, bestiary => 도감, other => 기타, eject => 사출, sigurd는 그대로입니다.)").Value;
            confirmString = (string)base.Config.Bind<string>("접근성", "확정 키워드", "confirm", "기본값은 confirm입니다.\n컨펌 노드 (Confirm)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;
            denyString = (string)base.Config.Bind<string>("접근성", "취소 키워드", "deny", "기본값은 deny입니다.\n디나이 노드 (Deny)를 설정합니다. *초성, 띄어쓰기와 한 글자는 인식하지 못합니다!*").Value;

            toKG = (bool)base.Config.Bind<bool>("번역", "KG 변환", true, "기본값은 true입니다.\ntrue로 설정하면 무게 수치를 kg으로 변환합니다.").Value;
            
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LCKR is loaded");
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
                snareKorean = Bundle.LoadAsset<VideoClip>("SnareFleaTipChannel2.m4v");
                resetPanel = Bundle.LoadAsset<GameObject>("LCKRComponent.prefab");

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
