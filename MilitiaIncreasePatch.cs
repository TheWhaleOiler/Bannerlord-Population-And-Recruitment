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

            if (!(settlement.IsTown || settlement.IsCastle || settlement.IsVillage))
                return;

            float flatIncrease = 0f;
            float gradualIncrease = 0f;
            string settlementType = null;

            if (settlement.IsTown) {
                flatIncrease = settings.TownMilitiaFlatIncrease;
                gradualIncrease = settings.TownMilitiaIncreaseMultiplier;
                settlementType = "Town";
            }

            else if (settlement.IsCastle) {
                flatIncrease = settings.CastleMilitiaFlatIncrease;
                gradualIncrease = settings.CastleMilitiaIncreaseMultiplier;
                settlementType = "Castle";
            }

            else if (settlement.IsVillage) {
                flatIncrease = settings.VillageMilitiaFlatIncrease;
                gradualIncrease = settings.VillageMilitiaIncreaseMultiplier;
                settlementType = "Village";
            }

            float originalChange = __result.ResultNumber;

            if (flatIncrease > 0f)
                __result.Add(flatIncrease, new TextObject($"{settlementType} Militia Flat Increase"));

            if (originalChange > 0) {
                float bonus = (float)Math.Log10(originalChange * gradualIncrease + 1) * 2;

                if (bonus > 0)
                    __result.Add(bonus, new TextObject($"{settlementType} Militia Gradual Growth"));
            }
        }
    }
}
