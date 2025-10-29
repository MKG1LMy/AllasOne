using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;



namespace AllasOne.Patch.Patch_PawnGenerator
{
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GenerateTraitsFor))]
    public static class AAO_Patch_PawnGenerator_GenerateTraitsFor
    {
        public static bool Prefix(Pawn pawn, int traitCount, PawnGenerationRequest req , bool growthMomentTrait, ref List<Trait> __result)
        {
            if (pawn == null || pawn.kindDef == null || pawn.kindDef.backstoryFilters == null || !pawn.kindDef.backstoryFilters.Any()) 
            { 
                return true;
            } // 回退到原方法

            List<Trait> list = new List<Trait>();
            int num = 0;

            List<TraitDef> traits = DefDatabase<TraitDef>.AllDefsListForReading.Where( td => td.exclusionTags != null && pawn.kindDef.backstoryFilters.Any(filter => filter.categories != null && filter.categories.Any( cat => td.exclusionTags.Contains(cat) )  )).ToList();
            if (traits.NullOrEmpty()) 
            {
                return true; 
            }

            while (list.Count < traitCount && ++num < traitCount + 500)
            {
                TraitDef newTraitDef;
                if (!traits.TryRandomElement(out newTraitDef) || newTraitDef == null)
                { 
                    continue; 
                }
                if (pawn.story.traits.HasTrait(newTraitDef) || list.Any(tt => tt.def == newTraitDef))
                {
                    continue;
                }
                if (pawn.story.traits.allTraits.Any((Trait tr) => tr.def.conflictingTraits.Contains(newTraitDef)) || newTraitDef.conflictingTraits.Any(tr => pawn.story.traits.allTraits.Any(TR => TR.def == tr)))
                {
                    continue;
                }
                if (list.Any(existing => existing.def.conflictingTraits.Contains(newTraitDef) || newTraitDef.conflictingTraits.Contains(existing.def)))
                {
                    continue;
                }

                int degree;
                if (newTraitDef.degreeDatas == null || newTraitDef.degreeDatas.Count == 0)
                {
                    continue;
                }
                if (newTraitDef.degreeDatas.Count == 1)
                {
                    degree = newTraitDef.degreeDatas[0].degree;
                }
                else
                { 
                    degree = newTraitDef.degreeDatas.RandomElementByWeight((TraitDegreeData dd) => dd.commonality).degree; 
                }
                    
                Trait trait = new Trait(newTraitDef, degree);
                list.Add(trait);

            }
            if (num >= traitCount + 500)
            {
                Log.Warning($"Tried to generate {traitCount} traits for {pawn} over {500} extra times and failed.");
            }
            __result = list;
            Log.Message($"AAO Patch GenerateTraitsFor: set traits for {pawn} ");

            return false;
        }
    }
}
