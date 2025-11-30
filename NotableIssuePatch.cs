using System.Linq;
using System.Runtime.Serialization;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(IssuesCampaignBehavior))]
    public static class NotableIssuePatch {

        [HarmonyPrefix]
        [HarmonyPatch("CreateAnIssueForSettlementNotables")]
        public static bool IssueCreation_Prefix(Settlement settlement, int totalDesiredIssueCount) {
            if (settlement.IsCastle)
                return false;
            return true;
        }
    }
}