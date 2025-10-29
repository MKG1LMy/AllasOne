using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static HarmonyLib.Code;

namespace AllasOne.Patch.Patch_Comp
{
    [HarmonyPatch(typeof(CompUsable))]
    [HarmonyPatch("CanBeUsedBy")]
    static class Patch_CompUsable_CanBeUsedBy_Prefix
    {
        public static bool Prefix(CompUsable __instance, Pawn p, ref AcceptanceReport __result, bool forced = false, bool ignoreReserveAndReachable = false)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || p?.GetOverseer() == null || p?.GetOverseer() != MC)
            {
                return true;
            }

            Pawn overseer = p?.GetOverseer();

            __result = true;

            if (overseer.IsMutant && !__instance.Props.allowedMutants.Contains(overseer.mutant.Def))
            {
                __result = false;
            }

            PlanetTile tile = p.MapHeld.Tile;
            if (tile.Valid && !__instance.Props.layerWhitelist.NullOrEmpty() && !__instance.Props.layerWhitelist.Contains(tile.LayerDef))
            {
                __result =  "CannotPerformPlanetLayer".Translate(tile.LayerDef.gerundLabel.Named("GERUND"), tile.LayerDef.label.Named("LAYER")).Resolve();
            }
            if (tile.Valid && !__instance.Props.layerBlacklist.NullOrEmpty() && __instance.Props.layerBlacklist.Contains(tile.LayerDef))
            {
                __result = "CannotPerformPlanetLayer".Translate(tile.LayerDef.gerundLabel.Named("GERUND"), tile.LayerDef.label.Named("LAYER")).Resolve();
            }
            if (__instance.parent.TryGetComp<CompPowerTrader>(out var comp) && !comp.PowerOn)
            {
                __result = "NoPower".Translate();
            }
            if (!ignoreReserveAndReachable && !p.CanReach(__instance.parent, PathEndMode.Touch, Danger.Deadly))
            {
                __result = "NoPath".Translate();
            }
            if (!ignoreReserveAndReachable && !p.CanReserve(__instance.parent, 1, -1, null, forced))
            {
                Pawn pawn = p.Map.reservationManager.FirstRespectedReserver(__instance.parent, p) ?? p.Map.physicalInteractionReservationManager.FirstReserverOf(__instance.parent);
                if (pawn != null)
                {
                    __result = "ReservedBy".Translate(pawn.LabelShort, pawn);
                }
                __result = "Reserved".Translate();
            }


            if (__instance.Props.userMustHaveHediff != null && !overseer.health.hediffSet.HasHediff(__instance.Props.userMustHaveHediff))
            {
                __result = "MustHaveHediff".Translate(__instance.Props.userMustHaveHediff);
            }

            List<ThingComp> allComps = __instance.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                if (allComps[i] is CompUseEffect compUseEffect)
                {
                    AcceptanceReport result = compUseEffect.CanBeUsedBy(p);
                    if (!result.Accepted)
                    {
                        __result = result;
                    }
                }
            }


            return false;

        }
    }
}
