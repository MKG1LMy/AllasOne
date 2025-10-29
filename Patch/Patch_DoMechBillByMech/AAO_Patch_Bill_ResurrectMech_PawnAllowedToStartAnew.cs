using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AllasOne.Patch.Patch_DoMechBillByMech
{
    [HarmonyPatch(typeof(Bill_ResurrectMech), "PawnAllowedToStartAnew")]
    static class Patch_Bill_ResurrectMech_PawnAllowedToStartAnew
    {
        // 委托类型：实例方法 (this, Pawn) -> bool
        private delegate bool BaseCallDelegate(Bill_ResurrectMech instance, Pawn pawn);
        private static BaseCallDelegate callBase; // 懒初始化

        // Prefix：我们完全接管。设置 __result 并返回 false 跳过原方法。
        public static bool Prefix(Bill_ResurrectMech __instance, Pawn p, ref bool __result)
        {
            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !p.RaceProps.IsMechanoid || p?.GetOverseer() != MC)
            {
                return true; 
            }


                // 如果需要基类行为，先确保委托已创建
                EnsureBaseDelegate();

            // 调用基类实现（非虚调用）
            bool baseOk = callBase(__instance, p);

            // 下面写你的自定义逻辑。示例：先用基类判断，不通过时按你自己的规则再试一次
            if (!baseOk)
            {
                __result = false;
                return false;
            }

            List<Corpse> corpses =new List<Corpse>();
            List<Thing> list = __instance.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            foreach (Corpse item in list)
            {
                if (__instance.recipe.fixedIngredientFilter.Allows(item) && !item.IsForbidden(p) && p.CanReach(item, PathEndMode.Touch, Danger.Deadly))
                {
                    corpses.Add(item);
                }
            }

            if (__instance.State == FormingState.Gathering && __instance.Gestator.ResurrectingMechCorpse == null)
            {
                
                if (corpses.Any() && !corpses.Any(c => p.GetOverseer().mechanitor.HasBandwidthToResurrect(c)))
                {
                    JobFailReason.Is("NotEnoughBandwidth".Translate());
                    __result = false;
                    return false;
                }
            }

            __result = true;
            return false; // 已完成，跳过原始方法
        }

        private static void EnsureBaseDelegate()
        {
            if (callBase != null) return;

            Type derived = typeof(Bill_ResurrectMech);
            Type baseType = derived.BaseType; // 假定基类里有 PawnAllowedToStartAnew
            if (baseType == null) throw new Exception("Cannot find base type for Bill_ResurrectMech");

            MethodInfo baseMethod = baseType.GetMethod("PawnAllowedToStartAnew",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (baseMethod == null) throw new Exception("Cannot find PawnAllowedToStartAnew on base type");

            // 创建 DynamicMethod，通过 call 指令直接调用基类实现（绕开虚调用）
            var dm = new DynamicMethod(
                $"__CallBase_{baseType.Name}_PawnAllowedToStartAnew",
                typeof(bool),
                new Type[] { typeof(Bill_ResurrectMech), typeof(Pawn) },
                typeof(Bill_ResurrectMech) // owner for access to non-public
            );

            ILGenerator il = dm.GetILGenerator();
            // load instance (arg0)
            il.Emit(OpCodes.Ldarg_0);
            // load pawn (arg1)
            il.Emit(OpCodes.Ldarg_1);
            // call base method non-virtually
            il.Emit(OpCodes.Call, baseMethod);
            il.Emit(OpCodes.Ret);

            callBase = (BaseCallDelegate)dm.CreateDelegate(typeof(BaseCallDelegate));
        }



  
    }
}
