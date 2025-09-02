using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Rogue;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VagabondsSoul : BaseSoul
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Items/Accessories/Souls/" + Name;
        protected override Color? nameColor => new Color(217, 144, 67);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += 0.22f;
            player.Calamity().rogueVelocity += 0.2f;
            player.GetCritChance<RogueDamageClass>() += 10;
            if (player.AddEffect<NanotechEffect>(Item))
            {
                ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Nanotech>()

                .AddIngredient<Valediction>()
                .AddIngredient<GodsParanoia>()
                .AddIngredient<Eradicator>()
                .AddIngredient<Wrathwing>()
                .AddIngredient<Seraphim>()

                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NanotechEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<Nanotech>();
    }
}
