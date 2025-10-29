using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static RimWorld.MechClusterSketch;
using static UnityEngine.GraphicsBuffer;

namespace AllasOne.BuildingComp
{
    public class AAO_Building_CompRemoteCharge: ThingComp
    {
        public float range;

        private int ticksChargeCounts = 0;

        private int CountToCharge;

        private Sustainer activeSustainer;

        private bool lastIntervalActive;

        public AAO_Building_CompProperties_RemoteCharge Props => (AAO_Building_CompProperties_RemoteCharge)props;

        private CompPowerTrader PowerTrader => parent.TryGetComp<CompPowerTrader>();

        private bool IsPawnAffected(Pawn target)
        {
            if (PowerTrader != null && !PowerTrader.PowerOn)
            {
                return false;
            }
            if (target.Dead)
            {
                return false;
            }
            if (target.RaceProps.IsMechanoid && target.IsColonyMech && target.needs?.energy != null)
            {
                return target.PositionHeld.DistanceTo(parent.PositionHeld) <= range;
            }
            return false;
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            range = Props.range;
            CountToCharge = Props.ticksTocharge / Props.checkInterval;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref range, "range", 0f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && range <= 0f)
            {
                range = Props.range;
            }
        }

        public override void CompTick()
        {
            MaintainSustainer();

            if (!parent.IsHashIntervalTick(Props.checkInterval))
            {
                return;
            }

            if (ticksChargeCounts < CountToCharge)
            {
                ticksChargeCounts++;
                return;
            }
            ticksChargeCounts = 0;

            CompPowerTrader compPowerTrader = parent.TryGetComp<CompPowerTrader>();
            if (compPowerTrader != null && !compPowerTrader.PowerOn)
            {
                return;
            }
            lastIntervalActive = false;
            
            if (!parent.SpawnedOrAnyParentSpawned)
            {
                return;
            }

            List<Pawn> MechPreTocharge = new List<Pawn>(Props.maxMech);
            foreach (Pawn item in parent.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (IsPawnAffected(item) && MechPreTocharge.Count < Props.maxMech)
                {
                    MechPreTocharge.Add(item);
                    if (MechPreTocharge.Count >= Props.maxMech) break;
                }
                if (item.carryTracker.CarriedThing is Pawn target && IsPawnAffected(target) && MechPreTocharge.Count < Props.maxMech)
                {
                    MechPreTocharge.Add(target);
                    if (MechPreTocharge.Count >= Props.maxMech) break;
                }               
            }
            foreach (var pawn in MechPreTocharge)
            {
                ChargeMech(pawn, MechPreTocharge.Count);
            }
            MechPreTocharge.Clear();

        }

        private void ChargeMech(Pawn target, int MechCount)
        {
            float energetocharge = Props.energyCharge / MechCount;
            if (target.GetStatValue(StatDefOf.BandwidthCost) <= 1)
            {
                target.needs.energy.CurLevel += energetocharge;
            }
            else if (target.GetStatValue(StatDefOf.BandwidthCost) > 1)
            {
                target.needs.energy.CurLevel += energetocharge/2;
            }
            lastIntervalActive = true;
            //Log.Message($"[AAO] 已为 {target.LabelShortCap} 充能 {energetocharge:0.##}。");
        }

        private void MaintainSustainer()
        {
            if (lastIntervalActive && Props.activeSound != null)
            {
                if (activeSustainer == null || activeSustainer.Ended)
                {
                    activeSustainer = Props.activeSound.TrySpawnSustainer(SoundInfo.InMap(new TargetInfo(parent)));
                }
                activeSustainer.Maintain();
            }
            else if (activeSustainer != null)
            {
                activeSustainer.End();
                activeSustainer = null;
            }
        }

        public override void PostDraw()
        {
            if (!Props.drawLines)
            {
                return;
            }
            int num = Mathf.Max(parent.Map.Size.x, parent.Map.Size.y);
            if (!Find.Selector.SelectedObjectsListForReading.Contains(parent) || !(range < (float)num))
            {
                return;
            }
            foreach (Pawn item in parent.Map.mapPawns.AllPawnsSpawned)
            {
                if (IsPawnAffected(item))
                {
                    GenDraw.DrawLineBetween(item.DrawPos, parent.DrawPos);
                }
            }
        }
    }
}
