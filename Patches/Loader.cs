using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using HarmonyLib;
using LCKorean;

namespace LCKorean.Patches
{
    [HarmonyPatch]
    class FontLoader
    {
        /*
        [HarmonyPrefix, HarmonyPatch(typeof(TMP_FontAsset), "Awake")]
        static void PatchFontAwake(TMP_FontAsset __instance)
        {
            string fontName = __instance.name;

            if (fontName == "b" || fontName == "DialogueText" || fontName.Contains("3270"))
            {
                __instance = Plugin.font3270;
                return;
            }

            if (fontName.Contains("edunline"))
            {
                __instance = Plugin.fontEdunline;
                return;
            }

            Plugin.mls.LogWarning($"[{fontName}] not patched");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TMP_Text), "font", MethodType.Setter)]
        static void PatchTextFontSetter(TMP_FontAsset value)
        {
            PatchFontAwake(value);
        }
        */

        [HarmonyPrefix, HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
        static void PatchTextAwake(TextMeshProUGUI __instance)
        {
            if (__instance.font == null || !Plugin.patchFont) return;
            
            string fontName = __instance.font.name;

            if (fontName == "3270-HUDIngame")
            {
                __instance.font = Plugin.font3270_HUDIngame;
                return;
            }
            else if (fontName == "3270-HUDIngame - Variant")
            {
                if (__instance.gameObject.name == "BodyText" && __instance.transform.parent.parent.gameObject.name == "GlobalNotification")
                {
                    __instance.font.material = Plugin.font3270_HUDIngame_Variant.material;
                    __instance.fontSize = __instance.fontSize - 0.2f;
                    __instance.characterSpacing = -3.5f;
                    __instance.font.fallbackFontAssetTable.Add(Plugin.font3270_HUDIngame_Variant);
                }
                else
                {
                    //__instance.font = Plugin.font3270_HUDIngame_Variant;
                }
                return;
            }
            else if (fontName == "3270-HUDIngameB")
            {
                __instance.font = Plugin.font3270_HUDIngameB;
                return;
            }
            else if (fontName == "3270-Regular SDF")
            {
                __instance.font = Plugin.font3270_Regular_SDF;
                return;
            }
            else if (fontName == "b")
            {
                __instance.font = Plugin.font3270_b;
                return;
            }
            else if (fontName == "DialogueText")
            {
                __instance.font = Plugin.font3270_DialogueText;
                return;
            }

            if (fontName.Contains("edunline"))
            {
                __instance.font = Plugin.fontEdunline;
                return;
            }
            if (fontName.Contains("Bangers-Regular SDF"))
            {
                __instance.font = Plugin.fontAds;
                return;
            }
            //PatchFontAwake(__instance.font);
        }

        static void DisableFont(TMP_FontAsset font)
        {
            font.characterLookupTable.Clear();
            font.atlasPopulationMode = AtlasPopulationMode.Static;
        }
    }
}
