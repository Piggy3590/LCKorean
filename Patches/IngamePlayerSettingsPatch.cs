using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
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
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace LCKorean.Patches
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
