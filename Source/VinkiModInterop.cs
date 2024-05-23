using Celeste.Mod.VinkiMod;
using MonoMod.ModInterop;
using System.Linq;

namespace VinkiMod.Module {
    [ModExportName("VinkiMod")]
    public static class VinkiModInterop {
    // NOTES
        // Note for anything asking for LevelIn, you need to append either _Normal, _BSide, or _CSide to the end of the string to make it work!
    // GRAFFITI COMPATIBLE CHARACTERS
        // Adds compatibility for a skin with the ID <characterID> to use graffiti.
        public static void AddGraffitiCharacter(string characterID) {
            VinkiModModule.graffitiUsers=VinkiModModule.graffitiUsers.Append(characterID).ToArray();
        }
    // CUSTOM GRAFFITI
        // Adds a textureNamespace/textureReplaceNamespace, and returns the index. If it returns -1, then something in the code went wrong.
        public static int AddTextureNamespaces(string textureNamespace, string textureReplaceNamespace) {
            VinkiModModule.textureNamespaces=VinkiModModule.textureNamespaces.Append(textureNamespace).ToArray();
            VinkiModModule.textureReplaceNamespaces=VinkiModModule.textureReplaceNamespaces.Append(textureReplaceNamespace).ToArray();
            if (VinkiModModule.textureNamespaces.Length==VinkiModModule.textureReplaceNamespaces.Length) {
                return VinkiModModule.textureNamespaces.Length;
            } else {
                return -1;
            }
        }
        // Adds a new location for artSpots and hasArtSpots. (This will not break if two mods try to create the same level, but it will slightly affect performance!)
        public static void AddArtSpotsLocation(string LevelIn) {
            VinkiModModule.hasArtSpots=VinkiModModule.hasArtSpots.Append(LevelIn).ToArray();
            VinkiModModule.artSpots=VinkiModModule.artSpots.Append([]).ToArray();
        }
        // Adds art spots to the specified level.
        public static void AddArtSpots(string LevelIn,int[][] theArtSpots) {
            if (System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)!=-1) {
                for (var a=0;a<theArtSpots.Length;a++) {
                    VinkiModModule.artSpots[System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)]=VinkiModModule.artSpots[System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)].Append(theArtSpots[a]).ToArray();
                }
            }
        }
        // Adds a single art spot to the specified level. Slightly optimized over using AddArtSpots with only one art spot added.
        public static void AddArtSpot(string LevelIn,int[] theArtSpot) {
            if (System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)!=-1) {
                VinkiModModule.artSpots[System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)]=VinkiModModule.artSpots[System.Array.IndexOf(VinkiModModule.hasArtSpots,LevelIn)].Append(theArtSpot).ToArray();
            }
        }
    // CUSTOM DECALS
        // Adds a DecalNamespace, and returns the index. Be sure to pair with AddTextureNamespaces!
        public static int AddDecalNamespace(string decalNamespace) {
            VinkiModModule.decalNamespaces=VinkiModModule.decalNamespaces.Append(decalNamespace).ToArray();
            return VinkiModModule.decalNamespaces.Length-1;
        }
        // Adds a new location for customDecals and hasCustomDecals. (This will not break if two mods try to create the same level, but it will slightly affect performance!)
        public static void AddCustomDecalsLocation(string LevelIn) {
            VinkiModModule.hasCustomDecals=VinkiModModule.hasCustomDecals.Append(LevelIn).ToArray();
            VinkiModModule.customDecals=VinkiModModule.customDecals.Append([]).ToArray();
        }
        // Adds custom decals to the specified level.
        public static void AddCustomDecals(string LevelIn,int[][] theCustomDecals) {
            if (System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)!=-1) {
                for (var a=0;a<theCustomDecals.Length;a++) {
                    VinkiModModule.customDecals[System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)]=VinkiModModule.customDecals[System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)].Append(theCustomDecals[a]).ToArray();
                }
            }
        }
        // Adds a single custom decal to the specified level. Slightly optimized over using AddCustomDecals with only one decal added.
        public static void AddCustomDecal(string LevelIn,int[] theCustomDecal) {
            if (System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)!=-1) {
                VinkiModModule.customDecals[System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)]=VinkiModModule.customDecals[System.Array.IndexOf(VinkiModModule.hasCustomDecals,LevelIn)].Append(theCustomDecal).ToArray();
            }
        }
    // ALSO REPLACE (compat stuff)
        // Adds an entry to AlsoReplace. This does require you to look at the code to understand.
        public static void AddAlsoReplace(int[] AddedArray) {
            VinkiModModule.alsoReplace=VinkiModModule.alsoReplace.Append([AddedArray[0],AddedArray[1]]).ToArray();
        }
    }
}