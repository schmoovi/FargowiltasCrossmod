using CalamityMod;
using FargowiltasSouls.Core.Systems;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Pets;
using FargowiltasSouls.Content.Items.Misc;
using Fargowiltas.Content.Items.Explosives;
using FargowiltasSouls.Content.Items.Accessories;
using Fargowiltas.Content.Items.Tiles;
using CalamityMod.Walls;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.FurnitureAcidwood;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.FurnitureMonolith;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.SunkenSea;
using Luminance.Core.Hooking;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria.DataStructures;
using System.Linq;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using CalamityMod.Items.SummonItems;

namespace FargowiltasCrossmod.Core.Calamity.Detours
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCItemDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601


        public override void Load()
        {
            On_Player.ItemCheck_CheckCanUse += AllowUseCalBossSummons;
        }

        private bool AllowUseCalBossSummons(On_Player.orig_ItemCheck_CheckCanUse orig, Player self, Item sItem)
        {
            if (CalDLCSets.Items.CalBossSummon[ sItem.type] && Fargowiltas.Common.Configs.FargoServerConfig.Instance.EasySummons)
            {
                return true;
            }
            return orig(self, sItem);
        }

        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(TungstenIncreaseWeaponSizeMethod, TungstenIncreaseWeaponSize_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenNeverAffectsProjMethod, TungstenNeverAffectsProj_Detour);

            HookHelper.ModifyMethodWithDetour(OriDotModifierMethod, OriDotModifier_Detour);

            HookHelper.ModifyMethodWithDetour(StarterBag_ModifyItemLoot_Method, StarterBag_ModifyItemLoot_Detour);
            HookHelper.ModifyMethodWithDetour(FargosSouls_DropDevianttsGift_Method, FargosSouls_DropDevianttsGift_Detour);

            HookHelper.ModifyMethodWithDetour(Instahouse_GetTiles_Method, Instahouse_GetTiles_Detour);
            HookHelper.ModifyMethodWithDetour(Instahouse_GetFurniture_Method, Instahouse_GetFurniture_Detour);

            HookHelper.ModifyMethodWithDetour(FMSVerticalSpeedMethod, FMSVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(FMSHorizontalSpeedMethod, FMSHorizontalSpeed_Detour);

            HookHelper.ModifyMethodWithDetour(LifeForceVerticalSpeedMethod, LifeForceVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(LifeForceHorizontalSpeedMethod, LifeForceHorizontalSpeed_Detour);

            HookHelper.ModifyMethodWithDetour(EmodeBalancePerID_Method, EmodeBalancePerID_Detour);

            HookHelper.ModifyMethodWithDetour(TryUnlimBuffMethod, TryUnlimBuff_Detour);

            HookHelper.ModifyMethodWithDetour(ApprenticeSupportMethod, ApprenticeSupport_Detour);
        }
        private static readonly MethodInfo TungstenIncreaseWeaponSizeMethod = typeof(TungstenEffect).GetMethod("TungstenIncreaseWeaponSize", LumUtils.UniversalBindingFlags);
        public delegate float Orig_TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer);
        internal static float TungstenIncreaseWeaponSize_Detour(Orig_TungstenIncreaseWeaponSize orig, FargoSoulsPlayer modPlayer)
        {
            float value = orig(modPlayer);
            if (modPlayer.Player.HeldItem == null)
                return value;
            if (CalDLCSets.Items.TungstenExclude[modPlayer.Player.HeldItem.type])
                return 1f;
            //if (modPlayer.Player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
            //    value -= (value - 1f) * 0.5f;
            return value;
        }
        private static readonly MethodInfo TungstenNeverAffectsProjMethod = typeof(TungstenEffect).GetMethod("TungstenNeverAffectsProj", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_TungstenNeverAffectsProj(Projectile projectile);
        internal static bool TungstenNeverAffectsProj_Detour(Orig_TungstenNeverAffectsProj orig, Projectile projectile)
        {
            bool value = orig(projectile);
            if (CalDLCSets.Projectiles.TungstenExclude[projectile.type])
                return true;
            return value;
        }

        private static readonly MethodInfo OriDotModifierMethod = typeof(OrichalcumEffect).GetMethod("OriDotModifier", LumUtils.UniversalBindingFlags);
        public delegate float Orig_OriDotModifier(NPC npc, FargoSoulsPlayer modPlayer);
        internal static float OriDotModifier_Detour(Orig_OriDotModifier orig, NPC npc, FargoSoulsPlayer modPlayer)
        {
            float value = orig(npc, modPlayer);

            value = 1.5f;
            if (modPlayer.Player.ForceEffect<OrichalcumEffect>())
            {
                value = 2f;
            }
            if (npc.Calamity().shellfishVore > 0)
            {
                value = ((value - 1) / 2) + 1; // halved bonus
            }
            return value;
        }

        private static readonly MethodInfo StarterBag_ModifyItemLoot_Method = typeof(StarterBag).GetMethod("ModifyItemLoot", LumUtils.UniversalBindingFlags);
        public delegate void Orig_StarterBag_ModifyItemLoot(StarterBag self, ItemLoot itemLoot);
        internal static void StarterBag_ModifyItemLoot_Detour(Orig_StarterBag_ModifyItemLoot orig, StarterBag self, ItemLoot itemLoot)
        {
            itemLoot.Add(ItemID.SilverPickaxe);
            itemLoot.Add(ItemID.SilverAxe);
            itemLoot.Add(ItemID.SilverHammer);



            LeadingConditionRule tin = itemLoot.DefineConditionalDropSet(() => WorldGen.SavedOreTiers.Copper == TileID.Tin);
            tin.Add(ItemID.TinBroadsword);
            tin.Add(ItemID.TinBow);
            tin.Add(ItemID.TopazStaff);
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBroadsword, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBow, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.AmethystStaff, 1));
            itemLoot.Add(ItemID.WoodenArrow, 1, 100, 100);
            itemLoot.Add(ModContent.ItemType<SquirrelSquireStaff>());
            itemLoot.Add(ModContent.ItemType<ThrowingBrick>(), 1, 150, 150);

            itemLoot.Add(ItemID.BugNet);
            itemLoot.Add(ItemID.WaterCandle);
            itemLoot.Add(ItemID.Torch, 1, 200, 200);
            itemLoot.Add(ItemID.LesserHealingPotion, 1, 15, 15);
            itemLoot.Add(ItemID.RecallPotion, 1, 15, 15);
            LeadingConditionRule mp = itemLoot.DefineConditionalDropSet(() => Main.netMode != NetmodeID.SinglePlayer);
            mp.Add(ItemID.WormholePotion, 1, 15, 15);
            LeadingConditionRule dontDigUp = itemLoot.DefineConditionalDropSet(() => Main.remixWorld || Main.zenithWorld);
            dontDigUp.Add(ItemID.ObsidianSkinPotion, 1, 5, 5);
            itemLoot.Add(ModContent.ItemType<EternityAdvisor>());
            itemLoot.Add(ModContent.ItemType<AutoHouse>(), 1, 2, 2);
            itemLoot.Add(ModContent.ItemType<MiniInstaBridge>(), 1, 2, 2);
            itemLoot.Add(ModContent.ItemType<EurusSock>());
            itemLoot.Add(ModContent.ItemType<PuffInABottle>());
            itemLoot.Add(ItemID.Squirrel);

            static bool isTerry(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName.ToLower().Contains("terry");
            }
            itemLoot.AddIf(isTerry, ModContent.ItemType<HalfInstavator>());
            itemLoot.AddIf(isTerry, ModContent.ItemType<RegalStatue>());
            itemLoot.AddIf(isTerry, ItemID.PlatinumCoin);
            itemLoot.AddIf(isTerry, ItemID.GrapplingHook);
            itemLoot.AddIf(isTerry, ItemID.LifeCrystal, new Fraction(1, 1), 4, 4);
            itemLoot.AddIf(isTerry, ItemID.ManaCrystal, new Fraction(1, 1), 2, 2);
            itemLoot.AddIf(isTerry, ModContent.ItemType<SandsofTime>());

            //fuck you terry
            static bool notreceivedStorage(DropAttemptInfo info)
            {
                return !WorldSavingSystem.ReceivedTerraStorage;
            }
            static bool TerryAndNotReceived(DropAttemptInfo info)
            {
                return notreceivedStorage(info) && isTerry(info);
            }
            static bool notTerryAndNotReceived(DropAttemptInfo info)
            {
                return notreceivedStorage(info) && !isTerry(info);
            }
            IItemDropRule notrecievedStorage = itemLoot.DefineConditionalDropSet(() => !WorldSavingSystem.ReceivedTerraStorage);
            if (ModLoader.HasMod("MagicStorage"))
            {
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorage", "StorageHeart").Type);
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorage", "CraftingAccess").Type);
                itemLoot.AddIf(TerryAndNotReceived, ModContent.Find<ModItem>("MagicStorage", "StorageUnit").Type, new Fraction(1, 1), 16, 16);
                itemLoot.AddIf(notTerryAndNotReceived, ModContent.Find<ModItem>("MagicStorage", "StorageUnit").Type, new Fraction(1, 1), 4, 4);

            }
            else if (ModLoader.HasMod("MagicStorageExtra"))
            {
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorageExtra", "StorageHeart").Type);
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorageExtra", "CraftingAccess").Type);
                itemLoot.AddIf(TerryAndNotReceived, ModContent.Find<ModItem>("MagicStorageExtra", "StorageUnit").Type, new Fraction(1, 1), 16, 16);
                itemLoot.AddIf(notTerryAndNotReceived, ModContent.Find<ModItem>("MagicStorageExtra", "StorageUnit").Type, new Fraction(1, 1), 4, 4);
            }
            //itemLoot.Add(isTerry);
            if (ModLoader.TryGetMod("CalamityModMusic", out Mod musicMod))
                itemLoot.Add(musicMod.Find<ModItem>("CalamityMusicbox").Type);

            // Awakening lore item
            itemLoot.Add(ModContent.ItemType<LoreAwakening>());

            // Aleksh donator item
            // Name specific: "Aleksh" or "Shark Lad"
            static bool getsLadPet(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Aleksh" || playerName == "Shark Lad";
            }
    ;
            itemLoot.AddIf(getsLadPet, ModContent.ItemType<JoyfulHeart>());

            // HPU dev item
            // Name specific: "Heart Plus Up"
            static bool getsHapuFruit(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Heart Plus Up";
            }
    ;
            itemLoot.AddIf(getsHapuFruit, ModContent.ItemType<HapuFruit>());

            // Apelusa dev item
            // Name specific: "Pelusa"
            static bool getsRedBow(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Pelusa";
            }

            itemLoot.AddIf(getsRedBow, ModContent.ItemType<RedBow>());

            // Mishiro dev vanity
            // Name specific: "Amber" or "Mishiro"
            static bool getsOracleHeadphones(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName is "Amber" or "Mishiro";
            }

            itemLoot.AddIf(getsOracleHeadphones, ModContent.ItemType<OracleHeadphones>());
        }
        private static readonly MethodInfo FargosSouls_DropDevianttsGift_Method = typeof(FargowiltasSouls.FargowiltasSouls).GetMethod("DropDevianttsGift", LumUtils.UniversalBindingFlags);
        public delegate void Orig_FargosSouls_DropDevianttsGift(Player player);
        internal static void FargosSouls_DropDevianttsGift_Detour(Orig_FargosSouls_DropDevianttsGift orig, Player player)
        {
            return;
        }

        private static readonly MethodInfo Instahouse_GetTiles_Method = typeof(Fargowiltas.Content.Projectiles.Explosives.AutoHouseProj).GetMethod("GetTiles", LumUtils.UniversalBindingFlags);
        public delegate void Orig_Instahouse_GetTiles(Player player, out int wallType, out int tileType, out int platformStyle, out bool moddedPlatform);
        internal static void Instahouse_GetTiles_Detour(Orig_Instahouse_GetTiles orig, Player player, out int wallType, out int tileType, out int platformStyle, out bool moddedPlatform)
        {
            orig(player, out wallType, out tileType, out platformStyle, out moddedPlatform);
            if (player.Calamity().ZoneSulphur)
            {
                wallType = ModContent.WallType<AcidwoodWall>();
                tileType = ModContent.TileType<AcidwoodTile>();
                platformStyle = ModContent.TileType<AcidwoodPlatformTile>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneAbyss)
            {
                wallType = ModContent.WallType<SmoothAbyssGravelWall>();
                tileType = ModContent.TileType<SmoothAbyssGravel>();
                platformStyle = ModContent.TileType<SmoothAbyssGravelPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneAstral)
            {
                wallType = ModContent.WallType<AstralMonolithWall>();
                tileType = ModContent.TileType<AstralMonolith>();
                platformStyle = ModContent.TileType<MonolithPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneCalamity)
            {
                wallType = ModContent.WallType<BrimstoneSlabWall>();
                tileType = ModContent.TileType<BrimstoneSlab>();
                platformStyle = ModContent.TileType<AshenPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneSunkenSea)
            {
                wallType = ModContent.WallType<SmoothNavystoneWall>();
                tileType = ModContent.TileType<SmoothNavystone>();
                platformStyle = ModContent.TileType<EutrophicPlatform>();
                moddedPlatform = true;
            }
        }
        private static readonly MethodInfo Instahouse_GetFurniture_Method = typeof(Fargowiltas.Content.Projectiles.Explosives.AutoHouseProj).GetMethod("GetFurniture", LumUtils.UniversalBindingFlags);
        public delegate void Orig_Instahouse_GetFurniture(Player player, out int doorStyle, out int chairStyle, out int tableStyle, out int torchStyle);
        internal static void Instahouse_GetFurniture_Detour(Orig_Instahouse_GetFurniture orig, Player player, out int doorStyle, out int chairStyle, out int tableStyle, out int torchStyle)
        {
            orig(player, out doorStyle, out chairStyle, out tableStyle, out torchStyle);
            if (player.Calamity().ZoneSulphur)
            {
                doorStyle = ModContent.TileType<AcidwoodDoorClosed>();
                chairStyle = 29; //palm wood chair because acidwood chair is 2 tiles wide 
                tableStyle = ModContent.TileType<AcidwoodTableTile>();
                torchStyle = ModContent.TileType<SulphurousTorch>();
            }
            if (player.Calamity().ZoneAbyss)
            {
                doorStyle = ModContent.TileType<AbyssDoorClosed>();
                chairStyle = ModContent.TileType<AbyssChair>();
                tableStyle = ModContent.TileType<AbyssTable>();
                torchStyle = ModContent.TileType<AbyssTorch>();
            }
            if (player.Calamity().ZoneAstral)
            {
                doorStyle = ModContent.TileType<MonolithDoorClosed>();
                chairStyle = ModContent.TileType<MonolithChair>();
                tableStyle = ModContent.TileType<MonolithTable>();
                torchStyle = ModContent.TileType<AstralTorch>();
            }
            if (player.Calamity().ZoneCalamity)
            {
                doorStyle = ModContent.TileType<AshenDoorClosed>();
                chairStyle = ModContent.TileType<AshenChair>();
                tableStyle = ModContent.TileType<AshenTable>();
                torchStyle = ModContent.TileType<GloomTorch>();
            }
            if (player.Calamity().ZoneSunkenSea)
            {
                doorStyle = ModContent.TileType<EutrophicDoorClosed>();
                chairStyle = ModContent.TileType<EutrophicChair>();
                tableStyle = ModContent.TileType<EutrophicTable>();
                torchStyle = ModContent.TileType<NavyPrismTorch>();
            }
        }

        public static bool NonFargoBossAlive() => Main.npc.Any(n => n.Alive() && n.boss && n.ModNPC != null && n.ModNPC.Mod != ModCompatibility.SoulsMod.Mod);

        private static readonly MethodInfo FMSVerticalSpeedMethod = typeof(FlightMasteryWings).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        public delegate void Orig_FMSVerticalSpeed(FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        internal static void FMSVerticalSpeed_Detour(Orig_FMSVerticalSpeed orig, FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive() && self is not EternitySoul)
            {
                player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (ascentWhenFalling > 0.95f)
                        ascentWhenFalling = 0.95f;
                    if (ascentWhenRising > 0.16f)
                        ascentWhenRising = 0.16f;
                    if (maxCanAscendMultiplier > 1.1f)
                        maxCanAscendMultiplier = 1.1f;
                    if (maxAscentMultiplier > 3.2f)
                        maxAscentMultiplier = 3.2f;
                    if (constantAscend > 0.145f)
                        constantAscend = 0.145f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (ascentWhenFalling > 1f)
                        ascentWhenFalling = 1f;
                    if (ascentWhenRising > 0.17f)
                        ascentWhenRising = 0.17f;
                    if (maxCanAscendMultiplier > 1.2f)
                        maxCanAscendMultiplier = 1.2f;
                    if (maxAscentMultiplier > 3.25f)
                        maxAscentMultiplier = 3.25f;
                    if (constantAscend > 0.15f)
                        constantAscend = 0.15f;
                }
            }
        }
        private static readonly MethodInfo FMSHorizontalSpeedMethod = typeof(FlightMasteryWings).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        public delegate void Orig_FMSHorizontalSpeed(FlightMasteryWings self, Player player, ref float speed, ref float acceleration);
        internal static void FMSHorizontalSpeed_Detour(Orig_FMSHorizontalSpeed orig, FlightMasteryWings self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive() && self is not EternitySoul)
            {
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (speed > 10.5f)
                        speed = 10.5f;
                    if (acceleration > 2.8f)
                        acceleration = 2.8f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (speed > 11.5f)
                        speed = 11.5f;
                    if (acceleration > 2.9f)
                        acceleration = 2.9f;
                }

                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }
        private static readonly MethodInfo LifeForceVerticalSpeedMethod = typeof(LifeForce).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        public delegate void Orig_LifeForceVerticalSpeed(LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        internal static void LifeForceVerticalSpeed_Detour(Orig_LifeForceVerticalSpeed orig, LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive())
            {
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
                if (ascentWhenFalling > 0.85f)
                    ascentWhenFalling = 0.85f;
                if (ascentWhenRising > 0.15f)
                    ascentWhenRising = 0.15f;
                if (maxCanAscendMultiplier > 1f)
                    maxCanAscendMultiplier = 1f;
                if (maxAscentMultiplier > 3f)
                    maxAscentMultiplier = 3f;
                if (constantAscend > 0.135f)
                    constantAscend = 0.135f;
            }
            else
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(1000);
        }
        private static readonly MethodInfo LifeForceHorizontalSpeedMethod = typeof(LifeForce).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        public delegate void Orig_LifeForceHorizontalSpeed(LifeForce self, Player player, ref float speed, ref float acceleration);
        internal static void LifeForceHorizontalSpeed_Detour(Orig_LifeForceHorizontalSpeed orig, LifeForce self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive())
            {

                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }

        private static readonly MethodInfo EmodeBalancePerID_Method = typeof(EmodeItemBalance).GetMethod("EmodeBalancePerID", LumUtils.UniversalBindingFlags);
        public delegate EmodeItemBalance.EModeChange Orig_EmodeBalancePerID(int itemType, ref float balanceNumber, ref string[] balanceTextKeys, ref string extra);
        internal static EmodeItemBalance.EModeChange EmodeBalancePerID_Detour(Orig_EmodeBalancePerID orig, int itemType, ref float balanceNumber, ref string[] balanceTextKeys, ref string extra)
        {
            if (CalDLCSets.GetValue(CalDLCSets.Items.DisabledEmodeChanges, itemType))
                return EmodeItemBalance.EModeChange.None;
            return orig(itemType, ref balanceNumber, ref balanceTextKeys, ref extra);
        }

        private static readonly MethodInfo TryUnlimBuffMethod = typeof(Fargowiltas.Content.Items.FargoGlobalItem).GetMethod("TryUnlimBuff", LumUtils.UniversalBindingFlags);
        public delegate void Orig_TryUnlimBuff(Item item, Player player);
        internal static void TryUnlimBuff_Detour(Orig_TryUnlimBuff orig, Item item, Player player)
        {
            if (item.type != ModContent.ItemType<AstralInjection>())
            {
                orig(item, player);

            }

        }
        private static readonly MethodInfo ApprenticeSupportMethod = typeof(ApprenticeSupport).GetMethod("PostUpdateEquips", LumUtils.UniversalBindingFlags);
        public delegate void Orig_PostUpdateEquips(ApprenticeSupport self, Player player);
        internal static void ApprenticeSupport_Detour(Orig_PostUpdateEquips orig, ApprenticeSupport self, Player player)
        {
            var heldItem = player.HeldItem;
            var rog = ModContent.GetInstance<RogueDamageClass>();
            var calPlayer = player.Calamity();
            var apprenticeSupport = ModContent.GetInstance<ApprenticeSupport>();
            var fargoPlayer = player.FargoSouls();
            if (heldItem.CountsAsClass(rog) && calPlayer.StealthStrikeAvailable() && player.HasEffect(apprenticeSupport))
            {
                if (fargoPlayer.ApprenticeItemCD > 0)
                    fargoPlayer.ApprenticeItemCD--; // account for cooldown not running on return while a stealth strike is available
                return;
            }
            orig(self, player);
        }
    }
}
