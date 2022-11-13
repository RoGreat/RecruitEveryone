using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace RecruitEveryone.CampaignSystem
{
    /* Reference CampaignCheats */
    public static class RECommands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("set_companion_limit_is_enabled", "recruit_everyone")]
        public static string SetCompanionLimitIsEnabled(List<string> strings)
        {
            RESettings settings = new();
            if (strings.Count != 1 || (strings[0] != "0" && strings[0] != "1"))
            {
                return "Input is incorrect.";
            }
            bool flag = strings[0] == "1";
            settings.ToggleCompanionLimit = flag;
            return "Setting companion limit is " + (flag ? "enabled." : "disabled.");
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_companion_limit", "recruit_everyone")]
        public static string SetCompanionLimit(List<string> strings)
        {
            RESettings settings = new();
            if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0))
            {
                return "Format is \"recruit_everyone.set_companion_limit [CompanionLimit]\".";
            }
            int num;
            if (!int.TryParse(strings[0], out num) || num < 0)
            {
                return "Please enter a number greater than or equal to 0";
            }
            settings.ToggleCompanionLimit = true;
            settings.CompanionLimit = num;
            return "Success";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_character_template", "recruit_everyone")]
        public static string SetCharacterTemplate(List<string> strings)
        {
            RESettings settings = new();
            if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0))
            {
                return "Format is \"recruit_everyone.set_character_template [\"default\"/\"wanderer\"]\".";
            }
            string template = CampaignCheats.ConcatenateString(strings);
            if (template is null)
            {
                return "Please enter \"default\" or \"wanderer\"";
            }
            else if (string.Equals(template, "default", StringComparison.OrdinalIgnoreCase))
            {
                settings.TemplateCharacter = "Default";
                return "Success";
            }
            else if (string.Equals(template, "wanderer", StringComparison.OrdinalIgnoreCase))
            {
                settings.TemplateCharacter = "Wanderer";
                return "Success";
            }
            return "Please enter \"default\" or \"wanderer\"";
        }
    }
}