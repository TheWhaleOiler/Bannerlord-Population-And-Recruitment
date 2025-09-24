
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace PopulationAndRecruitment {
    public class SettlementRecruitmentModel : DefaultSettlementAccessModel {
        public override bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAction settlementAction, out bool shouldBeDisabled, out TextObject? disabledText) {

            if (settlementAction == SettlementAction.RecruitTroops
                && settlement.IsTown
                && settlement.OwnerClan != Hero.MainHero.Clan) {
                shouldBeDisabled = true;
                disabledText = new TextObject("{=NoRecruitCastleNotOwner}You Do Not Own This Town.");
                return false;
            }
            return base.CanMainHeroDoSettlementAction(settlement, settlementAction, out shouldBeDisabled, out disabledText);
        }
    }
}

