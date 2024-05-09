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
    [HarmonyPatch(typeof(StunGrenadeItem))]
    internal class StunGrenadeItemPatch
    {
        [HarmonyPostfix, HarmonyPatch("SetControlTipForGrenade")]
        private static void SetControlTipForGrenade_Postfix()
        {
            /*
            foreach (TextMeshProUGUI text in ___controlTipLines)
            {
                text.text = text.text.Replace("Pull pin", "핀 뽑기");
                text.text = text.text.Replace("Use grenade", "수류탄 사용하기");
                text.text = text.text.Replace("Toss egg", "달걀 던지기");
                text.text = text.text.Replace("Turn safety off", "안전 모드 해제");
                text.text = text.text.Replace("Turn safety on", "안전 모드 설정");
                text.text = text.text.Replace("Quit terminal", "터미널 나가기");
            }
            */
        }
    }
}
