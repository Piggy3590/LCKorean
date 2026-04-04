using HarmonyLib;

namespace LCKR.Patches
{
    [HarmonyPatch(typeof(UnlockableSuit))]
    internal class UnlockableSuitPatch
    {
        /*
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void Update_Postfix(UnlockableSuit __instance)
        {
            __instance.GetComponent<InteractTrigger>().hoverTip = "슈트 변경하기: " + StartOfRound.Instance.unlockablesList.unlockables[__instance.suitID].unlockableName;
        }
        */
    }
}
