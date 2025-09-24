using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.Core.ItemCategory;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(DefaultVolunteerModel))]
    public class GetBasicVolunteerPatch {

        [HarmonyPrefix]
        [HarmonyPatch("GetBasicVolunteer")]
        public static bool GetBasicVolunteer_Postfix(ref CharacterObject __result, Hero sellerHero) {

            var settings = PopulationAndRecruitmentSettings.Instance;

            var settlement = sellerHero.CurrentSettlement;
            var clan = settlement.OwnerClan;
            var kingdom = clan?.Kingdom;
            var culture = sellerHero.Culture;

            if (settlement.IsTown) {
                var prosperity = settlement.Town.Prosperity;

                if (HighProsperityRoll(prosperity, settings.TownAsymptoteDenominatior))
                    if (settings.EnableGlobalEliteTroops)
                        __result = HighProsperityRoll(prosperity, settings.TownEliteAsymptoteDenominator) ? culture.EliteBasicTroop : culture.BasicTroop;
                    else
                        __result = culture.BasicTroop;
                else
                    __result = MBRandom.RandomFloat < 0.5 ? culture.MeleeMilitiaTroop : culture.RangedMilitiaTroop;
            }
            else if (settlement.IsCastle) {
                var prosperity = settlement.Town.Prosperity;

                if (HighProsperityRoll(prosperity, settings.CastleAsymptoteDenominatior))
                    __result = HighProsperityRoll(prosperity, settings.CastleEliteAsymptoteDenominator) 
                        ? culture.EliteBasicTroop 
                        : culture.BasicTroop;
                else
                    __result = MBRandom.RandomFloat < 0.5 ? culture.MeleeMilitiaTroop : culture.RangedMilitiaTroop;
            }
            else if (settlement.IsVillage) {
                var hearth = settlement.Village.Hearth;

                if (HighProsperityRoll(hearth, settings.VillageAsymptoteDenominatior)) {
                    __result = settings.VillageOnlySpawnsMilitia ? (MBRandom.RandomFloat < 0.5 ? culture.MeleeEliteMilitiaTroop : culture.RangedEliteMilitiaTroop)
                        : (settings.EnableGlobalEliteTroops ? (HighProsperityRoll(hearth, settings.VillageEliteAsymptoteDenominator) ? culture.EliteBasicTroop : culture.BasicTroop)
                        : culture.BasicTroop);
                }
                else
                    __result = MBRandom.RandomFloat < 0.5 ? culture.MeleeMilitiaTroop : culture.RangedMilitiaTroop;
            }
            //else
            //    __result = __result;

            // Demobilized Kingdom not at war
            if (!settings.VillageOnlySpawnsMilitia
                && (__result == culture.MeleeMilitiaTroop || __result == culture.RangedMilitiaTroop)
                && !checkIsAtWar(kingdom, clan)) {
                __result = null;
            }

            return false;

        }

        public static bool checkIsAtWar(Kingdom kingdom, Clan clan) {
            if (clan != null) {
                if (kingdom == null) {
                    return Campaign.Current.Factions
                        .Where(f => f != clan && f.IsKingdomFaction && clan.IsAtWarWith(f))
                        .Any();
                }
                else {
                    return Campaign.Current.Factions
                        .Where(f => f != kingdom && f.IsKingdomFaction && kingdom.IsAtWarWith(f))
                        .Any();
                }
            }
            return false;
        }

        private static bool HighProsperityRoll(float prosperity, float asymptoteDenominator) {
            return MBRandom.RandomFloat < Math.Pow(prosperity,3) / ( Math.Pow(prosperity,3) + Math.Pow(asymptoteDenominator,3) );
        }
    }

}
