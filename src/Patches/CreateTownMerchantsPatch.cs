using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
using RecruitEveryone.Models;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(TownMerchantsCampaignBehavior))]
    internal class CreateTownMerchantsPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CreateBlacksmith")]
        private static void Postfix1(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateMerchant")]
        private static void Postfix2(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateHorseTrader")]
        private static void Postfix3(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateArmorer")]
        private static void Postfix4(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateWeaponsmith")]
        private static void Postfix5(ref LocationCharacter __result)
        {
            __result.AgentData.Age(REAgeModel.NormalAgeDist);
        }
    }
}