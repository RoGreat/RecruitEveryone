using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace RecruitEveryone.Models
{
    internal class REAgeModel
    {
        private static readonly int _ofAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;

        private static readonly int _oldAge = Campaign.Current.Models.AgeModel.BecomeOldAge;

        private static readonly int _maxAge = Campaign.Current.Models.AgeModel.MaxAge;

        public static int configMaxAge { get; set; } = 80;

        public static int MaxAge
        {
            get
            {
                return MBMath.ClampInt(configMaxAge, _oldAge, _maxAge);
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