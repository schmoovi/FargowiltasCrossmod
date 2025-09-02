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
using FargowiltasSouls.Content.Items.Accessories.Eternity;
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

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCProjectileDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601
        public override void Load()
        {
            
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityProjectileCanDamageMethod, CalamityProjectileCanDamage_Detour);

            HookHelper.ModifyMethodWithDetour(BrimstoneMonsterCanHitPlayerMethod, BrimstoneMonsterCanHitPlayer_Detour);

            HookHelper.ModifyMethodWithDetour(FargoSoulsOnSpawnProjMethod, FargoSoulsOnSpawnProj_Detour);
        }
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);
        internal static bool CalamityProjectilePreAI_Detour(Orig_CalamityProjectilePreAI orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            int damage = projectile.damage;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
            }
            bool result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                projectile.damage = damage;
            }
            return result;
        }

        private static readonly MethodInfo CalamityProjectileCanDamageMethod = typeof(CalamityGlobalProjectile).GetMethod("CanDamage", LumUtils.UniversalBindingFlags);
        public delegate bool? Orig_CalamityProjectileCanDamage(CalamityGlobalProjectile self, Projectile projectile);
        public static bool? CalamityProjectileCanDamage_Detour(Orig_CalamityProjectileCanDamage orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool? result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }

        private static readonly MethodInfo BrimstoneMonsterCanHitPlayerMethod = typeof(BrimstoneMonster).GetMethod("CanHitPlayer", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_BrimstoneMonsterCanHitPlayer(BrimstoneMonster self, Player player);
        internal static bool BrimstoneMonsterCanHitPlayer_Detour(Orig_BrimstoneMonsterCanHitPlayer orig, BrimstoneMonster self, Player player)
        {
            if (self.Type != ModContent.ProjectileType<BrimstoneMonster>())
            {
                return orig(self, player);
            }
            float distSQ = self.Projectile.DistanceSQ(player.Center);
            float radiusSQ = MathF.Pow(170f * self.Projectile.scale, 2);
            if (distSQ > radiusSQ)
                return false;
            return orig(self, player);
        }

        private static readonly MethodInfo FargoSoulsOnSpawnProjMethod = typeof(FargoSoulsGlobalProjectile).GetMethod("OnSpawn", LumUtils.UniversalBindingFlags);
        public delegate void Orig_FargoSoulsOnSpawnProj(FargoSoulsGlobalProjectile self, Projectile projectile, IEntitySource source);
        internal static void FargoSoulsOnSpawnProj_Detour(Orig_FargoSoulsOnSpawnProj orig, FargoSoulsGlobalProjectile self, Projectile proj, IEntitySource source)
        {
            if (proj.type == ModContent.ProjectileType<TitaniumRailgunScope>())
            {
                proj.FargoSouls().CanSplit = false;
            }

            orig(self, proj, source);
        }
    }
}
