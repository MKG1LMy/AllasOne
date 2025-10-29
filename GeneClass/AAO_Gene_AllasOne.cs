using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.GeneClass
{
    public class AAO_Gene_AllasOne : Gene
    {
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);

            // 仅在持有者存在且为人类形态时工作
            if (pawn == null) return;
            if (!pawn.Spawned) return;
            if (!pawn.RaceProps.Humanlike) return;
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == pawn) return;

            try
            {
                HealthUtility.DamageUntilDead(pawn);
                Log.Message($"[AAO_Gene_AllasOne] {pawn.LabelShort} executed {pawn.LabelShort}.");

            }
            catch (Exception ex)
            {
                Log.Error($"[AAO_Gene_AllasOne] TickInterval error: {ex}");
            }
        }
    }
}
