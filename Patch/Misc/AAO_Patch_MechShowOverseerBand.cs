using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch
{
    // —— 转发器：独立实例，内部转发到原 bandwidthGizmo —— 
    public class Gizmo_ForwardOverseerBandwidth : Gizmo
    {
        public Pawn Overseer;
        public Gizmo Source; // overseer.mechanitor 的原 gizmo

        public Gizmo_ForwardOverseerBandwidth(Pawn overseer, Gizmo source)
        {
            Overseer = overseer;
            Source = source;
            Order = -100f; // 置顶，按需调整
        }

        public override float GetWidth(float maxWidth) => Source?.GetWidth(maxWidth) ?? 140f;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
            => Source != null ? Source.GizmoOnGUI(topLeft, maxWidth, parms) : new GizmoResult(GizmoState.Clear);
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class Patch_Pawn_GetGizmos_Mech_ShowOverseerBandwidth
    {
        // 缓存反射：取 mechanitor 私有字段 bandwidthGizmo
        private static readonly FieldInfo BwField =
            AccessTools.Field(AccessTools.TypeByName("RimWorld.Pawn_MechanitorTracker"), "bandwidthGizmo");

        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            var list = __result.ToList();

            // 仅玩家机仆
            if (__instance?.Faction == Faction.OfPlayer && (__instance.RaceProps?.IsMechanoid ?? false))
            {
                var overseer = __instance.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer);
                var tracker = overseer?.mechanitor;

                if (tracker != null)
                {
                    // —— 只让“同一 overseer 组”里的第一个机仆添加，避免重复 —— 
                    var sel = Find.Selector?.SelectedObjects;
                    if (sel != null)
                    {
                        Pawn firstWithThisOverseer = null;
                        foreach (var o in sel)
                        {
                            if (o is Pawn p
                                && p.Faction == Faction.OfPlayer
                                && (p.RaceProps?.IsMechanoid ?? false)
                                && p.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer) == overseer)
                            {
                                firstWithThisOverseer = p;
                                break;
                            }
                        }
                        if (firstWithThisOverseer != null && firstWithThisOverseer != __instance)
                            goto RETURN_ALL; // 这只机仆不再添加，交给“该组第一个”去显示
                    }

                    // —— 懒加载：枚举一次确保私有 gizmo 已创建 —— 
                    var trackerGizmos = tracker.GetGizmos().ToList();

                    // —— 取私有 bandwidthGizmo；失败则按名字兜底 —— 
                    var src = (BwField?.GetValue(tracker) as Gizmo)
                              ?? trackerGizmos.FirstOrDefault(z =>
                                   z?.GetType()?.Name?.IndexOf("Bandwidth", System.StringComparison.OrdinalIgnoreCase) >= 0);

                    if (src != null)
                    {
                        // 关键点：不要把 src 直接塞进 list（那是同一个实例），用“转发器”包一层
                        var forwarded = new Gizmo_ForwardOverseerBandwidth(overseer, src);
                        list.Insert(0, forwarded); // 置顶，想原顺序就改成 list.Add(forwarded);
                    }
                }
            }

        RETURN_ALL:
            foreach (var g in list) yield return g;
        }
    }
}
