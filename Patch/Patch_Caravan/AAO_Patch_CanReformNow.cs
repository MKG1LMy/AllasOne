using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(FormCaravanComp), "CanReformNow")]
    public static class CanReformNow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(FormCaravanComp __instance, ref bool __result)
        {
            if (!__result)
            {
                MapParent mapParent = (MapParent)__instance.parent;
                var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
                var myConscious = mgr?.MechanoidConsciousness;
                if (mapParent.HasMap && __instance.Reform && __instance.CanFormOrReformCaravanNow)
                {
                    foreach (Pawn pawn in mapParent.Map.mapPawns.AllPawns)
                    {
                        if (pawn.GetOverseer() == myConscious && pawn.Faction == Faction.OfPlayer)
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(FormCaravanComp), "CanFormOrReformCaravanNow", MethodType.Getter)]
    public static class CanFormOrReformCaravanNow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(FormCaravanComp __instance, ref bool __result)
        {
            if (!__result)
            {
                MapParent mapParent = (MapParent)__instance.parent;
                var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
                var myConscious = mgr?.MechanoidConsciousness;

                if (mapParent.HasMap && !(__instance.Reform && __instance.AnyActiveThreatNow))
                {
                    foreach (Pawn pawn in mapParent.Map.mapPawns.AllPawns)
                    {
                        if (pawn.GetOverseer() == myConscious && pawn.Faction == Faction.OfPlayer)
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
        }
    }

}
