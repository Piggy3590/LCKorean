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
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
        private static void SetHoverTipAndCurrentInteractTrigger_Postfix(ref TextMeshProUGUI ___cursorTip)
        {
            if (___cursorTip.text == "Inventory full!")
            {
                ___cursorTip.text = "인벤토리 가득 참!";
            }else if (___cursorTip.text == "(Cannot hold until ship has landed)")
            {
                ___cursorTip.text = "(함선이 착륙하기 전까지 집을 수 없음)";
            }else if (___cursorTip.text == "[Hands full]")
            {
                ___cursorTip.text = "[양 손 사용 중]";
            }else if (___cursorTip.text == "Grab : [E]")
            {
                ___cursorTip.text = "줍기 : [E]";
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("SpawnDeadBody")]
        private static void SpawnDeadBody_Postfix()
        {
            DeadBodyInfo[] deadBodyInfo = GameObject.FindObjectsOfType<DeadBodyInfo>();
            foreach (DeadBodyInfo info in deadBodyInfo)
            {
                ScanNodeProperties componentInChildren = info.gameObject.GetComponentInChildren<ScanNodeProperties>();
                componentInChildren.headerText = componentInChildren.headerText.Replace("Body of ", "");
                componentInChildren.headerText = componentInChildren.headerText + "의 시체";
                componentInChildren.subText = componentInChildren.subText.Replace("Cause of death: ", "사인:");
                componentInChildren.subText = componentInChildren.subText.Replace("Unknown", "알 수 없음");
                componentInChildren.subText = componentInChildren.subText.Replace("Bludgeoning", "구타");
                componentInChildren.subText = componentInChildren.subText.Replace("Gravity", "중력");
                componentInChildren.subText = componentInChildren.subText.Replace("Blast", "폭사");
                componentInChildren.subText = componentInChildren.subText.Replace("Kicking", "걷어차임");
                componentInChildren.subText = componentInChildren.subText.Replace("Strangulation", "교살");
                componentInChildren.subText = componentInChildren.subText.Replace("Suffocation", "질식");
                componentInChildren.subText = componentInChildren.subText.Replace("Mauling", "공격당함");
                componentInChildren.subText = componentInChildren.subText.Replace("Gunshots", "총격");
                componentInChildren.subText = componentInChildren.subText.Replace("Crushing", "압사");
                componentInChildren.subText = componentInChildren.subText.Replace("Drowning", "익사");
                componentInChildren.subText = componentInChildren.subText.Replace("Abandoned", "실종");
                componentInChildren.subText = componentInChildren.subText.Replace("Electrocution", "감전사");
            }
        }
    }
}
