using AllasOne.HediffComp;
using AllasOne.WorldandGame;   // 你的 WorldComponent 命名空间
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AllasOne.Patch
{
    [HarmonyPatch(typeof(ColonistBar), "CheckRecacheEntries")]
    public static class Patch_CheckRecacheEntries
    {
        public static void Postfix(ColonistBar __instance)
        {
            var sp = FindMechanoidConsciousness();
            if (sp == null) return;

            //if (Find.Maps == null || Find.CurrentMap ==null) return;

            var fEntries = AccessTools.Field(typeof(ColonistBar), "cachedEntries");
            var fGroups = AccessTools.Field(typeof(ColonistBar), "cachedReorderableGroups");
            var fDrawer = AccessTools.Field(typeof(ColonistBar), "drawLocsFinder");
            var fCachedDrawLocs = AccessTools.Field(typeof(ColonistBar), "cachedDrawLocs");
            var fCachedScale = AccessTools.Field(typeof(ColonistBar), "cachedScale");


            var entries = (List<ColonistBar.Entry>)fEntries.GetValue(__instance);
            var groups = (List<int>)fGroups.GetValue(__instance);
            var drawer = (ColonistBarDrawLocsFinder)fDrawer.GetValue(__instance);
            var cachedDrawLocs = (List<Vector2>)fCachedDrawLocs.GetValue(__instance);
            var cachedScale = (float)fCachedScale.GetValue(__instance);

            if (entries == null || groups == null) return;

            // 已在列表则不重复
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].pawn == sp) return;

            // 目标组仍放到最后一组，保持最小侵入
             int num = entries.Count > 0 ? entries[entries.Count-1].group : 0;

            if (Find.Maps == null || Find.CurrentMap == null)
            {
                var newEntry = new ColonistBar.Entry(sp, null, 0);
                entries.Add(newEntry);
                Log.Warning("AAO ColonistBar Add MechanoidConsciousness with null maps");
            }
            else
            {
                var newEntry = new ColonistBar.Entry(sp, Find.CurrentMap, 0);
                entries.Add(newEntry);
            }

            // 占位，保持长度对齐
            groups.Add(-1);

            __instance.drawer.Notify_RecachedEntries();
            //drawer.CalculateDrawLocs(cachedDrawLocs, out cachedScale, num);


        }

        private static Pawn FindMechanoidConsciousness()
        {
            // 先取你在 WorldComponent 里保存的引用
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (mgr?.MechanoidConsciousness != null) return mgr.MechanoidConsciousness;

            // 兜底扫描（可留可去）
            //var hdef = DefDatabase<HediffDef>.GetNamedSilentFail("AAO_Hediff_MechanoidConsciousnessNode");
            //if (hdef == null) return null;
            //foreach (var p in Find.WorldPawns.AllPawnsAlive)
            //{
            //    if (p?.Faction == null) continue;
            //    if (p.HostileTo(Faction.OfPlayer)) continue;
            //    if (p.health?.hediffSet?.HasHediff(hdef) == true) return p;
            //}
            return null;
        }
    }
}
