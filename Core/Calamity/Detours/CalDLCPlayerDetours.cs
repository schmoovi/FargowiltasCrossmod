using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using Fargowiltas.Content.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Fargowiltas.Content.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Systems;
using CalamityMod.Enums;
using Fargowiltas.Content.Items.Vanity;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.Localization;
using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using FargowiltasSouls.Content.UI;
using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Content.Calamity.Bosses.Perforators;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Common.Utilities;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Pets;
using FargowiltasSouls.Content.Items.Misc;
using Fargowiltas.Content.Items.Explosives;
using FargowiltasSouls.Content.Items.Accessories;
using Fargowiltas.Content.Items.Tiles;
using CalamityMod.Walls;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.FurnitureAcidwood;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.FurnitureMonolith;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.SunkenSea;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Terraria.GameContent;
using CalamityMod.Items.Accessories;

namespace FargowiltasCrossmod.Core.Calamity.Detours
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCPlayerDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601
        public override void Load()
        {

        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(FargoPlayerPreKill_Method, FargoPlayerPreKill_Detour);

            HookHelper.ModifyMethodWithDetour(EModePlayerPreUpdate_Method, EModePlayerPreUpdate_Detour);

            HookHelper.ModifyMethodWithDetour(CalamityPlayer_CatchFish_Method, CalamityPlayer_CatchFish_Detour);

            HookHelper.ModifyMethodWithDetour(ModifyHurtInfo_CalamityMethod, ModifyHurtInfo_Calamity_Detour);

            HookHelper.ModifyMethodWithDetour(GetAdrenalineDamage_Method, GetAdrenalineDamage_Detour);
        }
        private static readonly MethodInfo FargoPlayerPreKill_Method = typeof(FargoSoulsPlayer).GetMethod("PreKill", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_FargoPlayerPreKill(FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
        internal static bool FargoPlayerPreKill_Detour(Orig_FargoPlayerPreKill orig, FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retval = orig(self, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
            if (!retval)
            {
                CalamityPlayer calPlayer = self.Player.Calamity();
                calPlayer.chaliceBleedoutBuffer = 0D;
                calPlayer.chaliceDamagePointPartialProgress = 0D;
                calPlayer.chaliceHitOriginalDamage = 0;
            }
            return retval;
        }

        private static readonly MethodInfo EModePlayerPreUpdate_Method = typeof(EModePlayer).GetMethod("PreUpdate", LumUtils.UniversalBindingFlags);
        public delegate void Orig_EModePlayerPreUpdate(EModePlayer self);
        internal static void EModePlayerPreUpdate_Detour(Orig_EModePlayerPreUpdate orig, EModePlayer self)
        {
            FargoSoulsPlayer soulsPlayer = self.Player.FargoSouls();
            bool antibodies = soulsPlayer.MutantAntibodies;
            if (self.Player.Calamity().oceanCrest || self.Player.Calamity().aquaticEmblem)
            {
                soulsPlayer.MutantAntibodies = true;
            }
            orig(self);
            soulsPlayer.MutantAntibodies = antibodies;
        }

        private static readonly MethodInfo CalamityPlayer_CatchFish_Method = typeof(CalamityPlayer).GetMethod("CatchFish", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityPlayer_CatchFish(CalamityPlayer self, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);
        internal static void CalamityPlayer_CatchFish_Detour(Orig_CalamityPlayer_CatchFish orig, CalamityPlayer self, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            int index = self.Player.FindBuffIndex(BuffID.Gills);
            if (index >= 0) self.Player.buffType[index] = 0;
            orig(self, attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition);
            if (index >= 0) self.Player.buffType[index] = BuffID.Gills;

        }
        private static readonly MethodInfo ModifyHurtInfo_CalamityMethod = typeof(CalamityPlayer).GetMethod("ModifyHurtInfo_Calamity", LumUtils.UniversalBindingFlags);
        public delegate void Orig_ModifyHurtInfo_Calamity(CalamityPlayer self, ref Player.HurtInfo info);
        internal static void ModifyHurtInfo_Calamity_Detour(Orig_ModifyHurtInfo_Calamity orig, CalamityPlayer self, ref Player.HurtInfo info)
        {
            bool chalice = self.chaliceOfTheBloodGod;
            if (self.Player.FargoSouls().GuardRaised || self.Player.FargoSouls().MutantPresence)
                self.chaliceOfTheBloodGod = false;
            orig(self, ref info);
            self.chaliceOfTheBloodGod = chalice;
        }

        private static readonly MethodInfo GetAdrenalineDamage_Method = typeof(CalamityUtils).GetMethod("GetAdrenalineDamage", LumUtils.UniversalBindingFlags);
        public delegate float Orig_GetAdrenalineDamage(CalamityPlayer mp);
        internal static float GetAdrenalineDamage_Detour(Orig_GetAdrenalineDamage orig, CalamityPlayer mp)
        {
            float value = orig(mp);
            if (WorldSavingSystem.EternityMode)
                value *= 0.5f;
            return value;
        }
    }
}
