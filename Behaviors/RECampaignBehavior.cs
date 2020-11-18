using HarmonyLib;
using MountAndBlade.CampaignBehaviors;
using RecruitEveryone.Models;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace RecruitEveryone.Behaviors
{
    internal class RECampaignBehavior : CampaignBehaviorBase
    {
        private static List<int> _recruitedAgents;

        private List<CharacterObject> _wandererTemplates;

        protected void AddDialogs(CampaignGameStarter starter)
        {
            // Reset them as wanderers after game restarts
            foreach (Hero hero in Hero.All)
            {
                if (hero.CharacterObject.Occupation != Occupation.Wanderer && hero.IsPlayerCompanion)
                {
                    AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer);
                }
            }

            // Skip the nonexistent introduction
            starter.AddDialogLine("wanderer_skip_intro", "wanderer_preintroduction", "hero_main_options", "{=LUiQ6bpo}Very well, then. What is it?", new ConversationSentence.OnConditionDelegate(conversation_wanderer_skip_preintroduction_on_condition), null, 200, null);

            RecruitCharacter(starter, "hero_main_options", "lord_pretalk");
            RecruitCharacter(starter, "tavernkeeper_talk", "tavernkeeper_pretalk");
            RecruitCharacter(starter, "tavernmaid_talk");
            RecruitCharacter(starter, "talk_bard_player");
            // CharacterHire(starter, "arena_master_talk");     Single person, awkward
            // RecruitCharacter(starter, "ransom_broker_talk");    Crashes
            RecruitCharacter(starter, "taverngamehost_talk", "start");
            RecruitCharacter(starter, "town_or_village_player", "town_or_village_pretalk");
            RecruitCharacter(starter, "weaponsmith_talk_player", "merchant_response_3");
            RecruitCharacter(starter, "barber_question1", "no_haircut_conversation_token");
            RecruitCharacter(starter, "shopworker_npc_player", "start_2");
            RecruitCharacter(starter, "blacksmith_player", "player_blacksmith_after_craft");

            // Haven't tested
            // CharacterHire(starter, "prison_guard_talk");
            // CharacterHire(starter, "castle_guard_talk");
        }

        private bool conversation_wanderer_skip_preintroduction_on_condition()
        {
            if (Hero.OneToOneConversationHero != null)
            {
                string stringId = Hero.OneToOneConversationHero.Template.StringId;
                TextObject result;
                if (!GameTexts.TryGetText("prebackstory", out result, stringId))
                {
                    return true;
                }
            }
            return false;
        }

        private void RecruitCharacter(CampaignGameStarter starter, string start, string end = "close_window")
        {
            starter.AddPlayerLine("player_starts_discussion_RE", start, start + "_discussion_RE", "{=lord_conversations_343}There is something I'd like to discuss.", new ConversationSentence.OnConditionDelegate(conversation_player_can_initiate_discussion), null, 101, null, null);
            starter.AddDialogLine("character_agrees_to_discussion_RE", start + "_discussion_RE",  start + "_agreed_to_discussion_RE", "{=OD1m1NYx}{STR_INTRIGUE_AGREEMENT}", new ConversationSentence.OnConditionDelegate(conversation_character_agrees_to_discussion_on_condition), null, 100, null);
            starter.AddPlayerLine("player_talks_to_recruit_RE", start + "_agreed_to_discussion_RE", start + "_recruit_RE", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(able_to_recruit), null, 100, null, null);
            starter.AddPlayerLine("player_plans_never_mind_RE", start + "_agreed_to_discussion_RE", end, "{=D33fIGQe}Never mind.", null, null, 100, null, null);
            starter.AddDialogLine("character_recruited_RE", start + "_recruit_RE", "close_window", "{=QffdjUxf}Very well. I'll get my gear and join you outside.[rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(recruit_character), 100, null);
        }

        private bool conversation_player_can_initiate_discussion()
        {
            if (Hero.OneToOneConversationHero != null)
            {
                if (Hero.OneToOneConversationHero.IsNotable)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool conversation_character_agrees_to_discussion_on_condition()
        {
            MBTextManager.SetTextVariable("STR_INTRIGUE_AGREEMENT", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_lord_intrigue_accept", CharacterObject.OneToOneConversationCharacter), false);
            return true;
        }

        private bool able_to_recruit()
        {
            int agent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
            if (_recruitedAgents == null)
            {
                _recruitedAgents = new List<int>();
            }
            else if (_recruitedAgents.Contains(agent))
            {
                return false;
            }
            return Clan.PlayerClan.Companions.Count < Clan.PlayerClan.CompanionLimit;
        }

        private void recruit_character()
        {
            Hero hero; 
            if (Hero.OneToOneConversationHero == null)
            {
                int recruitedAgent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
                _recruitedAgents.Add(recruitedAgent);

                // Agents for some reason always have default weight and build bodyproperties
                Agent agent = (Agent)MissionConversationHandler.Current.ConversationManager.OneToOneConversationAgent;
                CharacterObject character = CharacterObject.OneToOneConversationCharacter;
                RESubModule.Debug("Character Age: " + character.Age + " Agent Age: " + agent.Age);

                // In case a character is extremely old somehow
                int age;
                age = MBMath.ClampInt((int)agent.Age, Campaign.Current.Models.AgeModel.BecomeTeenagerAge, REAgeModel.MaxAge);

                // Create companion action
                hero = HeroCreator.CreateSpecialHero(character, Settlement.CurrentSettlement, null, null, age);

                // Change body properties to what the agent is currently
                BodyProperties agentBodyProperties = agent.BodyPropertiesValue;
                RESubModule.Debug("DynamicProperties: " + agentBodyProperties.DynamicProperties.ToString());

                AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(hero, agentBodyProperties.StaticProperties);

                // Wanderer templates
                _wandererTemplates = new List<CharacterObject>(from x in CharacterObject.Templates where x.Occupation == Occupation.Wanderer && x.Culture == character.Culture && x.IsFemale == character.IsFemale select x);

                int count = _wandererTemplates.Count;
                int num = MBRandom.RandomInt(count - 1);

                // Set traits
                int level;
                foreach (TraitObject trait in DefaultTraits.All)
                {
                    level = _wandererTemplates[num].GetTraitLevel(trait);
                    hero.SetTraitLevel(trait, level);
                }
                int num2;
                if (age < 20)
                {
                    foreach (TraitObject trait in DefaultTraits.All)
                    {
                        num2 = 12 + 4 * hero.GetTraitLevel(trait);
                        if (age < num2)
                        {
                            age = num2;
                        }
                    }
                }

                // After CreateSpecialHero
                Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, _wandererTemplates[num]);
                // Wanderer equipment
                AccessTools.Property(typeof(Hero), "BattleEquipment").SetValue(hero, _wandererTemplates[num].BattleEquipments.GetRandomElement());
                // Civilian equipment
                // AccessTools.Property(typeof(Hero), "BattleEquipment").SetValue(hero, hero.CivilianEquipment);

                // Remove agent after recruitment (prevent doppelgangers)
                // Will crash if used outside of the tavern in most cases
                // Causes crashing with ransom broker
                if (CampaignMission.Current.Location != null && CampaignMission.Current.Location.StringId == "tavern")
                {
                    Location location = CampaignMission.Current.Location;
                    location.RemoveLocationCharacter(location.GetLocationCharacter(agent.Origin));
                }
                hero.HasMet = true;
                hero.ChangeState(Hero.CharacterStates.Active);
            }
            else
            {
                hero = Hero.OneToOneConversationHero;
            }

            // Create companion from cheat menu
            // GiveGoldAction.ApplyBetweenCharacters(null, hero, 20000, true);
            // hero.HasMet = true;
            // hero.ChangeState(Hero.CharacterStates.Active);
            // AddCompanionAction.Apply(Clan.PlayerClan, hero);
            // AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
            // CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);

            // Change occupation accordingly
            AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer);

            AddCompanionAction.Apply(Clan.PlayerClan, hero);
            AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
            CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}