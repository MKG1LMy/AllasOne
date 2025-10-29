using AllasOne.WorldandGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllasOne.Patch.Patch_MechanitorUtility
{
    [HarmonyPatch(typeof(MechanitorUtility))]
    [HarmonyPatch(nameof(MechanitorUtility.AnyMechanitorInPlayerFaction))]
    public static class AAO_Patch_MechanitorUtility_AnyMechanitorInPlayerFaction
    {
        public static bool Prefix(ref bool __result)
        {

            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null)
            { 
                return true; 
            }
            else
            {
                __result = true; // 只要有 MC 就视为有“我方机仆”
                return false; // 跳过原方法执行
            }


        }
    }
}
