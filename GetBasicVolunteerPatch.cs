using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarmonyLib;
using MCM.Common;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using static TaleWorlds.Core.ItemCategory;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(DefaultVolunteerModel))]
    public class GetBasicVolunteerPatch {

        [HarmonyPrefix]
        [HarmonyPatch("GetBasicVolunteer")]
        public static bool GetBasicVolunteer_Postfix(ref CharacterObject __result, Hero sellerHero) {

            var settings = PopulationAndRecruitmentSettings.Instance;
            var settlement = sellerHero?.CurrentSettlement;
            var clan = settlement?.OwnerClan;
            var kingdom = clan?.Kingdom;
            var culture = sellerHero?.Culture;

            if (culture == null 
                || settlement == null)
                return false;


            var isDemobilizing = settings.EnableAiSettlementMilitiaDemobilization && checkIsAtWar(kingdom, clan);

            if (settlement.IsTown) {
                var prosperity = settlement.Town.Prosperity;

                if (HighProsperityRoll(prosperity, settings.TownAsymptoteDenominatior)
                    || isDemobilizing)
                    
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

                if (HighProsperityRoll(prosperity, settings.CastleAsymptoteDenominatior)
                    || isDemobilizing)

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
            //if (settings.EnableAiSettlementMilitiaDemobilization
            //    && TroopPool.IsMilitia(__result)
            //    && !checkIsAtWar(kingdom, clan)) {

            //    __result = null;

            //    if (MBRandom.RandomFloat > 0.95f && settlement.IsVillage) // 5% Chance for militia troop to spawn for lords who doesnt own fief in peacetime
            //        __result = TroopPool.GetMilitiaTroop(culture);
            //}

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

        private static Dictionary<string, Dictionary<string, List<CharacterObject>>> cultureTroopPool = new();

        private static List<CharacterObject> allMilitiaTroops = new();

        private static Random _random = new Random();

        public static void ReInitalizeTroopPool(Dictionary<string, Dictionary<string, List<string>>> troopMap) {

            bool allLoadedFlag = true;

            cultureTroopPool = new();

            String[] troopTypes = new[] { "EliteTroop", "BasicTroop", "EliteMilitiaTroop", "MilitiaTroop" };

            List<String> militiaTypes = new List<string> { "EliteMilitiaTroop", "MilitiaTroop" };

        
            foreach (var culture in MBObjectManager.Instance.GetObjectTypeList<CultureObject>()) {
                if (culture.MeleeMilitiaTroop != null) {
                    allMilitiaTroops.Add(culture.MeleeMilitiaTroop);
                }
                if (culture.RangedMilitiaTroop != null) {
                    allMilitiaTroops.Add(culture.RangedMilitiaTroop);
                }
            }


            foreach (string culture in troopMap.Keys) {
                if (!cultureTroopPool.ContainsKey(culture)) {
                    cultureTroopPool[culture] = new Dictionary<string, List<CharacterObject>>();
                }

                foreach (string type in troopMap[culture].Keys) {

                    if (!cultureTroopPool[culture].ContainsKey(type))
                        cultureTroopPool[culture][type] = new ();

                    foreach (string troop_id in troopMap[culture][type]) {
                        CharacterObject troop = CharacterObject.Find(troop_id);
                        if (troop == null) {
                            allLoadedFlag = false;
                            InformationManager.DisplayMessage(new InformationMessage($"{culture} : {type}"));
                            continue;
                        }

                        CharacterObject foundTroop = CharacterObject.Find(troop_id);

                        if (militiaTypes.Contains(type)) {
                            allMilitiaTroops.Add(foundTroop);
                        }

                        cultureTroopPool[culture][type].Add(foundTroop);
                    }
                }
            }

            if (allLoadedFlag == false)
                InformationManager.DisplayMessage(new InformationMessage("Some Troop config loaded unsuccessfully.", Colors.Red));
            else
                InformationManager.DisplayMessage(new InformationMessage($"{PopulationAndRecruitmentSettings.Instance.TroopConfigDropDown.SelectedValue} Troop config loaded successfully."));
        }

        public static bool IsMilitia(CharacterObject c) {
            return allMilitiaTroops.Contains(c);
        }
        private static bool GetTroop(CultureObject co, string troopType, out CharacterObject refTroop) {

            string cultureName = co.GetName().ToString().ToLower();

            if (cultureTroopPool.ContainsKey(cultureName)
                && cultureTroopPool[cultureName].ContainsKey(troopType)
                && cultureTroopPool[cultureName][troopType].Count > 0) {

                var troop_count = cultureTroopPool[cultureName][troopType].Count;

                int choice = _random.Next(troop_count);

                refTroop = cultureTroopPool[cultureName][troopType][choice];
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
        public static void OnGameLoaded_PostFix() => InitializeTroopConfig();

        [HarmonyPostfix]
        [HarmonyPatch("OnNewGameCreated")]
        public static void OnNewGameCreated_Postfix() => InitializeTroopConfig();

        internal static void InitializeTroopConfig() {
            Dictionary<string, Dictionary<string, List<string> > > TroopPoolMap = new Dictionary<string, Dictionary<string, List<string>>>();

            var settings = PopulationAndRecruitmentSettings.Instance;

            InitalizeTroopConfigDropDown();

            string configName = settings.TroopConfigDropDown.SelectedValue;

            if (configName == "None")
                return;

            string configPath = Path.Combine(BasePath.Name, "Modules", "PopulationAndRecruitment", "troop_configs", configName);

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
                            TroopPoolMap[culture] = new Dictionary<string, List<string>>();

                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        List<string> value_list = value.Trim('[', ']')
                            .Split(',')
                            .Select(s => s.Trim())
                            .ToList();

                        TroopPoolMap[culture][key] = value_list;
                        
                    }

                    TroopPool.ReInitalizeTroopPool(TroopPoolMap);

                }
                catch (Exception ex) {
                    InformationManager.DisplayMessage(new InformationMessage($"Error loading troop config: {ex.Message}"));
                }
            }
            else {
                InformationManager.DisplayMessage(new InformationMessage($"{configName} Troop config file not found."));
            }
        }

        internal static void InitalizeTroopConfigDropDown() {
            var settings = PopulationAndRecruitmentSettings.Instance;

            var selectedIndex = settings.TroopConfigDropDown.SelectedIndex;

            var newDropDown = new Dropdown<string>(FindAllTroopConfigs(), selectedIndex);

            settings.TroopConfigDropDown = newDropDown;
        }
        private static List<string> FindAllTroopConfigs() {
            List<string> allTroopConfigs = new();

            string configPath = Path.Combine(BasePath.Name, "Modules", "PopulationAndRecruitment", "troop_configs");

            if (Directory.Exists(configPath)) {
                string[] files = Directory.GetFiles(configPath);
                foreach (string file in files) {
                    allTroopConfigs.Add(Path.GetFileName(file));
                }
            }
            return allTroopConfigs;
        }
    }

    [HarmonyPatch(typeof(InitialState))]
    internal static class InitialStatePatch {
        [HarmonyPostfix]
        [HarmonyPatch("OnActivate")]
        public static void OnActivate_Postfix() {
            InitializeTroopPool.InitalizeTroopConfigDropDown();
        }
    }

}
