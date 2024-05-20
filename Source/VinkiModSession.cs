using System;
using System.Collections.Generic;

namespace Celeste.Mod.VinkiMod;
public class VinkiModSession : EverestModuleSession {
    public int[][] sessionArtSpots = [];
    public bool sessionStuffLoaded = false;
    public int[] vinkiRenderIt = [0,0,0,0];
    public bool AlwaysGrafButton = false;
}