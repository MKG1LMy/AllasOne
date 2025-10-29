using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AllasOne.WorldandGame;


namespace AllasOne.Patch.Patch_PawnsFinder
{
    [HarmonyPatch(typeof(PawnsFinder), "get_AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists")]
    public static class AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_Postfix
    {
        // Postfix 方法：我们在原方法执行后加入额外的角色
        public static void Postfix(ref List<Pawn> __result)
        {
            Pawn MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;

            if (MC == null)
            { 
                return; 
            }

            List<Pawn> allMapsCaravansAndTravellingTransporters_Alive = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive;

            for (int i = 0; i < allMapsCaravansAndTravellingTransporters_Alive.Count; i++)
            {
                Pawn pawn = allMapsCaravansAndTravellingTransporters_Alive[i];
                if (pawn.IsColonyMech && pawn?.GetOverseer() == MC )
                {
                    __result.Add(allMapsCaravansAndTravellingTransporters_Alive[i]);
                }
            }


        }


    }


    [HarmonyPatch(typeof(PawnsFinder), "get_AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoSuspended")]
    public static class AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoSuspended_Postfix
    {
        // Postfix 方法：我们在原方法执行后加入额外的角色
        public static void Postfix(ref List<Pawn> __result)
        {
            Pawn MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;

            if (MC == null)
            {
                return;
            }

            List<Pawn> allMapsCaravansAndTravellingTransporters_Alive = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive;

            for (int i = 0; i < allMapsCaravansAndTravellingTransporters_Alive.Count; i++)
            {
                Pawn pawn = allMapsCaravansAndTravellingTransporters_Alive[i];
                if (pawn.IsColonyMech && pawn?.GetOverseer() == MC)
                {
                    __result.Add(allMapsCaravansAndTravellingTransporters_Alive[i]);
                }
            }


        }


    }
}
