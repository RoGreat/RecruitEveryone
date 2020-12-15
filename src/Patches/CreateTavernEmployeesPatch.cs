using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
using RecruitEveryone.Models;
using SandBox.Source.Towns;

namespace RecruitEveryone
{
    [HarmonyPatch(typeof(TavernEmployeesCampaignBehavior))]
    internal class CreateTavernEmployeesPatch
	{
		// Always younger
        [HarmonyPostfix]
        [HarmonyPatch("CreateTavernWench")]
		private static void Postfix1(ref LocationCharacter __result)
        {
			__result.AgentData.Age(REAgeModel.YoungRandAge);
		}

		// Middle age
		[HarmonyPostfix]
		[HarmonyPatch("CreateTavernkeeper")]
		private static void Postfix2(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.MiddleAgeDist);
		}

		// Normal
		[HarmonyPostfix]
		[HarmonyPatch("CreateMusician")]
		private static void Postfix3(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.NormalAgeDist);
		}

		// Middle age
		[HarmonyPostfix]
		[HarmonyPatch("CreateRansomBroker")]
		private static void Postfix4(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.MiddleAgeDist);
		}
    }

	[HarmonyPatch(typeof(BoardGameCampaignBehavior), "CreateGameHost")]
	internal class CreateGameHostPatch
	{
		// Always older
		private static void Postfix(ref LocationCharacter __result)
		{
			__result.AgentData.Age(REAgeModel.OldRandAge);
		}
	}
}