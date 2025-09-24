using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateCurrentMercenaryTroopAndCount")]
    public static class TavernMercenariesSpawnRatePatch {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            var findNumberOfMercenariesMethod = AccessTools.Method(
                typeof(RecruitmentCampaignBehavior),
                "FindNumberOfMercenariesWillBeAdded"
            );

            var multiplyMethod = AccessTools.Method(
                typeof(TavernMercenariesSpawnRatePatch),
                nameof(MultiplyMercenaryCount)
            );

            for (int i = 0; i < codes.Count; i++) {

                if (codes[i].opcode == OpCodes.Call && codes[i].operand as MethodInfo == findNumberOfMercenariesMethod) {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_1));

                    codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, multiplyMethod));
                    i += 2;
                }
            }

            return codes;
        }

        public static int MultiplyMercenaryCount(int originalValue, Town town) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            return MathF.Round(originalValue * settings.TavernMercenarySpawnRate);
        }
    }
}
