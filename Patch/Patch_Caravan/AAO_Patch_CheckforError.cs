using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Patch_Caravan
{
    [HarmonyPatch(typeof(Dialog_FormCaravan))]
    [HarmonyPatch("CheckForErrors", new Type[] { typeof(List<Pawn>) })]
    static class Patch_Dialog_FormCaravan_CheckForErrors_Prefix
    {
        public static bool Prefix(Dialog_FormCaravan __instance, List<Pawn> pawns, ref bool __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !pawns.Any(p => p.GetOverseer() == MC))
            {
                return true;
            }


            // 反射获取所需成员的值
            object GetMember(string name)
            {
                var t = __instance.GetType();
                var pi = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (pi != null) return pi.GetValue(__instance);
                var fi = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) return fi.GetValue(__instance);
                return null;
            }
            bool GetBool(string name)
            {
                var v = GetMember(name);
                if (v is bool b) return b;
                return false;
            }
            bool IsTileValid(string name)
            {
                var v = GetMember(name);
                if (v == null) return false;
                // if it's an int, treat >= 0 as valid
                if (v is int iv) return iv >= 0;
                // if it has a bool Valid property, use it
                var validProp = v.GetType().GetProperty("Valid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (validProp != null && validProp.PropertyType == typeof(bool))
                {
                    return (bool)validProp.GetValue(v);
                }
                // fallback: try ToString parse
                if (int.TryParse(v.ToString(), out int parsed)) return parsed >= 0;
                return false;
            }


            // 读取所需成员
            bool mustChooseRoute = GetBool("MustChooseRoute");
            bool reform = GetBool("reform"); // field named 'reform' in Dialog_FormCaravan
            bool destinationValid = IsTileValid("destinationTile");
            bool startingValid = IsTileValid("startingTile");



            __result = true;

            if (mustChooseRoute && !destinationValid)
            {
                Messages.Message("MessageMustChooseRouteFirst".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                __result = false;
            }
            if (!reform && !startingValid)
            {
                Messages.Message("MessageNoValidExitTile".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                __result = false;
            }
            if (!reform && __instance.MassUsage > __instance.MassCapacity)
            {
                try
                {
                    var mi = __instance.GetType().GetMethod("FlashMass", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    mi?.Invoke(__instance, null);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error invoking FlashMass: {ex}");
                }
                Messages.Message("TooBigCaravanMassUsage".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                __result = false;
            }




            return false;

        }



    }
}
