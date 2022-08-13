using MarryAnyone.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace RecruitEveryone.Models
{
    internal class RecruitEveryoneClanModel : DefaultClanTierModel
    {
        public override int GetCompanionLimit(Clan clan)
        {
            IRESettingsProvider settings = new RESettings();

            if (settings.ToggleCompanionLimit)
            {
                return settings.CompanionLimit;
            }
            return base.GetCompanionLimit(clan);
        }
    }
}