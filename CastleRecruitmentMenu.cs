using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace PopulationAndRecruitment {
    internal class CastleRecruitmentMenu : CampaignBehaviorBase {
        public override void RegisterEvents() {
            CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, AddCastleRecruitMenus);
        }

        public void AddCastleRecruitMenus(CampaignGameStarter campaignGameSystemStarter) {
            campaignGameSystemStarter.AddGameMenuOption("castle", "recruit_volunteers", "{=E31IJyqs}Recruit troops", game_menu_recruit_castle_volunteers_on_condition, game_menu_recruit_castle_volunteers_on_consequence, isLeave: false, 4);
        }

        public static void game_menu_recruit_castle_volunteers_on_consequence(MenuCallbackArgs args) {
            args.MenuContext.OpenRecruitVolunteers();
        }

        public static bool game_menu_recruit_castle_volunteers_on_condition(MenuCallbackArgs args) {
            args.optionLeaveType = GameMenuOption.LeaveType.Recruit;

            Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
            if (currentSettlement != null && currentSettlement.IsCastle) {
                if (currentSettlement.OwnerClan == Clan.PlayerClan) {
                    return true;
                }
                else {
                    args.Tooltip = new TextObject("{=NoRecruitCastleNotOwner}You Do Not Own This Castle.");
                    args.IsEnabled = false;
                    return true;
                }
            }

            return false;
        }


        public override void SyncData(IDataStore dataStore) {
        }
    }
}
