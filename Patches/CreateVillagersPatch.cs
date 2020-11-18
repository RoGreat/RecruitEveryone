using HarmonyLib;
using SandBox.Source.Towns;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
            __result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageManCarryingStuff")]
        private static void Postfix2(ref LocationCharacter __result)
        {
            __result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageWoman")]
        private static void Postfix3(ref LocationCharacter __result)
        {
            __result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateVillageWomanCarryingStuff")]
        private static void Postfix4(ref LocationCharacter __result)
        {
            __result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
        }
    }
}