using BepInEx.Configuration;
using Reptile;
using System;
using System.Collections.Generic;
using System.IO;

namespace MovementPlus
{
    internal class MPPresetManager
    {
        public static void LaunchPreset()
        {
            if (MovementPlusPlugin.ConfigSettings.Misc.presetEnabled.Value)
            {
                if (MovementPlusPlugin.ConfigSettings.Misc.preset.Value == "None")
                {
                    NoPreset();
                    return;
                }
                ApplyPreset(MovementPlusPlugin.ConfigSettings.Misc.preset.Value);
            }
        }

        public static void ApplyPreset(string presetName)
        {
            NoPreset();
            MovementPlusPlugin.ConfigSettings.Misc.preset.Value = presetName;

            var Instance = MovementPlusPlugin.Instance;
            var MyGUID = MovementPlusPlugin.MyGUID;
            var ConfigSettings = MovementPlusPlugin.ConfigSettings;
            string configPath = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\" + presetName + ".cfg";

            MyConfig configFile = new MyConfig(new ConfigFile(configPath, false));
            MovementPlusPlugin.ConfigSettings = configFile;
            if (MovementPlusPlugin.player != null)
            {
                //UpdateInitVars();
            }
        }

        public static void NoPreset()
        {
            MyConfig configFile = new MyConfig(MovementPlusPlugin.Instance.Config);
            MovementPlusPlugin.ConfigSettings = configFile;
            MovementPlusPlugin.ConfigSettings.Misc.preset.Value = "None";
        }

        public static List<string> GetAvailablePresets()
        {
            var Instance = MovementPlusPlugin.Instance;
            string movementPlusPath = Path.Combine(Path.GetDirectoryName(Instance.Config.ConfigFilePath), "MovementFunk");
            List<string> presetNames = new List<string>();

            if (Directory.Exists(movementPlusPath))
            {
                string[] presetFiles = Directory.GetFiles(movementPlusPath, "*.cfg");
                foreach (string file in presetFiles)
                {
                    presetNames.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                Console.WriteLine("MovementFunk directory not found.");
            }

            return presetNames;
        }

        private static void UpdateInitVars()
        {
            if (MovementPlusPlugin.player != null)
            {
                Player player = MovementPlusPlugin.player;
                MyConfig config = MovementPlusPlugin.ConfigSettings;

                player.grindAbility.grindDeccAboveNormal = config.RailGeneral.Decc.Value;

                player.boostAbility.decc = config.BoostGeneral.decc.Value;

                player.motor.maxFallSpeed = config.Misc.maxFallSpeed.Value;

                player.wallrunAbility.minDurationBeforeJump = config.WallGeneral.minDurJump.Value;
                player.wallrunAbility.wallrunDecc = config.WallGeneral.decc.Value;
            }
        }
    }
}
