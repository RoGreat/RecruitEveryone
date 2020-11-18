using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
using TaleWorlds.Core;
using RecruitEveryone.Models;
using SandBox.Source.Towns;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(TavernEmployeesCampaignBehavior))]
    internal class CreateTavernEmployeesPatch
	{
        [HarmonyPostfix]
        [HarmonyPatch("CreateTavernWench")]
		private static void Postfix1(ref LocationCharacter __result)
        {
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTavernkeeper")]
		private static void Postfix2(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateMusician")]
		private static void Postfix3(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateRansomBroker")]
		private static void Postfix4(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}
    }

	[HarmonyPatch(typeof(BoardGameCampaignBehavior), "CreateGameHost")]
	internal class CreateGameHostPatch
	{
		private static void Postfix(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.BecomeOldAge, REAgeModel.MaxAge));
		}
	}
}