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
    [HarmonyPatch(typeof(EntranceTeleport))]
    internal class EntranceTeleportPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(EntranceTeleport __instance)
        {
            InteractTrigger ic = __instance.gameObject.GetComponent<InteractTrigger>();
            ic.hoverTip = ic.hoverTip.Replace("[Near activity detected!]", "[근처에서 활동이 감지되었습니다!]");
            ic.hoverTip = ic.hoverTip.Replace("Enter :", "들어가기 :");
        }
    }
}
