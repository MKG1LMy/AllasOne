using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_MechanitorUtility
{
    [HarmonyPatch(typeof(MechanitorUtility), nameof(MechanitorUtility.IsMechanitor))]
    public static class Patch_MechanitorUtility_IsMechanitor
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result || pawn == null) return;

            // 你的特殊 Pawn（群智控制者）
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager
                .Instance?.MechanoidConsciousness;
            if (MC == null) return;

            // 仅当目标为机械族且其 Overseer 是 MC 时放行
            if (pawn.IsColonyMech == true && pawn.GetOverseer() == MC)
            {
                __result = true;
            }
        }
    }
}
