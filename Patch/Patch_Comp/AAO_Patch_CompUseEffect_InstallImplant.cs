using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace AllasOne.Patch.Patch_Comp
{
    [HarmonyPatch(typeof(CompUseEffect_InstallImplant))]
    [HarmonyPatch("CanBeUsedBy")]
    public static class Patch_CompUseEffect_InstallImplant_CanBeUsedBy
    {
        public static bool Prefix(CompUseEffect_InstallImplant __instance, Pawn p, ref AcceptanceReport __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || p?.GetOverseer() == null ||p?.GetOverseer() != MC)
            {
                return true;
            }


            __result = true;
            Pawn overseer = p.GetOverseer();

            Hediff existingImplant = __instance.GetExistingImplant(overseer);
            if (__instance.Props.requiresExistingHediff && existingImplant == null)
            {
                __result = "InstallImplantHediffRequired".Translate(__instance.Props.hediffDef.label);
            }
            if (existingImplant != null)
            {
                if (!__instance.Props.canUpgrade)
                {
                    __result = "InstallImplantAlreadyInstalled".Translate();
                }
                Hediff_Level hediff_Level = (Hediff_Level)existingImplant;
                if ((float)hediff_Level.level >= hediff_Level.def.maxSeverity)
                {
                    __result = "InstallImplantAlreadyMaxLevel".Translate();
                }
                if (__instance.Props.maxSeverity <= (float)hediff_Level.level)
                {
                    __result = string.Concat("InstallImplantAlreadyMaxLevel".Translate() + " ", __instance.Props.maxSeverity.ToString());
                }
                if (__instance.Props.minSeverity > (float)hediff_Level.level)
                {
                    __result = "InstallImplantMinLevel".Translate(__instance.Props.minSeverity);
                }
            }

            return false;

        }
    }




    [HarmonyPatch(typeof(CompUseEffect_InstallImplant))]
    [HarmonyPatch("DoEffect")]
    public static class Patch_CompUseEffect_InstallImplant_DoEffect_Prefix
    {
        public static bool Prefix(CompUseEffect_InstallImplant __instance, Pawn user)
        { 
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || user?.GetOverseer() == null || user?.GetOverseer() != MC)
            {
                return true;
            }

            Pawn overseer = user.GetOverseer();

            BodyPartRecord bodyPartRecord = overseer.RaceProps.body.GetPartsWithDef(__instance.Props.bodyPart).FirstOrFallback();
            Hediff firstHediffOfDef = overseer.health.hediffSet.GetFirstHediffOfDef(__instance.Props.hediffDef);

            if (firstHediffOfDef == null && !__instance.Props.requiresExistingHediff)
            {
                overseer.health.AddHediff(__instance.Props.hediffDef, bodyPartRecord);
            }
            else if (__instance.Props.canUpgrade)
            {
                ((Hediff_Level)firstHediffOfDef).ChangeLevel(1);
            }
            return false;
        }
    }





}
