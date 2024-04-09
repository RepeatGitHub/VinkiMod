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
    //public static FieldInfo vinkiSkinLoaded;
    //EverestModuleMetadata skinModHelperPlus = new() {
    //    Name = "SkinModHelperPlus",
    //    Version = new Version(0,9,0)
    //};

    public override void Load() {
        //Logger.Log(LogLevel.Debug, "VinkiMod", "Obama");
        Everest.Events.Level.OnTransitionTo += triggerVinkiGUI1;
        Everest.Events.Level.OnEnter += triggerVinkiGUI2;
        //Logger.Log(LogLevel.Debug, "VinkiMod", "Obama 2");
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
                //Logger.Log(LogLevel.Info, "VinkiMod", SaveData.settingsArtChanged.Length.ToString()+" "+textureNamespaces.Length.ToString());
            }
        } // above used to be in graffitisetup
        //if (Everest.Loader.TryGetDependency(Instance.skinModHelperPlus, out EverestModule skinModule)) {
            //Assembly skinAssembly = skinModule.GetType().Assembly;
            //Type skinModule2 = skinAssembly.GetType("Celeste.Mod.SkinModHelper.SkinModHelperSession");
            //if (skinModule2==null) {
            //    Logger.Log(LogLevel.Info, "VinkiMod", "Uh oh, looks like SkinModHelperPlus's module is being wack!");
            //} else {
            //    Logger.Log(LogLevel.Info, "VinkiMod", "SkinModHelperPlus found successfully!");
            //}
            //Logger.Log(LogLevel.Info, "VinkiMod", SkinModHelperModule.GetPlayerSkinName(-1));
            //object skinModule3 = skinModule2.GetField("GetPlayerSkinName");
            //DynamicData skinData = DynamicData.For(skinModule3);
            //vinkiSkinLoaded = skinModule2.GetField("GetPlayerSkinName", BindingFlags.Public | BindingFlags.Static);
            //Logger.Log(LogLevel.Info, "VinkiMod", skinData.Get<string>("GetPlayerSkinName"));
        //}
        //if (Instance.vinkiSkinLoaded?.GetValue(null).ToString()=="Vinki") {
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
        //}
        //Logger.Log(LogLevel.Info, "VinkiMod", "Obama");
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
            //if (Celeste.Instance.scene!=null) {
            //if (AreaData.Get(Celeste.Instance.scene).SID!=null) {
            //if (Array.IndexOf(hasArtSpots,AreaData.Get(Celeste.Instance.scene).Name.DialogCleanOrNull(Dialog.Languages["english"]))!=-1) {
            //    Session.sessionArtSpots=artSpots[Array.IndexOf(hasArtSpots,AreaData.Get(Celeste.Instance.scene).Name.DialogCleanOrNull(Dialog.Languages["english"]))];
            //}
            //Logger.Log(LogLevel.Info, "VinkiMod", session.Area.SID);
            //}
            //}
            // see the vinkibuttonpress for what was gonna be here

            //if (Celeste.Instance.scene!=null)
            //    Logger.Log(LogLevel.Info, "VinkiMod", "4");
            //if (AreaData.Get(Celeste.Instance.scene)!=null)
            //    Logger.Log(LogLevel.Info, "VinkiMod", "3");
            //if (AreaData.Get(Celeste.Instance.scene).Name!=null)
            //    Logger.Log(LogLevel.Info, "VinkiMod", "2");
            //if (AreaData.Get(Celeste.Instance.scene).Name.DialogCleanOrNull(Dialog.Languages["english"])!=null)
            //    Logger.Log(LogLevel.Info, "VinkiMod", "1");
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
                    //Logger.Log(LogLevel.Warn, "VinkiMod", Session.sessionArtSpots.Length.ToString());
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
                            //Logger.Log(LogLevel.Warn, "VinkiMod", "BOING");
                        } else {
                            //Logger.Log(LogLevel.Warn, "VinkiMod", Session.sessionArtSpots[a][0].ToString());
                        }
                    }
                }
                //Logger.Log(LogLevel.Warn, "VinkiMod", "x"+self.X.ToString()+"y"+self.Y.ToString());
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