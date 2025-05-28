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
       var Instance = MovementPlusPlugin.Instance;
       var MyGUID = MovementPlusPlugin.MyGUID; 
       string movementFunkConfig = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
       if (!Directory.Exists(movementFunkConfig)){
         Console.WriteLine("MovementFunk directory not found.");
         Console.WriteLine($"Creating new MovementFunk directory at \"{movementFunkConfig}\"");
         CreateConfigDirectory(movementFunkConfig);
       }
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
        string movementFunkConfig = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
        if(Directory.Exists(movementFunkConfig)){
          string configPath = movementFunkConfig + presetName + ".cfg";
          MyConfig configFile = new MyConfig(new ConfigFile(configPath, false));
          MovementPlusPlugin.ConfigSettings = configFile;
          if (MovementPlusPlugin.player != null)
          {
            //UpdateInitVars();
          }
        }
        else{
          Console.WriteLine("MovementFunk directory not found.");
          Console.WriteLine($"Creating new MovementFunk directory at \"{movementFunkConfig}\"");
          CreateConfigDirectory(movementFunkConfig);
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
        private static void CreateConfigDirectory(string movementFunkConfig){
          try{
            Directory.CreateDirectory(movementFunkConfig);
          }
          catch(Exception e){
            if(e is DirectoryNotFoundException){
              Console.WriteLine($"Directory \"{movementFunkConfig}\" does not exist!");
              Console.WriteLine("Check that above-level directories exist.");
            }
            if(e is UnauthorizedAccessException){
              Console.WriteLine($"You do not have permission to create directory \"{movementFunkConfig}\"!");
              Console.WriteLine("Make sure you have the right permissions to write to BepInEx\\config.");
            }
            if(e is IOException || e is ArgumentException || e is ArgumentNullException || e is NotSupportedException){
              Console.WriteLine($"Bad directory path \"{movementFunkConfig}\"!");
              Console.WriteLine("Make sure that the path does not contain invalid characters, is empty, or points to a file.");
            }
            if(e is PathTooLongException){
              Console.WriteLine($"MovementFunk yapped too hard. "+
                  "(Directory ${movementFunkConfig} exceeds the maximum path length set by your operating system)!");
            }
          }
          NoPreset();
        }
    }
}
