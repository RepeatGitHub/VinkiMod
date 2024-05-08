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

    //delegate void SpriteBatchPushSprite_orig(
    //    SpriteBatch batch,
    //    Texture2D texture,
    //    float sourceX, float sourceY, float sourceW, float sourceH,
    //    float destinationX, float destinationY, float destinationW, float destinationH,
    //    Color color,
    //    float originX, float originY, float rotationSin, float rotationCos,
    //    float depth,
    //    byte effects
    //);
    //private static void OnSpriteBatchPushSprite(
    //    SpriteBatchPushSprite_orig orig,SpriteBatch batch,Texture2D texturee,
    //    float sourceX, float sourceY, float sourceW, float sourceH,
    //    float destinationX, float destinationY, float destinationW, float destinationH,
    //    Color color,
    //    float originX, float originY, float rotationSin, float rotationCos,
    //    float depth,
    //    byte effects
    //) {
    //    var among = -1;
    //    // Check if the player is ingame
    //    if (VinkiModModule.SaveData!=null) {
    //        // If so, check if the settingsArtChanged.length is equal to or more than textureNamespaces.Length to prevent errors
    //        if (VinkiModModule.SaveData.settingsArtChanged.Length>=VinkiModModule.textureNamespaces.Length) {
    //            // If so, check each textureNamespace to see if it's changed in the save data.
    //            for (var a=0;a<VinkiModModule.textureNamespaces.Length;a++) {
    //                if (VinkiModModule.SaveData.settingsArtChanged[a]) {
    //                    // If a texture is changed, is the texture being pushed the same one as this texture?
    //                    if (texturee == GFX.Game[VinkiModModule.textureNamespaces[a]].Texture.Texture) {
    //                        among=a;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if (among>-1) {
    //        // Among was changed? Alright, render it how you would've normally, but with the texture replaced.
    //        orig(
    //            batch,
    //            GFX.Game[VinkiModModule.textureReplaceNamespaces[among]].Texture.Texture,
    //            sourceX, sourceY, sourceW, sourceH,
    //            destinationX, destinationY, destinationW, destinationH,
    //            color,
    //            originX, originY, rotationSin, rotationCos,
    //            depth,
    //            effects
    //        );
    //    } else {
    //        // Among was unchanged? Oh well, off to render it as usual.
    //        orig(
    //            batch,
    //            texturee,
    //            sourceX, sourceY, sourceW, sourceH,
    //            destinationX, destinationY, destinationW, destinationH,
    //            color,
    //            originX, originY, rotationSin, rotationCos,
    //            depth,
    //            effects
    //        );
    //    }
    //}
}