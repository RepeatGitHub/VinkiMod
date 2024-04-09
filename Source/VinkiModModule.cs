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
    public static String[] textureNamespaces = ["scenery/car/body"];
    public static String[] textureReplaceNamespaces = ["scenery/vinki/car/body"];
    public static String[] textureUnReplaceNamespaces = ["scenery/madeline/car/body"];
    public static String[] hasArtSpots = ["Celeste/0-Intro"];
    public static int[][][] artSpots = [//x,y,w,h,textureNamespaces directory,tempx,tempy
        [[-180,120,80,50,0,10,12]]
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
    }

    public override void Unload() {
        Everest.Events.Level.OnTransitionTo -= triggerVinkiGUI1;
        Everest.Events.Level.OnEnter -= triggerVinkiGUI2;
        On.Celeste.Player.Update -= vinkiButtonPress;
        Everest.Events.LevelLoader.OnLoadingThread -= vinkiRenderer;
    }

    private static void triggerVinkiGUI1(Level level, LevelData next, Microsoft.Xna.Framework.Vector2 direction) {vinkiGUI();}
    private static void triggerVinkiGUI2(Session session, bool fromSaveData) {
        vinkiGUI();
        graffitiSetup(session);
    }
    private static void vinkiGUI() {
        //Session.vinkiRenderIt[4]=-1;
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
        } else {
            GFX.Gui["hover/highlight"] = GFX.Game["Gui/hover/madeline/highlight"];
            GFX.Gui["hover/idle"] = GFX.Game["Gui/hover/madeline/idle"];
            GFX.Game["pico8/atlas"] = GFX.Game["pico8/madeline/atlas"];
            GFX.Game["pico8/consolebg"] = GFX.Game["pico8/madeline/consolebg"];
        }
    }

    private static void graffitiSetup(Session session) {
        var rand = new Random();
        Session.vinkiRenderIt[3]=rand.Next(0,2);
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            for (var a=0;a<textureNamespaces.Length;a++) {
                if (SaveData.settingsArtChanged[a]) {
                    GFX.Game[textureNamespaces[a]]=GFX.Game[textureReplaceNamespaces[a]];
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
                            Session.vinkiRenderIt = [1,self.X,self.Y,Session.vinkiRenderIt[3],-1,Session.sessionArtSpots[a][5],Session.sessionArtSpots[a][6]];
                            if (Settings.GraffitiButton.Pressed) {
                                Session.vinkiRenderIt[4]=1;
                                doGraffiti(Session.sessionArtSpots[a][4]);
                            }
                            a=Session.sessionArtSpots.Length;
                        } else {
                        }
                    }
                }
            }
        }
    }

    private static void vinkiRenderer(Level self) {
        if (SkinModHelperModule.GetPlayerSkinName(-1)=="Vinki_Scug") {
            self.Add(new GraffitiIndicator());
            self.Add(new GraffitiTemp());
        }
    }
    
    public static void doGraffiti(int whichTexture) {
        SaveData.settingsArtChanged[whichTexture]=true;
        GFX.Game[textureNamespaces[whichTexture]]=GFX.Game[textureReplaceNamespaces[whichTexture]];
    }

    public static void ARRGH_NOTEXTURES_FORYE() {
        for (var a=0;a<textureNamespaces.Length;a++) {
            GFX.Game[textureNamespaces[a]]=GFX.Game[textureUnReplaceNamespaces[a]];
        }
        // put whatever texture reloader thingy from doGraffiti here
    }
}