using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_MapPawn
{
    [HarmonyPatch(typeof(MapPawns), nameof(MapPawns.FreeHumanlikesSpawnedOfFaction))]
    public static class Patch_MapPawns_FreeHumanlikesSpawnedOfFaction
    {
        public static void Postfix(MapPawns __instance, Faction faction, ref List<Pawn> __result)
        {
            if (__instance == null || faction == null) return;

            // 1) 当前地图的殖民地机械族
            List<Pawn> mechs = __instance.SpawnedColonyMechs;
            if (mechs == null || mechs.Count == 0) return;

            // 2) 你的“群智”控制者 MC
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager
                .Instance?.MechanoidConsciousness;
            if (MC == null) return;

            // 3) 只要有至少一个殖民地机械的 Overeer == MC，则准备把 MC 放进结果清单
            bool anyOwnedByMC = false;
            List<Pawn> Mech = new List<Pawn>(); //麾下机械族

            for (int i = 0; i < mechs.Count; i++)
            {
                Pawn m = mechs[i];

                if (m == null) continue;

                try
                {
                    if (m?.GetOverseer() == MC)
                    {
                        anyOwnedByMC = true;
                        Mech.Add(m);
                    }
                }
                catch
                {
                    // 若扩展不存在或异常，安全退出
                }
            }
            if (!anyOwnedByMC) return;

            foreach (var m in Mech)
            {
                if (m.Faction == faction && !__result.Contains(m))
                {
                    __result.Add(m);
                }
            }

            //__result.AddRange(Mech);
            //__result.Add(MC);


        }
    }







    [HarmonyPatch(typeof(MapPawns), nameof(MapPawns.FreeHumanlikesOfFaction))]
    public static class Patch_MapPawns_FreeHumanlikesOfFaction
    {
        public static void Postfix(MapPawns __instance, Faction faction, ref List<Pawn> __result)
        {
            if (__instance == null || faction == null) return;

            // 1) 当前地图的殖民地机械族
            List<Pawn> mechs = __instance.SpawnedColonyMechs;
            if (mechs == null || mechs.Count == 0) return;

            // 2) 你的“群智”控制者 MC
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager
                .Instance?.MechanoidConsciousness;
            if (MC == null) return;

            // 3) 只要有至少一个殖民地机械的 Overeer == MC，则准备把 MC 放进结果清单
            bool anyOwnedByMC = false;
            List<Pawn> Mech = new List<Pawn>(); //麾下机械族

            for (int i = 0; i < mechs.Count; i++)
            {
                Pawn m = mechs[i];

                if (m == null) continue;

                try
                {
                    if (m?.GetOverseer() == MC)
                    {
                        anyOwnedByMC = true;
                        Mech.Add(m);
                    }
                }
                catch
                {
                    // 若扩展不存在或异常，安全退出
                }
            }
            if (!anyOwnedByMC) return;


            var SMB = AllasOne.WorldandGame.AAO_WorldComponent_ShowMechOnBar.Instance;


            foreach (var m in Mech)
            {
                if (m.Faction == faction && !__result.Contains(m) && SMB.thingNumber.Contains(m.thingIDNumber))
                {
                    __result.Add(m);
                }
            }

            //__result.AddRange(Mech);
            if (!__result.Contains(MC))
            { 
                __result.Add(MC); 
            }


        }
    }
}
