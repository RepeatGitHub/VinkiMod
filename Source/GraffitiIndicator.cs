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
        //Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
        Depth = Depths.CrystalSpinners - 1;
        AddTag(Tags.Persistent);
    }

    public override void Update() {
        base.Update();
    //
    //    // thank u extended variants
    //    if (Collider != null && CollideAll<Solid>().Any(solid => solid.Depth < (Depths.FGTerrain + Depths.FGDecals) / 2)) {
    //        Depth = Depths.FakeWalls - 1;
    //    } else {
    //        Depth = (Depths.FGTerrain + Depths.FGDecals) / 2;
    //    }
        Player self = null;
        Logger.Log(LogLevel.Warn,"VinkiModa","aaaa");
        if (Scene.Tracker.GetEntity<Player>()!=null) {
            self = Scene.Tracker.GetEntity<Player>();
            Logger.Log(LogLevel.Warn,"VinkiModc","cccc");
        }
        VinkiModModule.Session.vinkiRenderIt[0]=0;
        if (VinkiModModule.isGraffitiUser()&&self!=null) {
            if (!VinkiModModule.Session.sessionStuffLoaded) {
                if (Array.IndexOf(VinkiModModule.hasArtSpots,self.SceneAs<Level>().Session.Area.SID+"_"+self.SceneAs<Level>().Session.Area.Mode.ToString())!=-1) {
                    VinkiModModule.Session.sessionArtSpots=VinkiModModule.artSpots[Array.IndexOf(VinkiModModule.hasArtSpots,self.SceneAs<Level>().Session.Area.SID+"_"+self.SceneAs<Level>().Session.Area.Mode.ToString())];
                }
                VinkiModModule.Session.sessionStuffLoaded=true;
            }
            if (VinkiModModule.Session.sessionArtSpots.Length>0&&VinkiModModule.SaveData.settingsArtChanged.Length>=VinkiModModule.textureNamespaces.Length) {
                for (var a=0;a<VinkiModModule.Session.sessionArtSpots.Length;a++) {
                    if (!VinkiModModule.SaveData.settingsArtChanged[VinkiModModule.Session.sessionArtSpots[a][4]]) {
                        // collision
                        int[] wh = [8,12];
                        
                        //thank u snippy for the self?.Dead part
                        if (!(self?.Dead??true)&&self.X+wh[0]>VinkiModModule.Session.sessionArtSpots[a][0]&&self.X<VinkiModModule.Session.sessionArtSpots[a][0]+VinkiModModule.Session.sessionArtSpots[a][2]&&self.Y+wh[1]>VinkiModModule.Session.sessionArtSpots[a][1]&&self.Y<VinkiModModule.Session.sessionArtSpots[a][1]+VinkiModModule.Session.sessionArtSpots[a][3]) {
                            // [0/1 toggle for GraffitiIndicator, player x, player y, type of indicator]
                            VinkiModModule.Session.vinkiRenderIt = [1,Convert.ToInt16(self.X)+0,Convert.ToInt16(self.Y)+0,VinkiModModule.Session.vinkiRenderIt[3]+0];
                            if (VinkiModModule.Settings.GraffitiButton.Pressed) {
                                VinkiModModule.doGraffiti(VinkiModModule.Session.sessionArtSpots[a][4]);
                            }
                            Logger.Log(LogLevel.Warn,"vinkibutonpres",VinkiModModule.Session.vinkiRenderIt[1].ToString());
                            a=VinkiModModule.Session.sessionArtSpots.Length;
                        }
                    }
                }
            }
            Logger.Log(LogLevel.Warn,"VinkiModb",VinkiModModule.Session.vinkiRenderIt[2].ToString());
        }
    }

    public override void Render() {
        base.Render();
        if (VinkiModModule.Session.vinkiRenderIt[0]!=0) {
            GFX.Gui["vinki/graffiti-icon_"+VinkiModModule.Session.vinkiRenderIt[3]].Draw(new Microsoft.Xna.Framework.Vector2 (VinkiModModule.Session.vinkiRenderIt[1]-2,VinkiModModule.Session.vinkiRenderIt[2]-27));
        }
        //GFX.Game["decals/madeline/big_sign_b"].Draw(new Microsoft.Xna.Framework.Vector2 (VinkiModModule.Session.vinkiRenderIt[1],VinkiModModule.Session.vinkiRenderIt[2]));
    }
}