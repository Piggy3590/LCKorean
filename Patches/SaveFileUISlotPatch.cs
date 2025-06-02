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
    [HarmonyPatch(typeof(SaveFileUISlot))]
    internal class SaveFileUISlotPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        private static void OnEnable_Postfix(SaveFileUISlot __instance, ref string ___fileString)
        {
            if (__instance.fileNum == -1)
            {
                __instance.fileNameText.text = GameNetworkManager.Instance.GetNameForWeekNumber(-1);
            }
            if (ES3.FileExists(___fileString))
            {
                if (__instance.fileNum != -1)
                {
                    int num = ES3.Load<int>("GroupCredits", ___fileString, 0);
                    int num2 = ES3.Load<int>("Stats_DaysSpent", ___fileString, 0);
                    __instance.fileStatsText.text = string.Format("${0}\n{1}일차", num, num2);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetFileToThis")]
        private static void SetFileToThis_Postfix(ref TextMeshProUGUI ___specialTipText)
        {
            if (___specialTipText != null && ___specialTipText.enabled)
            {
                ___specialTipText.text = ___specialTipText.text.Replace("This is the weekly challenge moon. You have one day to make as much profit as possible.",
                    "주간 챌린지 위성입니다.하루 안에 가능한 한 많은 수익을 획득하세요.");
            }
        }
    }
}
