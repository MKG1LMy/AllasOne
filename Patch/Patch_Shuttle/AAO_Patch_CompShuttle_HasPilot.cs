using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Shuttle
{
    [HarmonyPatch(typeof(CompShuttle), "get_HasPilot")]
    public static class Patch_CompShuttle_HasPilot
    {
        public static bool Prefix(CompShuttle __instance, ref bool __result)
        {
            try
            {
                var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
                if (MC != null)
                {
                    __result = true;
                    return false;
                }

                // 否则继续执行原逻辑
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error($"[AAO][HasPilot Patch] Exception: {ex}");
                return true; // 出错时放行原版
            }
        }


    }
}
