﻿using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod;
using Terraria.Audio;
using System.Collections.Generic;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using CalamityMod.Buffs.StatDebuffs;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimflameEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(204, 42, 60);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);
            foreach (TooltipLine tooltip in tooltips)
            {
                int index = tooltip.Text.IndexOf("[button]");
                if (index != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index, 8);
                    tooltip.Text = tooltip.Text.Insert(index, CalamityKeybinds.RageHotKey.TooltipHotkeyString());
                }
            }

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Brimflame = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BrimflameScowl>());
            recipe.AddIngredient(ModContent.ItemType<BrimflameRobes>());
            recipe.AddIngredient(ModContent.ItemType<BrimflameBoots>());
            recipe.AddIngredient(ModContent.ItemType<Brimlance>());
            recipe.AddIngredient(ModContent.ItemType<ChaosStone>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void BrimflameBuffActivate()
        {
            if (CalamityKeybinds.RageHotKey.JustPressed && BrimflameCooldown == 0)
            {
                if (ForceEffect(ModContent.ItemType<BrimflameEnchantment>()))
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.AddBuff(ModContent.BuffType<Enraged>(), 300, false, false);
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < Main.npc.Length; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && !npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Player.Center, npc.Center) <= 3000f)
                            {
                                npc.AddBuff(ModContent.BuffType<Enraged>(), 300, false);
                            }
                        }
                    }
                }
                else
                {
                    Player.AddBuff(ModContent.BuffType<BrimflameBuff>(), 300);
                }
                BrimflameCooldown = 360;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/BrimflameAbility"));
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(Player.Center, 0, 0, DustID.Rain_BloodMoon, newColor: new Color(200, 200, 200) * 0.75f, Scale: 2);
                    dust.noGravity = true;
                    dust.velocity *= 5;
                }
            }
        }
    }
}
