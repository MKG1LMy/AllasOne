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
    [HarmonyPatch(typeof(CaravanUtility), nameof(CaravanUtility.IsOwner))]
    internal static class Patch_CaravanUtility_IsOwner
    {
        private static void Postfix(ref bool __result, Pawn pawn, Faction caravanFaction)
        {
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;
            if (!__result && pawn.GetOverseer() == myConscious)
            {
                __result = true;
            }
        }
    }
}
