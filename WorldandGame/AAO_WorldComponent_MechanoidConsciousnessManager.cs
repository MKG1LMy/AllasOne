using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static System.Net.Mime.MediaTypeNames;

namespace AllasOne.WorldandGame
{
    public class AAO_WorldComponent_MechanoidConsciousnessManager : WorldComponent
    {
        public Pawn MechanoidConsciousness; // 会被Scribe保存/加载

        public AAO_WorldComponent_MechanoidConsciousnessManager(World world) : base(world) { }

        public override void WorldComponentTick()
        {
            if (MechanoidConsciousness != null && Find.TickManager.TicksGame >15000)
            {
                MechanoidConsciousness.DoTick();

            }

            //Map map = QuestGen_Get.GetMap();
            //Log.Message("AAO Patch GetMap: current map is " + (map == null ? "null" : map.ToString()));
            //Map map = Find.CurrentMap;
            //List<Pawn> pawns = map.mapPawns.FreeColonists;
            //Log.Message("AAO Patch GetMap: current free colonists are " + (pawns == null ? "null" : string.Join(", ", pawns.Select(p => p.LabelShort))));

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref MechanoidConsciousness, "AAO_specialPawn");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                //NewUpdateMessage();
            }
        }

        public static AAO_WorldComponent_MechanoidConsciousnessManager Instance =>
            Find.World?.GetComponent<AAO_WorldComponent_MechanoidConsciousnessManager>();

        public void NewUpdateMessage()
        {
            if (MechanoidConsciousness == null)
            {
                return;
            }
            string text = "AAO：\r\n你好玩家，在最近的更新中由于特殊角色的装备出现无法解决的bug因此我将其从游戏中删除并使用了替代的渲染方式。这可能导致您的游戏存档载入时出现错误报告，如果该报告是指向一件装备的缺失，这是一个正常的现象，不会对您的游戏进度造成影响，该mod处于测试阶段，造成这样的影响请多见谅。\r\n\nAAO:\r\nHello players, in a recent update, due to unresolved bugs with special character equipment, I removed it from the game and implemented an alternative rendering method. This may cause error reports when loading your save game. If these reports refer to missing equipment, this is normal and will not affect your game progress. This mod is still in testing, so please forgive any inconvenience caused.";
            DiaNode diaNode = new DiaNode(text);
            DiaOption diaOption = new DiaOption();
            diaOption.resolveTree = true;
            diaOption.clickSound = null;
            diaNode.options.Add(diaOption);
            Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode);
            dialog_NodeTree.closeAction = delegate
            {
                Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
            };
            Find.WindowStack.Add(dialog_NodeTree);
            Find.Archive.Add(new ArchivedDialog(diaNode.text));


        }


    }
}
