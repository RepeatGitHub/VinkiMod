using System;
using System.Reflection;
using System.Numerics;
using MonoMod.Utils;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using Celeste.Mod.SkinModHelper;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.UI;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.VinkiMod;

public class GraffitiTemp : Entity { // this should not show up in ahorn/lonn hopefully
    public GraffitiTemp() {
        Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
        AddTag(Tags.Persistent);
    }

    public override void Update() {
        base.Update();

        // thank u extended variants but im not using this
        //Depth = (Depths.BGTerrain + Depths.BGDecals) / 2;
        Depth = 1;
    }

    public override void Render() {
        base.Render();

        if (VinkiModModule.Session.vinkiRenderIt[4]!=-1) {
            GFX.Game[VinkiModModule.textureReplaceNamespaces[VinkiModModule.Session.vinkiRenderIt[4]]].Draw(new Microsoft.Xna.Framework.Vector2 (VinkiModModule.Session.vinkiRenderIt[5],VinkiModModule.Session.vinkiRenderIt[6]));
        }
    }

    delegate void SpriteBatchPushSprite_orig(
        SpriteBatch batch,
        Texture2D texture,
        float sourceX, float sourceY, float sourceW, float sourceH,
        float destinationX, float destinationY, float destinationW, float destinationH,
        Color color,
        float originX, float originY, float rotationSin, float rotationCos,
        float depth,
        byte effects
    );
    private static void OnSpriteBatchPushSprite(
        SpriteBatchPushSprite_orig orig,SpriteBatch batch,Texture2D texturee,
        float sourceX, float sourceY, float sourceW, float sourceH,
        float destinationX, float destinationY, float destinationW, float destinationH,
        Color color,
        float originX, float originY, float rotationSin, float rotationCos,
        float depth,
        byte effects
    ) {
        var among = -1;
        //for (var a=0;a<VinkiModModule.SaveData.settingsArtChanged.Length;a++) {
        //    if (texturee == GFX.Game[VinkiModModule.textureNamespaces[a]].Texture.Texture) {
        //        among=a;
        //    }
        //}
        if (among>-1) {
        //    orig(
        //        batch,
        //        ((Monocle.VirtualTexture)(object)GFX.Game[VinkiModModule.textureNamespaces[among]].Texture).Texture_Safe,
        //        sourceX, sourceY, sourceW, sourceH,
        //        destinationX, destinationY, destinationW, destinationH,
        //        color,
        //        originX, originY, rotationSin, rotationCos,
        //        depth,
        //        effects
        //    );
        } else {
            orig(
                batch,
                texturee,
                sourceX, sourceY, sourceW, sourceH,
                destinationX, destinationY, destinationW, destinationH,
                color,
                originX, originY, rotationSin, rotationCos,
                depth,
                effects
            );
        }
    }
}