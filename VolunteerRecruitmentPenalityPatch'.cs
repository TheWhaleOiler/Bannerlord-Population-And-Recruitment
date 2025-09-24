using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PopulationAndRecruitment {

    public class VolunteerCostTracker {
        public CharacterObject Troop;
        public bool Recruited;
    }


    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement")]
    public static class VolunteerSpawnPatch {

        internal static readonly Dictionary<(Hero, int), VolunteerCostTracker> _slotCosts = new();

        [HarmonyPrefix]
        static void Prefix(Settlement settlement)
        {
            if (settlement == null) return;

            foreach (var notable in settlement.Notables) {
                for (int i = 0; i < notable.VolunteerTypes.Length; i++) {
                    var key = (notable, i);
                    var current = notable.VolunteerTypes[i];

                    if (!_slotCosts.ContainsKey(key))
                        _slotCosts.Add(key, new VolunteerCostTracker());

                    var tracker = _slotCosts[key];

                    tracker.Troop = current;
                }
            }
        }


        [HarmonyPostfix]
        static void Postfix(Settlement settlement)
        {
            var settings = PopulationAndRecruitmentSettings.Instance;
            
            if (settlement == null) return;

            foreach (var notable in settlement.Notables) {
                for (int i = 0; i < notable.VolunteerTypes.Length; i++) {

                    var key = (notable, i);
                    var current = notable.VolunteerTypes[i];

                    if (!_slotCosts.ContainsKey(key))
                        _slotCosts[key] = new VolunteerCostTracker();

                    var tracker = _slotCosts[key];

                    if (tracker.Troop == null && current != null) {
                        if (settings.DisableSpawnCostDuringKingdomPeace) {
                            tracker.Troop = current;
                        }
                        else if (CanAffordVolunteer(settlement)) {
                            ApplyRecruitmentCost(settlement);
                            tracker.Troop = current;
                        }
                        else {
                            notable.VolunteerTypes[i] = null;
                        }
                    }
                }
            }
        }

        private static void ApplyRecruitmentCost(Settlement settlement) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            if ( (settlement.IsTown || settlement.IsCastle) && settlement.Town != null) {
                settlement.Town.Prosperity -= settings.ProsperityDecreaseOnRecruitment;
                settlement.Militia = MBMath.ClampFloat(settlement.Militia - 1, 0, settlement.Militia);
            }
            else if (settlement.IsVillage && settlement.Village != null) {
                settlement.Village.Hearth -= settings.HearthDecreaseOnRecruitment;
                settlement.Militia = MBMath.ClampFloat(settlement.Militia - 1, 0, settlement.Militia);
            }
        }

        internal static bool CanAffordVolunteer(Settlement settlement) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            if (settlement.IsTown && settlement.Town != null) {
                return settlement.Town.Prosperity >= settings.MinimumFiefProsperityRequired &&
                       settlement.Militia >= settings.MinimumTownMilitiaRequired;
            }

            if (settlement.IsCastle && settlement.Town != null) {
                return settlement.Town.Prosperity >= settings.MinimumFiefProsperityRequired &&
                       settlement.Militia >= settings.MinimumCastleMilitiaRequired;
            }

            if (settlement.IsVillage && settlement.Village != null) {
                return settlement.Village.Hearth >= settings.MinimumVillageHearthRequired &&
                       settlement.Militia >= settings.MinimumVillageMilitiaRequired;
            }
            return false;
        }
    }
}