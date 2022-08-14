using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using RecruitEveryone.Behaviors;
using RecruitEveryone.Models;

namespace RecruitEveryone
{
    internal sealed class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("mod.bannerlord.everyone.recruit").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new RecruitEveryoneCampaignBehavior());
                campaignGameStarter.AddModel(new RecruitEveryoneClanModel());
            }
        }
    }
}