using HarmonyLib;
using SandBox.Source.Towns;
using TaleWorlds.CampaignSystem;
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
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateSlowTownsMan")]
		private static void Postfix2(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManForTavern")]
		private static void Postfix3(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManTavernDrinker")]
		private static void Postfix4(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsManCarryingStuff")]
		private static void Postfix5(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWoman")]
		private static void Postfix6(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWomanCarryingStuff")]
		private static void Postfix7(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateTownsWomanForTavern")]
		private static void Postfix8(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateBroomsWoman")]
		private static void Postfix9(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		// Always younger
		[HarmonyPostfix]
		[HarmonyPatch("CreateDancer")]
		private static void Postfix10(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.YoungRandAge);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateBeggar")]
		private static void Postfix11(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		[HarmonyPostfix]
		[HarmonyPatch("CreateFemaleBeggar")]
		private static void Postfix12(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}
	}

	[HarmonyPatch(typeof(BarberCampaignBehavior), "CreateBarber")]
	internal class CreateBarberPatch
    {
		private static void Postfix(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}
	}
}