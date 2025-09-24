using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Core;
using System.Linq;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "HourlyTickParty")]
    public static class RecruitmentHourlyPatch {
        static void Postfix(MobileParty mobileParty) {
            if (!mobileParty.IsLordParty || mobileParty.MapEvent != null || mobileParty == MobileParty.MainParty)
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
            if ((settlement.IsCastle || settlement.IsTown) 
                && settlement.OwnerClan != mobileParty.LeaderHero?.Clan) {
                return false;
            }
            return true;
        }
    }

}