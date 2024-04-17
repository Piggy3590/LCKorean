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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using System.Reflection;
using System.Collections.Specialized;
using UnityEngine.Video;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(TVScript))]
    internal class TVScriptPatch
    {
        [HarmonyPostfix, HarmonyPatch("SetTVScreenMaterial")]
        private static void SetTVScreenMaterial_Postfix(ref VideoClip[] ___tvClips)
        {
            for (int i = 0; i < ___tvClips.Length; i++)
            {
                if (___tvClips[i].name == "SnareFleaTipChannel2")
                {
                    ___tvClips[i] = Plugin.snareKorean;
                }
            }
        }
    }
}
