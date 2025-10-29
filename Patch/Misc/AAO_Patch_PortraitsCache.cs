using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AllasOne.Patch.Misc
{
    [HarmonyPatch(typeof(PortraitsCache), nameof(PortraitsCache.Get))]
    public static class PortraitsCache_Get_Patch
    {
        public static bool Prefix(
            Pawn pawn,
            Vector2 size,
            Rot4 rotation,
            Vector3 cameraOffset,
            float cameraZoom,
            bool supersample,
            bool compensateForUIScale,
            bool renderHeadgear,
            bool renderClothes,
            IReadOnlyDictionary<Apparel, Color> overrideApparelColors,
            Color? overrideHairColor,
            bool stylingStation,
            PawnHealthState? healthStateOverride,
            ref RenderTexture __result)
        {
            try
            {
                // 直接调用原方法（不走 Harmony 路径，避免递归）
                var method = AccessTools.Method(typeof(PortraitsCache), nameof(PortraitsCache.Get), new Type[]
                {
                typeof(Pawn),
                typeof(Vector2),
                typeof(Rot4),
                typeof(Vector3),
                typeof(float),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(IReadOnlyDictionary<Apparel, Color>),
                typeof(Color?),
                typeof(bool),
                typeof(PawnHealthState?)
                });

                object[] args = {
                pawn, size, rotation, cameraOffset, cameraZoom,
                supersample, compensateForUIScale, renderHeadgear, renderClothes,
                overrideApparelColors, overrideHairColor, stylingStation, healthStateOverride
            };

                var result = method.Invoke(null, args);
                if (result != null)
                {
                    __result = (RenderTexture)result;
                    return false; // 跳过原方法
                }
            }
            catch
            {
                // 任意异常都被吞掉并返回安全贴图
                __result = null;
            }

            return false; // 阻止原方法执行
        }
    }
}
