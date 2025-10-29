using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_MainTabWindow
{
    [HarmonyPatch(typeof(MainTabWindow_Work))]
    [HarmonyPatch("Pawns", MethodType.Getter)]  // 明确指定是属性的 getter
    public static class Patch_MainTabWindow_Work_Pawns
    {
        public static void Postfix(ref IEnumerable<Pawn> __result)
        {
            // 先拿到原结果
            var list = __result.ToList();


            __result = list.Where(pawn =>
                !pawn.DevelopmentalStage.Baby() && !pawn.RaceProps.IsMechanoid
            );


        }
    }

    [HarmonyPatch(typeof(MainTabWindow_Schedule))]
    [HarmonyPatch("Pawns", MethodType.Getter)]  // 明确指定是属性的 getter
    public static class Patch_MainTabWindow_Schedule_Pawns
    {
        public static void Postfix(ref IEnumerable<Pawn> __result)
        {
            // 先拿到原结果
            var list = __result.ToList();


            __result = list.Where(pawn =>
                !pawn.RaceProps.IsMechanoid
            );


        }
    }

    [HarmonyPatch(typeof(MainTabWindow_Assign))]
    [HarmonyPatch("Pawns", MethodType.Getter)]  // 明确指定是属性的 getter
    public static class Patch_MainTabWindow_Assign_Pawns
    {
        public static void Postfix(ref IEnumerable<Pawn> __result)
        {
            // 先拿到原结果
            var list = __result.ToList();


            __result = list.Where(pawn =>
                !pawn.RaceProps.IsMechanoid
            );


        }
    }




}
