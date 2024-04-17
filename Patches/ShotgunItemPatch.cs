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
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {
        [HarmonyPostfix, HarmonyPatch("SetSafetyControlTip")]
        private static void SetSafetyControlTip_Postfix(ShotgunItem __instance, ref bool ___safetyOn)
        {
            string text;
            if (___safetyOn)
            {
                text = "안전 모드 해제: [Q]";
            }
            else
            {
                text = "안전 모드 설정: [Q]";
            }
            if (__instance.IsOwner)
            {
                HUDManager.Instance.ChangeControlTip(3, text, false);
            }
        }
    }
}
