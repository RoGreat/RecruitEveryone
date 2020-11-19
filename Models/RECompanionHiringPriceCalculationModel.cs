using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace RecruitEveryone.Models
{
    internal class RECompanionHiringPriceCalculationModel
    {
		public static int GetCompanionHiringPrice(CharacterObject character, CharacterObject templateCharacter, Equipment battleEquipment)
		{
			Hero mainHero = Hero.MainHero;
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, null);
			Settlement currentSettlement = mainHero.CurrentSettlement;
			Town town = currentSettlement?.Town;
			if (town == null)
			{
				town = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown).Town;
			}
			float num = 0f;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				EquipmentElement itemRosterElement = battleEquipment[equipmentIndex];
				if (itemRosterElement.Item != null)
				{
					num += town.GetItemPrice(itemRosterElement, null, false);
				}
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				EquipmentElement itemRosterElement2 = character.FirstCivilianEquipment[equipmentIndex2];
				if (itemRosterElement2.Item != null)
				{
					num += town.GetItemPrice(itemRosterElement2, null, false);
				}
			}
			explainedNumber.Add(num / 2f, null, null);
			explainedNumber.Add(templateCharacter.Level * 10, null, null);
			if (Hero.MainHero.IsPartyLeader && Hero.MainHero.GetPerkValue(DefaultPerks.Steward.PaidInPromise))
			{
				explainedNumber.AddFactor(DefaultPerks.Steward.PaidInPromise.PrimaryBonus * 0.01f, null);
			}
			if (mainHero != null && mainHero.PartyBelongedTo.HasPerk(DefaultPerks.Trade.GreatInvestor, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.GreatInvestor, Hero.MainHero.PartyBelongedTo, false, ref explainedNumber);
			}
			return (int)explainedNumber.ResultNumber;
		}
	}
}