using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using RecruitEveryone.Patches;
using System.Collections.Generic;
using TaleWorlds.Library;
using SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace RecruitEveryone.Behaviors
{
    internal class RecruitEveryoneCampaignBehavior : CampaignBehaviorBase
    {
        public static LordConversationsCampaignBehavior? LordConversationsCampaignBehaviorInstance;

        public static CompanionsCampaignBehavior? CompanionsCampaignBehaviorInstance;

        public static CharacterDevelopmentCampaignBehavior? CharacterDevelopmentCampaignBehaviorInstance;

        private Hero? _hero = null;

        private static Dictionary<int, Hero>? _heroes;

        private static List<int>? _hired;

        private int _key;

        public RecruitEveryoneCampaignBehavior()
        {
            _hired = new();
            _heroes = new();
            LordConversationsCampaignBehaviorInstance = new();
            CompanionsCampaignBehaviorInstance = new();
            CharacterDevelopmentCampaignBehaviorInstance = new();
        }

        /* LordConversationsCampaignBehavior */
        protected void AddDialogs(CampaignGameStarter starter)
        {
            RecruitCharacter(starter, "hero_main_options", "lord_pretalk");
            RecruitCharacter(starter, "tavernkeeper_talk", "tavernkeeper_pretalk");
            RecruitCharacter(starter, "tavernmaid_talk");
            RecruitCharacter(starter, "talk_bard_player");
            RecruitCharacter(starter, "taverngamehost_talk", "start");
            RecruitCharacter(starter, "town_or_village_player", "town_or_village_pretalk");
            RecruitCharacter(starter, "weaponsmith_talk_player", "merchant_response_3");
            RecruitCharacter(starter, "barber_question1", "no_haircut_conversation_token");
            RecruitCharacter(starter, "shopworker_npc_player", "start_2");
            RecruitCharacter(starter, "blacksmith_player", "player_blacksmith_after_craft");
            RecruitCharacter(starter, "alley_talk_start");
        }

        private void RecruitCharacter(CampaignGameStarter starter, string start, string end = "close_window")
        {
            starter.AddPlayerLine("RE_main_option_faction_hire", start, start + "_companion_hire", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(conversation_hero_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(create_new_hero_consequence), 100, null, null);
            starter.AddDialogLine("RE_companion_hire", start + "_companion_hire", start + "_player_companion_hire_response", "{=fDjQOR5s}{HIRING_COST_EXPLANATION}", new ConversationSentence.OnConditionDelegate(LordConversationsCampaignBehaviorPatches.conversation_companion_hire_gold_on_condition), null, 100, null);
            starter.AddPlayerLine("RE_companion_hire_capacity_full", start + "_player_companion_hire_response", end, "{=afdN8ZU7}Thinking again, I already have more companions than I can manage.", new ConversationSentence.OnConditionDelegate(LordConversationsCampaignBehaviorPatches.too_many_companions), new ConversationSentence.OnConsequenceDelegate(conversation_exit_consequence), 100, null, null);
            starter.AddPlayerLine("RE_player_companion_hire_response_1", start + "_player_companion_hire_response", "hero_leave", "{=EiFPu9Np}Right... {GOLD_AMOUNT} Here you are.", new ConversationSentence.OnConditionDelegate(LordConversationsCampaignBehaviorPatches.conversation_companion_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(new_hero_hired), 100, null, null);
            starter.AddPlayerLine("RE_player_companion_hire_response_2", start + "_player_companion_hire_response", end, "{=65UMAav2}I can't afford that just now.", () => !LordConversationsCampaignBehaviorPatches.too_many_companions(), new ConversationSentence.OnConsequenceDelegate(conversation_exit_consequence), 100, null, null);
        }

        /* CommonVillagersCampaignBehavior -> conversation_town_or_village_start_on_condition */
        private bool conversation_hero_hire_on_condition()
        {
            _key = MathF.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());

            if (Hero.OneToOneConversationHero is null)
            {
                if (_hired!.Contains(_key))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private void create_new_hero_consequence()
        {
            Settings settings = new();

            CharacterObject character = Campaign.Current.ConversationManager.OneToOneConversationCharacter;
            Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;

            if (!_heroes!.ContainsKey(_key))
            {
                CharacterObject template = character;
                // CompanionCampaignBehavior -> IntializeCompanionTemplateList()
                if (settings.TemplateCharacter == "Wanderer")
                {
                    // Give hero random wanderer's focus, skills, and combat equipment with same culture and sex
                    template = character.Culture.NotableAndWandererTemplates.GetRandomElementWithPredicate((CharacterObject x) => x.Occupation == Occupation.Wanderer && x.IsFemale == character.IsFemale);
                }
                else if (settings.TemplateCharacter == "Lords")
                {
                    // Give hero random lord's focus, skills, and combat equipment with same culture and sex
                    template = character.Culture.LordTemplates.GetRandomElementWithPredicate((CharacterObject x) => x.IsFemale == character.IsFemale);
                }

                // Create a new hero!
                _hero = HeroCreator.CreateSpecialHero(template, Hero.MainHero.CurrentSettlement, null, null, (int)agent.Age);

                // Meet character for first time
                _hero.HasMet = true;

                // Add hero to heroes list
                _heroes.Add(_key, _hero);

                // Give hero the agent's appearance
                // hero.StaticBodyProperties = agent.BodyPropertiesValue.StaticProperties;
                AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(_hero, agent.BodyPropertiesValue.StaticProperties);

                // Give hero agent's equipment
                Equipment civilianEquipment = agent.SpawnEquipment.Clone();
                // CharacterObject -> RandomBattleEquipment
                Equipment battleEquipment = template.AllEquipments.GetRandomElementWithPredicate((Equipment e) => !e.IsCivilian).Clone();
                EquipmentHelper.AssignHeroEquipmentFromEquipment(_hero, civilianEquipment);
                EquipmentHelper.AssignHeroEquipmentFromEquipment(_hero, battleEquipment);
                // Do equipment adjustment with companions
                // Not exactly sure what it does...
                // this.AdjustEquipment(_hero);
                AccessTools.Method(typeof(CompanionsCampaignBehavior), "AdjustEquipment").Invoke(CompanionsCampaignBehaviorInstance, new object[] { _hero });

                HeroHelper.DetermineInitialLevel(_hero);
                CharacterDevelopmentCampaignBehaviorInstance!.DevelopCharacterStats(_hero);
            }
            else
            {
                // Use existing hero
                _heroes.TryGetValue(_key, out _hero);
            }

            // Attach hero to character for now
            if (character.HeroObject is null)
            {
                // character.HeroObject = _hero;
                AccessTools.Property(typeof(CharacterObject), "HeroObject").SetValue(character, _hero);
            }
        }

        /* Review CampaignCheats -> AddCompanion */
        private void new_hero_hired()
        {
            _hero!.SetNewOccupation(Occupation.Wanderer);
            _hero.ChangeState(Hero.CharacterStates.Active);
            // AddCompanionAction and AddHeroToPartyAction
            LordConversationsCampaignBehaviorPatches.conversation_companion_hire_on_consequence();
            _hired!.Add(_key);
            RemoveHeroObjectFromCharacter();
        }

        private void conversation_exit_consequence()
        {
            RemoveHeroObjectFromCharacter();

            // Learned that this is used in some Issues to disable quest heroes!
            DisableHeroAction.Apply(_hero);
        }

        private void RemoveHeroObjectFromCharacter()
        {
            CharacterObject character = Campaign.Current.ConversationManager.OneToOneConversationCharacter;

            // Remove hero association from character
            if (character.HeroObject is not null)
            {
                // character.HeroObject = null;
                AccessTools.Property(typeof(CharacterObject), "HeroObject").SetValue(character, null);
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}