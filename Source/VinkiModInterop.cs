using Celeste.Mod.VinkiMod;
using ExtendedVariants.Variants;
using MonoMod.ModInterop;
using System;
using System.Linq;

namespace VinkiMod.Module {
    // A class providing some methods that can be called from Lua cutscenes, by doing:
    // local luaCutscenesUtils = require("#ExtendedVariants.Module.LuaCutscenesUtils")
    // luaCutscenesUtils.TriggerIntegerVariant(...)
    [ModExportName("VinkiMod")]
    public static class VinkiModInterop {
        public static void AddGraffitiCharacter(string characterID) {
            VinkiModModule.graffitiUsers=VinkiModModule.graffitiUsers.Append(characterID).ToArray();
        }
    }
}