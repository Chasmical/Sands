using BepInEx;
using RogueLibsCore;
using UnityEngine;

namespace SuperHot
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]
    public sealed class SuperHotPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = @"abbysssal.streetsofrogue.superhot";
        public const string PluginName = "[S&S] Super Hot";
        public const string PluginVersion = "1.0.0";

        public new static BepInEx.Logging.ManualLogSource Logger = null!;

        public void Awake()
        {
            Logger = base.Logger;
            RogueLibs.LoadFromAssembly();
            RoguePatcher patcher = new RoguePatcher(this);
            patcher.Postfix(typeof(GameController), nameof(GameController.SetTimeScale));
        }
        public static void GameController_SetTimeScale(GameController __instance)
            => __instance.musicPlayer.pitch = Time.timeScale;
    }
	[ItemCategories(RogueCategories.Passive, RogueCategories.Movement, RogueCategories.Defense, RogueCategories.NPCsCantPickUp)]
    public class SuperHotWatch : CustomItem, IDoLateUpdate
    {
		[RLSetup]
        public static void Setup()
        {
            RogueLibs.CreateCustomItem<SuperHotWatch>()
                     .WithName(new CustomNameInfo("Super Hot Watch"))
                     .WithDescription(new CustomNameInfo("Time moves as you do. This watch is extremely hot to the touch."))
                     .WithSprite(Properties.Resources.SuperHotWatch)
                     .WithUnlock(new ItemUnlock { UnlockCost = 10, CharacterCreationCost = 20, LoadoutCost = 10 });
        }
        public override void SetupDetails()
        {
            Item.itemType = ItemTypes.Tool;
        }

        public void LateUpdate()
        {
            if (Owner is null || Owner.isPlayer is 0) return;
			int num = Owner.isPlayer - 1;
            PlayerControl pc = gc.playerControl;
			bool playerCanMove = pc.cantPressGameplayButtonsP[num] == 0 && pc.cantPressGameplayButtonsPB[num] == 0;
			bool playerMoving = pc.heldLeftK[num] || pc.heldRightK[num] || pc.heldDownK[num] || pc.heldUpK[num];
            bool playerBusy = Owner.melee.attackAnimPlaying || pc.cantPressButtons;

            gc.secondaryTimeScale = !playerCanMove || Owner.dead || playerMoving || playerBusy ? -1 : 1f / 60f;
            gc.SetTimeScale();
        }

    }
}
