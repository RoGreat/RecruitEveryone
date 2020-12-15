using HarmonyLib;
using SandBox.Source.Towns;
using TaleWorlds.CampaignSystem;
using RecruitEveryone.Models;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(CommonVillagersCampaignBehavior))]
    internal class CreateVillagersPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageMan")]
        private static void Postfix1(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageManCarryingStuff")]
        private static void Postfix2(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageWoman")]
        private static void Postfix3(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageWomanCarryingStuff")]
        private static void Postfix4(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }
    }
}