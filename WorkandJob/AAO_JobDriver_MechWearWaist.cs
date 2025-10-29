using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Noise;


namespace AllasOne.WorkandJob
{
    public class AAO_JobDriver_MechWearWaist : JobDriver_Wear
    {
        private int duration;

        private int unequipBuffer;

        private const TargetIndex ApparelInd = TargetIndex.A;

        private const TargetIndex ApparelSourceIndex = TargetIndex.B;

        private Apparel Apparel => (Apparel)job.GetTarget(TargetIndex.A).Thing;

        private bool TargetIsOnApparelSource
        {
            get
            {
                Apparel apparel = Apparel;
                if (apparel != null && !apparel.Spawned && apparel.ParentHolder is IApparelSource apparelSource)
                {
                    return apparelSource is Thing;
                }
                return false;
            }
        }

        private IApparelSource ApparelSource => (IApparelSource)job.GetTarget(TargetIndex.B).Thing;


        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(TargetIndex.A);
            bool usingSource = TargetIsOnApparelSource;
            if (usingSource)
            {
                yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.InteractionCell).FailOnDespawnedNullOrForbidden(TargetIndex.B);
            }
            else
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            }
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.tickIntervalAction = delegate (int delta)
            {
                unequipBuffer += delta;
                pawn.rotationTracker.FaceTarget(Apparel.PositionHeld);
            };
            toil.WithProgressBarToilDelay((!usingSource) ? TargetIndex.A : TargetIndex.B);
            toil.FailOnDespawnedNullOrForbidden((!usingSource) ? TargetIndex.A : TargetIndex.B);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = duration;
            toil.handlingFacing = true;
            toil.PlaySustainerOrSound(GetCurrentWearSound);
            yield return toil;
            yield return Toils_General.Do(delegate
            {
                Apparel apparel = Apparel;
                if (usingSource)
                {
                    ApparelSource.RemoveApparel(apparel);
                }

                var wornApparel = GetWornApparel(pawn);
                if (wornApparel != null)
                {
                    if (apparel.holdingOwner != null)
                    {
                        apparel.holdingOwner = null;
                    }
                    wornApparel.TryAdd(apparel, false);
                    //pawn.apparel.WornApparel.Add(apparel);
                    //apparel.holdingOwner = wornApparel;
                }
                apparel.DeSpawnOrDeselect();
                if (pawn.outfits != null && job.playerForced)
                {
                    pawn.outfits.forcedHandler.SetForced(apparel, forced: true);
                }
            });
        }



        private SoundDef GetCurrentWearSound()
        {
            Apparel apparel = Apparel;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int num = wornApparel.Count - 1; num >= 0; num--)
            {
                if (!ApparelUtility.CanWearTogether(apparel.def, wornApparel[num].def, pawn.RaceProps.body))
                {
                    if (unequipBuffer >= (int)(wornApparel[num].GetStatValue(StatDefOf.EquipDelay) * 60f))
                    {
                        break;
                    }
                    return wornApparel[num].def.apparel.soundRemove;
                }
            }
            return apparel.def.apparel.soundWear;
        }


        public static ThingOwner<Apparel> GetWornApparel(Pawn pawn)
        {
            // 1. 取得 Pawn 的 ApparelTracker
            var tracker = pawn?.apparel;
            if (tracker == null) return null;

            // 2. 取得类型对象
            var type = typeof(Pawn_ApparelTracker);

            // 3. 获取私有字段 FieldInfo
            var field = type.GetField("wornApparel",
                BindingFlags.Instance | BindingFlags.NonPublic);

            // 4. 取出字段值
            return field?.GetValue(tracker) as ThingOwner<Apparel>;
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            if (TargetIsOnApparelSource)
            {
                job.targetB = (Thing)Apparel.ParentHolder;
            }
            duration = (int)(Apparel.GetStatValue(StatDefOf.EquipDelay) * 60f);
            Apparel apparel = Apparel;
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int num = wornApparel.Count - 1; num >= 0; num--)
            {
                if (!ApparelUtility.CanWearTogether(apparel.def, wornApparel[num].def, pawn.RaceProps.body))
                {
                    duration += (int)(wornApparel[num].GetStatValue(StatDefOf.EquipDelay) * 60f);
                }
            }
        }

    }
}
