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
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    internal class ShipTeleporterPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(ref InteractTrigger ___buttonTrigger)
        {
            ___buttonTrigger.disabledHoverTip = ___buttonTrigger.disabledHoverTip.Replace("Cooldown", "재사용 대기 중");
            ___buttonTrigger.disabledHoverTip = ___buttonTrigger.disabledHoverTip.Replace(" sec.", "초 남음.");
        }
    }
}
