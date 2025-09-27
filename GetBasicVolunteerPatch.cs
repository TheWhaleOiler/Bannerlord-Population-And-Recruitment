using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
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
                    if (settings.EnableTroopCompatibility)
                        __result = __result; // No-op
                    else if (settings.EnableGlobalEliteTroops)
                        __result = HighProsperityRoll(prosperity, settings.TownEliteAsymptoteDenominator) 
                            ? TroopPool.GetEliteTroop(culture) 
                            : TroopPool.GetBasicTroop(culture);
                    else
                        __result = TroopPool.GetBasicTroop(culture);
                else
                    __result = TroopPool.GetMilitiaTroop(culture);
            }
            else if (settlement.IsCastle) {
                var prosperity = settlement.Town.Prosperity;

                if (HighProsperityRoll(prosperity, settings.CastleAsymptoteDenominatior))
                    if (settings.EnableTroopCompatibility)
                        __result = __result;
                    else
                        __result = HighProsperityRoll(prosperity, settings.CastleEliteAsymptoteDenominator)
                            ? TroopPool.GetEliteTroop(culture)
                            : TroopPool.GetBasicTroop(culture);
                else
                    __result = TroopPool.GetMilitiaTroop(culture);
            }
            else if (settlement.IsVillage) {
                var hearth = settlement.Village.Hearth;

                if (HighProsperityRoll(hearth, settings.VillageAsymptoteDenominatior)) {

                    if (settings.VillageOnlySpawnsMilitia)
                        __result = TroopPool.GetEliteMilitiaTroop(culture);
                    else if (settings.EnableTroopCompatibility)
                        __result = __result; // No-op
                    else if (settings.EnableGlobalEliteTroops)
                        __result = HighProsperityRoll(hearth, settings.VillageEliteAsymptoteDenominator)
                            ? TroopPool.GetEliteTroop(culture)
                            : TroopPool.GetBasicTroop(culture);
                    else
                        __result = TroopPool.GetBasicTroop(culture);
                }
                else
                    __result = TroopPool.GetMilitiaTroop(culture);
            }
            //else
            //    __result = __result;

            // Demobilized Kingdom not at war
            if (!settings.VillageOnlySpawnsMilitia
                && (__result == culture.MeleeMilitiaTroop || __result == culture.RangedMilitiaTroop || __result == TroopPool.GetMilitiaTroop(culture))
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

    internal static class TroopPool {

        private static Dictionary<string, Dictionary<string, CharacterObject>> cultureTroopPool = new();

        public static void ReInitalizeTroopPool(Dictionary<string, Dictionary<string, string>> troopMap) {
            string troopId;

            bool allLoadedFlag = true;

            String[] troopTypes = new[] { "EliteTroop", "BasicTroop", "EliteMilitiaTroop", "MilitiaTroop" };

            foreach (string culture in troopMap.Keys) {
                if (!cultureTroopPool.ContainsKey(culture)) {
                    cultureTroopPool[culture] = new Dictionary<string, CharacterObject>();
                }

                foreach (string type in troopTypes) {
                    CharacterObject troop = CharacterObject.Find(troopMap[culture][type]);
                    if (troop == null) {
                        allLoadedFlag = false;
                        InformationManager.DisplayMessage(new InformationMessage($"{culture} : {type}"));
                        continue;
                    }
                    cultureTroopPool[culture][type] = CharacterObject.Find(troopMap[culture][type]);
                }
            }

            if (allLoadedFlag == false)
                InformationManager.DisplayMessage(new InformationMessage("Some Troop config loaded unsuccessfully.", Colors.Red));
            else
                InformationManager.DisplayMessage(new InformationMessage("Troop config loaded successfully."));
        }

        private static bool GetTroop(CultureObject co, string troopType, out CharacterObject refTroop) {

            string cultureName = co.GetName().ToString().ToLower();

            if (cultureTroopPool.ContainsKey(cultureName)
                && cultureTroopPool[cultureName].ContainsKey(troopType)) {
                refTroop = cultureTroopPool[cultureName][troopType];
                return true;
            }
            refTroop = null;
            return false;

        }
        public static CharacterObject GetEliteTroop(CultureObject co) {
            if (GetTroop(co, "EliteTroop", out CharacterObject outTroop))
                return outTroop;
            else
                return co.EliteBasicTroop;
        }

        public static CharacterObject GetBasicTroop(CultureObject co) {
            if (GetTroop(co, "BasicTroop", out CharacterObject outTroop))
                return outTroop;
            else
                return co.BasicTroop;
        }

        public static CharacterObject GetEliteMilitiaTroop(CultureObject co) {
            if (GetTroop(co, "EliteMilitiaTroop", out CharacterObject outTroop))
                return outTroop;
            else
                return MBRandom.RandomFloat < 0.5 ? co.MeleeEliteMilitiaTroop : co.RangedEliteMilitiaTroop;
        }

        public static CharacterObject GetMilitiaTroop(CultureObject co) {
            if (GetTroop(co, "MilitiaTroop", out CharacterObject outTroop))
                return outTroop;
            else
                return MBRandom.RandomFloat < 0.5 ? co.MeleeMilitiaTroop : co.RangedMilitiaTroop; ;
        }
    }

    [HarmonyPatch(typeof(Campaign))]
    public static class InitializeTroopPool {

        [HarmonyPostfix]
        [HarmonyPatch("OnGameLoaded")]
        public static void OnGameLoaded_Postfix() {
            Dictionary<string, Dictionary<string, string> > TroopPoolMap = new Dictionary<string, Dictionary<string, string>>();

            string configPath = Path.Combine(BasePath.Name, "Modules", "PopulationAndRecruitment", "troop_config.txt");

            if (File.Exists(configPath)) {
                try {

                    var culture = "";

                    foreach (var line in File.ReadAllLines(configPath)) {
                        var trimmedLine = line.Trim();

                        // Skip empty lines or comments
                        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                            continue;

                        var parts = trimmedLine.Split('=');

                        if (parts.Length != 2)
                            continue;

                        if (parts[0].ToLower() == "culture") {
                            culture = parts[1].ToLower();
                            continue;
                        }

                        if (!TroopPoolMap.ContainsKey(culture))
                            TroopPoolMap[culture] = new Dictionary<string, string>();

                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        TroopPoolMap[culture][key] = value;
                        
                    }

                    TroopPool.ReInitalizeTroopPool(TroopPoolMap);

                }
                catch (Exception ex) {
                    InformationManager.DisplayMessage(new InformationMessage($"Error loading troop config: {ex.Message}"));
                }
            }
            else {
                InformationManager.DisplayMessage(new InformationMessage("Troop config file not found."));
            }
        }
    }

}
