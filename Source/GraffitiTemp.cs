using System;
using System.Reflection;
using System.Numerics;
using MonoMod.Utils;
using MonoMod.ModInterop;
using Celeste.Mod.SkinModHelper;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.UI;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.VinkiMod;

public class GraffitiTemp : Entity { // this should not show up in ahorn/lonn hopefully
   public GraffitiTemp() {
        Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
    }

    public override void Update() {
        base.Update();

        // thank u extended variants
        if (Collider != null && CollideAll<Solid>().Any(solid => solid.Depth < (Depths.FGTerrain + Depths.FGDecals) / 2)) {
            Depth = Depths.FakeWalls - 1;
        } else {
            Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
        }
    }

    public override void Render() {
        base.Render();

        if (VinkiModModule.Session.vinkiRenderIt[4]!=-1) {
            GFX.Game[VinkiModModule.textureReplaceNamespaces[VinkiModModule.Session.vinkiRenderIt[4]]].Draw(new Microsoft.Xna.Framework.Vector2 (VinkiModModule.Session.vinkiRenderIt[5],VinkiModModule.Session.vinkiRenderIt[6]));
        }
    }
}