using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.PawnRenderMisc
{
    public class PawnRenderNode_LikeColor : PawnRenderNode
    {
        public PawnRenderNode_LikeColor(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Color ColorFor(Pawn pawn)
        {
            return pawn.story.favoriteColor.color;
        }

    }
}
