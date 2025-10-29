using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_DoMechBillByMech
{
    [HarmonyPatch(typeof(Bill_Mech), "PawnAllowedToStartAnew")]
    public static class Bill_Mech_PawnAllowedToStartAnew
    {
        public static void Prefix(ref Pawn p)
        {
            var myConscious = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;

            if (p != null && p.RaceProps.IsMechanoid && p.GetOverseer() == myConscious)
            {
                p = p.GetOverseer();
            }
        }
    }
}
