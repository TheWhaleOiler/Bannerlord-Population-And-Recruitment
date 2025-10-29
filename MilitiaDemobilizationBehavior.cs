using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.ObjectSystem;

namespace PopulationAndRecruitment {
    public class MilitiaPurgeCampaignBehavior : CampaignBehaviorBase {

        public override void RegisterEvents() {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore) {}


        private void OnDailyTick() {

            var settings = PopulationAndRecruitmentSettings.Instance;

            if (settings.AiMilitiaDemobilizationRate <= 0)
                return;

            foreach (var party in MobileParty.All.Where(p => p.IsLordParty && !p.IsMainParty)) {
                PurgeMilitia(party);
            }
        }

        private static IEnumerable<CharacterObject> GetUpgradePath(CharacterObject character) {
            var visited = new HashSet<CharacterObject>();
            var stack = new Stack<CharacterObject>();
            stack.Push(character);

            while (stack.Count > 0) {
                var current = stack.Pop();
                if (visited.Add(current)) {
                    yield return current;
                    foreach (var upgrade in current.UpgradeTargets) {
                        stack.Push(upgrade);
                    }
                }
            }
        }


        private void PurgeMilitia(MobileParty party) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            var faction = party.LeaderHero?.MapFaction;

            int troopsToRemove = settings.AiMilitiaDemobilizationRate;

            if (troopsToRemove <= 0)
                return;

            if (CheckRecruitingPatch.HasAvailableFiefToRecruitFrom(party.LeaderHero))
                return;
            
            if (faction == null 
                || GetBasicVolunteerPatch.checkIsAtWar(party.LeaderHero?.Clan?.Kingdom, party.LeaderHero?.Clan)) {
                return;
            }

            var militiaElements = party.MemberRoster.GetTroopRoster()
                                  .Where(e => e.Character != null && TroopPool.IsMilitia(e.Character))
                                  .SelectMany(e => GetUpgradePath(e.Character))
                                  .Distinct()
                                  .ToList();

            if (!militiaElements.Any()) {
                return;
            }


            foreach (var character in militiaElements) {

                int countInRoster = party.MemberRoster.GetElementNumber(character);
                int amountToRemove = Math.Min(troopsToRemove, countInRoster);

                party.MemberRoster.AddToCounts(character, -amountToRemove);

                troopsToRemove -= amountToRemove;
            }
        }
    }
}
