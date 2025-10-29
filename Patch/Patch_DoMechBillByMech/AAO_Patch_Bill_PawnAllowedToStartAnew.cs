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
    [HarmonyPatch(typeof(Bill), nameof(Bill.PawnAllowedToStartAnew))]
    public static class Bill_PawnAllowedToStartAnew_Patch
    {

        public static bool Prefix(Bill __instance, Pawn p, ref bool __result)
        {


            //Log.Message($"[AllasOne] Bill.PawnAllowedToStartAnew Prefix called for bill {__instance.Label} and pawn {p?.LabelShort}.");

            var myConscious = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (myConscious == null || p.RaceProps.IsMechanoid != true)
            {
                return true;
            }


            var overseer = p.GetOverseer();
            if (overseer == null || overseer != myConscious)
            {
                return true;
            }
            
            __result = __instance.PawnAllowedToStartAnew(p.GetOverseer());
            return false; // 跳过原方法



        }
    }
}
