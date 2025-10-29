using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Misc
{
    [HarmonyPatch(typeof(QualityUtility))]
    [HarmonyPatch(nameof(QualityUtility.GenerateQualityCreatedByPawn))]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(SkillDef), typeof(bool) })]
    public static class QualityUtility_GenerateQualityCreatedByPawn_Patch
    {
        public static bool Prefix(Pawn pawn, SkillDef relevantSkill, bool consumeInspiration, ref QualityCategory __result)
        {
            Pawn MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null || !pawn.RaceProps.IsMechanoid || !pawn.IsColonyMech || pawn.GetOverseer() != MC)
            {
                return true;
            }

            
            int relevantSkillLevel = pawn.skills.GetSkill(relevantSkill).Level;
            Log.Message("GenerateQualityCreatedByPawn Patched for Mechanoid: " + pawn.LabelCap +" pawn.skills = " + relevantSkillLevel);
            bool flag = consumeInspiration && pawn?.InspirationDef == InspirationDefOf.Inspired_Creativity;
            QualityCategory qualityCategory = QualityUtility .GenerateQualityCreatedByPawn(relevantSkillLevel, flag);
            if (ModsConfig.IdeologyActive && pawn?.Ideo != null)
            {
                Precept_Role role = pawn?.Ideo?.GetRole(pawn);
                if (role != null && role?.def.roleEffects != null)
                {
                    RoleEffect roleEffect = role?.def.roleEffects.FirstOrDefault((RoleEffect eff) => eff is RoleEffect_ProductionQualityOffset);
                    if (roleEffect != null)
                    {
                        qualityCategory = AddLevels(qualityCategory, ((RoleEffect_ProductionQualityOffset)roleEffect).offset);
                    }
                }
            }
            if (flag)
            {
                pawn?.mindState?.inspirationHandler.EndInspiration(InspirationDefOf.Inspired_Creativity);
            }

            __result = qualityCategory;
            return false;

        }

        private static QualityCategory AddLevels(QualityCategory quality, int levels)
        {
            return (QualityCategory)Mathf.Min((int)quality + levels, 6);
        }

    }
}
