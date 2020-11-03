//using HarmonyLib;
//using SandBox.Source.Towns;
//using System;
//using System.Collections.Generic;
//using TaleWorlds.CampaignSystem;
//using TaleWorlds.CampaignSystem.Actions;
//using TaleWorlds.CampaignSystem.SandBox;
//using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
//using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.Towns;
//using TaleWorlds.CampaignSystem.SandBox.GameComponents;
//using TaleWorlds.Core;
//using TaleWorlds.Localization;

//namespace RecruitEveryone
//{
//    [HarmonyPatch(typeof(DefaultAgeModel), "MaxAge", MethodType.Getter)]
//    internal class AgeModelMaxAge
//    {
//		private static void Postfix(ref int __result)
//        {
//            __result = 80;
//		}
//    }
//}