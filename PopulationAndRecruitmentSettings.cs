using System.Collections.Generic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace PopulationAndRecruitment {

    public class PopulationAndRecruitmentSettings : AttributeGlobalSettings<PopulationAndRecruitmentSettings> {
        public override string Id => "PopulationAndRecruitmentSettings";
        public override string DisplayName => "Population & Recruitment";
        public override string FolderName => "PopulationAndRecruitment";
        public override string FormatType => "json";


        [SettingPropertyInteger("Town", 0, 20000, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate", GroupOrder = 1)]
        public int TownAsymptoteDenominatior { get; set; } = 4000;

        [SettingPropertyInteger("Castle", 0, 20000, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate", GroupOrder = 1)]
        public int CastleAsymptoteDenominatior { get; set; } = 1500;

        [SettingPropertyInteger("Village (Hearth)", 0, 10000, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate", GroupOrder = 1)]
        public int VillageAsymptoteDenominatior { get; set; } = 500;

        [SettingPropertyInteger("Castle Elite Troop", 0, 20000, "0", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate", GroupOrder = 1)]
        public int CastleEliteAsymptoteDenominator { get; set; } = 1000;

        [SettingPropertyInteger("Town Elite Troop", 0, 20000, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops", GroupOrder = 1)]
        public int TownEliteAsymptoteDenominator { get; set; } = 2000;

        [SettingPropertyInteger("Village (Hearth) Elite Troop", 0, 20000, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops", GroupOrder = 1)]
        public int VillageEliteAsymptoteDenominator { get; set; } = 600;

        [SettingPropertyBool("Enable Global Elite Troops", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops", GroupOrder = 1)]
        public bool EnableGlobalEliteTroops { get; set; } = false;


        [SettingPropertyFloatingInteger("Fief Prosperity Decrease On Spawn", 0f, 1000f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Prosperity", GroupOrder = 1)]
        public float ProsperityDecreaseOnRecruitment { get; set; } = 0f;

        [SettingPropertyFloatingInteger("Village Hearth Decrease On Spawn", 0f, 100f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Prosperity", GroupOrder = 1)]
        public float HearthDecreaseOnRecruitment { get; set; } = 0f;


        [SettingPropertyFloatingInteger("Fief Militia Decrease On Spawn", 0f, 100f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Militia", GroupOrder = 1)]
        public float FiefMilitiaDecreaseOnRecruitment { get; set; } = 1f;

        [SettingPropertyFloatingInteger("Village Militia Decrease On Spawn", 0f, 100f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Militia", GroupOrder = 1)]
        public float VillageMIlitiaDecreaseOnRecruitment { get; set; } = 1f;


        [SettingPropertyFloatingInteger("Minimum Fief Prosperity Required", 0f, 10000f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum", GroupOrder = 1)]
        public float MinimumFiefProsperityRequired { get; set; } = 600f;

        [SettingPropertyFloatingInteger("Minimum Village Hearth Required", 0f, 1000f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum", GroupOrder = 1)]
        public float MinimumVillageHearthRequired { get; set; } = 75f;


        [SettingPropertyFloatingInteger("Minimum Town Militia Required", 0f, 10000f, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum", GroupOrder = 1)]
        public float MinimumTownMilitiaRequired { get; set; } = 250f;

        [SettingPropertyFloatingInteger("Minimum Castle Militia Required", 0f, 10000f, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumCastleMilitiaRequired { get; set; } = 100f;

        [SettingPropertyFloatingInteger("Minimum Village Militia Required", 0f, 1000f, "0", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum", GroupOrder = 1)]
        public float MinimumVillageMilitiaRequired { get; set; } = 25f;


        [SettingPropertyBool("Disable Spawn Cost During Peace", Order = 3, RequireRestart = false, HintText = "Enable To Allow Kingdoms To Recover After War")]
        [SettingPropertyGroup("Recruitment Costs", GroupOrder = 1)]
        public bool DisableSpawnCostDuringKingdomPeace { get; set; } = false;


        [SettingPropertyFloatingInteger("Mercenary Spawnrate", 0f, 1f, "#0%", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Spawnrate", GroupOrder = 1)]
        public float TavernMercenarySpawnRate { get; set; } = 0.3f;


        [SettingPropertyInteger("AI Party Militia Demobilization", 0, 100, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Demobilization", GroupOrder = 1)]
        public int AiMilitiaDemobilizationRate { get; set; } = 1;

        [SettingPropertyBool("AI Settlement Militia Demobilization", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Demobilization", GroupOrder = 1)]
        public bool EnableAiSettlementMilitiaDemobilization { get; set; } = false;

        [SettingPropertyFloatingInteger("Village Militia Gradual Increase", 0f, 50f, "0", Order = 0, RequireRestart = false, HintText = "Value x in Log10(x*hearths + 1) * 2")]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public float VillageMilitiaIncreaseMultiplier { get; set; } = 5f;

        [SettingPropertyFloatingInteger("Town Militia Gradual Increase", 0f, 50f, "0", Order = 1, RequireRestart = false, HintText = "Value x in Log10(x*hearths + 1) * 2")]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public float TownMilitiaIncreaseMultiplier { get; set; } = 0f;

        [SettingPropertyFloatingInteger("Castle Militia Gradual Increase", 0f, 50f, "0", Order = 2, RequireRestart = false, HintText = "Value x in Log10(x*hearths + 1) * 2")]
        [SettingPropertyGroup("Militia", GroupOrder = 1 )]
        public float CastleMilitiaIncreaseMultiplier { get; set; } = 0f;

        [SettingPropertyFloatingInteger("Village Militia Flat Increase", 0f, 10f, "0", Order = 3, RequireRestart = false, HintText = "Flat Militia Increase")]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public float VillageMilitiaFlatIncrease { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("Town Militia Flat Increase", 0f, 10f, "0", Order = 4, RequireRestart = false, HintText = "Flat Militia Increase")]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public float TownMilitiaFlatIncrease { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("Castle Militia Flat Increase", 0f, 10f, "0", Order = 5, RequireRestart = false, HintText = "Flat Militia Increase")]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public float CastleMilitiaFlatIncrease { get; set; } = 0.5f;

        [SettingPropertyBool("Village Only Spawns Militia", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Militia", GroupOrder = 1)]
        public bool VillageOnlySpawnsMilitia { get; set; } = true;


        [SettingPropertyBool("Disable Player Parties Can Recruiting From Player Fiefs", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Miscellaneous", GroupOrder = 1)]
        public bool DisablePlayerPartiesCanRecruitFromPlayerFiefs { get; set; } = false;

        [SettingPropertyBool("Lords Can Only Recruit From Kingdom", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Miscellaneous", GroupOrder = 1)]
        public bool LordsCanOnlyRecruitFromKingdom { get; set; } = false;

        [SettingPropertyBool("Enable Lazy Troop Compatbility Patch", Order = 2, RequireRestart = false, HintText = "Only needed if you didn't specify the troops in troop_config file")]
        [SettingPropertyGroup("Miscellaneous", GroupOrder = 1)]

        public bool EnableTroopCompatibility { get; set; } = false;

        [SettingPropertyBool("Enable Only Settlement Owner Can Recruit From Settlement", Order = 0, RequireRestart = false, HintText = "Disables Ai Clan Members From Recruiting From Their Lord's Fiefs")]
        [SettingPropertyGroup("Miscellaneous", GroupOrder = 1)]
        public bool DisableAiPartiesCanRecruitFromAiLordFiefs { get; set; } = false;


        private Dropdown<string> _troopConfigDropdown = new Dropdown<string>(
            new List<string> {"None"}, 0
        );

        [SettingPropertyDropdown("Choose Troop Config", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Troop Config", GroupOrder = 0)]
        public Dropdown<string> TroopConfigDropDown {
            get => _troopConfigDropdown;
            set => _troopConfigDropdown = value;
        }


        [SettingPropertyBool("Debug", Order = 999, RequireRestart = false, HintText = "Debug")]
        [SettingPropertyGroup("Debug", GroupOrder = 999)]
        public bool DebugLog { get; set; } = false;


    }
}
