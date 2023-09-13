﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemTechMelee : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechYellowGem";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 300;
            Projectile.scale = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, TorchID.Yellow);
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch).noGravity = true;
        }

    }
}
