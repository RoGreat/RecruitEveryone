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
    [HarmonyPatch(typeof(TavernEmployeesCampaignBehavior))]
    internal class CreateTavernEmployeesPatch
	{
        [HarmonyPrefix]
        [HarmonyPatch("CreateTavernWench")]
		private static bool Prefix1(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
			AgentData agentData = new AgentData(new SimpleAgentOrigin(culture.TavernWench, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.BecomeTeenagerAge, Campaign.Current.Models.AgeModel.BecomeOldAge));
			__result = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "sp_tavern_wench", true, relation, "as_human_barmaid", true, false, null, false, false, true)
			{
				PrefabNamesForBones =
				{
					{
						agentData.AgentMonster.OffHandItemBoneIndex,
						"kitchen_pitcher_b_tavern"
					}
				}
			};
            return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("CreateTavernkeeper")]
		private static bool Prefix2(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			__result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Tavernkeeper, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "spawnpoint_tavernkeeper", true, relation, "as_human_tavern_keeper", true, false, null, false, false, true);
			return false;
		}


		[HarmonyPrefix]
		[HarmonyPatch("CreateMusician")]
		private static bool Prefix3(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			__result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Musician, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.BecomeTeenagerAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "musician", true, relation, "as_human_musician", true, true, null, false, false, true);
            return false;
        }

		[HarmonyPrefix]
		[HarmonyPatch("CreateRansomBroker")]
		private static bool Prefix4(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			__result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.RansomBroker, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlement).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", true, relation, null, true, false, null, false, false, true);
			return false;
		}
    }
}