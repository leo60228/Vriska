using Celeste.Mod;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using On.Celeste;
using Mono.Cecil.Cil;

namespace Celeste.Mod.Rainbow {
    public class RainbowModule : EverestModule {

        public static RainbowModule Instance;

        public override Type SettingsType => typeof(RainbowModuleSettings);
        public static RainbowModuleSettings Settings => (RainbowModuleSettings) Instance._Settings;

        private static FieldInfo NormalHairColor = typeof(Player).GetField("NormalHairColor");

        private static MTexture Horns;

        public RainbowModule() {
            Instance = this;
        }

        public override void Load() {
            // This is a runtime mod, but we can still manipulate the game.

            On.Celeste.Player.Update += Player_Update;
            On.Celeste.PlayerHair.Render += RenderHair;
            On.Celeste.PlayerSprite.ctor += PlayerSprite_ctor;
        }

        public override void LoadContent(bool firstLoad) {
            Horns = GFX.Game["characters/player_no_backpack/horns"];
        }

        public override void Unload() {
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.PlayerHair.Render -= RenderHair;
            On.Celeste.PlayerSprite.ctor -= PlayerSprite_ctor;
        }

        public void PlayerSprite_ctor(On.Celeste.PlayerSprite.orig_ctor orig, PlayerSprite self, PlayerSpriteMode mode) {
            if (mode == PlayerSpriteMode.Madeline) {
                mode = PlayerSpriteMode.MadelineNoBackpack;
            }
            orig(self, mode);
        }

        private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (self.GetType().Name == "Ghost")
            {
                orig(self);
                return;
            }

            NormalHairColor.SetValue(null, new Color(0, 0, 0));
            Player.P_DashB.Color = new Color(0, 0, 0);

            orig(self);
        }

        public static void RenderHair(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self) {
            Player player = self.Entity as Player;
            if (player == null || self.GetSprite().Mode == PlayerSpriteMode.Badeline) {
                orig(self);
                return;
            }

            PlayerSprite sprite = self.GetSprite();
            Vector2 origin = new Vector2(5f, 5f);
            if (!sprite.HasHair || (self.Border * self.Alpha).A <= 0 || sprite.HairCount <= 0 || player.StateMachine.State == Player.StDreamDash)
                return;


            Vector2 pos;

            self.Nodes[0] = self.Nodes[0].Floor();

            pos = self.Nodes[0];
            Horns.Draw(pos + new Vector2(2f, -4f), origin);

            orig(self);
        }
    }
}
