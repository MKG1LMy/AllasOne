using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace AllasOne
{
    public class AllasOneMod : Mod
    {
        public const string HarmonyId = "allasone_mzksg";

        public AllasOneMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(); // 扫描当前程序集内的所有 [HarmonyPatch] 并应用

            Log.Message($"[AllasOne] Harmony initialized. ID={HarmonyId}");
        }
    }
}
