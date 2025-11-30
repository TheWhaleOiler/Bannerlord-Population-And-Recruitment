using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace PopulationAndRecruitment {
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyWithPartyTemplate")]
    public class PatchFillPartyStacks {

        static bool Prefix(MobileParty __instance) {

            if (__instance.IsBandit)
                return true;

            var culture = __instance.LeaderHero?.Culture;
            if (culture != null) {
                var troop = TroopPool.GetMilitiaTroop(culture);

                __instance.MemberRoster.AddToCounts(troop, MBRandom.RandomInt(5,10)); 
                
                return false;
            }
            return true;
        }

    }
}
