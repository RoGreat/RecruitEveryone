using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Actions;
using SandBox.CampaignBehaviors;
using Helpers;
using HarmonyLib;
using HarmonyLib.BUTR.Extensions;
using TaleWorlds.Localization;

namespace RecruitEveryone.Behaviors
{
    internal class RELordConversationsCampaignBehavior : CampaignBehaviorBase
    {
        private Hero? _companionHero;

        private readonly List<Agent>? _hired;

        private readonly Dictionary<Agent, Hero> _heroes;

        /* LordConversationsCampaignBehavior Delegates */
        /* Conditions */
        private delegate bool conversation_companion_hire_gold_on_condition_delegate(LordConversationsCampaignBehavior instance);
        private static readonly conversation_companion_hire_gold_on_condition_delegate? conversation_companion_hire_gold_on_condition = AccessTools2.GetDelegate<conversation_companion_hire_gold_on_condition_delegate>(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_gold_on_condition");

        private delegate bool too_many_companions_delegate(LordConversationsCampaignBehavior instance);
        private static readonly too_many_companions_delegate? too_many_companions = AccessTools2.GetDelegate<too_many_companions_delegate>(typeof(LordConversationsCampaignBehavior), "too_many_companions");

        private delegate bool conversation_companion_hire_on_condition_delegate(LordConversationsCampaignBehavior instance);
        private static readonly conversation_companion_hire_on_condition_delegate? conversation_companion_hire_on_condition = AccessTools2.GetDelegate<conversation_companion_hire_on_condition_delegate>(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_on_condition");

        /* Consequences */
        private delegate void conversation_companion_hire_on_consequence_delegate (LordConversationsCampaignBehavior instance);
        private static readonly conversation_companion_hire_on_consequence_delegate? conversation_companion_hire_on_consequence = AccessTools2.GetDelegate<conversation_companion_hire_on_consequence_delegate>(typeof(LordConversationsCampaignBehavior), "conversation_companion_hire_on_consequence");

        /* Outside Delegates */
        /* Fields */
        private static readonly AccessTools.FieldRef<Agent, TextObject>? AgentName = AccessTools2.FieldRefAccess<Agent, TextObject>("_name");

        /* Property Setters */
        private delegate void SetHeroObjectDelegate(CharacterObject instance, Hero @value);
        private static readonly SetHeroObjectDelegate? SetHeroObject = AccessTools2.GetPropertySetterDelegate<SetHeroObjectDelegate>(typeof(CharacterObject), "HeroObject");

        private delegate void SetHeroStaticBodyPropertiesDelegate(Hero instance, StaticBodyProperties @value);
        private static readonly SetHeroStaticBodyPropertiesDelegate? SetHeroStaticBodyProperties = AccessTools2.GetPropertySetterDelegate<SetHeroStaticBodyPropertiesDelegate>(typeof(Hero), "StaticBodyProperties");

        /* Methods */
        private delegate void CompanionAdjustEquipmentDelegate(CompanionsCampaignBehavior instance, Hero companion);
        private static readonly CompanionAdjustEquipmentDelegate? CompanionAdjustEquipment = AccessTools2.GetDelegate<CompanionAdjustEquipmentDelegate>(typeof(CompanionsCampaignBehavior), "AdjustEquipment");

        public RELordConversationsCampaignBehavior()
        {
            _hired = new();
            _heroes = new();
        }

        /* LordConversationsCampaignBehavior */
        protected void AddDialogs(CampaignGameStarter starter)
        {
            // Wanderers
            RecruitCharacter(starter, "hero_main_options", "lord_pretalk");
            // Tavernkeeper
            RecruitCharacter(starter, "tavernkeeper_talk", "tavernkeeper_pretalk");
            // Tavernmaid
            RecruitCharacter(starter, "tavernmaid_talk");
            // Bard
            RecruitCharacter(starter, "talk_bard_player");
            // Tavern Game Host
            RecruitCharacter(starter, "taverngamehost_talk", "start");
            // Townsfolk and Villager
            RecruitCharacter(starter, "town_or_village_player", "town_or_village_pretalk");
            // Weaponsmith
            RecruitCharacter(starter, "weaponsmith_talk_player", "merchant_response_3");
            // Barber
            RecruitCharacter(starter, "barber_question1", "no_haircut_conversation_token");
            // Shopworker
            RecruitCharacter(starter, "shopworker_npc_player", "start_2");
            // Blacksmith
            RecruitCharacter(starter, "blacksmith_player", "player_blacksmith_after_craft");
            // Thug
            RecruitCharacter(starter, "alley_talk_start");
            // Arena Master
            RecruitCharacter(starter, "arena_master_talk");
            // Ransom Broker
            RecruitCharacter(starter, "ransom_broker_talk");
            // Probably more that I don't recall...
        }

        private void RecruitCharacter(CampaignGameStarter starter, string start, string end = "close_window")
        {
            starter.AddPlayerLine("RE" + "main_option_faction_hire", start, start + "companion_hire", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(conversation_hero_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(create_new_hero_consequence), 100, null, null);
            starter.AddDialogLine("RE" + "companion_hire", start + "companion_hire", start + "player_companion_hire_response", "{=fDjQOR5s}{HIRING_COST_EXPLANATION}", new ConversationSentence.OnConditionDelegate(RE_conversation_companion_hire_gold_on_condition), null, 100, null);
            starter.AddPlayerLine("RE" + "companion_hire_capacity_full", start + "player_companion_hire_response", end, "{=afdN8ZU7}Thinking again, I already have more companions than I can manage.", new ConversationSentence.OnConditionDelegate(RE_too_many_companions), new ConversationSentence.OnConsequenceDelegate(conversation_exit_consequence), 100, null, null);
            starter.AddPlayerLine("RE" + "player_companion_hire_response_1", start + "player_companion_hire_response", "hero_leave", "{=EiFPu9Np}Right... {GOLD_AMOUNT} Here you are.", new ConversationSentence.OnConditionDelegate(RE_conversation_companion_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(new_hero_hired), 100, null, null);
            starter.AddPlayerLine("RE" + "player_companion_hire_response_2", start + "player_companion_hire_response", end, "{=65UMAav2}I can't afford that just now.", () => !RE_too_many_companions(), new ConversationSentence.OnConsequenceDelegate(conversation_exit_consequence), 100, null, null);
        }

        private bool RE_conversation_companion_hire_gold_on_condition()
        {
            LordConversationsCampaignBehavior instance = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordConversationsCampaignBehavior>();
            return conversation_companion_hire_gold_on_condition!(instance);
        }

        private bool RE_too_many_companions()
        {
            LordConversationsCampaignBehavior instance = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordConversationsCampaignBehavior>();
            return too_many_companions!(instance);
        }

        private bool RE_conversation_companion_hire_on_condition()
        {
            LordConversationsCampaignBehavior instance = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordConversationsCampaignBehavior>();
            return conversation_companion_hire_on_condition!(instance);
        }

        /* CommonVillagersCampaignBehavior -> conversation_town_or_village_start_on_condition */
        private bool conversation_hero_hire_on_condition()
        {
            // Clear cached companion hero when starting a new conversation
            _companionHero = null;

            if (Hero.OneToOneConversationHero is not null)
            {
                return false;
            }

            Agent conversationAgent;
            try
            {
                conversationAgent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
            }
            catch
            {
                return false;
            }

            // Added an age check that should have been here
            // Appears to be a bug when recruiting younger agents
            if (_hired!.Contains(conversationAgent) || conversationAgent.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge)
            {
                return false;
            }
            return true;
        }

        private void create_new_hero_consequence()
        {
            if (Hero.OneToOneConversationHero is not null)
            {
                return;
            }

            Agent conversationAgent;
            try
            {
                conversationAgent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
            }
            catch
            {
                return;
            }

            CharacterObject conversationCharacter = Campaign.Current.ConversationManager.OneToOneConversationCharacter;

            // Remembering the last agents you talked to
            if (_heroes.ContainsKey(conversationAgent))
            {
                // Use existing hero
                _heroes.TryGetValue(conversationAgent, out _companionHero);
                SetHeroObject!(conversationCharacter, _companionHero);
                return;
            }

            // Remove agents not in scene
            RemoveAgents();

            RESettings settings = new();
            CharacterObject template = conversationCharacter;
            // CompanionCampaignBehavior -> IntializeCompanionTemplateList()
            if (settings.TemplateCharacter == "Wanderer")
            {
                // Give hero random wanderer's focus, skills, and combat equipment with same culture and sex
                template = conversationCharacter.Culture.NotableAndWandererTemplates.GetRandomElementWithPredicate(
                    (CharacterObject x) => x.Occupation == Occupation.Wanderer && x.IsFemale == conversationCharacter.IsFemale);
            }

            // Create a new hero!
            _companionHero = HeroCreator.CreateSpecialHero(template, Hero.MainHero.CurrentSettlement, null, null, (int)conversationAgent.Age);

            // Name permanence from the adoption module of old
            AgentName!(conversationAgent) = _companionHero.Name;

            // Meet character for first time
            _companionHero.SetHasMet();

            // Add hero to heroes list
            _heroes.Add(conversationAgent, _companionHero);

            // Give hero the agent's appearance
            SetHeroStaticBodyProperties!(_companionHero, conversationAgent.BodyPropertiesValue.StaticProperties);

            // Give hero agent's equipment
            Equipment civilianEquipment = conversationAgent.SpawnEquipment.Clone();
            // CharacterObject -> RandomBattleEquipment
            Equipment battleEquipment = template.AllEquipments.GetRandomElementWithPredicate((Equipment e) => !e.IsCivilian).Clone();
            EquipmentHelper.AssignHeroEquipmentFromEquipment(_companionHero, civilianEquipment);
            EquipmentHelper.AssignHeroEquipmentFromEquipment(_companionHero, battleEquipment);

            // Adjust Equipment like the wanderer do
            CompanionsCampaignBehavior companionsCampaignBehaviorInstance = Campaign.Current.CampaignBehaviorManager.GetBehavior<CompanionsCampaignBehavior>();
            CompanionAdjustEquipment!(companionsCampaignBehaviorInstance, _companionHero);

            CharacterDevelopmentCampaignBehavior characterDevelopmentCampaignBehaviorInstance = Campaign.Current.CampaignBehaviorManager.GetBehavior<CharacterDevelopmentCampaignBehavior>();

            //character.HeroObject = _companionHero;
            SetHeroObject!(conversationCharacter, _companionHero);
        }

        private void RemoveAgents()
        {
            // If the list is empty then return
            if (_heroes.IsEmpty())
            {
                return;
            }
            // Clear all items in _heroes if none of the agents are in the current mission
            foreach (Agent agent in Mission.Current.Agents)
            {
                // If there is an agent present then return
                if (_heroes.ContainsKey(agent))
                {
                    return;
                }
            }
            _heroes.Clear();
        }

        /* Review CampaignCheats -> AddCompanion */
        private void new_hero_hired()
        {
            Agent conversationAgent;
            try
            {
                conversationAgent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
            }
            catch
            {
                return;
            }
            LordConversationsCampaignBehavior instance = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordConversationsCampaignBehavior>();
            _companionHero!.SetNewOccupation(Occupation.Wanderer);
            _companionHero.ChangeState(Hero.CharacterStates.Active);
            conversation_companion_hire_on_consequence!(instance);
            _hired!.Add(conversationAgent);
            RemoveHeroObjectFromCharacter();
        }

        private void conversation_exit_consequence()
        {
            RemoveHeroObjectFromCharacter();

            // Learned that this is used in some Issues to disable quest heroes!
            DisableHeroAction.Apply(_companionHero);
        }

        private void RemoveHeroObjectFromCharacter()
        {
            CharacterObject character = Campaign.Current.ConversationManager.OneToOneConversationCharacter;

            // Remove hero association from character
            if (character.HeroObject is not null)
            {
                // character.HeroObject = null;
                SetHeroObject!(character, null!);
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
