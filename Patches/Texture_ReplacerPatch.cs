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
using Texture_Replacer_BE5;
using Texture_Replacer;
using System.IO;
using LCKorean;
/*
namespace Texture_Replacer
{
    [HarmonyPatch(typeof(Texture_Replacer))]
    internal class Texture_ReplacerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Setup")]
        private static void Setup_Postfix()
        {
            Texture_Replacer.imagesPath_str = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location) + "LCKR_Tex");
        }
    }
}
*/