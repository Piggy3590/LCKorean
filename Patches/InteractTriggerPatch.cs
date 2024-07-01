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
    [HarmonyPatch(typeof(InteractTrigger))]
    internal class InteractTriggerPatch
    {
        [HarmonyPostfix, HarmonyPatch("Update")]
        private static void Update_Postfix(ref string ___hoverTip, ref string ___disabledHoverTip, ref string ___holdTip)
        {
            TranslateHudTip(___hoverTip, ___disabledHoverTip, ___holdTip);
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void Start_Prefix(ref string ___hoverTip, ref string ___disabledHoverTip, ref string ___holdTip)
        {
            ___hoverTip = ___hoverTip.Replace("Play record", "레코드 재생하기");
            ___hoverTip = ___hoverTip.Replace("Flush", "물 내리기");
            ___hoverTip = ___hoverTip.Replace("Squeeze", "인형 만지기");

            ___hoverTip = ___hoverTip.Replace("Pull cord", "코드 당기기");
            ___hoverTip = ___hoverTip.Replace("(Hold)", "(길게 누르기)");

            ___hoverTip = ___hoverTip.Replace("Set candles", "양초 켜기");
            ___hoverTip = ___hoverTip.Replace("Hit pumpkin", "호박 치기");
            ___hoverTip = ___hoverTip.Replace("Switch water", "샤워기 전환하기");

            ___hoverTip = ___hoverTip.Replace("Lift glass", "덮개 여닫기");

            ___hoverTip = ___hoverTip.Replace("Push", "밀기");

            ___hoverTip = ___hoverTip.Replace("Eject", "비상 탈출");
            ___holdTip = ___holdTip.Replace("Eject", "비상 탈출");

            ___hoverTip = ___hoverTip.Replace("Tune radio", "라디오 조정하기");
            ___hoverTip = ___hoverTip.Replace("Toggle radio", "라디오 전원 전환하기");

            ___hoverTip = ___hoverTip.Replace("Pull up", "문 올리기");
            ___hoverTip = ___hoverTip.Replace("Pull down", "문 내리기");
            ___hoverTip = ___hoverTip.Replace("Place item", "아이템 배치하기");

            ___hoverTip = ___hoverTip.Replace("Remove key", "열쇠 제거하기");
            ___hoverTip = ___hoverTip.Replace("Try ignition", "시동 걸기");

            ___holdTip = ___holdTip.Replace("Trying ignition", "시동 거는 중");

            ___hoverTip = ___hoverTip.Replace("Drive", "전진");
            ___hoverTip = ___hoverTip.Replace("Reverse", "후진");
            ___hoverTip = ___hoverTip.Replace("Park", "주차");

            ___hoverTip = ___hoverTip.Replace("Sit down", "앉기");
            ___hoverTip = ___hoverTip.Replace("Honk", "경적 울리기");
            ___holdTip = ___holdTip.Replace("Honking", "경적 울리는 중");

            ___hoverTip = ___hoverTip.Replace("Switch headlights", "전조등 전환하기");
            ___hoverTip = ___hoverTip.Replace("Toggle cabin window", "트렁크 창문 전환하기");
            ___hoverTip = ___hoverTip.Replace("Toggle wipers", "와이퍼 전환하기");
            
            ___hoverTip = ___hoverTip.Replace("Open/shut hood", "후드 열기/닫기");
            ___holdTip = ___holdTip.Replace("Open/shut hood", "후드 열기/닫기");

            ___hoverTip = ___hoverTip.Replace("Beam up", "작동하기");
            ___hoverTip = ___hoverTip.Replace("Beam up", "작동하기");

            if (___hoverTip == "Charge item : [LMB]")
            {
                ___hoverTip = "아이템 충전하기 : [LMB]";
            }
            else if (___hoverTip == "Access terminal : [LMB]")
            {
                ___hoverTip = "터미널 접근하기 : [LMB]";
            }
            else if (___hoverTip == "Disable speaker: [LMB]")
            {
                ___hoverTip = "스피커 끄기: [LMB]";
            }
            else if (___hoverTip == "Switch lights : [LMB]")
            {
                ___hoverTip = "전등 전환하기 : [LMB]";
            }
            else if (___hoverTip == "Change suit")
            {
                ___hoverTip = "슈트 변경하기";
            }
            else if (___hoverTip == "Open : [LMB]")
            {
                ___hoverTip = "열기 : [LMB]";
            }
            else if (___hoverTip == "Open: [LMB]")
            {
                ___hoverTip = "열기: [LMB]";
            }
            else if (___hoverTip == "Open door : [LMB]")
            {
                ___hoverTip = "문 열기 : [LMB]";
            }
            else if (___hoverTip == "Close door : [LMB]")
            {
                ___hoverTip = "문 닫기 : [LMB]";
            }
            else if (___hoverTip == "Enter : [LMB]")
            {
                ___hoverTip = "들어가기 : [LMB]";
            }
            else if (___hoverTip == "Exit : [LMB]")
            {
                ___hoverTip = "나가기 : [LMB]";
            }
            else if (___hoverTip == "Use door : [LMB]")
            {
                ___hoverTip = "문 사용하기 : [LMB]";
            }
            else if (___hoverTip == "Store item : [LMB]")
            {
                ___hoverTip = "아이템 보관하기 : [LMB]";
            }
            else if (___hoverTip == "Use ladder : [LMB]")
            {
                ___hoverTip = "사다리 사용하기 : [LMB]";
            }
            else if (___hoverTip == "Climb : [LMB]")
            {
                ___hoverTip = "오르기 : [LMB]";
            }
            else if (___hoverTip == "Let go : [LMB]")
            {
                ___hoverTip = "내리기 : [LMB]";
            }
            else if (___hoverTip == "Switch camera : [LMB]")
            {
                ___hoverTip = "카메라 전환하기 : [LMB]";
            }
            else if (___hoverTip.Contains("Switch TV"))
            {
                ___hoverTip = ___hoverTip.Replace("Switch TV", "TV 전환하기");
            }

            if (___disabledHoverTip == "(Requires battery-powered item)")
            {
                ___disabledHoverTip = "(배터리로 작동하는 아이템 필요)";
            }
            else if (___disabledHoverTip == "[Nothing to store]")
            {
                ___disabledHoverTip = "[보관할 아이템이 없음]";
            }
            else if (___disabledHoverTip == "Locked")
            {
                ___disabledHoverTip = "잠김";
            }
            ___hoverTip = ___hoverTip.Replace("Ring bell", "종 울리기");
            ___hoverTip = ___hoverTip.Replace("Pull switch", "스위치 당기기");
            ___hoverTip = ___hoverTip.Replace("Flip switch", "스위치 전환하기");
            ___hoverTip = ___hoverTip.Replace("Pull valve", "밸브 돌리기");
            ___holdTip = ___holdTip.Replace("[Pulling valve]", "[밸브 돌리는 중]");
            ___disabledHoverTip = ___disabledHoverTip.Replace("[Cannot pull valve]", "[밸브를 돌릴 수 없음]");
        }

        static void TranslateHudTip(string ___hoverTip, string ___disabledHoverTip, string ___holdTip)
        {
            if (___hoverTip != null)
            {
                ___hoverTip = ___hoverTip.Replace("Let go", "내리기");
                ___hoverTip = ___hoverTip.Replace("Climb", "오르기");
                ___hoverTip = ___hoverTip.Replace("Use ladder", "사다리 사용하기");

                ___hoverTip = ___hoverTip.Replace("Locked (pickable)", "잠김 (자물쇠 따개 사용 가능)");
                ___hoverTip = ___hoverTip.Replace("Use door", "문 사용하기");
            }
            else if (___disabledHoverTip != null)
            {
                ___disabledHoverTip = ___disabledHoverTip.Replace("Picking lock", "자물쇠 따는 중");
                ___disabledHoverTip = ___disabledHoverTip.Replace(" sec.", "초 남음.");

                ___disabledHoverTip = ___disabledHoverTip.Replace("Use key", "열쇠 사용하기");
                ___disabledHoverTip = ___disabledHoverTip.Replace("Locked", "잠김");
            }
            else if (___holdTip != null)
            {
                ___holdTip = ___holdTip.Replace("Picking lock", "자물쇠 따는 중");
            }
        }
    }
}
