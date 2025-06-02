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
using Steamworks;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
        private static void SetHoverTipAndCurrentInteractTrigger_Postfix(ref TextMeshProUGUI ___cursorTip)
        {
            ___cursorTip.text = ___cursorTip.text.Replace("Inventory full!", "인벤토리 가득 참!");
            ___cursorTip.text = ___cursorTip.text.Replace("(Cannot hold until ship has landed)", "(함선이 착륙하기 전까지 집을 수 없음)");
            ___cursorTip.text = ___cursorTip.text.Replace("[Hands full]", "[양 손 사용 중]");
            ___cursorTip.text = ___cursorTip.text.Replace("Grab", "줍기");
            ___cursorTip.text = ___cursorTip.text.Replace("Locked", "잠김");

            ___cursorTip.text = ___cursorTip.text.Replace("Picking lock", "자물쇠 따는 중");
            ___cursorTip.text = ___cursorTip.text.Replace(" sec.", "초 남음.");

            ___cursorTip.text = ___cursorTip.text.Replace("Use door", "문 사용하기");
        }

        [HarmonyPostfix]
        [HarmonyPatch("SpawnDeadBody")]
        private static void SpawnDeadBody_Postfix()
        {
            DeadBodyInfo[] deadBodyInfo = GameObject.FindObjectsOfType<DeadBodyInfo>();
            foreach (DeadBodyInfo info in deadBodyInfo)
            {
                ScanNodeProperties componentInChildren = info.gameObject.GetComponentInChildren<ScanNodeProperties>();
                if (!componentInChildren.headerText.Contains("의 시체"))
                {
                    componentInChildren.headerText = componentInChildren.headerText.Replace("Body of ", "");
                    componentInChildren.headerText = componentInChildren.headerText + "의 시체";
                }
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
                componentInChildren.subText = componentInChildren.subText.Replace("Burning", "불탐");
                componentInChildren.subText = componentInChildren.subText.Replace("Stabbing", "찔림");
                componentInChildren.subText = componentInChildren.subText.Replace("Fan", "환풍기");
                componentInChildren.subText = componentInChildren.subText.Replace("Inertia", "관성");
                componentInChildren.subText = componentInChildren.subText.Replace("Snipped", "절단됨");
            }
        }
    }
}
