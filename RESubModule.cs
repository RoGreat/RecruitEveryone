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
    internal class RESubModule : NoHarmonyLoader
    {
        private static Harmony _harmony;

        private static bool _log = false;

        public static void Debug(string message)
        {
            if (_log)
            {
                InformationManager.DisplayMessage(new InformationMessage(message, new Color(0.6f, 0.2f, 1f)));
            }
        }

        public override void NoHarmonyInit()
        {
            LogFile = "RENoHarmony.txt";
            LogDateFormat = "MM/dd/yy HH:mm:ss.fff";
        }
        
        public override void NoHarmonyLoad()
        {
            ReplaceModel<REAgeModel, DefaultAgeModel>();
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