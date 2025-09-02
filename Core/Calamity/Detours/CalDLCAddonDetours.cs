using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Mono.Cecil.Cil;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Luminance.Core.Hooking;
using FargowiltasSouls.Content.Items;
using System.Reflection;
using CalamityMod.CalPlayer;

namespace FargowiltasCrossmod.Core.Calamity.Detours
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601
        #region MethodInfo
        #endregion

        #region Orig
        #endregion

        public override void Load()
        {
            On_Player.RefreshDoubleJumps += RefreshDoubleJumps_Detour;
            On_Player.RefreshMovementAbilities += RefreshMovementAbilities_Detour;
            On_Player.GrappleMovement += GrappleMovement_Detour;
        }

        void ICustomDetourProvider.ModifyMethods()
        {
            
        }

        private void RefreshDoubleJumps_Detour(On_Player.orig_RefreshDoubleJumps orig, Player self)
        {
            AerospecJumpEffect.ResetAeroCrit(self);
            orig(self);
        }
        private void RefreshMovementAbilities_Detour(On_Player.orig_RefreshMovementAbilities orig, Player self, bool doubleJumps = true)
        {
            AerospecJumpEffect.ResetAeroCrit(self);
            orig(self, doubleJumps);
        }
        private void GrappleMovement_Detour(On_Player.orig_GrappleMovement orig, Player self)
        {
            orig(self);
            if (self.grappling[0] < 0)
                return;
            AerospecJumpEffect.ResetAeroCrit(self);
        }
    }
}
