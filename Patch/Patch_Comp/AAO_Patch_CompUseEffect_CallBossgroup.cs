using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AllasOne.Patch.Patch_Comp
{
    [HarmonyPatch(typeof(CompUseEffect_CallBossgroup), nameof(CompUseEffect_CallBossgroup.CanBeUsedBy))]
    public static class AAO_Patch_CanBeUsedBy_Prefix
    {
        public static bool Prefix(CompUseEffect_CallBossgroup __instance, Pawn p, ref AcceptanceReport __result)
        {
            Pawn MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !p.IsColonyMech || p?.GetOverseer() != MC)
            {
                return true;
            }

            __result = __instance.Props.bossgroupDef.Worker.CanResolve(p);
            return false;


        }
    }
}
