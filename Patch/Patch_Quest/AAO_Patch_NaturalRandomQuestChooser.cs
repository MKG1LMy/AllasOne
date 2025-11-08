using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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
}
