using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace RecruitEveryone.Models
{
    internal class REAgeModel
    {
        private static readonly int _ofAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;

        private static readonly int _oldAge = Campaign.Current.Models.AgeModel.BecomeOldAge;

        public static int MaxAge
        {
            get
            {
                return 80;
            }
        }

        public static int YoungRandAge
        {
            get
            {
                return MBRandom.RandomInt(_ofAge, _oldAge);
            }
        }

        public static int OldRandAge
        {
            get
            {
                return MBRandom.RandomInt(_oldAge, MaxAge);
            }
        }

        public static int NormalAgeDist
        {
            get
            {
                return MBRandom.RandomInt(_ofAge, OldRandAge);
            }
        }

        public static int MiddleAgeDist
        {
            get
            {
                return MBRandom.RandomInt(YoungRandAge, OldRandAge);
            }
        }
    }
}