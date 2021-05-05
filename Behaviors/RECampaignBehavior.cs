using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using Helpers;
using MountAndBlade.CampaignBehaviors;
using RecruitEveryone.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace RecruitEveryone.Behaviors
{
	internal class RECampaignBehavior : CampaignBehaviorBase
	{
		public MBReadOnlyList<CharacterObject>? WandererTemplates { get; private set; }

		private void OccupationToWanderer(CharacterObject character)
		{
			if (character.Occupation != Occupation.Wanderer)
			{
				AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(character, Occupation.Wanderer);
			}
		}

		private void RemoveNotable(Hero hero)
        {
			List<Hero> _notables = (List<Hero>)AccessTools.Field(typeof(NotablePowerManagementBehavior), "_notables").GetValue(Campaign.Current.GetCampaignBehavior<NotablePowerManagementBehavior>());
			_notables.Remove(hero);
			AccessTools.Field(typeof(NotablePowerManagementBehavior), "_notables").SetValue(Campaign.Current.GetCampaignBehavior<NotablePowerManagementBehavior>(), _notables);
		}

		protected void AddDialogs(CampaignGameStarter starter)
		{
			foreach (Hero hero in Hero.All.ToListQ())
			{
				// Fix if they are not a proper companion
				if (hero.IsPlayerCompanion)
				{
					if (hero.IsNotable)
					{
						RemoveNotable(hero);
					}
					OccupationToWanderer(hero.CharacterObject);
				}
			}
			foreach (Hero hero in Hero.MainHero.CompanionsInParty.ToListQ())
			{
				// Fix tied equipment bug
				Equipment battleEquipment = hero.BattleEquipment.Clone();
				Equipment civilianEquipment = hero.CivilianEquipment.Clone();
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, battleEquipment);
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, civilianEquipment);
			}

			starter.AddDialogLine("wanderer_skip_intro", "wanderer_preintroduction", "hero_main_options", "{=LUiQ6bpo}Very well, then. What is it?", new ConversationSentence.OnConditionDelegate(conversation_wanderer_skip_preintroduction_on_condition), null, 200, null);

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

		private bool conversation_wanderer_skip_preintroduction_on_condition()
		{
			if (Hero.OneToOneConversationHero is not null)
			{
				if (!GameTexts.TryGetText("prebackstory", out _, Hero.OneToOneConversationHero.Template.StringId))
				{
					return true;
				}
			}
			return false;
		}

		private void RecruitCharacter(CampaignGameStarter starter, string start, string end = "close_window")
		{
			starter.AddPlayerLine("RE_main_option_faction_hire", start, start + "_companion_hire", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(conversation_hero_hire_on_condition), null, 200, null, null);
			starter.AddDialogLine("RE_companion_hire", start + "_companion_hire", start + "_player_companion_hire_response", "{=fDjQOR5s}{HIRING_COST_EXPLANATION}", new ConversationSentence.OnConditionDelegate(conversation_companion_hire_gold_on_condition), null, 100, null);
			starter.AddPlayerLine("RE_companion_hire_capacity_full", start + "_player_companion_hire_response", end, "{=afdN8ZU7}Thinking again, I already have more companions than I can manage.", new ConversationSentence.OnConditionDelegate(too_many_companions), null, 100, null, null);
			starter.AddPlayerLine("RE_player_companion_hire_response_1", start + "_player_companion_hire_response", "hero_leave", "{=EiFPu9Np}Right... {GOLD_AMOUNT} Here you are.", new ConversationSentence.OnConditionDelegate(conversation_companion_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(conversation_companion_hire_on_consequence), 100, null, null);
			starter.AddPlayerLine("RE_player_companion_hire_response_2", start + "_player_companion_hire_response", end, "{=65UMAav2}I can't afford that just now.", () => !too_many_companions(), null, 100, null, null);
		}

		private bool conversation_companion_hire_on_condition()
		{
			GameTexts.SetVariable("STR1", _hiringPrice);
			GameTexts.SetVariable("STR2", "{=!}<img src=\"Icons\\Coin@2x\">");
			MBTextManager.SetTextVariable("GOLD_AMOUNT", GameTexts.FindText("str_STR1_STR2", null), false);
			return Hero.MainHero.Gold > _hiringPrice && !too_many_companions();
		}

		private bool too_many_companions()
		{
			return Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit;
		}

		private bool conversation_companion_hire_gold_on_condition()
		{
			CharacterObject character = CharacterObject.OneToOneConversationCharacter;
			_battleEquipment = null;
			if (_characterTemplates is null)
			{
				_characterTemplates = new Dictionary<int, CharacterObject>();
			}
			if (_battleEquipments is null)
			{
				_battleEquipments = new Dictionary<int, Equipment>();
			}
			if (_wandererTemplates is null)
            {
                _wandererTemplates = new List<CharacterObject>(from x in CharacterObject.Templates where x.Occupation == Occupation.Wanderer select x);
				WandererTemplates = new MBReadOnlyList<CharacterObject>(_wandererTemplates);
			}
			if (Hero.OneToOneConversationHero is null)
			{
				if (!_characterTemplates.TryGetValue(_agent, out _wandererTemplate))
				{
					_wandererTemplate = WandererTemplates.GetRandomElement();

					AddTemplate:
					if (_wandererTemplate is not null)
					{
						_characterTemplate = CharacterObject.CreateFrom(_wandererTemplate, false);
						_characterTemplate.IsFemale = character.IsFemale;
						_characterTemplate.Culture = character.Culture;
						_characterTemplates.Add(_agent, _characterTemplate);
						// To hopefully curtail custom culture crashes
						if (character.Culture == Settlement.CurrentSettlement.Culture)
						{
							List<CharacterObject>  battleEquipmentTemplates = WandererTemplates.WhereQ((CharacterObject c) => c.Culture == Settlement.CurrentSettlement.Culture && c.IsFemale == character.IsFemale).ToListQ();
							CharacterObject battleEquipmentTemplate = battleEquipmentTemplates.GetRandomElementInefficiently();
							_battleEquipment = battleEquipmentTemplate.RandomBattleEquipment.Clone();
						}
						else
						{
							List<CharacterObject> battleEquipmentTemplates = WandererTemplates.WhereQ((CharacterObject c) => c.IsFemale == character.IsFemale).ToListQ();
							CharacterObject battleEquipmentTemplate = battleEquipmentTemplates.GetRandomElementInefficiently();
							_battleEquipment = _wandererTemplate.RandomBattleEquipment.Clone();
						}
						// Going by the dictionary system instead of doing the wanderer equipment system...
						// Does increase the cost though.
						_battleEquipments.Add(_agent, _battleEquipment);
					}
					else
					{
						_wandererTemplate = character;
						goto AddTemplate;
					}
				}
				_battleEquipments.TryGetValue(_agent, out _battleEquipment);
			}
			else
            {
				_characterTemplate = character;
				_wandererTemplate = WandererTemplates.GetRandomElement();
				_battleEquipment = Hero.OneToOneConversationHero.BattleEquipment.Clone();
			}
			_hiringPrice = RECompanionHiringPriceCalculationModel.GetCompanionHiringPrice(character, _characterTemplate!, _battleEquipment!);

			MBTextManager.SetTextVariable("GOLD_AMOUNT", _hiringPrice);
			MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=7sAm6qwp}Very well. I'm going to need about {GOLD_AMOUNT}{GOLD_ICON} to settle up some debts, though. Can you pay?[rb:trivial]", false);

			if (_characterTemplate!.GetTraitLevel(DefaultTraits.Mercy) + _characterTemplate.GetTraitLevel(DefaultTraits.Honor) < 0 && _characterTemplate.GetPersona() == DefaultTraits.PersonaIronic)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=8Mx8gMmw}One other small thing... I've had to take some money from some fairly dangerous people around here. I'll need {GOLD_AMOUNT}{GOLD_ICON} to get that beast off my back. Do you reckon you can pay me that?[rb:trivial]", false);
			}
			else if (_characterTemplate.GetTraitLevel(DefaultTraits.Mercy) + _characterTemplate.GetTraitLevel(DefaultTraits.Generosity) > 0 && _characterTemplate.GetTraitLevel(DefaultTraits.Honor) < 0 && !character.IsFemale)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=K1RtrtvH}So, uh, there's a young woman around here. I really need to leave her some money before I go anywhere. Let's say {GOLD_AMOUNT}{GOLD_ICON} - can you pay me that?[rb:trivial]", false);
			}
			else if (_characterTemplate.GetPersona() == DefaultTraits.PersonaCurt && _characterTemplate.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=PlhbjNOE}Just so you know... I'm not cheap. I want {GOLD_AMOUNT}{GOLD_ICON} as an advance, or there's no deal.[rb:trivial]", false);
			}
			else if (_characterTemplate.GetPersona() == DefaultTraits.PersonaCurt)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9kHU4AMD}Great. Going to need some money in advance though - {GOLD_AMOUNT}{GOLD_ICON}. Can you pay?[rb:trivial]", false);
			}
			else if (_characterTemplate.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=loLetAI9}Very well. But the world being as it is, I'm going to need {GOLD_AMOUNT}{GOLD_ICON} as a down payment on my services. Can you pay that?[rb:trivial]", false);
			}
			else if (_characterTemplate.GetTraitLevel(DefaultTraits.Mercy) > 0 || _characterTemplate.GetTraitLevel(DefaultTraits.Generosity) > 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9g6FB5Y7}There are some townspeople who've looked after me here, made sure I was fed and that. I'd like to give them something before I go. Could I ask for {GOLD_AMOUNT}{GOLD_ICON} as an advance?[rb:trivial]", false);
			}
			return true;
		}

		private bool conversation_hero_hire_on_condition()
		{
			if (Campaign.Current.ConversationManager.OneToOneConversationAgent.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge)
            {
				return false;
            }
			if (Hero.OneToOneConversationHero is null)
			{
				_agent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
				if (_recruitedAgents is null)
				{
					_recruitedAgents = new List<int>();
				}
				if (_recruitedAgents.Contains(_agent))
				{
					return false;
				}
				return true;
			}
			return Hero.OneToOneConversationHero.IsNotable;
		}

		private void conversation_companion_hire_on_consequence()
		{
			Hero hero;
			if (Hero.OneToOneConversationHero is null)
			{
				Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
				if (_recruitedAgents is null)
				{
					_recruitedAgents = new List<int>();
				}
				_recruitedAgents.Add(_agent);
				int age = MBMath.ClampInt((int)agent.Age, Campaign.Current.Models.AgeModel.HeroComesOfAge, REAgeModel.MaxAge);

				hero = HeroCreator.CreateSpecialHero(_characterTemplate, Settlement.CurrentSettlement, null, null, age);

				// In NameGenerator
				TextObject textObject = new("{=nameoccupation}{FIRST_NAME}{OCCUPATION}");
				textObject.SetTextVariable("FIRST_NAME", hero.FirstName);
				textObject.SetTextVariable("OCCUPATION", FormerOccupation());
				hero.CharacterObject.Name = textObject;

				AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(hero, agent.BodyPropertiesValue.StaticProperties);

				Equipment civilianEquipment = agent.SpawnEquipment.Clone();
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, civilianEquipment);

				if (CampaignMission.Current.Location is not null)
				{
					if (CampaignMission.Current.Location.StringId == "tavern")
					{
						Location location = CampaignMission.Current.Location;
						location.RemoveLocationCharacter(location.GetLocationCharacter(agent.Origin));
					}
				}
				hero.HasMet = true;
				hero.ChangeState(Hero.CharacterStates.Active);
			}
			else
			{
				hero = Hero.OneToOneConversationHero;
				CharacterObject character = hero.CharacterObject;
				_characterTemplate = CharacterObject.CreateFrom(_wandererTemplate, false);

				Equipment civilianEquipment = hero.CivilianEquipment.Clone();

				_characterTemplate.IsFemale = character.IsFemale;
				_characterTemplate.Culture = character.Culture;
				_characterTemplate.Name = hero.Name;

				StaticBodyProperties staticBodyProperties = (StaticBodyProperties)AccessTools.Property(typeof(Hero), "StaticBodyProperties").GetValue(hero);
				AccessTools.Property(typeof(Hero), "CharacterObject").SetValue(hero, _characterTemplate);

				AccessTools.Property(typeof(CharacterObject), "HeroObject").SetValue(_characterTemplate, hero);
				AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(_characterTemplate.HeroObject, staticBodyProperties);

				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, civilianEquipment);

				RemoveNotable(hero);
			}

			EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, _battleEquipment);

			OccupationToWanderer(hero.CharacterObject);

			foreach (TraitObject trait in DefaultTraits.All)
			{
				int traitLevel = _wandererTemplate!.GetTraitLevel(trait);
				hero.SetTraitLevel(trait, traitLevel);
			}
			Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, _characterTemplate);

			foreach (CaravanPartyComponent caravanParty in hero.OwnedCaravans.ToListQ())
			{
				MobileParty mobileParty = caravanParty.MobileParty;
				if (mobileParty is not null)
				{
					CaravanPartyComponent.TransferCaravanOwnership(mobileParty, Hero.MainHero);
				}
			}
			if (hero.Issue is not null)
			{
				hero.Issue.CompleteIssueWithCancel();
			}
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, hero, _hiringPrice, false);
			AddCompanionAction.Apply(Clan.PlayerClan, hero);
			AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
			CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
		}

		private TextObject FormerOccupation()
        {
            TextObject textObject;
            switch (CharacterObject.OneToOneConversationCharacter.Occupation)
			{
				// Tavern Employees:
				case Occupation.Tavernkeeper:
					textObject = new TextObject("{=thetavernkeeper} the Tavern Keeper");
					break;
				case Occupation.TavernWench:
					textObject = new TextObject("{=thetavernmaid} the Tavern Maid");
					break;
				case Occupation.Musician:
					textObject = new TextObject("{=themusician} the Musician");
					break;
				case Occupation.TavernGameHost:
					textObject = new TextObject("{=thegamehost} the Game Host");
					break;
				// Traders:
				case Occupation.GoodsTrader:
					textObject = new TextObject("{=thetrader} the Trader");
					break;
				case Occupation.Weaponsmith:
					textObject = new TextObject("{=theweaponsmith} the Weaponsmith");
					break;
				case Occupation.Armorer:
					textObject = new TextObject("{=thearmorer} the Armorer");
					break;
				case Occupation.HorseTrader:
					textObject = new TextObject("{=thehorsetrader} the Horse Trader");
					break;
				case Occupation.Blacksmith:
					textObject = new TextObject("{=theblacksmith} the Blacksmith");
					break;
				case Occupation.ShopWorker:
					textObject = new TextObject("{=theshopworker} the Shop Worker");
					break;
				// New possible recruit!
				case Occupation.Gangster:
					textObject = new TextObject("{=thethug} the Thug");
					break;
				default:
					textObject = new TextObject();
					break;
			}
			// Different due to them being associated with the culture object and not the occupation enum
			if (IsConversationAgentBarber())
			{
				textObject = new TextObject("{=thebarber} the Barber");
			}
			if (IsConversationAgentDancer())
			{
				textObject = new TextObject("{=thedancer} the Dancer");
			}
			if (IsConversationAgentBeggar())
			{
				textObject = new TextObject("{=thebeggar} the Beggar");
			}
			return textObject;
        }

		private bool IsConversationAgentBeggar()
		{
			return Settlement.CurrentSettlement.Culture.Beggar == CharacterObject.OneToOneConversationCharacter || Settlement.CurrentSettlement.Culture.FemaleBeggar == CharacterObject.OneToOneConversationCharacter;
		}

		private bool IsConversationAgentDancer()
		{
			return Settlement.CurrentSettlement.Culture.FemaleDancer == CharacterObject.OneToOneConversationCharacter;
		}

		private bool IsConversationAgentBarber()
		{
			return Settlement.CurrentSettlement.Culture.Barber == CharacterObject.OneToOneConversationCharacter;
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

		private Dictionary<int, CharacterObject>? _characterTemplates;

		private CharacterObject? _characterTemplate;

		private List<CharacterObject>? _wandererTemplates;

		private CharacterObject? _wandererTemplate;

		private Dictionary<int, Equipment>? _battleEquipments;

		private Equipment? _battleEquipment;

		private List<int>? _recruitedAgents;

		private int _agent;

		private int _hiringPrice;
	}
}
