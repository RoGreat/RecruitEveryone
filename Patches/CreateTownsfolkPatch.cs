using HarmonyLib;
using SandBox.Source.Towns;
using System;
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
    [HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior))]
    internal class CreateTownsfolkPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateTownsMan")]
		private static bool Prefix1(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            Tuple<string, Monster> randomTownsManActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsManActionSetAndMonster();
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Townsman, -1, null, default)).Monster(randomTownsManActionSetAndMonster.Item2).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", false, relation, randomTownsManActionSetAndMonster.Item1, true, false, null, false, false, true);
            return false;
		}

        [HarmonyPrefix]
        [HarmonyPatch("CreateSlowTownsMan")]
        private static bool Prefix2(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Townsman, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlementSlow).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", false, relation, "as_human_villager_3", true, false, null, false, false, true);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateTownsManTavernDrinker")]
        private static bool Prefix3(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
			BasicCharacterObject townsman = culture.Townsman;
			string actionSetCode;
			string value;
			if (culture != null && (culture.StringId.ToLower() == "aserai" || culture.StringId.ToLower() == "khuzait"))
			{
				actionSetCode = "as_human_villager_drinker_with_bowl";
				value = "khuzait_pot_for_drink";
			}
			else
			{
				actionSetCode = "as_human_villager_drinker_with_mug";
				value = "kitchen_mug_a_drink_anim";
			}
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsman, -1, null, default)).Monster(Campaign.Current.HumanMonsterSettlementSlow).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge));
			__result = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, relation, actionSetCode, true, false, null, false, false, true)
			{
				PrefabNamesForBones =
				{
					{
						agentData.AgentMonster.MainHandItemBoneIndex,
						value
					}
				}
			};
			return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateTownsManCarryingStuff")]
        private static bool Prefix4(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
			BasicCharacterObject townsman = culture.Townsman;
			string randomStuff = CommonTownsfolkCampaignBehavior.GetRandomStuff(false);
			Monster monster;
			string actionSetAndMonsterForItem = CommonTownsfolkCampaignBehavior.GetActionSetAndMonsterForItem(randomStuff, false, out monster);
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsman, -1, null, default)).Monster(monster).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge));
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(randomStuff);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common_limited", false, relation, actionSetAndMonsterForItem, true, false, @object, false, false, true);
			if (@object == null)
			{
				locationCharacter.PrefabNamesForBones.Add(agentData.AgentMonster.MainHandItemBoneIndex, randomStuff);
			}
			__result = locationCharacter;
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("CreateTownsWoman")]
		private static bool Prefix5(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			Tuple<string, Monster> randomTownsManActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsManActionSetAndMonster();
			__result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Townsman, -1, null, default)).Monster(randomTownsManActionSetAndMonster.Item2).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "npc_common", false, relation, randomTownsManActionSetAndMonster.Item1, true, false, null, false, false, true);
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch("CreateTownsWomanCarryingStuff")]
		private static bool Prefix6(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			BasicCharacterObject townswoman = culture.Townswoman;
			string randomStuff = CommonTownsfolkCampaignBehavior.GetRandomStuff(true);
			Monster monster;
			string actionSetAndMonsterForItem = CommonTownsfolkCampaignBehavior.GetActionSetAndMonsterForItem(randomStuff, false, out monster);
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default)).Monster(monster).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge));
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(randomStuff);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common_limited", false, relation, actionSetAndMonsterForItem, true, false, @object, false, false, true);
			if (@object == null)
			{
				locationCharacter.PrefabNamesForBones.Add(agentData.AgentMonster.MainHandItemBoneIndex, randomStuff);
			}
			__result = locationCharacter;
			return false;
		}
	}
}