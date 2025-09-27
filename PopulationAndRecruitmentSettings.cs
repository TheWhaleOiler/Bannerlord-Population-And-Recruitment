using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Attributes.v2;

namespace PopulationAndRecruitment {

    public class PopulationAndRecruitmentSettings : AttributeGlobalSettings<PopulationAndRecruitmentSettings> {
        public override string Id => "PopulationAndRecruitmentSettings";
        public override string DisplayName => "Population & Recruitment";
        public override string FolderName => "PopulationAndRecruitment";
        public override string FormatType => "json";


        [SettingPropertyInteger("Town", 0, 20000, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate")]
        public int TownAsymptoteDenominatior { get; set; } = 4000;

        [SettingPropertyInteger("Castle", 0, 20000, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate")]
        public int CastleAsymptoteDenominatior { get; set; } = 1500;

        [SettingPropertyInteger("Village (Hearth)", 0, 10000, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate")]
        public int VillageAsymptoteDenominatior { get; set; } = 500;

        [SettingPropertyInteger("Castle Elite Troop", 0, 20000, "0", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate")]
        public int CastleEliteAsymptoteDenominator { get; set; } = 1000;

        [SettingPropertyInteger("Town Elite Troop", 0, 20000, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops")]
        public int TownEliteAsymptoteDenominator { get; set; } = 2000;

        [SettingPropertyInteger("Village (Hearth) Elite Troop", 0, 20000, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops")]
        public int VillageEliteAsymptoteDenominator { get; set; } = 600;

        [SettingPropertyBool("Enable Global Elite Troops", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Prosperity Needed For Equal Spawnrate/Global Elite Troops")]
        public bool EnableGlobalEliteTroops { get; set; } = false;


        [SettingPropertyFloatingInteger("Fief Prosperity Decrease On Spawn", 0f, 1000f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Prosperity")]
        public float ProsperityDecreaseOnRecruitment { get; set; } = 0f;

        [SettingPropertyFloatingInteger("Village Hearth Decrease On Spawn", 0f, 100f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Prosperity")]
        public float HearthDecreaseOnRecruitment { get; set; } = 0f;


        [SettingPropertyFloatingInteger("Fief Militia Decrease On Spawn", 0f, 100f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Militia")]
        public float FiefMilitiaDecreaseOnRecruitment { get; set; } = 1f;

        [SettingPropertyFloatingInteger("Village Militia Decrease On Spawn", 0f, 100f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Militia")]
        public float VillageMIlitiaDecreaseOnRecruitment { get; set; } = 1f;


        [SettingPropertyFloatingInteger("Minimum Fief Prosperity Required", 0f, 10000f, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumFiefProsperityRequired { get; set; } = 600f;

        [SettingPropertyFloatingInteger("Minimum Village Hearth Required", 0f, 1000f, "0", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumVillageHearthRequired { get; set; } = 75f;


        [SettingPropertyFloatingInteger("Minimum Town Militia Required", 0f, 10000f, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumTownMilitiaRequired { get; set; } = 250f;

        [SettingPropertyFloatingInteger("Minimum Castle Militia Required", 0f, 10000f, "0", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumCastleMilitiaRequired { get; set; } = 100f;

        [SettingPropertyFloatingInteger("Minimum Village Militia Required", 0f, 1000f, "0", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Recruitment Costs/Minimum")]
        public float MinimumVillageMilitiaRequired { get; set; } = 25f;


        [SettingPropertyBool("Disable Spawn Cost During Peace", Order = 3, RequireRestart = false, HintText = "Enable To Allow Kingdoms To Recover After War")]
        [SettingPropertyGroup("Recruitment Costs")]
        public bool DisableSpawnCostDuringKingdomPeace { get; set; } = false;


        [SettingPropertyFloatingInteger("Mercenary Spawnrate", 0f, 1f, "#0%", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Spawnrate")]
        public float TavernMercenarySpawnRate { get; set; } = 0.3f;


        [SettingPropertyInteger("AI Militia Demobilization", 0, 100, "0", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Militia")]
        public int AiMilitiaDemobilizationRate { get; set; } = 1;

        [SettingPropertyFloatingInteger("Village Militia Gradual Increase", 0f, 50f, "0", Order = 0, RequireRestart = false, HintText = "Value x in Log10(x*hearths + 1) * 2")]
        [SettingPropertyGroup("Militia")]
        public float VillageMilitiaIncreaseMultiplier { get; set; } = 5f;

        [SettingPropertyFloatingInteger("Village Militia Flat Increase", 0f, 10f, "0", Order = 1, RequireRestart = false, HintText = "Flat Militia Increase")]
        [SettingPropertyGroup("Militia")]
        public float VillageMilitiaFlatIncrease { get; set; } = 0.5f;

        [SettingPropertyBool("Village Only Spawns Militia", Order = 2, RequireRestart = false, HintText = "Disables Village/Party Demobilization")]
        [SettingPropertyGroup("Militia")]
        public bool VillageOnlySpawnsMilitia { get; set; } = true;


        [SettingPropertyBool("Disable Player Parties Can Recruiting From Player Fiefs", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Miscellaneous")]
        public bool DisablePlayerPartiesCanRecruitFromPlayerFiefs { get; set; } = false;

        [SettingPropertyBool("Lords Can Only Recruit From Kingdom", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Miscellaneous")]
        public bool LordsCanOnlyRecruitFromKingdom { get; set; } = false;

        [SettingPropertyBool("Enable Troop Compatbility Patch", Order = 0, RequireRestart = false, HintText = "Only needed if you didn't specify the troops in troop_config file")]
        [SettingPropertyGroup("Miscellaneous")]

        public bool EnableTroopCompatibility { get; set; } = false;

        [SettingPropertyBool("Debug", Order = 999, RequireRestart = false, HintText = "Debug")]
        [SettingPropertyGroup("Debug")]
        public bool DebugLog { get; set; } = false;


    }
}
