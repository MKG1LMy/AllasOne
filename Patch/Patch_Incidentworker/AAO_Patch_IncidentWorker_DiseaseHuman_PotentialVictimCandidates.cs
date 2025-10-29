using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Incidentworker
{
    [HarmonyPatch(typeof(IncidentWorker_DiseaseHuman))]
    [HarmonyPatch("PotentialVictimCandidates")]
    [HarmonyPatch(new Type[] { typeof(IIncidentTarget) })]
    public class Patch_IncidentWorker_DiseaseHuman_PotentialVictimCandidates_Postfix
    {
        
        // Postfix 修改原方法返回的 IEnumerable<Pawn>
        public static void Postfix(IIncidentTarget target, ref IEnumerable<Pawn> __result)
        {
            if (__result == null) return;
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null || MC.MechanoidConsciousness == null) return;

            __result = __result.Where(pawn =>  pawn != MC.MechanoidConsciousness );
        }



    }
}
