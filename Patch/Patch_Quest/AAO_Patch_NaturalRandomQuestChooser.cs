using AllasOne.WorldandGame;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace AllasOne.Patch.Patch_Quest
{
    [HarmonyPatch(typeof(NaturalRandomQuestChooser), nameof(NaturalRandomQuestChooser.ChooseNaturalRandomQuest))]
    public static class AAO_Patch_NaturalRandomQuestChooser
    {
        public static void Postfix(ref QuestScriptDef __result, float points, IIncidentTarget target)
        {
            if (__result.defName == "OpportunitySite_PrisonerWillingToJoin" && __result.root is QuestNode_Sequence QN)
            {
                QuestNode_AddPawnReward QN_apr = QN.nodes.OfType<QuestNode_AddPawnReward>().FirstOrDefault();
                if (QN_apr == null) return;
                Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
                if (QN_apr.pawn == MC)
                {
                    __result = NaturalRandomQuestChooser.ChooseNaturalRandomQuest(points,target);
                }
                Log.Message($"[AAO][NaturalRandomQuestChooser] Chose QuestScriptDef: {__result.defName}");
            }
        }
    }




    [HarmonyPatch(typeof(IncidentWorker_GiveQuest), "CanFireNowSub")]
    public static class IncidentWorker_GiveQuest_CanFireNowSub_Patch
    {
        // Prefix 在原方法执行前跑。返回 true 则执行原方法，返回 false 则跳过原方法（并使用 __result）。
        public static bool Prefix(IncidentWorker_GiveQuest __instance, IncidentParms parms, ref bool __result)
        {
            // 安全检查
            if (parms == null) return true;

            // 计算要判断的 QuestScriptDef（与原方法一致的方式）
            QuestScriptDef questScriptDef = __instance.def.questScriptDef ?? parms.questScriptDef;
            if (questScriptDef == null) return true;

            if (questScriptDef.defName == "OpportunitySite_PrisonerWillingToJoin" && questScriptDef.root is QuestNode_Sequence QN)
            {
                QuestNode_AddPawnReward QN_apr = QN.nodes.OfType<QuestNode_AddPawnReward>().FirstOrDefault();
                if (QN_apr == null) return true;
                Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
                if (QN_apr.pawn == MC)
                {
                    __result = false;
                }
                Log.Message($"[AAO][GiveQuest CanFireNowSub Patch] QuestScriptDef: {questScriptDef.defName}, CanFireNowSub: {__result}");
                return false;
            }

            return true; // 继续执行原方法
        }
    }
}
