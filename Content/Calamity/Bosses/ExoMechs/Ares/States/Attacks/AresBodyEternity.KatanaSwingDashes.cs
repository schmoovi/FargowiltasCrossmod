﻿using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The amount of slash dashes Ares has performed during the KatanaSwingDashes attack.
        /// </summary>
        public ref float KatanaSwingDashes_SlashCounter => ref NPC.ai[0];

        /// <summary>
        /// How long Ares spends redirecting to get near the target during the KatanaSwingDashes attack.
        /// </summary>
        public static int KatanaSwingDashes_RedirectTime => LumUtils.SecondsToFrames(0.6f);

        /// <summary>
        /// How long Ares spends flying away from the target during the KatanaSwingDashes attack.
        /// </summary>
        public static int KatanaSwingDashes_FlyAwayTime => LumUtils.SecondsToFrames(1.1f);

        /// <summary>
        /// How many slash dashes should be performed during the KatanaSwingDashes attack. 
        /// </summary>
        public static int KatanaSwingDashes_SlashCount => 9;

        /// <summary>
        /// The sound played when Ares performs a slash.
        /// </summary>
        public static readonly SoundStyle SlashSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/Slash") with { Volume = 1.2f, MaxInstances = 0 };

        /// <summary>
        /// AI update loop method for the KatanaSlashes attack.
        /// </summary>
        public void DoBehavior_KatanaSwingDashes()
        {
            AnimationState = AresFrameAnimationState.Laugh;

            bool drawBlurSlash = false;

            if (AITimer <= KatanaSwingDashes_RedirectTime)
            {
                float redirectSpeed = MathHelper.Lerp(0.05f, 0.2f, LumUtils.Convert01To010(LumUtils.InverseLerp(0f, 30f, AITimer).Squared()));
                redirectSpeed *= LumUtils.InverseLerp(KatanaSwingDashes_RedirectTime, KatanaSwingDashes_RedirectTime - 45f, AITimer);

                Vector2 hoverDestination = Target.Center + new Vector2(NPC.HorizontalDirectionTo(Target.Center) * -400f, -150f);
                NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, redirectSpeed, 1f - redirectSpeed, 50f);
            }
            else if (AITimer <= KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.velocity.X.NonZeroSign() * 93f, 0.099f);
                NPC.velocity.Y *= 1.018f;

                if (AITimer == KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime)
                {
                    ScreenShakeSystem.StartShake(10f);
                    SoundEngine.PlaySound(LaughSound with { Volume = 10f });
                }
            }
            else
            {
                // Teleport to the other side of the player.
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime + 1)
                {
                    if (LumUtils.CountProjectiles(ModContent.ProjectileType<AresSwingingKatanas>()) <= 0)
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AresSwingingKatanas>(), KatanaDamage, 0f);

                    Vector2 teleportOffset = (NPC.SafeDirectionTo(Target.Center) * new Vector2(1f, 0.5f)).SafeNormalize(Vector2.UnitX);
                    if (KatanaSwingDashes_SlashCounter <= 0f)
                        teleportOffset *= -1f;

                    NPC.Center = Target.Center + teleportOffset * 1850f;
                    NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 37f;

                    KatanaSwingDashes_SlashCounter++;
                    if (KatanaSwingDashes_SlashCounter >= KatanaSwingDashes_SlashCount)
                    {
                        IProjOwnedByBoss<AresBody>.KillAll();
                        NPC.Center = Target.Center - Vector2.UnitY * 800f;
                        NPC.velocity = Vector2.Zero;
                        SelectNewState();
                    }

                    NPC.netUpdate = true;
                }

                if (AITimer % 11f == 10f)
                    SoundEngine.PlaySound(SlashSound, NPC.Center);

                NPC.velocity *= 1.025f;

                if (AITimer >= KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime + 24 && !NPC.WithinRange(Target.Center, 1950f))
                {
                    AITimer = KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime;
                    NPC.netUpdate = true;
                }

                drawBlurSlash = true;
                ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 6f, MathHelper.PiOver2, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f, 700f, 950f);
            }

            NPC.rotation *= 0.45f;

            InstructionsForHands[0] = new(h => KatanaSwingHandUpdate(h, new Vector2(-400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 0, !drawBlurSlash));
            InstructionsForHands[1] = new(h => KatanaSwingHandUpdate(h, new Vector2(-280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 1, !drawBlurSlash));
            InstructionsForHands[2] = new(h => KatanaSwingHandUpdate(h, new Vector2(280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 2, !drawBlurSlash));
            InstructionsForHands[3] = new(h => KatanaSwingHandUpdate(h, new Vector2(400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 3, !drawBlurSlash));
        }

        /// <summary>
        /// Updates one of Ares' hands for the Katana Slashes attack.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand.</param>
        /// <param name="attackDelay">How long the hand should wait before attacking.</param>
        /// <param name="attackCycleTime">The attack cycle time for the slash.</param>
        /// <param name="armIndex">The index of the hand.</param>
        /// <param name="canRender">Whether the hand can be rendered or not.</param>
        public void KatanaSwingHandUpdate(AresHand hand, Vector2 hoverOffset, int attackDelay, int attackCycleTime, int armIndex, bool canRender)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = NPC.Center + hoverOffset * NPC.scale;

            hand.KatanaInUse = true;
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.EnergyKatana;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(0f, 30f, AITimer);
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = 0;
            hand.CanRender = canRender;
            handNPC.damage = 0;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.3f);
            handNPC.Center = hoverDestination;
            handNPC.velocity = Vector2.Zero;
            handNPC.rotation = handNPC.AngleFrom(NPC.Center).AngleLerp(hand.ShoulderToHandDirection, 0.3f);

            if (canRender)
                KatanaSlashesHandUpdate_CreateParticles(hand, handNPC);
        }
    }
}
