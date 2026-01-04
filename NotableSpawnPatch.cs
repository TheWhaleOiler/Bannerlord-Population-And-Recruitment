using System.Linq;
using System.Runtime.Serialization;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(Campaign))]
    public static class NotableSpawnPatch {

        [HarmonyPostfix]
        [HarmonyPatch("OnNewGameCreated")]
        public static void OnNewGameCreated_Postfix() {
            foreach (Settlement settlement in Settlement.All) {
                if (settlement.IsCastle && settlement.Notables.Count == 0) {
                    AddNotablesToCastle(settlement);
                }
            }
        }

        private static void AddNotablesToCastle(Settlement castle) {

            HeroCreator.CreateNotable(Occupation.Headman, castle);

            for (int i = 0; i < 4; i++) {
                HeroCreator.CreateNotable(Occupation.RuralNotable, castle);
            }
        }
    }
}