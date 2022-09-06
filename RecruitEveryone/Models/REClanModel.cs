using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace RecruitEveryone.Models
{
    internal sealed class REClanModel : DefaultClanTierModel
    {
        public override int GetCompanionLimit(Clan clan)
        {
            RESettings settings = new();

            if (settings.ToggleCompanionLimit)
            {
                return settings.CompanionLimit;
            }
            return base.GetCompanionLimit(clan);
        }
    }
}