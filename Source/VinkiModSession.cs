using System;
using System.Collections.Generic;

namespace Celeste.Mod.VinkiMod;
public class VinkiModSession : EverestModuleSession {
    public int[][] sessionArtSpots = [];
    public bool sessionStuffLoaded = false;
    public float[] vinkiRenderIt = [0,0,0,0,0,0,0];
}