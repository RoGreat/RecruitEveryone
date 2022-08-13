using HarmonyLib;
using RecruitEveryone.Behaviors;
using SandBox.CampaignBehaviors;
using System;

namespace RecruitEveryone.Patches
{
    [HarmonyPatch]
    internal class CampaignBehaviorPatches
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "too_many_companions")]
        private static bool too_many_companions_patch(object instance)
        {
            throw new NotImplementedException();
        }
        public static bool too_many_companions()
        {
            return too_many_companions_patch(RecruitEveryoneCampaignBehavior.LordConversationsCampaignBehaviorInstance!);
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_gold_on_condition")]
        private static bool conversation_companion_hire_gold_on_condition_patch(object instance)
        {
            throw new NotImplementedException();
        }
        public static bool conversation_companion_hire_gold_on_condition()
        {
            return conversation_companion_hire_gold_on_condition_patch(RecruitEveryoneCampaignBehavior.LordConversationsCampaignBehaviorInstance!);
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_on_condition")]
        private static bool conversation_companion_hire_on_condition_patch(object instance)
        {
            throw new NotImplementedException();
        }
        public static bool conversation_companion_hire_on_condition()
        {
            return conversation_companion_hire_on_condition_patch(RecruitEveryoneCampaignBehavior.LordConversationsCampaignBehaviorInstance!);
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_on_consequence")]
        private static void conversation_companion_hire_on_consequence_patch(object instance)
        {
            throw new NotImplementedException();
        }
        public static void conversation_companion_hire_on_consequence()
        {
            conversation_companion_hire_on_consequence_patch(RecruitEveryoneCampaignBehavior.LordConversationsCampaignBehaviorInstance!);
        }
    }
}