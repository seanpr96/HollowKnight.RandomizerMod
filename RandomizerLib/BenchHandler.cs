﻿using JetBrains.Annotations;
using SeanprCore;

namespace RandomizerLib
{
    [PublicAPI]
    public static class BenchHandler
    {
        public static bool NoClaw { get; set; }

        public static void Hook()
        {
            UnHook();

            On.PlayerData.SetBenchRespawn_RespawnMarker_string_int += HandleBenchSave;
            On.PlayerData.SetBenchRespawn_string_string_bool += HandleBenchSave;
            On.PlayerData.SetBenchRespawn_string_string_int_bool += HandleBenchSave;
            On.HutongGames.PlayMaker.Actions.BoolTest.OnEnter += HandleBenchBoolTest;
        }

        public static void UnHook()
        {
            On.PlayerData.SetBenchRespawn_RespawnMarker_string_int -= HandleBenchSave;
            On.PlayerData.SetBenchRespawn_string_string_bool -= HandleBenchSave;
            On.PlayerData.SetBenchRespawn_string_string_int_bool -= HandleBenchSave;
            On.HutongGames.PlayMaker.Actions.BoolTest.OnEnter -= HandleBenchBoolTest;
        }

        private static void HandleBenchSave(On.PlayerData.orig_SetBenchRespawn_RespawnMarker_string_int orig,
            PlayerData self, RespawnMarker spawnMarker, string sceneName, int spawnType)
        {
            if (CanSaveInRoom(sceneName))
            {
                orig(self, spawnMarker, sceneName, spawnType);
            }
        }

        private static void HandleBenchSave(On.PlayerData.orig_SetBenchRespawn_string_string_bool orig, PlayerData self,
            string spawnMarker, string sceneName, bool facingRight)
        {
            if (CanSaveInRoom(sceneName))
            {
                orig(self, spawnMarker, sceneName, facingRight);
            }
        }

        private static void HandleBenchSave(On.PlayerData.orig_SetBenchRespawn_string_string_int_bool orig,
            PlayerData self, string spawnMarker, string sceneName, int spawnType, bool facingRight)
        {
            if (CanSaveInRoom(sceneName))
            {
                orig(self, spawnMarker, sceneName, spawnType, facingRight);
            }
        }

        private static void HandleBenchBoolTest(On.HutongGames.PlayMaker.Actions.BoolTest.orig_OnEnter orig,
            HutongGames.PlayMaker.Actions.BoolTest self)
        {
            if (self.State?.Name == "Rest Burst" && self.boolVariable?.Name == "Set Respawn")
            {
                self.boolVariable.Value = CanSaveInRoom(Ref.GM.GetSceneNameString());
            }

            orig(self);
        }

        private static bool CanSaveInRoom(string sceneName)
        {
            PlayerData pd = Ref.PD;

            switch (sceneName)
            {
                case SceneNames.Abyss_18: // Basin bench
                    return pd.hasWalljump || (NoClaw && pd.hasDoubleJump);
                case SceneNames.GG_Waterways: // Godhome
                    return pd.hasWalljump;
                case SceneNames.Room_Slug_Shrine: // Unn bench
                    return pd.hasDash || pd.hasDoubleJump || pd.hasAcidArmour && pd.hasWalljump;
                case SceneNames.Ruins1_02: // Quirrel bench
                case SceneNames.Waterways_02: // Waterways bench
                    return pd.hasWalljump || pd.hasDoubleJump;
                default:
                    return true;
            }
        }
    }
}