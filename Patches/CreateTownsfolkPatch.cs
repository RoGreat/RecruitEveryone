using HarmonyLib;
using SandBox.Source.Towns;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using RecruitEveryone.Models;

namespace RecruitEveryone
{
	[HarmonyPatch(typeof(CommonTownsfolkCampaignBehavior))]
	internal class CreateTownsfolkPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsMan")]
		private static void Postfix1(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateSlowTownsMan")]
		private static void Postfix2(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManForTavern")]
		private static void Postfix3(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManTavernDrinker")]
		private static void Postfix4(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManCarryingStuff")]
		private static void Postfix5(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWoman")]
		private static void Postfix6(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWomanCarryingStuff")]
		private static void Postfix7(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWomanForTavern")]
		private static void Postfix8(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateBroomsWoman")]
		private static void Postfix9(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateBeggar")]
		private static void Postfix10(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.BecomeTeenagerAge, REAgeModel.MaxAge));
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateFemaleBeggar")]
		private static void Postfix11(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.BecomeTeenagerAge, REAgeModel.MaxAge));
		}
	}

	[HarmonyPatch(typeof(BarberCampaignBehavior), "CreateBarber")]
	internal class CreateBarberPatch
    {
		private static void Postfix(ref LocationCharacter __result)
		{
			__result.AgentData.Age(MBRandom.RandomInt(Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge));
		}
	}
}