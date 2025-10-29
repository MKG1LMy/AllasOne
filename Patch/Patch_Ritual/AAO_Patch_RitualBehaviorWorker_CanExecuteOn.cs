using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(RitualBehaviorWorker), nameof(RitualBehaviorWorker.CanExecuteOn))]
    public static class Patch_RitualBehaviorWorker_CanExecuteOn
    {
        // 条件满足 -> 运行原版 (return true)
        // 条件不满足 -> 运行自定义逻辑 (设置 __result, return false)
        static bool Prefix(RitualBehaviorWorker __instance, ref bool __result, TargetInfo target, RitualObligation obligation)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null)
            {
                return true;
            }

            else
            {
                __result = true;
                Log.Message("AAO Patch Replace CanExecuteOn result");
                return false;
            }
        }
    }
}
