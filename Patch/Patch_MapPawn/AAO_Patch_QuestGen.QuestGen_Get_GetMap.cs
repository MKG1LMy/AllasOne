using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


namespace AllasOne.Patch.Patch_MapPawn
{
    [HarmonyPatch(typeof(QuestGen_Get), nameof(QuestGen_Get.GetMap))]
    public static class Patch_QuestGen_Get_GetMap
    {
        public static void Postfix(ref Map __result, bool mustBeInfestable, int? preferMapWithMinFreeColonists, bool canBeSpace)
        {

            if (__result != null)
            { 
                Log.Message("AAO Patch GetMap: Has original map , It is " + __result.ToString());
                return; 
            }

            Log.Message("AAO Patch GetMap: original map is null, trying to set a new one");

            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null) return;

            try
            {
                if(Find.Maps != null) 
                {
                    List<Map> source = Find.Maps.Where((Map map) => map.mapPawns.SpawnedColonyMechs.Count >0 || map.mapPawns.FreeColonistsSpawnedCount>0).ToList();
                    foreach (Map map1 in source)
                    {
                        if (!canBeSpace && map1.Tile.LayerDef.isSpace)
                        {
                            source.Remove(map1);
                        }
                        if (!(map1.IsPlayerHome && (!mustBeInfestable || InfestationCellFinder.TryFindCell(out var _, map1))))
                        {
                            source.Remove(map1);
                        }
                        if (map1.IsPocketMap)
                        {
                            source.Remove(map1);
                        }
                        if (!map1.IsPlayerHome)
                        {
                            source.Remove(map1);              
                        }
                    }
                    source.TryRandomElement(out __result);

                    if (__result == null && Find.CurrentMap != null && Find.CurrentMap.IsPlayerHome)
                    {
                        __result = Find.CurrentMap;
                        //Log.Message("AAO Patch GetMap: fallback to current map ");
                    }
                    else
                    {
                        //Log.Message("AAO Patch GetMap: set to filtered map " + __result.ToString());
                    }
                }
            }
            catch
            {
                // 若扩展不存在或异常，安全退出
            }


        }
    }
}
