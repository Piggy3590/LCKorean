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
using UnityEngine.UI;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(ShipBuildModeManager))]
    internal class ShipBuildModeManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateGhostObjectAndHighlight")]
        private static void CreateGhostObjectAndHighlight_Postfix()
        {
            string modifiedText = HUDManager.Instance.buildModeControlTip.text.Replace("Confirm", "배치");
            modifiedText = modifiedText.Replace("Rotate", "회전");
            modifiedText = modifiedText.Replace("Store", "보관");
            HUDManager.Instance.buildModeControlTip.text = modifiedText;
        }
    }
}
