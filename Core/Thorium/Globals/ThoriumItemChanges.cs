
using System.Collections.Generic;
using FargowiltasSouls.Content.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Items.BossForgottenOne;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.HealerItems;

namespace FargowiltasCrossmod.Core.Thorium.Globals
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    class ThoriumItemBalance : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true;
        }
        
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            const string BalanceLine = "Cross-mod Balance: ";
            const string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
            const string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";
            
            if (ThoriumPotionNerfs.NerfedPotions.ContainsKey(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDisable", Language.GetTextValue($"{BalanceDownLine}Disabled")));
            }
            if (item.ModItem is BardItem bardItem && item.damage > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Only 4 empowerments can be active at once")));
            }
            if (item.type == ModContent.ItemType<LeechBolt>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Will only add 1/4 of bonus healing")));
            }
            if (item.type == ModContent.ItemType<ShinobiSigil>() && !item.social)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Critical strike effect can only be activated every 5 seconds")));
            }
            if (item.type == ModContent.ItemType<AbyssalShell>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Shell only lasts for 10 seconds and has a 60 second cooldown")));
            }
        }
    }
}

