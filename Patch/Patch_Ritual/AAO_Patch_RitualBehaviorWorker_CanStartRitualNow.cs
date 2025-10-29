using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


namespace AllasOne.Patch.Patch_Ritual
{
    [HarmonyPatch(typeof(RitualBehaviorWorker), nameof(RitualBehaviorWorker.CanStartRitualNow))]
    public static class Patch_RitualBehaviorWorker_CanStartRitualNow
    {
        // 条件满足 -> 运行原版 (return true)
        // 条件不满足 -> 运行自定义逻辑 (设置 __result, return false)
        static bool Prefix(RitualBehaviorWorker __instance, ref string __result, TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn,Dictionary<string, Pawn> forcedForRole)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance;
            if (MC == null) return true;

            else 
            {
                if (target.IsValid && target.Map.Tile.Valid)
                {
                    PlanetLayerDef layerDef = target.Map.Tile.LayerDef;
                    if ((!ritual.layerWhitelist.NullOrEmpty() && !ritual.layerWhitelist.Contains(layerDef)) || (!ritual.layerBlacklist.NullOrEmpty() && ritual.layerBlacklist.Contains(layerDef)))
                    {
                        __result = "CantStartRitualLayer".Translate(ritual.Label.Named("RITUAL"), layerDef.gerundLabel.Named("GERUND"), layerDef.LabelCap.Named("LAYER")).CapitalizeFirst();
                    }
                }
                if (!ritual.allowOtherInstances)
                {
                    foreach (LordJob_Ritual activeRitual in Find.IdeoManager.GetActiveRituals(target.Map))
                    {
                        if (activeRitual.Ritual == ritual)
                        {
                            __result = "CantStartRitualAlreadyInProgress".Translate(ritual.Label).CapitalizeFirst();
                        }
                    }
                }
                if (selectedPawn != null && ritual.behavior?.def.roles != null)
                {
                    foreach (RitualRole role2 in ritual.behavior.def.roles)
                    {
                        if (role2.defaultForSelectedColonist && !role2.AppliesToPawn(selectedPawn, out var reason, target, null, null, ritual))
                        {
                            if (reason.NullOrEmpty())
                            {
                                __result = "CantStartRitualSelectedPawnCannotBeRole".Translate(selectedPawn.Named("PAWN"), role2.Label.Named("ROLE")).CapitalizeFirst();
                            }
                            __result = reason;
                        }
                    }
                }
                if (target.IsValid)
                {
                    List<Pawn> list = target.Map.mapPawns.FreeColonistsAndPrisonersSpawned.ToList();

                    bool AnyMCMech =false;

                    foreach (Pawn pawn in list)
                    {
                        if (pawn.IsColonyMech && pawn.GetOverseer() == MC?.MechanoidConsciousness)
                        {
                            AnyMCMech = true;
                        }

                    }

                    list.AddRange(target.Map.mapPawns.SpawnedColonyAnimals);

                    if (!ritual.behavior.def.roles.NullOrEmpty() && !AnyMCMech)
                    {
                        foreach (RitualRole role in ritual.behavior.def.roles)
                        {
                            if (!role.required || role.substitutable)
                            {
                                continue;
                            }
                            IEnumerable<RitualRole> source = ((role.mergeId == null) ? Gen.YieldSingle(role) : ritual.behavior.def.roles.Where((RitualRole r) => r.mergeId == role.mergeId));
                            if (list.Count((Pawn p) => role.AppliesToPawn(p, out var _, target, null, null, null, skipReason: true)) < source.Count() && (forcedForRole == null || !forcedForRole.ContainsKey(role.id)))
                            {
                                Precept precept = ritual.ideo.PreceptsListForReading.FirstOrDefault((Precept p) => p.def == role.precept);
                                if (precept != null)
                                {
                                    __result = "MessageNeedAssignedRoleToBeginRitual".Translate(role.missingDesc ?? Find.ActiveLanguageWorker.WithIndefiniteArticle(precept.LabelCap), ritual.Label);
                                }
                                if (!role.noCandidatesGizmoDesc.NullOrEmpty())
                                {
                                    __result = role.noCandidatesGizmoDesc;
                                }
                                if (source.Count() == 1)
                                {
                                    __result = "MessageNoRequiredRolePawnToBeginRitual".Translate(role.missingDesc ?? Find.ActiveLanguageWorker.WithIndefiniteArticle(role.Label), ritual.Label);
                                }
                                __result = "MessageNoRequiredRolePawnToBeginRitual".Translate(source.Count() + " " + (role.missingDesc ?? Find.ActiveLanguageWorker.Pluralize(role.Label)), ritual.Label);
                            }
                        }
                    }
                }
                __result = null;
                //Log.Message("AAO Patch Replace CanStartRitualNow");
                return false;
            }
        }
    }


}
