using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(Caravan_CarryTracker), "CalculatePawnsWhoCanCarry")]
    public static class CalculatePawnsWhoCanCarry_Patch
    {

        public static bool Prefix(Caravan_CarryTracker __instance, List<Pawn> outPawns)
        {
            Caravan caravan = __instance.caravan;
            outPawns.Clear();
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;
            for (int i = 0; i < caravan.pawns.Count; i++)
            {
                Pawn pawn = caravan.pawns[i];
                if ((pawn.RaceProps.Humanlike || pawn.GetOverseer() == myConscious) && !pawn.Downed && !pawn.InMentalState && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && !WantsToBeCarried(pawn))
                {
                    outPawns.Add(pawn);
                    //Log.Message($"Add pawn to outPawn because pawn is:{__instance}");
                }
            }
            return false;  // 返回false将阻止原始方法的执行
        }

        private static bool WantsToBeCarried(Pawn p)
        {
            if (p.health.beCarriedByCaravanIfSick)
            {
                return CaravanCarryUtility.WouldBenefitFromBeingCarried(p);
            }
            return false;
        }


    }
}
