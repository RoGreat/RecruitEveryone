using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using RecruitEveryone.Behaviors;

namespace RecruitEveryone
{
    internal class RESubModule : MBSubModuleBase
    {
        private static Harmony _harmony;

        private static readonly bool _log = false;

        public static void Debug(string message)
        {
            if (_log)
            {
                InformationManager.DisplayMessage(new InformationMessage(message, new Color(0.6f, 0.2f, 1f)));
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            _harmony = new Harmony("mod.bannerlord.everyone.recruit");
            _harmony.PatchAll();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony.UnpatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameLoaded(game, gameStarter);
            if (game.GameType is Campaign && gameStarter is CampaignGameStarter starter)
            {
                CampaignGameStarter gameInitializer = starter;
                AddBehaviors(gameInitializer);
            }
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new RECampaignBehavior());
        }
    }
}