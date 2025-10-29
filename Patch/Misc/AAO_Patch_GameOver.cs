using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch
{
    [HarmonyPatch(typeof(GameEnder), nameof(GameEnder.CheckOrUpdateGameOver))]
    public static class Patch_CheckOrUpdateGameOver_Prefix
    {
        public static bool Prefix(GameEnder __instance)
        {
            if (AAO_GameOverBlocker.ShouldBlockGameOver())
            {
                __instance.gameEnding = false;
                return false; // 跳过原方法，相当于“提前 return”
            }
            return true; // 正常执行原方法
        }
    }

    public static class AAO_GameOverBlocker
    {
        public static bool ShouldBlockGameOver()
        {
            var MCM = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MCM != null) return true;

            else return false;
        }
    }
}
