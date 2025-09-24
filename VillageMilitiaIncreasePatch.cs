using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;


namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(DefaultSettlementMilitiaModel))]
    public static class VillageMilitiaGrowthPatches {
        [HarmonyPostfix]
        [HarmonyPatch("CalculateMilitiaChange")]
        public static void CalculateMilitiaChange_Postfix(Settlement settlement, ref ExplainedNumber __result) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            if (settlement == null) return;

            if (settlement.IsVillage) {
                float originalChange = __result.ResultNumber;

                if (settings.VillageMilitiaFlatIncrease > 0f)
                    __result.Add(settings.VillageMilitiaFlatIncrease, new TextObject("Village Militia Flat Increase"));

                if (originalChange > 0) {
                    float bonus = (float) Math.Log10( originalChange * settings.VillageMilitiaIncreaseMultiplier + 1 ) * 2;

                    if (bonus > 0) 
                        __result.Add(bonus, new TextObject("Village Militia Gradual Growth"));
                }
            }
        }
    }
}
