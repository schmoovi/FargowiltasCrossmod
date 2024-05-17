﻿using System;
using CalamityMod;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EyeOfCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDeerclops : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Deerclops);
        public override bool SafePreAI(NPC npc)
        {
            // Code mostly from Calamity Mod Deerclops.

            DeerclopsAI.shouldDrawEnrageBorder = WorldSavingSystem.EternityMode;
            float modifier = 1f; // multiplier for aura size

            if (npc.target.WithinBounds(Main.player.Length) && Main.player[npc.target].dead)
            {
                DeerclopsAI.hasTargetBeenInRange = false;
                DeerclopsAI.borderScalar = 0.9f * modifier;
            }
            if (DeerclopsAI.borderDelay > 0f)
                DeerclopsAI.borderDelay -= 1f;

            if (!DeerclopsAI.hasTargetBeenInRange && npc.target.WithinBounds(Main.player.Length) && !Main.player[npc.target].dead)
            {
                // Target entered the border for the first time
                DeerclopsAI.hasTargetBeenInRange = true;
                if (DeerclopsAI.borderDelay > 120f)
                    DeerclopsAI.borderDelay = 120f;
            }
            if (DeerclopsAI.innerBorder != DeerclopsAI.increaseDRTriggerDistance || DeerclopsAI.maxDRIncreaseDistance != DeerclopsAI.outerBorder)
            {
                // Adjust the border IF the new value is lower (helps prevent jumping if you enter the border early while it's on screen but not finished zooming in)
                var LerpValue = Utils.GetLerpValue(DeerclopsAI.hasTargetBeenInRange ? 120f : 180f, 0f, DeerclopsAI.borderDelay, true);
                var newInner = MathHelper.Lerp(DeerclopsAI.maxDRIncreaseDistance * 5f, DeerclopsAI.increaseDRTriggerDistance, LerpValue);
                if (newInner < DeerclopsAI.innerBorder)
                    DeerclopsAI.innerBorder = newInner;
                var newOuter = MathHelper.Lerp(DeerclopsAI.maxDRIncreaseDistance * 5f, DeerclopsAI.maxDRIncreaseDistance, LerpValue);
                if (newOuter < DeerclopsAI.outerBorder)
                    DeerclopsAI.outerBorder = newOuter;
            }
            if ((DeerclopsAI.hasTargetBeenInRange && DeerclopsAI.borderScalar < 1f) || DeerclopsAI.borderDelay > 0f)
            {
                // Fade in, with full opacity only available after being inside the border for the first time
                DeerclopsAI.borderScalar = MathHelper.Clamp(DeerclopsAI.borderScalar + 0.015f, 0f, (DeerclopsAI.hasTargetBeenInRange ? 1f : 0.9f) * modifier);
            }
            DeerclopsAI.lastDeerclopsPosition = npc.Center;
            return base.SafePreAI(npc);
        }
    }
}
