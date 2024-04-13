using System;
using System.Reflection;
using System.Numerics;
using MonoMod.Utils;
using MonoMod.ModInterop;
using Celeste.Mod.SkinModHelper;
using Celeste.Mod.UI;
using System.Linq;
using IL.Monocle;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

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
    public static String[] textureNamespaces = ["scenery/car/body","decals/1-forsakencity/big_sign_b","decals/1-forsakencity/camping_medium","decals/1-forsakencity/hanging_sign","decals/1-forsakencity/big_sign_e","decals/1-forsakencity/big_sign_d","decals/1-forsakencity/big_sign","decals/1-forsakencity/big_sign_c","scenery/memorial/memorial"];
    public static String[] textureReplaceNamespaces = ["scenery/vinki/car/body","decals/vinki/big_sign_b","decals/vinki/camping_medium","decals/vinki/hanging_sign","decals/vinki/big_sign_e","decals/vinki/big_sign_d","decals/vinki/big_sign","decals/vinki/big_sign_c","scenery/vinki/memorial"];
    public static String[] textureUnReplaceNamespaces = ["scenery/madeline/car/body","decals/madeline/big_sign_b","decals/madeline/camping_medium","decals/madeline/hanging_sign","decals/madeline/big_sign_e","decals/madeline/big_sign_d","decals/madeline/big_sign","decals/madeline/big_sign_c","scenery/madeline/memorial"];
    public static String[] hasArtSpots = ["Celeste/0-Intro","Celeste/1-ForsakenCity"];
    public static int[][][] artSpots = [//x,y,w,h,textureNamespaces directory,tempx,tempy
        [[-180,120,80,50,0,-192,134]],
        [[1115,-1072,30,20,1,1104,-1083],[695,-1064,40,30,2,692,-1105],[1742,-1440,38,22,3,1740,-1456],[2233,-1344,40,66,4,2220,-1356],[2665,-1600,20,25,5,2664,-1612],[3340,-1950,70,35,6,3337,-1963],[3465,-2575,75,30,7,3456,-2579],[3985,-3140,40,80,8,3976,-3144]]
    ];

    public VinkiModModule() {
        Instance = this;
//#if DEBUG
        // debug builds use verbose logging
        //Logger.SetLogLevel(nameof(ModsModule), LogLevel.Verbose);
//#else
        // release builds use info logging to reduce spam in log files
        //Logger.SetLogLevel(nameof(ModsModule), LogLevel.Info);
//#endif
    }

    public override void Load() {
        Everest.Events.Level.OnTransitionTo += triggerVinkiGUI1;
        Everest.Events.Level.OnEnter += triggerVinkiGUI2;
        On.Celeste.Player.Update += vinkiButtonPress;
        Everest.Events.LevelLoader.OnLoadingThread += vinkiRenderer;
        On.Celeste.IntroCar.Added += introCarScrewery;
    }

    public override void Unload() {
        Everest.Events.Level.OnTransitionTo -= triggerVinkiGUI1;
        Everest.Events.Level.OnEnter -= triggerVinkiGUI2;
        On.Celeste.Player.Update -= vinkiButtonPress;
        Everest.Events.LevelLoader.OnLoadingThread -= vinkiRenderer;
        On.Celeste.IntroCar.Added -= introCarScrewery;
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
        if (Session.vinkiRenderIt.Length<7) {
            for (var a=Session.vinkiRenderIt.Length;a<7;a=Session.vinkiRenderIt.Length) {
                Session.vinkiRenderIt=Session.vinkiRenderIt.Append(0).ToArray();
            }
        }
        Session.vinkiRenderIt[4]=-1;
        if (SaveData.settingsArtChanged.Length<textureNamespaces.Length) {
            for (var a=SaveData.settingsArtChanged.Length;a<textureNamespaces.Length;a=SaveData.settingsArtChanged.Length) {
                SaveData.settingsArtChanged=SaveData.settingsArtChanged.Append(false).ToArray();
            }
        } // above used to be in graffitisetup
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
        var rand = new Random();
        Session.vinkiRenderIt[3]=rand.Next(0,3);
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            for (var a=0;a<textureNamespaces.Length;a++) {
                if (SaveData.settingsArtChanged[a]) {
                    GFX.Game[textureNamespaces[a]]=GFX.Game[textureReplaceNamespaces[a]];
                    Logger.Log(LogLevel.Warn,"VinkiMod",a.ToString()+" "+textureNamespaces[a]+" "+textureReplaceNamespaces[a]);
                }
            }
        } else {
            ARRGH_NOTEXTURES_FORYE();
        }
    }

    public static void vinkiButtonPress(On.Celeste.Player.orig_Update orig, Player self) {
        orig(self);
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
                            // [0/1 toggle for GraffitiIndicator, player x, player y, type of indicator, which texture for GraffitiTemp (-1 is off), x of GraffitiTemp, y of GraffitiTemp]
                            Session.vinkiRenderIt = [1,Convert.ToInt16(self.X),Convert.ToInt16(self.Y),Session.vinkiRenderIt[3],Session.vinkiRenderIt[4],Session.sessionArtSpots[a][5],Session.sessionArtSpots[a][6]];
                            if (Settings.GraffitiButton.Pressed) {
                                //Session.vinkiRenderIt[4]=Session.sessionArtSpots[a][4];
                                doGraffiti(Session.sessionArtSpots[a][4]);
                            }
                            a=Session.sessionArtSpots.Length;
                        } else {
                        }
                    }
                }
            }
        }
        if (Session.vinkiRenderIt[4]!=-1) {
            //Logger.Log(LogLevel.Warn,"VinkiMod",Session.vinkiRenderIt[4].ToString());
        }
    }

    private static void vinkiRenderer(Level self) {
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            self.Add(new GraffitiIndicator());
            self.Add(new GraffitiTemp());
            if (Array.IndexOf(hasArtSpots,self.Session.Area.SID)==0) {
                self.Session.LevelData.Entities[2].Values["depth"]=2;
            }
        }
    }
    
    public static void doGraffiti(int whichTexture) {
        SaveData.settingsArtChanged[whichTexture]=true;
        GFX.Game[textureNamespaces[whichTexture]]=GFX.Game[textureReplaceNamespaces[whichTexture]];
        Session.vinkiRenderIt[4]=whichTexture;
    }

    public static void ARRGH_NOTEXTURES_FORYE() {
        for (var a=0;a<textureNamespaces.Length;a++) {
            GFX.Game[textureNamespaces[a]]=GFX.Game[textureUnReplaceNamespaces[a]];
        }
        // put whatever texture reloader thingy from doGraffiti here
    }

    public static void introCarScrewery(On.Celeste.IntroCar.orig_Added orig, IntroCar self, Monocle.Scene scene) {
        orig(self,scene);
        self.Depth=2;
    }
}