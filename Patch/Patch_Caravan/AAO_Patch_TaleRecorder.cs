using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
    public static class TaleRecorder_Patch
    {
        public static bool Prefix(TaleDef def, ref object[] args)
        {
            if (def == TaleDefOf.CaravanAmbushDefeated)
            {
                Map map = args[0] as Map;
                if (map == null || map.mapPawns.FreeColonists == null || !map.mapPawns.FreeColonists.Any())
                {
                    return false; // 如果为null或者空，阻止原方法的执行
                }
                return true;

            }
            else { return true; }

        }
    }
}
