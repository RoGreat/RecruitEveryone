using HarmonyLib;
using SandBox.Source.Towns;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(TownMerchantsCampaignBehavior))]
    internal class CreateTownMerchantsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateMerchant")]
        private static bool Prefix1(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Merchant, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_merchant", true, relation, "as_human_seller", true, false, null, false, false, true);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateHorseTrader")]
        private static bool Prefix2(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.HorseMerchant, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_horse_merchant", true, relation, "as_human_seller", true, false, null, false, false, true);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateArmorer")]
        private static bool Prefix3(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Armorer, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_armorer", true, relation, "as_human_seller", true, false, null, false, false, true);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateArmorer")]
        private static bool Prefix4(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Weaponsmith, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_weaponsmith", true, relation, "as_human_weaponsmith", true, false, null, false, false, true);
            return false;
        }
    }
}