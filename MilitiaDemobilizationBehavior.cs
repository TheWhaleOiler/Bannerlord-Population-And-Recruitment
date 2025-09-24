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
        private List<CharacterObject> _allMilitiaTroops;

        public override void RegisterEvents() {

            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, (starter) => InitializeMilitiaTroopsList());

            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, (starter) => InitializeMilitiaTroopsList());

            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore) {}

        private void InitializeMilitiaTroopsList() {

            var allCultures = new List<CultureObject>();

            foreach (CultureObject objectType in MBObjectManager.Instance.GetObjectTypeList<CultureObject>()) {
                if (objectType.IsMainCulture) {
                    allCultures.Add(objectType);
                }
            }

            _allMilitiaTroops = new List<CharacterObject>();
            foreach (var culture in allCultures) {
                if (culture.MeleeMilitiaTroop != null) {
                    _allMilitiaTroops.Add(culture.MeleeMilitiaTroop);
                }
                if (culture.RangedMilitiaTroop != null) {
                    _allMilitiaTroops.Add(culture.RangedMilitiaTroop);
                }
            }

        }

        private void OnDailyTick() {
            foreach (var party in MobileParty.All.Where(p => p.IsLordParty && !p.IsMainParty)) {
                PurgeMilitia(party);
            }
        }

        private void PurgeMilitia(MobileParty party) {
            var settings = PopulationAndRecruitmentSettings.Instance;

            if (settings.AiMilitiaDemobilizationRate <= 0 
                || settings.VillageOnlySpawnsMilitia)
                return;

            var faction = party.LeaderHero?.MapFaction;
            if (faction == null || GetBasicVolunteerPatch.checkIsAtWar(party.LeaderHero?.Clan?.Kingdom, party.LeaderHero?.Clan)) {
                return;
            }

            var militiaElements = party.MemberRoster.GetTroopRoster()
                                  .Where(e => e.Character != null && _allMilitiaTroops.Contains(e.Character))
                                  .ToList();

            if (!militiaElements.Any()) {
                return;
            }

            int troopsToRemove = settings.AiMilitiaDemobilizationRate;

            foreach (var element in militiaElements) {
                if (troopsToRemove <= 0) break;

                int countInRoster = party.MemberRoster.GetElementNumber(element.Character);
                int amountToRemove = Math.Min(troopsToRemove, countInRoster);

                party.MemberRoster.AddToCounts(element.Character, -amountToRemove);

                troopsToRemove -= amountToRemove;
            }
        }
    }
}
