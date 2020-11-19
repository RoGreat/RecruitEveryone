using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MountAndBlade.CampaignBehaviors;
using RecruitEveryone.Models;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace RecruitEveryone.Behaviors
{
	internal class RECampaignBehavior : CampaignBehaviorBase
	{
		private static Dictionary<int, CharacterObject> _characterTemplates;

		private List<CharacterObject> _wandererTemplates;

		private CharacterObject _wandererTemplate;

		private Equipment _battleEquipment;

		private static List<int> _recruitedAgents;

		private CharacterObject _character;

		private int _agent;

		protected void AddDialogs(CampaignGameStarter starter)
		{
			foreach (Hero hero in Hero.All)
			{
				if (hero.CharacterObject.Occupation != Occupation.Wanderer && hero.IsPlayerCompanion)
				{
					AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer);
				}
			}

			starter.AddDialogLine("wanderer_skip_intro", "wanderer_preintroduction", "hero_main_options", "{=LUiQ6bpo}Very well, then. What is it?", new ConversationSentence.OnConditionDelegate(conversation_wanderer_skip_preintroduction_on_condition), null, 200, null);

			RecruitCharacter(starter, "hero_main_options", "lord_pretalk");
			RecruitCharacter(starter, "tavernkeeper_talk", "tavernkeeper_pretalk");
			RecruitCharacter(starter, "tavernmaid_talk", "close_window");
			RecruitCharacter(starter, "talk_bard_player", "close_window");
			RecruitCharacter(starter, "taverngamehost_talk", "start");
			RecruitCharacter(starter, "town_or_village_player", "town_or_village_pretalk");
			RecruitCharacter(starter, "weaponsmith_talk_player", "merchant_response_3");
			RecruitCharacter(starter, "barber_question1", "no_haircut_conversation_token");
			RecruitCharacter(starter, "shopworker_npc_player", "start_2");
			RecruitCharacter(starter, "blacksmith_player", "player_blacksmith_after_craft");
		}

		private bool conversation_wanderer_skip_preintroduction_on_condition()
		{
			if (Hero.OneToOneConversationHero != null)
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
			GameTexts.SetVariable("STR1", RECompanionHiringPriceCalculationModel.GetCompanionHiringPrice(_character, _wandererTemplate, _battleEquipment));
			GameTexts.SetVariable("STR2", "{=!}<img src=\"Icons\\Coin@2x\">");
			MBTextManager.SetTextVariable("GOLD_AMOUNT", GameTexts.FindText("str_STR1_STR2", null), false);
			return Hero.MainHero.Gold > RECompanionHiringPriceCalculationModel.GetCompanionHiringPrice(_character, _wandererTemplate, _battleEquipment) && !too_many_companions();
		}

		private bool too_many_companions()
		{
			return Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit;
		}

		private bool conversation_companion_hire_gold_on_condition()
		{
			_character = CharacterObject.OneToOneConversationCharacter;

			if (Hero.OneToOneConversationHero == null)
			{
				_wandererTemplates = new List<CharacterObject>(from x in CharacterObject.Templates where x.StringId.Contains("spc_wanderer_") && x.Culture == _character.Culture select x);
				if (_characterTemplates == null)
				{
					_characterTemplates = new Dictionary<int, CharacterObject>();
				}
				if (!_characterTemplates.TryGetValue(_agent, out _wandererTemplate))
				{
					int count = _wandererTemplates.Count;
					int index = MBRandom.RandomInt(count - 1);
					_wandererTemplate = _wandererTemplates[index];
                    _characterTemplates.Add(_agent, _wandererTemplate);
                    _characterTemplates.TryGetValue(_agent, out _wandererTemplate);
				}
			}

			_battleEquipment = _wandererTemplate.BattleEquipments.GetRandomElement<Equipment>();
			AdjustEquipmentImp(_battleEquipment);

			MBTextManager.SetTextVariable("GOLD_AMOUNT", RECompanionHiringPriceCalculationModel.GetCompanionHiringPrice(_character, _wandererTemplate, _battleEquipment));
			MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=7sAm6qwp}Very well. I'm going to need about {GOLD_AMOUNT}{GOLD_ICON} to settle up some debts, though. Can you pay?[rb:trivial]", false);

			if (_wandererTemplate.GetTraitLevel(DefaultTraits.Mercy) + _wandererTemplate.GetTraitLevel(DefaultTraits.Honor) < 0 && _wandererTemplate.GetPersona() == DefaultTraits.PersonaIronic)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=8Mx8gMmw}One other small thing... I've had to take some money from some fairly dangerous people around here. I'll need {GOLD_AMOUNT}{GOLD_ICON} to get that beast off my back. Do you reckon you can pay me that?[rb:trivial]", false);
			}
			else if (_wandererTemplate.GetTraitLevel(DefaultTraits.Mercy) + _wandererTemplate.GetTraitLevel(DefaultTraits.Generosity) > 0 && _wandererTemplate.GetTraitLevel(DefaultTraits.Honor) < 0 && !_character.IsFemale)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=K1RtrtvH}So, uh, there's a young woman around here. I really need to leave her some money before I go anywhere. Let's say {GOLD_AMOUNT}{GOLD_ICON} - can you pay me that?[rb:trivial]", false);
			}
			else if (_wandererTemplate.GetPersona() == DefaultTraits.PersonaCurt && _wandererTemplate.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=PlhbjNOE}Just so you know... I'm not cheap. I want {GOLD_AMOUNT}{GOLD_ICON} as an advance, or there's no deal.[rb:trivial]", false);
			}
			else if (_wandererTemplate.GetPersona() == DefaultTraits.PersonaCurt)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9kHU4AMD}Great. Going to need some money in advance though - {GOLD_AMOUNT}{GOLD_ICON}. Can you pay?[rb:trivial]", false);
			}
			else if (_wandererTemplate.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=loLetAI9}Very well. But the world being as it is, I'm going to need {GOLD_AMOUNT}{GOLD_ICON} as a down payment on my services. Can you pay that?[rb:trivial]", false);
			}
			else if (_wandererTemplate.GetTraitLevel(DefaultTraits.Mercy) > 0 || _wandererTemplate.GetTraitLevel(DefaultTraits.Generosity) > 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9g6FB5Y7}There are some townspeople who've looked after me here, made sure I was fed and that. I'd like to give them something before I go. Could I ask for {GOLD_AMOUNT}{GOLD_ICON} as an advance?[rb:trivial]", false);
			}
			return true;
		}

		private void AdjustEquipmentImp(Equipment equipment)
		{
			ItemModifier @object = MBObjectManager.Instance.GetObject<ItemModifier>("companion_armor");
			ItemModifier object2 = MBObjectManager.Instance.GetObject<ItemModifier>("companion_weapon");
			ItemModifier object3 = MBObjectManager.Instance.GetObject<ItemModifier>("companion_horse");
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				EquipmentElement equipmentElement = equipment[equipmentIndex];
				if (equipmentElement.Item != null)
				{
					if (equipmentElement.Item.ArmorComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, @object);
					}
					else if (equipmentElement.Item.HorseComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, object3);
					}
					else if (equipmentElement.Item.WeaponComponent != null)
					{
						equipment[equipmentIndex] = new EquipmentElement(equipmentElement.Item, object2);
					}
				}
			}
		}

		private bool conversation_hero_hire_on_condition()
		{
			bool result;
			if (Hero.OneToOneConversationHero == null)
			{
				_agent = Math.Abs(Campaign.Current.ConversationManager.OneToOneConversationAgent.GetHashCode());
				if (_recruitedAgents == null)
				{
                    _recruitedAgents = new List<int>();
				}
				else if (_recruitedAgents.Contains(_agent))
				{
					return false;
				}
				result = true;
			}
			else
			{
				bool isNotable = Hero.OneToOneConversationHero.IsNotable;
				result = isNotable;
			}
			return result;
		}

		private void conversation_companion_hire_on_consequence()
		{
			Hero hero;
			if (Hero.OneToOneConversationHero == null)
			{
                _recruitedAgents.Add(_agent);
				Agent agent = (Agent)MissionConversationHandler.Current.ConversationManager.OneToOneConversationAgent;

				RESubModule.Debug("Character Age: " + _character.Age.ToString() + " Agent Age: " + agent.Age.ToString());
				int age = MBMath.ClampInt((int)agent.Age, Campaign.Current.Models.AgeModel.BecomeTeenagerAge, REAgeModel.MaxAge);

				hero = HeroCreator.CreateSpecialHero(_character, Settlement.CurrentSettlement, null, null, age);

				BodyProperties bodyPropertiesValue = agent.BodyPropertiesValue;
				RESubModule.Debug("DynamicProperties: " + bodyPropertiesValue.DynamicProperties.ToString());
				AccessTools.Property(typeof(Hero), "StaticBodyProperties").SetValue(hero, bodyPropertiesValue.StaticProperties);

				foreach (TraitObject trait in DefaultTraits.All)
				{
					int traitLevel = _wandererTemplate.GetTraitLevel(trait);
					hero.SetTraitLevel(trait, traitLevel);
				}
				AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer);
				Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, null);
				AccessTools.Property(typeof(Hero), "BattleEquipment").SetValue(hero, _battleEquipment);

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
				AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(hero.CharacterObject, Occupation.Wanderer);
			}

			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, hero, RECompanionHiringPriceCalculationModel.GetCompanionHiringPrice(_character, _wandererTemplate, _battleEquipment), false);
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
