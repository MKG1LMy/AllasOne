using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Misc
{
    [HarmonyPatch(typeof(HistoryAutoRecorderWorker_ColonistMood), "PullRecord")]
    public static class HistoryAutoRecorderWorker_ColonistMood_Prefix
    {

        public static bool Prefix(HistoryAutoRecorderWorker_ColonistMood __instance, ref float __result)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null)
            {
                return true; // 继续执行原方法
            }


            List<Pawn> allMaps_FreeColonists = PawnsFinder.AllMaps_FreeColonists;
            if (!allMaps_FreeColonists.Any())
            {
                __result = 0f;
            }
            __result = allMaps_FreeColonists.Where((Pawn x) => !x.RaceProps.IsMechanoid && x.needs.mood != null ).Average((Pawn x) => x.needs.mood.CurLevel * 100f);


            return false;
        }
    }
}
