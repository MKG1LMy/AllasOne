using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Patch_Comp
{
    [HarmonyPatch(typeof(CompOverseerSubject), nameof(CompOverseerSubject.CompGetGizmosExtra))]
    public static class AAO_PatchCompOverseerSubject_CompGetGizmosExtra_Postfix
    {
        private static readonly CachedTexture ShowAllMechOnBarIcon = new CachedTexture("UI/Gizmos/ShowAllMechOnBar");

        public static void Postfix(CompOverseerSubject __instance, ref IEnumerable<Gizmo> __result)
        {




            Pawn MC= AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;
            if (MC == null) return;




            // 独立加入图标
            Texture icon;
            // 最后兜底，避免 null 引发异常
            try
            {
                icon = PortraitsCache.Get(__instance.Parent, new Vector2(75, 75f),
                                              Rot4.South, new Vector3(0f, 0f, 0.3f), 1f);
            }
            catch
            {
                icon = BaseContent.BadTex;
            }

            var SMB = AllasOne.WorldandGame.AAO_WorldComponent_ShowMechOnBar.Instance;
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.defaultLabel = "ShowThisMechOnBar".Translate();
            command_Toggle.defaultDesc = "ShowThisMechOnBarDesc".Translate();
            command_Toggle.isActive = () => SMB.thingNumber.Contains(__instance.Parent.thingIDNumber);
            command_Toggle.icon = icon;
            command_Toggle.toggleAction = delegate
            {
                if (SMB.thingNumber.Contains(__instance.Parent.thingIDNumber))
                {
                    SMB.thingNumber.Remove(__instance.Parent.thingIDNumber);
                    Log.Message($"AAO: Removed mech {__instance.Parent.Name} from show on bar list.");
                    Find.ColonistBar.MarkColonistsDirty();
                }
                else
                {
                   SMB.thingNumber.Add(__instance.Parent.thingIDNumber);
                    Log.Message($"AAO: Added mech {__instance.Parent.Name} to show on bar list.");
                    Find.ColonistBar.MarkColonistsDirty();
                }
            };


            //全部加入图标

            List<int> allSelectMechnumber = Find.Selector.SelectedPawns.Where((Pawn p) => p?.GetOverseer() == MC).Select(p => p.thingIDNumber).ToList();
            bool isAllSelectedMechShown = allSelectMechnumber.All(number => SMB.thingNumber.Contains(number));
            Command_Toggle command_Toggle2 = new Command_Toggle();
            command_Toggle2.defaultLabel = "ShowAllMechOnBar".Translate();
            command_Toggle2.defaultDesc = "ShowAllMechOnBarDesc".Translate();
            command_Toggle2.isActive = () => isAllSelectedMechShown;
            command_Toggle2.icon = ShowAllMechOnBarIcon.Texture;
            command_Toggle2.toggleAction = delegate
            {
                if (isAllSelectedMechShown)
                {
                    foreach (int number in allSelectMechnumber)
                    {
                        SMB.thingNumber.Remove(number);
                        Find.ColonistBar.MarkColonistsDirty();
                    }
                }
                else
                {
                    foreach (int number in allSelectMechnumber)
                    {
                        if (!SMB.thingNumber.Contains(number))
                        {
                            SMB.thingNumber.Add(number);
                            Find.ColonistBar.MarkColonistsDirty();
                        }
                    }
                }

            };









            List<Gizmo> gizmoList = new List<Gizmo>(__result);  // 将原有的 gizmo 列表复制到一个新的列表

            if (__instance.parent is Pawn pawn && pawn?.GetOverseer() == MC && Find.Selector.SelectedPawns.Count < 3)
            {
                gizmoList.Add(command_Toggle);  // 将独立图标添加到列表
            }
            if (__instance.parent is Pawn pawn2 && pawn2?.GetOverseer() == MC && Find.Selector.SelectedPawns.Count >= 3)
            {
                gizmoList.Add(command_Toggle2);
            }

            // 更新 __result，返回修改后的列表
            __result = gizmoList;






            // 以下内容仅在开发者模式下显示
            if (!DebugSettings.ShowDevGizmos)
            {
                return;
            }

            // 新加一个按钮将目标机械族绑定给MC
            Command_Action command_Action4 = new Command_Action();
            command_Action4.defaultLabel = "AAO_DEV: Assign to MC";
            command_Action4.action = delegate
            {
                foreach (Pawn item2 in Find.Selector.SelectedPawns.Where((Pawn p) => p.RaceProps.IsMechanoid))
                {
                    item2.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, item2);
                    item2.SetFaction(Faction.OfPlayer);
                    MC.relations.AddDirectRelation(PawnRelationDefOf.Overseer, item2);
                }
            };



            gizmoList.Add(command_Action4);  // 将新图标添加到列表

            // 更新 __result，返回修改后的列表
            __result = gizmoList;


        }



    }
}
