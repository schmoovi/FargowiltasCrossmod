using System;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core;

public static class ModCompatibility
{
    public static class MutantMod
    {
        public const string Name = "Fargowiltas";
        public static bool Loaded => ModLoader.HasMod(Name);
        private static Mod mod = null;
        public static Mod Mod
        {
            get
            {
                mod ??= ModLoader.GetMod(Name);
                return mod;
            }
        }
    }
    public static class SoulsMod
    {
        public const string Name = "FargowiltasSouls";
        public static bool Loaded => ModLoader.HasMod(Name);
        private static FargowiltasSouls.FargowiltasSouls mod = null;
        public static FargowiltasSouls.FargowiltasSouls Mod
        {
            get
            {
                mod ??= (FargowiltasSouls.FargowiltasSouls)ModLoader.GetMod(Name);
                return mod;
            }
        }
       
    }
    public static class Calamity
    {
        // Please use this to avoid typo bugs
        public const string Name = "CalamityMod";

        // TODO: cache, lazy property
        public static bool Loaded => ModLoader.HasMod(Name);
        private static Mod mod = null;
        public static Mod Mod
        {
            get
            {
                mod ??= ModLoader.GetMod(Name);
                return mod;
            }
        }

    }
    public static class ThoriumMod
    {
        public const string Name = "ThoriumMod";
        public static bool Loaded => ModLoader.HasMod(Name);
        private static Mod mod = null;
        public static Mod Mod
        {
            get
            {
                mod ??= ModLoader.GetMod(Name);
                return mod;
            }
        }
    }
    public static class InfernumMode
    {
        public const string Name = "InfernumMode";
        public static bool Loaded => ModLoader.HasMod(Name);
        private static Mod mod = null;
        public static Mod Mod
        {
            get
            {
                mod ??= ModLoader.GetMod(Name);
                return mod;
            }
        }
        public static bool InfernumDifficulty => Loaded && (bool)Mod.Call("GetInfernumActive");
    }
    public static class WrathoftheGods
    {
        public const string Name = "NoxusBoss";
        public static bool Loaded => ModLoader.HasMod(Name);
        private static Mod mod = null;
        public static Mod Mod
        {
            get
            {
                mod ??= ModLoader.GetMod(Name);
                return mod;
            }
        }

        public static ModNPC NoxusBoss1 = Mod.Find<ModNPC>(Mod.Version >= new Version(1, 2, 0) ? "AvatarRift" : "NoxusEgg");
        public static ModNPC NoxusBoss2 = Mod.Find<ModNPC>(Mod.Version >= new Version(1, 2, 0) ? "AvatarOfEmptiness" : "EntropicGod");
        public static ModNPC NamelessDeityBoss = Mod.Find<ModNPC>("NamelessDeityBoss");
    }
    public static class BossChecklist
    {
        public static void AdjustValues()
        {

        }
    }
}