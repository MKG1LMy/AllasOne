using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Shuttle
{
    [HarmonyPatch(typeof(TransportersArrivalActionUtility), nameof(TransportersArrivalActionUtility.AnyNonDownedColonist))]
    public static class TransportersArrivalActionUtility_AnyNonDownedColonist_Patch
    {
        static bool Prefix(IEnumerable<IThingHolder> pods, ref bool __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null)
            {
                return true;
            }
            if (pods == null) return true;

            foreach (IThingHolder pod in pods)
            {
                if (pod == null) continue;
                var directlyHeldThings = pod.GetDirectlyHeldThings();
                if (directlyHeldThings == null) continue;

                for (int i = 0; i < directlyHeldThings.Count; i++)
                {
                    if (directlyHeldThings[i] is Pawn pawn && pawn.IsColonyMech && pawn?.GetOverseer() == MC)
                    {
                        __result = true;                       
                        return false; // 已满足条件，跳过原方法
                    }
                }
            }

            return true; // 不满足，放行原方法执行
        }
    }
}
