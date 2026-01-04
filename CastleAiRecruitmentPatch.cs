using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "HourlyTickParty")]
    public static class RecruitmentHourlyPatch {
        static void Postfix(MobileParty mobileParty) {
            if ((!mobileParty.IsCaravan && !mobileParty.IsLordParty) || mobileParty.MapEvent != null || mobileParty == MobileParty.MainParty)
                return;

            var settlement = MobilePartyHelper.GetCurrentSettlementOfMobilePartyForAICalculation(mobileParty);
            if (settlement != null && settlement.IsCastle && !settlement.IsUnderSiege) {
                var method = AccessTools.Method(typeof(RecruitmentCampaignBehavior), "CheckRecruiting");
                if (method != null) {
                    method.Invoke(
                        Campaign.Current.GetCampaignBehavior<RecruitmentCampaignBehavior>(),
                        new object[] { mobileParty, settlement }
                    );
                }
            }
        }
    }

    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "CheckRecruiting")]
    public static class CheckRecruitingPatch {

        static bool Prefix(MobileParty mobileParty, Settlement settlement) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            var hero = mobileParty.LeaderHero;

            if (mobileParty.IsCaravan)
                return true;
            
            if ((settlement.IsCastle || settlement.IsTown)) {
                if(settlement.OwnerClan != hero?.Clan)
                    return false;

                if (settings.DisablePlayerPartiesCanRecruitFromPlayerFiefs
                    && hero != settlement.Owner
                    && hero.Clan == Hero.MainHero.Clan)
                    return false;

                if (settings.DisableAiPartiesCanRecruitFromAiLordFiefs
                    && hero != settlement.Owner)
                    return false;
            }

            if(settings.EnableAiSettlementMilitiaDemobilization
                && settlement.IsVillage
                && mobileParty.IsLordParty
                && mobileParty.LeaderHero != null) {

                var clan = hero.Clan;
                var kingdom = clan.Kingdom;

                // skip viillage recruiting when demobilizaing and clan has available fief to recruit from
                if (!GetBasicVolunteerPatch.checkIsAtWar(kingdom, clan)
                    && HasAvailableFiefToRecruitFrom(mobileParty.LeaderHero))
                    return false;
            }

            return true;
        }

        public static bool HasAvailableFiefToRecruitFrom(Hero hero) {

            if (hero == null) return false;

            var settings = PopulationAndRecruitmentSettings.Instance;

            if (settings.DisableAiPartiesCanRecruitFromAiLordFiefs
                && hero != hero.Clan.Leader)
                return false;


            if (settings.DisablePlayerPartiesCanRecruitFromPlayerFiefs
                && hero.Clan == Hero.MainHero.Clan
                && hero != Hero.MainHero)
                return false;


            bool hasAvailableFiefToRecuit = hero.Clan.Fiefs
                .Where(f => (f.IsTown
                                && f.Prosperity >= settings.MinimumFiefProsperityRequired
                                && f.Militia >= settings.MinimumTownMilitiaRequired)
                            || (f.IsCastle
                                && f.Prosperity >= settings.MinimumFiefProsperityRequired
                                && f.Militia >= settings.MinimumCastleMilitiaRequired))
                .Any();

            if (hasAvailableFiefToRecuit)
                return true;

            return false;
        }

    }

}