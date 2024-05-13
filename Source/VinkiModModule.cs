using System;
using System.Reflection;
using System.Numerics;
using System.Collections.Generic;
using MonoMod.Utils;
using MonoMod.ModInterop;
using MonoMod.RuntimeDetour;
using Celeste.Mod.SkinModHelper;
using Celeste.Mod.UI;
using System.Linq;
using Monocle;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;

namespace Celeste.Mod.VinkiMod;

public class VinkiModModule : EverestModule {
    
    public static VinkiModModule Instance;// { get; private set; }

    public override Type SettingsType => typeof(VinkiModSettings);
    public static VinkiModSettings Settings => (VinkiModSettings) Instance._Settings;

    public override Type SessionType => typeof(VinkiModSession);
    public static VinkiModSession Session => (VinkiModSession) Instance._Session;

    public override Type SaveDataType => typeof(VinkiModSaveData);
    public static VinkiModSaveData SaveData => (VinkiModSaveData) Instance._SaveData;

    //but here's the :hunterglee: (the constants)
    public static String[] textureNamespaces = [
        "scenery/car/body","decals/1-forsakencity/big_sign_b","decals/1-forsakencity/camping_medium","decals/1-forsakencity/hanging_sign","decals/1-forsakencity/big_sign_e",//0-4
        "decals/1-forsakencity/big_sign_d","decals/1-forsakencity/big_sign","decals/1-forsakencity/big_sign_c","scenery/memorial/memorial","decals/3-resort/painting_d",//5-9
        "decals/4-cliffside/rockaline","decals/5-temple/statue_f","decals/5-temple/statue_c","decals/SJ2021/BeginnerLobby/jizo_game_a"//10-14
    ];
    public static String[] textureReplaceNamespaces = ["decals/vinki/car/body","decals/vinki/big_sign_b","decals/vinki/camping_medium","decals/vinki/hanging_sign","decals/vinki/big_sign_e",
        "decals/vinki/big_sign_d","decals/vinki/big_sign","decals/vinki/big_sign_c","decals/vinki/memorial","decals/vinki/painting_d",//5-9
        "decals/vinki/rockavink","decals/vinki/statue_f","decals/vinki/statue_c","decals/vinki/jizo_game_a"//10-14
    ];
    public static String[] hasArtSpots = ["Celeste/0-Intro","Celeste/1-ForsakenCity","Celeste/2-OldSite","Celeste/3-CelestialResort","Celeste/4-GoldenRidge","Celeste/5-MirrorTemple","StrawberryJam2021/0-Lobbies/1-Beginner"];
    public static int[][][] artSpots = [//x,y,w,h,textureNamespaces directory
        [[-180,120,80,50,0]],//intro
        [[1115,-1072,30,20,1],[695,-1064,40,30,2],[1742,-1440,38,22,3],[3040,-1880,40,24,3],[2233,-1344,40,66,4],[2665,-1600,20,25,5],[3340,-1950,70,35,6],[3465,-2575,75,30,7],[3985,-3140,40,80,8]],//forsaken city
        [[790,1725,50,20,2]],//old site
        [[1590,-75,50,30,9]],//celestial hotel
        [[5145,-1425,100,25,10]],//golden ridge
        [[3960,424,80,120,11],[7248,-504,240,50,12]],//mirror temple
        [[3272,324,64,32,13]]//sj beginner lobby
    ];

    public static String[] hasCustomDecals = ["Celeste/0-Intro"];

    public static int[][][] customDecals = [//x,y,w,h,decals/vinki/graffiti/(this)(_x or _y depending on off/on status).png
        [[0,100,1,1,0]]//intro
    ];

    public VinkiModModule() {
        Instance = this;
    }
    private static List<ILHook> hooks = new List<ILHook>();
    public override void Load() {
        if (!Settings.MasterSwitch) {
            Everest.Events.Level.OnTransitionTo += triggerVinkiGUI1;
            Everest.Events.Level.OnEnter += triggerVinkiGUI2;
            On.Celeste.Player.Update += vinkiButtonPress;
            Everest.Events.LevelLoader.OnLoadingThread += vinkiRenderer;
            On.Celeste.IntroCar.Added += introCarScrewery;

            foreach(MethodInfo method in typeof(MTexture).GetMethods()) {
                if(!method.Name.StartsWith("Draw")) continue;
                hooks.Add(new ILHook(method, DrawManipulator));
            }
        }
    }

