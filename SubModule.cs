using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;


namespace PopulationAndRecruitment
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();

            // patch any other bits you need
            new Harmony("mod.population&recruitment").PatchAll();

            InformationManager.DisplayMessage(new InformationMessage(
                     "Population And Recruitment Loaded",
                    Colors.White
                ));
        }

        protected override void OnGameStart(Game game, IGameStarter starter) {
            var campaignGameStarter = (CampaignGameStarter)starter;
            campaignGameStarter.AddBehavior(new MilitiaPurgeCampaignBehavior());
            campaignGameStarter.AddBehavior(new CastleRecruitmentMenu());
            campaignGameStarter.AddBehavior(new CastleAiRecruitmentDecision());
            campaignGameStarter.AddModel(new SettlementRecruitmentModel());
        }
    }
}