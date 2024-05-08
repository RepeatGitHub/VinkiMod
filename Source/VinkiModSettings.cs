using System;
using System.ComponentModel;
using System.Linq;
using Celeste.Mod.UI;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;

namespace Celeste.Mod.VinkiMod;

public class VinkiModSettings : EverestModuleSettings {
    [DefaultButtonBinding(0, Keys.Q)]
    public ButtonBinding GraffitiButton { get; set; }

    public ResetCurrentSave ResetCurrentSaveButton { get; set; } = new ResetCurrentSave();

    [SettingSubMenu]
    public class ResetCurrentSave {
        //[YamlIgnore]
        [SettingInGame(true)]
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