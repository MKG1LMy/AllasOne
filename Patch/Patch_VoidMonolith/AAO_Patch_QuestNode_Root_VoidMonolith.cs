using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_VoidMonolith
{

    [HarmonyPatch(typeof(QuestNode_Root_VoidMonolith), "RunInt")]
    public static class Patch_QN_Root_VoidMonolith_RunInt_Override
    {
        // 返回 false 表示跳过原方法
        public static bool Prefix()
        {
            try
            {
                Quest quest = QuestGen.quest;
                Slate slate = QuestGen.slate;
                Map map = slate?.Get<Map>("map");   // ← 修改点
                Building_VoidMonolith monolith = slate?.Get<Building_VoidMonolith>("monolith");

                if (quest == null || slate == null)
                {
                    Log.Error("[AAO][VoidMonolith/Override] Quest 或 Slate 为 null，跳过。");
                    return false;
                }


                // 保留原逻辑
                QuestUtility.AddQuestTag(
                    questTagToAdd: QuestGenUtility.HardcodedTargetQuestTagWithQuestID("monolithMap"),
                    questTags: ref map.Parent.questTags
                );

                string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("monolithMap.Destroyed");

                quest.AddPart(new QuestPart_MonolithPart(monolith));
                quest.End(
                    QuestEndOutcome.Fail,
                    0,
                    null,
                    inSignal,
                    QuestPart.SignalListenMode.OngoingOnly,
                    sendStandardLetter: true
                );

                QuestPart_Hyperlinks part = new QuestPart_Hyperlinks();
                quest.AddPart(part);

                //Log.Message($"[AAO][VoidMonolith/Override] 已使用 slate.Get<Map>(\"map\") 执行 RunInt，map={map?.ToStringSafe() ?? "null"}，monolith={monolith?.ToStringSafe() ?? "null"}。");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[AAO][VoidMonolith/Override] 执行异常：{ex}");
            }
            return false; // 跳过原 RunInt
        }
    }

}
