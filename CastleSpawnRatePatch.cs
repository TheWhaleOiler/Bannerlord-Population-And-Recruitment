using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PopulationAndRecruitment {

    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement")]
    public class CastleSpawnRatePatch {
        [HarmonyPostfix]
        static void Postfix(Settlement settlement) {

            if (!settlement.IsCastle || settlement.IsUnderSiege) {
                return;
            }

            foreach (Hero notable in settlement.Notables) {
                if (!notable.CanHaveRecruits || !notable.IsAlive) {
                    continue;
                }
                bool flag = false;
                CharacterObject basicVolunteer = Campaign.Current.Models.VolunteerModel.GetBasicVolunteer(notable);

                if (basicVolunteer == null)
                    continue;

                for (int i = 0; i < 6; i++) {
                    if (!(MBRandom.RandomFloat <= TaleWorlds.Library.MathF.Clamp(settlement.Town.Prosperity / 2000f, 0f, 0.5f))) {
                        continue;
                    }
                    CharacterObject characterObject = notable.VolunteerTypes[i];
                    if (characterObject == null) {
                        notable.VolunteerTypes[i] = basicVolunteer;
                        flag = true;
                    }
                    else if (characterObject.UpgradeTargets.Length != 0 && characterObject.Tier < 5) {
                        float num = TaleWorlds.Library.MathF.Log(notable.Power / (float)characterObject.Tier, 2f) * 0.01f;
                        if (MBRandom.RandomFloat < num) {
                            notable.VolunteerTypes[i] = characterObject.UpgradeTargets[MBRandom.RandomInt(characterObject.UpgradeTargets.Length)];
                            flag = true;
                        }
                    }
                }

                if (!flag) {
                    continue;
                }

                CharacterObject[] volunteerTypes = notable.VolunteerTypes;
                for (int j = 1; j < 6; j++) {
                    CharacterObject characterObject2 = volunteerTypes[j];
                    if (characterObject2 == null) {
                        continue;
                    }

                    int num2 = 0;
                    int num3 = j - 1;
                    CharacterObject characterObject3 = volunteerTypes[num3];
                    while (num3 >= 0 && (characterObject3 == null || (float)characterObject2.Level + (characterObject2.IsMounted ? 0.5f : 0f) < (float)characterObject3.Level + (characterObject3.IsMounted ? 0.5f : 0f))) {
                        if (characterObject3 == null) {
                            num3--;
                            num2++;
                            if (num3 >= 0) {
                                characterObject3 = volunteerTypes[num3];
                            }

                            continue;
                        }

                        volunteerTypes[num3 + 1 + num2] = characterObject3;
                        num3--;
                        num2 = 0;
                        if (num3 >= 0) {
                            characterObject3 = volunteerTypes[num3];
                        }
                    }
                    volunteerTypes[num3 + 1 + num2] = characterObject2;
                }
            }
        }
    }
}
