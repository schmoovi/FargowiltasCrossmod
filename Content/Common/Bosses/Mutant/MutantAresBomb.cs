using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantAresBomb : MutantBomb
    {
        public override string Texture => "FargowiltasCrossmod/Content/Common/Bosses/Mutant/MutantAresBomb";
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 60 * 5);
            base.OnHitPlayer(target, info);
        }
        public int vfxinterpolant;
        public override bool PreAI()
        {
            vfxinterpolant++;
            return base.PreAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D flare2 = FargoAssets.Smoke.Value;
            Texture2D flare = FargoAssets.Scorch.Value;

            float smokesize = MathHelper.Lerp(0, 1.5f, vfxinterpolant * 0.07f);

            Main.spriteBatch.Draw(flare2, Projectile.Center - Main.screenPosition, null, Color.Gray with { A = 0 } * Projectile.Opacity, Projectile.rotation, flare2.Size() * 0.5f, smokesize, 0, 0f);
            Main.spriteBatch.Draw(flare, Projectile.Center - Main.screenPosition, null, Color.Lime with { A = 0 } * Projectile.Opacity * 0.75f, Projectile.rotation, flare.Size() * 0.5f, smokesize, 0, 0f);
            Main.spriteBatch.Draw(flare, Projectile.Center - Main.screenPosition, null, Color.LimeGreen with { A = 0 } * Projectile.Opacity, Projectile.rotation, flare.Size() * 0.5f, smokesize * 0.5f, 0, 0f);
            return false;
        }
    }
}
