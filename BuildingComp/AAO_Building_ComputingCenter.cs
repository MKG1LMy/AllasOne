using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;
using AllasOne.WorldandGame;

namespace AllasOne.BuildingComp
{
    internal class AAO_Building_ComputingCenter:ThingComp
    {
        public AAO_Building_ComProperties_ComputingCenter Props => (AAO_Building_ComProperties_ComputingCenter)props;

        private ResearchProjectDef Project => Find.ResearchManager.GetProject();

        public float SavePoint = 0;

        public override void CompTick()
        {
            base.CompTick();

            if(!parent.IsHashIntervalTick(Props.checkInterval))
            {
                return;
            }

            CompPowerTrader compPowerTrader = parent.TryGetComp<CompPowerTrader>();
            if (compPowerTrader != null && !compPowerTrader.PowerOn)
            {
                return;
            }


            if (Project == null)
            {
                SavePoint += Props.researchPoint;
                return;
            }

            Pawn MC = AAO_WorldComponent_MechanoidConsciousnessManager.Instance?.MechanoidConsciousness;

            float statValue;
            float ResearchPoint = Props.researchPoint * 121;

            if (MC != null && Props.ResearchByMc)
            {
                statValue = MC.GetStatValue(StatDefOf.ResearchSpeed);
                Find.ResearchManager.ResearchPerformed(Props.ResearchSpeedFactor * statValue * ResearchPoint, MC);
                MC.skills.Learn(SkillDefOf.Intellectual, 0.1f * ResearchPoint);
                if (SavePoint >= 0)
                {
                    float num = SavePoint * 121f;
                    Find.ResearchManager.ResearchPerformed(Props.ResearchSpeedFactor * statValue * num, MC);
                    MC.skills.Learn(SkillDefOf.Intellectual, 0.1f * num);
                    SavePoint = 0;
                }
            }
            else
            {
                statValue = 1;
                Find.ResearchManager.AddProgress(Project , Props.ResearchSpeedFactor * statValue * Props.researchPoint);
                if (SavePoint >= 0)
                {
                    float num = SavePoint;
                    Find.ResearchManager.AddProgress(Project, Props.ResearchSpeedFactor * statValue * num);
                    SavePoint = 0;
                }

            }



        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref SavePoint, "SavePoint");
        }

    }
}
