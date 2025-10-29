using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Alert
{
    [HarmonyPatch(typeof(Alert_BrawlerHasRangedWeapon), "get_BrawlersWithRangedWeapon")]
    public static class Patch_Alert_BrawlerHasRangedWeapon_Filter
    {
        public static bool Prefix(Alert_BrawlerHasRangedWeapon __instance, ref List<Pawn> __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null) return true; // 放行原版


            var field = AccessTools.Field(typeof(Alert_BrawlerHasRangedWeapon), "brawlersWithRangedWeaponResult");
            var list = (List<Pawn>)field.GetValue(__instance);
            list.Clear();

            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                // 自定义提前跳出条件
                if (!ShouldCheckPawn(pawn))
                    continue;

                if (pawn.story.traits.HasTrait(TraitDefOf.Brawler)
                    && pawn.equipment.Primary != null
                    && pawn.equipment.Primary.def.IsRangedWeapon)
                {
                    list.Add(pawn);
                }
            }

            __result = list;
            return false; // 跳过原 getter
        }

        // 自定义检测逻辑，只改这里
        static bool ShouldCheckPawn(Pawn p)
        {
            if (p.RaceProps.IsMechanoid)
            {
                return false; // 机械人跳过
            }

            return true; // 默认全部检查
        }
    }
}
