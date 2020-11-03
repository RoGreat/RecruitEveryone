using HarmonyLib;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using NoHarmony;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace RecruitEveryone
{
    internal class REAgeModel : DefaultAgeModel
    {
        public override int MaxAge
        {
            get
            {
                return 50;
            }
        }
    }
}