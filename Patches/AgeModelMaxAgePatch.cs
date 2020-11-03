using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(DefaultAgeModel), "MaxAge", MethodType.Getter)]
    internal class AgeModelMaxAge
    {
        private static void Postfix(ref int __result)
        {
            __result = 80;
        }
    }
}