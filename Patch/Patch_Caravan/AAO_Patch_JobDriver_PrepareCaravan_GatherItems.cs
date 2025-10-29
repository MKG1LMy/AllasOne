using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld.Planet;


namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(JobDriver_PrepareCaravan_GatherItems), "IsUsableCarrier")]
    public static class IsUsableCarrier_Patch
    {
        public static bool Prefix(Pawn p, Pawn forPawn, bool allowColonists, ref bool __result)
        {
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;
            if (p.GetOverseer() == myConscious)
            {
                __result = true;
                if (!p.IsFormingCaravan())
                {
                    __result = false;

                }
                if (p == forPawn)
                {
                    __result = true;
                }
                if (p.DestroyedOrNull() || !p.Spawned || p.inventory.UnloadEverything || !forPawn.CanReach(p, PathEndMode.Touch, Danger.Deadly))
                {
                    __result = false;
                }
                Log.Message($"IsUsableCarrier have change by MechanoidConsciousness");
                return __result;
            }

            else
            {
                // 执行原方法
                return __result;
            }
        }
    }
}
