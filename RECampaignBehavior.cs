using HarmonyLib;
using MountAndBlade.CampaignBehaviors;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace RecruitEveryone
{
    internal class RECampaignBehavior : CampaignBehaviorBase
    {
        private static List<int> _hiredAgents;

        private List<CharacterObject> _companionTemplatesFemale;

        private List<CharacterObject> _companionTemplatesMale;

        protected void AddDialogs(CampaignGameStarter starter)
        {
            RecruitCharacter(starter, "tavernkeeper_talk");
            RecruitCharacter(starter, "tavernmaid_talk");
            RecruitCharacter(starter, "talk_bard_player");
            // CharacterHire(starter, "arena_master_talk");     Single person, awkward
            // CharacterHire(starter, "ransom_broker_talk");    Crashes
            RecruitCharacter(starter, "taverngamehost_talk");
            RecruitCharacter(starter, "town_or_village_player");
            RecruitCharacter(starter, "weaponsmith_talk_player");
            RecruitCharacter(starter, "barber_question1");

            // Haven't tested
            // CharacterHire(starter, "prison_guard_talk");
            // CharacterHire(starter, "castle_guard_talk");
        }

        private void RecruitCharacter(CampaignGameStarter starter, string conversation)
        {
            starter.AddPlayerLine("player_starts_discussion", conversation, conversation + "_discussion_RE", "{=lord_conversations_343}There is something I'd like to discuss.", null, null, 101, null, null);
            starter.AddDialogLine("character_agrees_to_discussion", conversation + "_discussion_RE",  conversation + "_agreed_to_discussion_RE", "{=OD1m1NYx}{STR_INTRIGUE_AGREEMENT}", new ConversationSentence.OnConditionDelegate(conversation_character_agrees_to_discussion_on_condition), null, 100, null);
            starter.AddPlayerLine("player_talks_to_recruit", conversation + "_agreed_to_discussion_RE", conversation + "_recruit_RE", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(able_to_recruit), null, 100, null, null);
            starter.AddPlayerLine("player_plans_never_mind", conversation + "_agreed_to_discussion_RE", "start", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
            starter.AddDialogLine("character_recruited", conversation + "_recruit_RE", "close_window", "{=QffdjUxf}Very well. I'll get my gear and join you outside.[rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(recruit_character), 100, null);
        }

        private bool conversation_character_agrees_to_discussion_on_condition()
        {
            MBTextManager.SetTextVariable("STR_INTRIGUE_AGREEMENT", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_lord_intrigue_accept", CharacterObject.OneToOneConversationCharacter), false);
            return true;
        }

        private bool able_to_recruit()
        {
            int agent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
            if (_hiredAgents == null)
            {
                _hiredAgents = new List<int>();
            }
            else if (_hiredAgents.Contains(agent))
            {
                return false;
            }
            return Clan.PlayerClan.Companions.Count < Clan.PlayerClan.CompanionLimit;
        }

        private void recruit_character()
        {
            int hiredAgent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
            _hiredAgents.Add(hiredAgent);

            // Agents for some reason always have default weight and build bodyproperties
            Agent agent = (Agent)MissionConversationHandler.Current.ConversationManager.OneToOneConversationAgent;
            CharacterObject character = CharacterObject.OneToOneConversationCharacter;
            RESubModule.Debug("Character: " + character.Age + "   Agent: " + agent.Age);

            // Create companion action
            Hero hero = HeroCreator.CreateSpecialHero(character, Settlement.CurrentSettlement, null, null, (int)agent.Age);

            // Change body properties to what the agent is currently
            BodyProperties agentBodyProperties = agent.BodyPropertiesValue;
            // RESubModule.Debug("DynamicProperties: " + agentBodyProperties.DynamicProperties.ToString());

            AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(hero, agentBodyProperties.StaticProperties);
            // Weight and build are fixed at the maximum. Seems to be systemic.
            //AccessTools.Property(typeof(Hero), "Weight").SetValue(hero, MBRandom.RandomFloatRanged(agentBodyProperties.DynamicProperties.Weight, character.GetBodyPropertiesMax().Weight));
            //AccessTools.Property(typeof(Hero), "Build").SetValue(hero, MBRandom.RandomFloatRanged(character.StaticBodyPropertiesMin.Weight, character.GetBodyPropertiesMax().Weight));
            //RESubModule.Debug("New Weight: " + agentBodyProperties.Weight + "   New Build: " + agentBodyProperties.Build);

            //float minWeight = 0.3f;
            //float minBuild = 0.3f;
            //float maxWeight = 1.0f;
            //float maxBuild = 1.0f;
            // Beggar
            //if (character.Name.GetID() == "HZRKBDyB")
            //{
            //    RESubModule.Debug("Beggar");
            //    minWeight = 0.0f;
            //    minBuild = 0.05f;
            //    maxWeight = 0.6f;
            //    maxBuild = 0.7f;
            //}
            // Tavern Maid
            //if (character.Name.GetID() == "z6RMWTaA" || character.Name.GetID() == "X3KYysuv")
            //{
            //    RESubModule.Debug("Beauty");
            //    maxWeight = 0.8f;
            //    maxBuild = 0.7f;
            //}
            //hero.Weight = MBRandom.RandomFloatRanged(minWeight, maxWeight);
            //hero.Build = MBRandom.RandomFloatRanged(minBuild, maxBuild);
            // RESubModule.Debug(hero.Weight.ToString());
            // RESubModule.Debug(hero.Build.ToString());

            // Different skill templates for each gender
            _companionTemplatesFemale = new List<CharacterObject>(from x in CharacterObject.Templates where x.Occupation == Occupation.Wanderer && x.IsFemale select x);
            _companionTemplatesMale = new List<CharacterObject>(from x in CharacterObject.Templates where x.Occupation == Occupation.Wanderer && !x.IsFemale select x);
            if (character.IsFemale)
            {
                int count = _companionTemplatesFemale.Count;
                int num = MBRandom.RandomInt(count - 1);
                Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, _companionTemplatesFemale[num]);
            }
            else
            {
                int count = _companionTemplatesMale.Count;
                int num = MBRandom.RandomInt(count - 1);
                Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, _companionTemplatesMale[num]);
            }

            // Companion finalizing
            // GiveGoldAction.ApplyBetweenCharacters(null, hero, 20000, true);
            //hero.HasMet = true;
            //hero.ChangeState(Hero.CharacterStates.Active);
            //AddCompanionAction.Apply(Clan.PlayerClan, hero);
            //AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
            //CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);

            AddCompanionAction.Apply(Clan.PlayerClan, hero);
            AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);

            // Change occupation accordingly
            AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer, null);

            // Remove agent after recruitment (prevent doppelgangers)
            // Will crash if used outside of the tavern in most cases
            // Probably causes crashing with ransom broker
            if (CampaignMission.Current.Location != null && CampaignMission.Current.Location.StringId == "tavern")
            {
                Location location = CampaignMission.Current.Location;
                location.RemoveLocationCharacter(location.GetLocationCharacter(agent.Origin));
            }
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