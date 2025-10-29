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


namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(Dialog_BeginLordJob), "get_CanBegin")]
    static class Patch_DialogBeginLordJob_get_CanBegin
    {
        public static bool Prefix(Dialog_BeginLordJob __instance, ref bool __result)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            // 你的条件

            if  (MC != null)
            {
                __result = true;   // 或 false
                return false;      // 跳过原版 getter
            }
            return true;           // 走原版
        }


    }


    [HarmonyPatch]
    static class Patch_DialogBeginLordJob_BlockingIssues_All //不论什么仪式都表示咱们能做！
    {
        // 统一枚举基类与全部子类的同名方法
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var baseType = typeof(Dialog_BeginLordJob);
            yield return AccessTools.Method(baseType, "BlockingIssues", Type.EmptyTypes);

            foreach (var t in AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(t => t.IsSubclassOf(baseType)))
            {
                var m = AccessTools.Method(t, "BlockingIssues", Type.EmptyTypes);
                if (m != null) yield return m;
            }
        }

        public static bool Prefix(object __instance, ref IEnumerable<string> __result)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (__instance is Dialog_BeginLordJob inst && MC !=null )
            {
                var msg = "AAO_I Have Connected in. We Can Do!".Translate().ToString(); 
                __result = new[] { msg };
                return false; // 跳过该类的原实现
            }
            return true;
        }


    }
}
