using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using RecruitEveryone.Behaviors;
using RecruitEveryone.Models;
using RecruitEveryone.Settings;

namespace RecruitEveryone
{
    internal sealed class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("mod.bannerlord.everyone.recruit").PatchAll();
            REConfig.Initialize();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new RELordConversationsCampaignBehavior());
                campaignGameStarter.AddModel(new REClanModel());
            }
        }
    }
}