    public override void Unload() {
        Everest.Events.Level.OnTransitionTo -= triggerVinkiGUI1;
        Everest.Events.Level.OnEnter -= triggerVinkiGUI2;
        On.Celeste.Player.Update -= vinkiButtonPress;
        Everest.Events.LevelLoader.OnLoadingThread -= vinkiRenderer;
        On.Celeste.IntroCar.Added -= introCarScrewery;
        
        hooks.ForEach(h => h.Dispose());
        hooks.Clear();
    }

    private static void triggerVinkiGUI1(Level level, LevelData next, Microsoft.Xna.Framework.Vector2 direction) {
        vinkiGUI();
        graffitiSetup(level.Session);
    }
    private static void triggerVinkiGUI2(Session session, bool fromSaveData) {
        vinkiGUI();
        graffitiSetup(session);
    }
    private static void vinkiGUI() {
        // This here is a failsafe for whenever the Session.vinkiRenderIt is shorter than it should be.
        if (Session.vinkiRenderIt.Length<5) {
            for (var a=Session.vinkiRenderIt.Length;a<7;a=Session.vinkiRenderIt.Length) {
                Session.vinkiRenderIt=Session.vinkiRenderIt.Append(0).ToArray();
            }
        }
        // This line sets the GraffitiTemp rendering to off.
        Session.vinkiRenderIt[4]=-1;
        // This here is setting up the SaveData.settingsArtChanged to match the same length as textureNamespaces.
        if (SaveData.settingsArtChanged.Length<textureNamespaces.Length) {
            for (var a=SaveData.settingsArtChanged.Length;a<textureNamespaces.Length;a=SaveData.settingsArtChanged.Length) {
                SaveData.settingsArtChanged=SaveData.settingsArtChanged.Append(false).ToArray();
            }
        }
        // This section just replaces all the textures for Vinki that aren't graffiti-related.
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            GFX.Gui["hover/highlight"] = GFX.Game["Gui/hover/vinki/highlight"];
            GFX.Gui["hover/idle"] = GFX.Game["Gui/hover/vinki/idle"];
            GFX.Game["pico8/atlas"] = GFX.Game["pico8/vinki/atlas"];
            GFX.Game["pico8/consolebg"] = GFX.Game["pico8/vinki/consolebg"];
            GFX.Portraits["hug-light1"] = GFX.Portraits["vinki/hug-light1"];
            GFX.Portraits["hug-light2a"] = GFX.Portraits["vinki/hug-light2a"];
            GFX.Portraits["hug-light2b"] = GFX.Portraits["vinki/hug-light2b"];
            GFX.Portraits["hug-light2c"] = GFX.Portraits["vinki/hug-light2c"];
            GFX.Portraits["hug1"] = GFX.Portraits["vinki/hug1"];
            GFX.Portraits["hug2"] = GFX.Portraits["vinki/hug2"];
        } else {
            GFX.Gui["hover/highlight"] = GFX.Game["Gui/hover/madeline/highlight"];
            GFX.Gui["hover/idle"] = GFX.Game["Gui/hover/madeline/idle"];
            GFX.Game["pico8/atlas"] = GFX.Game["pico8/madeline/atlas"];
            GFX.Game["pico8/consolebg"] = GFX.Game["pico8/madeline/consolebg"];
            GFX.Portraits["hug-light1"] = GFX.Portraits["madeline/hug-light1"];
            GFX.Portraits["hug-light2a"] = GFX.Portraits["madeline/hug-light2a"];
            GFX.Portraits["hug-light2b"] = GFX.Portraits["madeline/hug-light2b"];
            GFX.Portraits["hug-light2c"] = GFX.Portraits["madeline/hug-light2c"];
            GFX.Portraits["hug1"] = GFX.Portraits["madeline/hug1"];
            GFX.Portraits["hug2"] = GFX.Portraits["madeline/hug2"];
        }
    }

    private static void graffitiSetup(Session session) {
        // The below two lines make the graffiti indicator randomized between all the existing textures with GFX.Gui["vinki/graffiti-icon_"] and a number.
        var rand = new Random();
        Session.vinkiRenderIt[3]=rand.Next(0,7);
        //if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
        //    for (var a=0;a<textureNamespaces.Length;a++) {
        //        if (SaveData.settingsArtChanged[a]) {
        //            Logger.Log(LogLevel.Warn,"VinkiMod_graffitiSetup",a.ToString()+" "+textureNamespaces[a]+" "+textureReplaceNamespaces[a]);
        //        } else {
        //        }
        //    }
        //}
    }

    public static void vinkiButtonPress(On.Celeste.Player.orig_Update orig, Player self) {
        // Before anything, the original Update function runs.
        orig(self);
        // Then, the graffiti indicator is turned off.
        Session.vinkiRenderIt[0]=0;
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            if (!Session.sessionStuffLoaded) {
                if (Array.IndexOf(hasArtSpots,self.SceneAs<Level>().Session.Area.SID)!=-1) {
                    Session.sessionArtSpots=artSpots[Array.IndexOf(hasArtSpots,self.SceneAs<Level>().Session.Area.SID)];
                }
                Session.sessionStuffLoaded=true;
            }
            if (Session.sessionArtSpots.Length>0&&SaveData.settingsArtChanged.Length>=textureNamespaces.Length) {
                for (var a=0;a<Session.sessionArtSpots.Length;a++) {
                    if (!SaveData.settingsArtChanged[Session.sessionArtSpots[a][4]]) {
                        // collision
                        int[] wh = [8,12];
                        if (self.X+wh[0]>Session.sessionArtSpots[a][0]&&self.X<Session.sessionArtSpots[a][0]+Session.sessionArtSpots[a][2]&&self.Y+wh[1]>Session.sessionArtSpots[a][1]&&self.Y<Session.sessionArtSpots[a][1]+Session.sessionArtSpots[a][3]) {
                            // [0/1 toggle for GraffitiIndicator, player x, player y, type of indicator, which texture being replaced? (-1 is off)]
                            Session.vinkiRenderIt = [1,Convert.ToInt16(self.X),Convert.ToInt16(self.Y),Session.vinkiRenderIt[3],Session.vinkiRenderIt[4]];
                            if (Settings.GraffitiButton.Pressed) {
                                //Session.vinkiRenderIt[4]=Session.sessionArtSpots[a][4];
                                doGraffiti(Session.sessionArtSpots[a][4]);
                            }
                            a=Session.sessionArtSpots.Length;
                        }
                    }
                }
            }
        }
    }
    // but here's the DECAL CODE
    private static void vinkiRenderer(Level self) {
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            // If Vinki's Skin is enabled, it adds these entities to the level first.
            self.Add(new GraffitiIndicator());
            // Also, if you're playing Prologue, the intro car's depth is set to 2. (I think.)
            if (Array.IndexOf(hasArtSpots,self.Session.Area.SID)==0) {
                self.Session.LevelData.Entities[2].Values["depth"]=2;
            }
        }
        if (Array.IndexOf(hasCustomDecals,self.Session.Area.SID)!=-1) {
            var myLvl=Array.IndexOf(hasCustomDecals,self.Session.Area.SID);
            for (var a=0;a<customDecals[myLvl].Length;a++) {
                Logger.Log(LogLevel.Warn,"VinkiMod_vinkiRenderer",a.ToString());
                self.Add(new Decal("vinki/graffiti/"+customDecals[myLvl][a][4]+"_x",new Microsoft.Xna.Framework.Vector2 (customDecals[myLvl][a][0],customDecals[myLvl][a][1]),new Microsoft.Xna.Framework.Vector2 (customDecals[myLvl][a][2],customDecals[myLvl][a][3]),1));
            }
        }
    }
    
    public static void doGraffiti(int whichTexture) {
        SaveData.settingsArtChanged[whichTexture]=true;
        Session.vinkiRenderIt[4]=whichTexture;
    }

    public static void introCarScrewery(On.Celeste.IntroCar.orig_Added orig, IntroCar self, Monocle.Scene scene) {
        orig(self,scene);
        self.Depth=2;
    }
    //oh my god popax is my savior
    private static void DrawManipulator(ILContext ctx) {
        ILCursor cursor = new ILCursor(ctx);
        cursor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
        cursor.EmitDelegate(TextureReplacer);
        cursor.Emit(Mono.Cecil.Cil.OpCodes.Starg,0);
    }
    private static MTexture TextureReplacer(MTexture tex) {
        var among = -1;
        // Check if the player is ingame
        if (SaveData!=null&&(SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug"||Settings.HideIfNotVinki==0)) {
            // If so, check if the settingsArtChanged.length is equal to or more than textureNamespaces.Length to prevent errors
            if (SaveData.settingsArtChanged.Length>=textureNamespaces.Length) {
                // If so, check each textureNamespace to see if it's changed in the save data.
                for (var a=0;a<textureNamespaces.Length;a++) {
                    if (SaveData.settingsArtChanged[a]) {
                        // If a texture is changed, is the texture being pushed the same one as this texture?
                        if (tex == GFX.Game[textureNamespaces[a]]) {
                            among=a;
                        }
                    }
                }
            }
        }
        if (among>-1) {
            // Among was changed? Alright, render it how you would've normally, but with the texture replaced.
            return GFX.Game[textureReplaceNamespaces[among]];
        } else {
            // Among was unchanged? Oh well, off to render it as usual.
            return tex;
        }
    }
}