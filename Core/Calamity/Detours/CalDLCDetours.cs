using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using Fargowiltas.Content.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Fargowiltas.Content.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Systems;
using CalamityMod.Enums;
using Fargowiltas.Content.Items.Vanity;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.Localization;
using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using FargowiltasSouls.Content.UI;
using CalamityMod.Items.LoreItems;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Terraria.GameContent;
using CalamityMod.Items.Accessories;
using FargowiltasCrossmod.Core.Calamity.Systems;

namespace FargowiltasCrossmod.Core.Calamity.Detours
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601

        public override void Load()
        {
            On_NPC.AddBuff += NPCAddBuff_Detour;
            On_ShimmerTransforms.IsItemTransformLocked += IsItemTransformLocked_Detour;
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(IsFargoSoulsItemMethod, IsFargoSoulsItem_Detour);

            HookHelper.ModifyMethodWithDetour(MinimalEffects_Method, MinimalEffects_Detour);

            HookHelper.ModifyMethodWithDetour(BRDialogueTick_Method, DialogueReplacement);

            HookHelper.ModifyMethodWithDetour(CanToggleEternity_Method, CanToggleEternity_Detour);

            HookHelper.ModifyMethodWithDetour(SoulTogglerOnActivate_Method, SoulTogglerOnActivate_Detour);

            HookHelper.ModifyMethodWithDetour(DetermineDrawEligibility_Method, DetermineDrawEligibility_Detour);

            HookHelper.ModifyMethodWithDetour(GetBestClassDamage_Method, GetBestClassDamage_Detour);

            HookHelper.ModifyMethodWithDetour(FargoSoulsUtil_HighestDamageTypeScaling_Method, HighestDamageTypeScaling_Detour);
        }

        #region Misc

        private static readonly MethodInfo IsFargoSoulsItemMethod = typeof(Squirrel).GetMethod("IsFargoSoulsItem", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_IsFargoSoulsItem(Item item);
        internal static bool IsFargoSoulsItem_Detour(Orig_IsFargoSoulsItem orig, Item item)
        {
            bool result = orig(item);
            if (item.ModItem is not null && item.ModItem.Mod == FargowiltasCrossmod.Instance)
                return true;
            return result;
        }

        private static readonly MethodInfo MinimalEffects_Method = typeof(ToggleBackend).GetMethod("MinimalEffects", LumUtils.UniversalBindingFlags);
        public delegate void Orig_MinimalEffects(ToggleBackend self);
        internal static void MinimalEffects_Detour(Orig_MinimalEffects orig, ToggleBackend self)
        {
            orig(self);
            Player player = Main.LocalPlayer;
            //player.SetToggleValue<OccultSkullCrownEffect>(true);
            player.SetToggleValue<PurityEffect>(true);
            player.SetToggleValue<TheSpongeEffect>(true);
            player.SetToggleValue<ChaliceOfTheBloodGodEffect>(true);
            player.SetToggleValue<YharimsGiftEffect>(true);
            player.SetToggleValue<DraedonsHeartEffect>(true);
            player.SetToggleValue<NebulousCoreEffect>(false);
            player.SetToggleValue<CalamityEffect>(true);

            player.SetToggleValue<AerospecJumpEffect>(true);

            player.SetToggleValue<NanotechEffect>(true);
            player.SetToggleValue<AbyssalDivingSuitEffect>(true);
            player.SetToggleValue<NucleogenesisEffect>(true);
            player.SetToggleValue<ElementalQuiverEffect>(true);
            player.SetToggleValue<ElementalGauntletEffect>(true);
            player.SetToggleValue<EtherealTalismanEffect>(true);
            player.SetToggleValue<AmalgamEffect>(true);
            player.SetToggleValue<AsgardianAegisEffect>(true);
            player.SetToggleValue<RampartofDeitiesEffect>(true);

        }
        private static readonly MethodInfo BRDialogueTick_Method = typeof(BossRushDialogueSystem).GetMethod("Tick", LumUtils.UniversalBindingFlags);
        public delegate void Orig_BRDialogueTick();
        public static void DialogueReplacement(Orig_BRDialogueTick orig)
        {

            BossRushDialoguePhase phase = BossRushDialogueSystem.Phase;
            FieldInfo tierInfo = typeof(BossRushEvent).GetField("CurrentTier");
            if (tierInfo != null)
            {
                tierInfo.SetValue(tierInfo, 1);
            }
            else
            {
                //Main.NewText(BossRushEvent.BossRushStage);
            }
            //BossRushEvent.BossRushStage = 16;
            //DownedBossSystem.startedBossRushAtLeastOnce = true;
            //Main.NewText(BossRushEvent.Bosses[BossRushEvent.Bosses.Count - 1].EntityID);
            //Main.NewText(ModContent.NPCType<MutantBoss>());
            if (BossRushDialogueSystem.CurrentDialogueDelay > 0 && phase == BossRushDialoguePhase.Start)
            {
                BossRushDialogueSystem.CurrentDialogueDelay -= 5;
                if (BossRushDialogueSystem.CurrentDialogueDelay < 0)
                {
                    BossRushDialogueSystem.CurrentDialogueDelay = 0;
                }
            }
            if (!BossRushEvent.BossRushActive || BossRushDialogueSystem.Phase == BossRushDialoguePhase.Start || BossRushDialogueSystem.Phase == BossRushDialoguePhase.None)
            {

                orig();
                return;
            }
            int currSequenceLength = 0;
            int currLine = BossRushDialogueSystem.currentSequenceIndex;

            if (phase == BossRushDialoguePhase.StartRepeat)
            {
                currSequenceLength = 1;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete)
            {
                currSequenceLength = 3;
            }

            if (BossRushDialogueSystem.CurrentDialogueDelay == 0)
            {
                if (phase == BossRushDialoguePhase.StartRepeat && currLine == 0)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.Start"), Color.Teal);
                    BossRushEvent.BossRushStage = 1;
                }
                if (phase == BossRushDialoguePhase.TierOneComplete)
                {
                    if (currLine == 0)
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.EndP1_1"), Color.Teal);
                    //if (currLine == 1)

                    if (currLine == 2)
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.EndP1_2"), Color.Teal);
                }
                BossRushDialogueSystem.CurrentDialogueDelay = 60;
                BossRushDialogueSystem.currentSequenceIndex += 1;

            }

            else
            {
                --BossRushDialogueSystem.CurrentDialogueDelay;
            }
            if (phase == BossRushDialoguePhase.End || phase == BossRushDialoguePhase.EndRepeat)
            {
                BossRushDialogueSystem.CurrentDialogueDelay = 0;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete && currLine < 6)
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 0, 0.05f);
            }
            if (phase == BossRushDialoguePhase.TierOneComplete && currLine > 6
                )
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 1, 0.001f);
            }
            if (BossRushEvent.BossRushSpawnCountdown < 180 && currLine < currSequenceLength)
                BossRushEvent.BossRushSpawnCountdown = BossRushDialogueSystem.CurrentDialogueDelay + 180;
        }
        private static readonly MethodInfo CanToggleEternity_Method = typeof(Masochist).GetMethod("CanToggleEternity", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CanToggleEternity();
        internal static bool CanToggleEternity_Detour(Orig_CanToggleEternity orig)
        {
            orig();
            return false;
        }
        private static readonly MethodInfo SoulTogglerOnActivate_Method = typeof(SoulTogglerButton).GetMethod("OnActivate", LumUtils.UniversalBindingFlags);
        public delegate void Orig_SoulTogglerOnActivate(SoulTogglerButton self);
        internal static void SoulTogglerOnActivate_Detour(Orig_SoulTogglerOnActivate orig, SoulTogglerButton self)
        {
            orig(self);
            self.OncomingMutant.TextHoldShift = $"{Language.GetTextValue("Mods.FargowiltasCrossmod.UI.ToggledWithCal")}]\n[c/787878:{self.OncomingMutant.TextHoldShift}";
        }
        private static readonly MethodInfo DetermineDrawEligibility_Method = typeof(BossRushSky).GetMethod("DetermineDrawEligibility", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_DetermineDrawEligibility();
        internal static bool DetermineDrawEligibility_Detour(Orig_DetermineDrawEligibility orig)
        {
            if (SkyManager.Instance["CalamityMod:BossRush"] != null && SkyManager.Instance["CalamityMod:BossRush"].IsActive())
                SkyManager.Instance.Deactivate("CalamityMod:BossRush", new object[0]);
            if (Filters.Scene["CalamityMod:BossRush"].IsActive())
                Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            return false;
        }

        private static readonly MethodInfo GetBestClassDamage_Method = typeof(CalamityUtils).GetMethod("GetBestClassDamage", LumUtils.UniversalBindingFlags);
        public delegate StatModifier Orig_GetBestClassDamage(Player player);
        internal static StatModifier GetBestClassDamage_Detour(Orig_GetBestClassDamage orig, Player player)
        {
            orig(player);
            StatModifier ret = StatModifier.Default;
            StatModifier classless = player.GetTotalDamage<GenericDamageClass>();
            ret.Base = classless.Base;
            ret *= classless.Multiplicative;
            ret.Flat = classless.Flat;
            List<float> bestClass =
            [
            1f,
            player.GetTotalDamage<MeleeDamageClass>().Additive,
            player.GetTotalDamage<RangedDamageClass>().Additive,
            player.GetTotalDamage<MagicDamageClass>().Additive,
            player.GetTotalDamage<SummonDamageClass>().Additive * 0.75f, // BalancingConstants.SummonAllClassScalingFactor
            player.GetTotalDamage<RogueDamageClass>().Additive - player.Calamity().stealthDamage // kill the exploit
            ];
            ret += bestClass.Max() - 1f;
            return ret;
        }
        private static readonly MethodInfo FargoSoulsUtil_HighestDamageTypeScaling_Method = typeof(FargoSoulsUtil).GetMethod("HighestDamageTypeScaling", LumUtils.UniversalBindingFlags);
        public delegate int Orig_FargoSoulsUtil_HighestDamageTypeScaling(Player player, int dmg);
        internal static int HighestDamageTypeScaling_Detour(Orig_FargoSoulsUtil_HighestDamageTypeScaling orig, Player player, int dmg)
        {
            orig(player, dmg);
            List<float> types =
            [
                player.ActualClassDamage(DamageClass.Melee),
                player.ActualClassDamage(DamageClass.Ranged),
                player.ActualClassDamage(DamageClass.Magic),
                player.ActualClassDamage(DamageClass.Summon),
                player.GetTotalDamage<RogueDamageClass>().Additive * player.GetTotalDamage<RogueDamageClass>().Multiplicative - player.Calamity().stealthDamage // prevent the exploit
            ];

            return (int)(types.Max() * dmg);
        }
        #endregion
        #region Vanilla Detours
        internal static void NPCAddBuff_Detour(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            if (self.TryGetGlobalNPC(out CalDLCNPCChanges n) && self.CalamityDLC().ImmuneToAllDebuffs)
                return;
            orig(self, type, time, quiet);
        }
        internal static bool IsItemTransformLocked_Detour(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            if (type == ModContent.ItemType<ProfanedSoulCrystal>() || type == ModContent.ItemType<LoreCynosure>())
                return !WorldSavingSystem.DownedMutant;
            return orig(type);
        }
        #endregion
    }
}
