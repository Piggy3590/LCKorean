using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    internal class IngamePlayerSettingsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SetSettingsOptionsText")]
        private static void SetSettingsOptionsText_Prefix(SettingsOptionType optionType, string setToText)
        {
            setToText = setToText.Replace("Current input device:", "현재 입력 장치:");
            setToText = setToText.Replace("No device found \n (click to refresh)", "장치 발견되지 않음 \n (클릭하여 새로고침)");
            setToText = setToText.Replace("MODE: Push to talk", "모드: 눌러서 말하기");
            setToText = setToText.Replace("MODE: Voice activation", "모드: 음성 감지");
        }
    }
}
