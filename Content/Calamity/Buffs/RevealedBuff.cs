using CalamityMod;
using CalamityMod.CalPlayer;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Assets.Textures;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class RevealedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer cal = player.Calamity();
            player.CalamityDLC().Revealed = true;
            //player.GetAttackSpeed<RogueDamageClass>() /= 1.5f;
            cal.rogueVelocity /= 1.5f;
            cal.stealthGenMoving /= 1.5f;
            cal.stealthGenStandstill /= 1.5f;

            player.GetDamage<RogueDamageClass>() /= 1.5f;
        }
    }
}
