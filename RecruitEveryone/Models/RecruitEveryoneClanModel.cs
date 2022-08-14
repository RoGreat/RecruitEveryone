using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace RecruitEveryone.Models
{
    internal sealed class RecruitEveryoneClanModel : DefaultClanTierModel
    {
        public override int GetCompanionLimit(Clan clan)
        {
            Settings settings = new();

            if (settings.ToggleCompanionLimit)
            {
                return settings.CompanionLimit;
            }
            return base.GetCompanionLimit(clan);
        }
    }
}