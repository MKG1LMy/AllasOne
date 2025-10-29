using AllasOne.WorldandGame;
using Fortified;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Patch_OtherMod
{
    [StaticConstructorOnStartup]
    static class AAO_Patch_WeaponUsableMech_SpawnSetup_Runtime
    {
        private const string TARGET_PACKAGE_ID = "aoba.framework";
        static AAO_Patch_WeaponUsableMech_SpawnSetup_Runtime()
        {
            try
            {
                bool modActive = ModsConfig.ActiveModsInLoadOrder.Any(m => string.Equals(m.PackageId, TARGET_PACKAGE_ID, StringComparison.OrdinalIgnoreCase));

                // 尝试通过类型名找到目标类型（runtime，不会在编译期要求类型存在）
                Type targetType = AccessTools.TypeByName("WeaponUsableMech") ?? AccessTools.TypeByName("Fortified.WeaponUsableMech") ?? AccessTools.TypeByName("Fortified.WeaponUsableMech, Fortified");

                if (!modActive && targetType == null)
                {
                    Log.Message("[AAO] aoba.framework not active and WeaponUsableMech type not found. Skipping patch.");
                    return;
                }

                if (targetType == null)
                {
                    Log.Warning("[AAO] aoba.framework reported active but WeaponUsableMech type not found. Skipping patch.");
                    return;
                }

                // 找到 SpawnSetup 方法（Map, bool）
                MethodInfo targetMethod = targetType.GetMethod("SpawnSetup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(Map), typeof(bool) }, null);
                if (targetMethod == null)
                {
                    Log.Warning("[AAO] WeaponUsableMech.SpawnSetup(Map,bool) not found. Skipping patch.");
                    return;
                }

                var harmony = new Harmony("allasone.patch.weaponusablemech.spawnsetup");
                MethodInfo prefix = typeof(AAO_Patch_WeaponUsableMech_SpawnSetup_Runtime).GetMethod(nameof(Prefix_Safe), BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo postfix = typeof(AAO_Patch_WeaponUsableMech_SpawnSetup_Runtime).GetMethod(nameof(Postfix_Safe), BindingFlags.Static | BindingFlags.NonPublic);
                harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix));

                Log.Message("[AAO] Patched WeaponUsableMech.SpawnSetup successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("[AAO] Failed to initialize WeaponUsableMech patch: " + ex);
            }
        }

        private static Dictionary<string, int> MCskills;

        static void Prefix_Safe(object __instance, Map map, bool respawningAfterLoad)
        {
            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness as Pawn;
            if (MC == null) return;

            MCskills = new Dictionary<string, int>(MC.skills.skills.Count);

            foreach (var skill in MC.skills.skills)
            {
                MCskills[skill.def.defName] = skill.Level;
            }
            Log.Message("[AAO] Saved MC skills before WeaponUsableMech.SpawnSetup.");

        }

        static void Postfix_Safe(object __instance, Map map, bool respawningAfterLoad)
        {
            var MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness as Pawn;
            if (MC == null) return;

            foreach (var skill in MC.skills.skills)
            {
                if (MCskills.TryGetValue(skill.def.defName, out int level))
                {
                    skill.Level = level;
                }

            }
            Log.Message("[AAO] Restored MC skills after WeaponUsableMech.SpawnSetup.");
        }
    }
}
