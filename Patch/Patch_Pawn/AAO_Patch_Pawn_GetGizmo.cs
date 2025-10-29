using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Misc
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    static class Patch_Pawn_GetGizmos_Postfix
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            // 1) 先转发原始 gizmos（延迟枚举，安全）
            if (__result != null)
            {
                using (var enumerator = __result.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }

            // 2) 基本空值检查与缓存，避免重复调用 GetOverseer(),获取机械师的Gizmos
            if (__instance == null) yield break;

            var mc = WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (mc == null) yield break;

            if (!__instance.RaceProps.IsMechanoid) yield break;

            var overseer = __instance.GetOverseer();
            if (overseer == null) yield break;

            // 只在该机械的 overseer 正好是 MC 时插入 gizmos
            if (!ReferenceEquals(overseer, mc)) yield break;

            // 取 mechanitor 并防空
            var mechanitor = overseer.mechanitor;
            if (mechanitor == null) yield break;

            foreach (Gizmo giz in mechanitor.GetGizmos())
            {
                yield return giz;
            }







        }
    }
}
