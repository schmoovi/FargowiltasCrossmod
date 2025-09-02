using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasSouls;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        public static int RepositionTime => 30;
        /// <summary>
        /// AI update loop method for the Reposition state. This state is executed inbetween each normal attack.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_Reposition(NPC npc, IExoTwin twinAttributes)
        {

            // Get near the target.
            if (AITimer < RepositionTime)
            {
                if (AITimer <= 2)
                    npc.velocity *= 0.3f;

                npc.Center = Vector2.Lerp(npc.Center, Target.Center + Target.SafeDirectionTo(npc.Center) * 640f, AITimer / (float)RepositionTime * 0.12f);

                npc.velocity *= 0.84f;
                float rotationMovementInterpolant = LumUtils.InverseLerp(0f, RepositionTime, AITimer);
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), rotationMovementInterpolant * 0.18f);
                return;
            }
            else
            {
                ExoTwinsStateManager.TransitionToNextState();
            }
        }
    }
}
