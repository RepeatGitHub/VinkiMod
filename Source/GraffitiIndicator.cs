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

public class GraffitiIndicator : Entity { // this should not show up in ahorn/lonn hopefully
   public GraffitiIndicator() {
        Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
        AddTag(Tags.Persistent);
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

        if (VinkiModModule.Session.vinkiRenderIt[0]!=0) {
            GFX.Gui["vinki/graffiti-icon_"+VinkiModModule.Session.vinkiRenderIt[3]].Draw(new Microsoft.Xna.Framework.Vector2 (VinkiModModule.Session.vinkiRenderIt[1]-2,VinkiModModule.Session.vinkiRenderIt[2]-27));
        }
    }
}