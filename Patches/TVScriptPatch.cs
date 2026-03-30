using HarmonyLib;
using UnityEngine.Video;

namespace LCKR.Patches
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
