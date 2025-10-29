using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AllasOne.Patch
{
    [HarmonyPatch(typeof(ColonistBarColonistDrawer), "HandleClicks")]
    public static class Patch_ColonistBar_HandleClicks
    {
        // 原方法签名：void HandleClicks(Rect rect, Pawn colonist, int reorderableGroup, out bool reordering)
        public static bool Prefix(Rect rect, Pawn colonist, int reorderableGroup, ref bool reordering)
        {
            reordering = false;
            if (colonist == null) return true; // 让原方法处理

            // 仅对你的特殊 Pawn 接管：
            var hdef = DefDatabase<HediffDef>.GetNamedSilentFail("AAO_Hediff_MechanoidConsciousnessNode");
            if (hdef == null || !colonist.health?.hediffSet?.HasHediff(hdef) == true)
                return true; // 不是特殊Pawn，走原逻辑

            // 点击检测（和原版一致用隐形按钮）
            if (!Widgets.ButtonInvisible(rect)) return false; // 消耗事件并阻止原方法的其它逻辑

            // 手动选择：即便 Despawned 也可以把它放进选择器（用于信息卡/检查面板）
            // 可选：播放选择音效
            SoundDefOf.ColonistSelected?.PlayOneShotOnCamera();

            Find.Selector.ClearSelection();
            // 某些版本对未Spawn的Thing拒绝选择；我们退一步直接打开信息卡窗口，体验更稳
            Find.WindowStack.Add(new Dialog_InfoCard(colonist));

            return false; // 阻止原方法，避免它因各种前置条件而不作为
        }
    }
}
