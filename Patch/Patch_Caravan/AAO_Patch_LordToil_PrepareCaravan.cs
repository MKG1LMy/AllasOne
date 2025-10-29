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

namespace AllasOne.Patch.Patch_Caravan
{
    //拿东西
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherItems), "UpdateAllDuties")]
    public static class UpdateAllDuties_Patch
    {
        public static void Postfix(LordToil_PrepareCaravan_GatherItems __instance)
        {
            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;
            // 循环所有pawn
            for (int i = 0; i < __instance.lord.ownedPawns.Count; i++)
            {
                Pawn pawn = __instance.lord.ownedPawns[i];
                // 如果有commander则将其mindstate改成iscolonist需要执行的
                if (pawn.GetOverseer() == myConscious)
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_GatherItems);
                }
            }
        }
    }

    //带人
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherDownedPawns), "UpdateAllDuties")]
    public static class UpdateAllDuties_GatherDownedPawns_Patch
    {
        public static void Postfix(LordToil_PrepareCaravan_GatherDownedPawns __instance)
        {
            // 使用AccessTools获取保护字段的值
            var meetingPointField = AccessTools.Field(typeof(LordToil_PrepareCaravan_GatherDownedPawns), "meetingPoint");
            IntVec3 meetingPoint = (IntVec3)meetingPointField.GetValue(__instance);

            var exitSpotField = AccessTools.Field(typeof(LordToil_PrepareCaravan_GatherDownedPawns), "exitSpot");
            IntVec3 exitSpot = (IntVec3)exitSpotField.GetValue(__instance);

            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;

            // 循环所有pawn
            for (int i = 0; i < __instance.lord.ownedPawns.Count; i++)
            {
                Pawn pawn = __instance.lord.ownedPawns[i];
                // 如果有commander则将其mindstate改成iscolonist需要执行的
                if (pawn.GetOverseer() == myConscious)
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.PrepareCaravan_GatherDownedPawns, meetingPoint, exitSpot);
                }
            }
        }
    }

    //带动物
    [HarmonyPatch(typeof(LordToil_PrepareCaravan_RopeAnimals), "UpdateAllDuties")]
    public static class UpdateAllDuties_RopeAnimals_Patch
    {
        public static void Postfix(LordToil_PrepareCaravan_RopeAnimals __instance)
        {
            // 使用AccessTools获取受保护字段的值
            var ropeeLimitField = AccessTools.Field(typeof(LordToil_PrepareCaravan_RopeAnimals), "ropeeLimit");
            int? ropeeLimit = (int?)ropeeLimitField.GetValue(__instance);

            // 使用AccessTools获取受保护方法的值
            var makeRopeDutyMethod = AccessTools.Method(typeof(LordToil_PrepareCaravan_RopeAnimals), "MakeRopeDuty");
            PawnDuty makeRopeDuty = (PawnDuty)makeRopeDutyMethod.Invoke(__instance, null);

            var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            var myConscious = mgr?.MechanoidConsciousness;

            // 循环所有pawn
            for (int i = 0; i < __instance.lord.ownedPawns.Count; i++)
            {
                Pawn pawn = __instance.lord.ownedPawns[i];
                // 如果有commander则将其mindstate改成iscolonist需要执行的
                if (pawn.GetOverseer() == myConscious)
                {
                    pawn.mindState.duty = makeRopeDuty;
                    pawn.mindState.duty.ropeeLimit = ropeeLimit;
                }
            }
        }

        //计时器
        [HarmonyPatch(typeof(LordToil_PrepareCaravan_GatherItems), "LordToilTick")]
        public static class LordToilTick_Patch
        {
            public static bool Prefix(LordToil_PrepareCaravan_GatherItems __instance)
            {
                var mgr = AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
                var myConscious = mgr?.MechanoidConsciousness;
                //查找是否有commander
                for (int i = 0; i < __instance.lord.ownedPawns.Count; i++)
                {
                    Pawn pawn = __instance.lord.ownedPawns[i];
                    if (pawn.GetOverseer() == myConscious && pawn.mindState.lastJobTag != JobTag.WaitingForOthersToFinishGatheringItems)
                    {
                        return false;
                    }
                }



                return true; // 否则，继续执行原方法
            }
        }
    }
}
