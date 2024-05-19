using System;
using Celeste;
using Celeste.Mod.VinkiMod;
using ExtendedVariants.Module;
using MonoMod.Utils;
using On.Celeste;

namespace ExtendedVariants.Variants;

public class AlwaysPressGraffitiButton : AbstractExtendedVariant
{
    public override void Load()
    {
        VinkiModModule.Session.AlwaysGrafButton=true;
    }

    public override void Unload()
    {
        VinkiModModule.Session.AlwaysGrafButton=false;
    }

    public override Type GetVariantType()
    {
        return typeof(bool);
    }

    public override object GetDefaultVariantValue()
    {
        return false;
    }

    public override object ConvertLegacyVariantValue(int value)
    {
        return value != 0;
    }
}