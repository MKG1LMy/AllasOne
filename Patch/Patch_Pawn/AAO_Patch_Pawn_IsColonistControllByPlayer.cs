using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AllasOne.Patch.Patch_Pawn
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.IsColonistPlayerControlled), MethodType.Getter)]
    public static class Pawn_IsColonistPlayerControlled_Patch
    {
        public static bool Prefix(Pawn __instance ,ref bool __result)
        {
            var MC = AllasOne.WorldandGame.AAO_WorldComponent_MechanoidConsciousnessManager.Instance.MechanoidConsciousness;
            if (MC == null || !__instance.IsColonyMech || __instance.GetOverseer() != MC)
            {
                return true; // 继续执行原方法
            }

            __result = true; // 强制返回 true
            return false;    // 阻止原方法执行
        }
    }
}
