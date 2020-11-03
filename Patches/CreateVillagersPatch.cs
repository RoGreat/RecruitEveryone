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
    [HarmonyPatch(typeof(CommonVillagersCampaignBehavior))]
    internal class CreateVillagersPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateVillageMan")]
		private static bool Prefix1(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            Tuple<string, Monster> randomTownsManActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsManActionSetAndMonster();
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.Villager, -1, null, default)).Monster(randomTownsManActionSetAndMonster.Item2).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), null, false, relation, randomTownsManActionSetAndMonster.Item1, true, false, null, false, false, true);
            return false;
		}

        [HarmonyPrefix]
        [HarmonyPatch("CreateVillageWoman")]
        private static bool Prefix2(ref LocationCharacter __result, CultureObject culture, LocationCharacter.CharacterRelations relation)
        {
            Tuple<string, Monster> randomTownsWomanActionSetAndMonster = CommonTownsfolkCampaignBehavior.GetRandomTownsWomanActionSetAndMonster();
            __result = new LocationCharacter(new AgentData(new SimpleAgentOrigin(culture.VillageWoman, -1, null, default)).Monster(randomTownsWomanActionSetAndMonster.Item2).Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, Campaign.Current.Models.AgeModel.MaxAge)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), null, false, relation, randomTownsWomanActionSetAndMonster.Item1, true, false, null, false, false, true);
            return false;
        }
    }
}