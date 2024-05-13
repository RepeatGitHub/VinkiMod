using System;
using System.ComponentModel;
using System.Linq;
using Celeste.Mod.UI;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;

namespace Celeste.Mod.VinkiMod;

public class VinkiModSettings : EverestModuleSettings {
    [DefaultButtonBinding(0, Keys.Q),SettingName("VinkiMod_GraffitiButton")]
    public ButtonBinding GraffitiButton { get; set; }

    [SettingNeedsRelaunch,SettingName("VinkiMod_MasterSwitch"),SettingSubText("Disables all code mod functionality from this mod.")]
    public bool MasterSwitch { get; set; } = false;//false means the mod is enabled

    public enum VinkiValues {
        All = 0,
        VinkiOnly = 1
    }
    [SettingName("VinkiMod_HideIfNotVinki")]
    public VinkiValues HideIfNotVinki { get; set; } = VinkiValues.All;

    [SettingInGame(true),SettingName("VinkiMod_ResetCurrentSaveButton")]
    public ResetCurrentSave ResetCurrentSaveButton { get; set; } = new ResetCurrentSave();

    [SettingSubMenu,SettingInGame(true)]
    public class ResetCurrentSave {
        //[YamlIgnore]
        [YamlIgnore,SettingInGame(true),SettingName("VinkiMod_AreYouSure")]
        public bool AreYouSure { get; set; } = false;
        //public void CreateAreYouSureEntry(TextMenuExt.SubMenu menu, bool inGame) {
        //    if (inGame) {
        //        menu.Add(new TextMenu.OnOff("Are you sure?", false));
        //    }
        //}
        [YamlIgnore]
        public bool ConfirmButton { get; set; } = false;
        public void CreateConfirmButtonEntry(TextMenuExt.SubMenu menu, bool inGame) {
            //TextMenu.Item item;
            //TextMenu.Item item2;
            if (inGame) {
                //menu.Add(item = new TextMenu.OnOff("Are you sure?",false));
                menu.Add(new TextMenu.Button("CONFIRM").Pressed(() => {
                    if (this.AreYouSure) {
                        //Logger.Log(LogLevel.Info, "VinkiMod", VinkiModModule.SaveData.settingsArtChanged.Length.ToString());
                        VinkiModModule.SaveData.settingsArtChanged = [];
                        this.AreYouSure=false;
                        //Logger.Log(LogLevel.Info, "VinkiMod", VinkiModModule.SaveData.settingsArtChanged.Length.ToString());
                    } else {
                        //Logger.Log(LogLevel.Info, "VinkiMod", this.AreYouSure.ToString());
                    }
                }));
            }
        }
    }
}