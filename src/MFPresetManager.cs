using BepInEx;
using BepInEx.Configuration;
using Reptile;
using System;
using System.Collections.Generic;
using System.IO;

namespace MovementFunk
{
    internal class MFPresetManager
    {
      private static ConfigFile movementCfgFile;
      private static ConfigFile speedometerCfgFile;

      public static void LaunchPreset()
      {
       var Instance = MovementFunkPlugin.Instance;
       var MyGUID = MovementFunkPlugin.MyGUID; 
       string movementFunkDir = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
       string movementDir = movementFunkDir + @"MovementPresets\";
       string speedometerDir = movementFunkDir + @"SpeedometerPresets\";
       if (!Directory.Exists(movementFunkDir)){
         MovementFunkPlugin.PubLogger.LogWarning("MovementFunk directory not found.");
         MovementFunkPlugin.PubLogger.LogWarning($"Creating new MovementFunk directory at \"{movementFunkDir}\"");
         CreateConfigDirectory(movementFunkDir);
         NoMovementPreset();
         NoSpeedometerPreset();
       }
       if(!Directory.Exists(movementDir)){
          MovementFunkPlugin.PubLogger.LogWarning("MovementPresets directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new MovementPresets directory at \"{movementDir}\"");
          CreateConfigDirectory(movementDir);
          NoMovementPreset();
       }
       if(!Directory.Exists(speedometerDir)){
          MovementFunkPlugin.PubLogger.LogWarning("SpeedometerPresets directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new SpeedometerPresets directory at \"{speedometerDir}\"");
          CreateConfigDirectory(speedometerDir);
          NoSpeedometerPreset();
       }
        
       movementCfgFile = new ConfigFile(movementFunkDir + "movement.cfg",
                                                   true,
                                                   MetadataHelper.GetMetadata(MovementFunkPlugin.Instance));
       speedometerCfgFile = new ConfigFile(movementFunkDir + "speedometer.cfg",
                                                      true,
                                                      MetadataHelper.GetMetadata(MovementFunkPlugin.Instance));

       MovementFunkPlugin.MovementSettings = new MovementConfig(movementCfgFile);
       MovementFunkPlugin.SpeedometerSettings = new SpeedometerConfig(speedometerCfgFile);

       if (MovementFunkPlugin.MovementSettings.Misc.MVPresetEnabled.Value)
        {
          if (MovementFunkPlugin.MovementSettings.Misc.MVPreset.Value == "None")
          {
            NoMovementPreset();
            return;
          }
          ApplyMovementPreset(MovementFunkPlugin.MovementSettings.Misc.MVPreset.Value);
        }
       if(MovementFunkPlugin.SpeedometerSettings.Misc.SPPresetEnabled.Value){
         if(MovementFunkPlugin.SpeedometerSettings.Misc.SPPreset.Value == "None"){
          NoSpeedometerPreset();
          return;
         }
         ApplySpeedometerPreset(MovementFunkPlugin.SpeedometerSettings.Misc.SPPreset.Value);
       }
      }
      public static void ApplyMovementPreset(string presetName){
        NoMovementPreset();
        MovementFunkPlugin.MovementSettings.Misc.MVPreset.Value = presetName;

        var Instance = MovementFunkPlugin.Instance;
        var MyGUID = MovementFunkPlugin.MyGUID;
        var MovementSettings = MovementFunkPlugin.MovementSettings;
        string movementFunkDir = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
        string movementDir = movementFunkDir + @"MovementPresets\";
        if(!Directory.Exists(movementFunkDir)){
          MovementFunkPlugin.PubLogger.LogWarning("MovementFunk directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new MovementFunk directory at \"{movementFunkDir}\"");
          CreateConfigDirectory(movementFunkDir);
          NoMovementPreset();
        }
        if(Directory.Exists(movementDir)){
          string configPath = movementDir + presetName + ".cfg";
          MovementConfig configFile = new MovementConfig(new ConfigFile(configPath, false));
          MovementFunkPlugin.MovementSettings = configFile;
          if (MovementFunkPlugin.player != null)
          {
            //UpdateInitVars();
          } 
        }
        else {
          MovementFunkPlugin.PubLogger.LogWarning("MovementPresets directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new MovementPresets directory at \"{movementDir}\"");
          CreateConfigDirectory(movementDir);
          NoMovementPreset();
        }
      }
      public static void ApplySpeedometerPreset(string presetName)
      {
        NoMovementPreset();
        MovementFunkPlugin.MovementSettings.Misc.MVPreset.Value = presetName;

        var Instance = MovementFunkPlugin.Instance;
        var MyGUID = MovementFunkPlugin.MyGUID;
        var MovementSettings = MovementFunkPlugin.MovementSettings;
        string movementFunkDir = Instance.Config.ConfigFilePath.Replace(MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
        string speedometerDir = movementFunkDir + @"SpeedometerPresets\";
        if(!Directory.Exists(movementFunkDir)){
          MovementFunkPlugin.PubLogger.LogWarning("MovementFunk directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new MovementFunk directory at \"{movementFunkDir}\"");
          CreateConfigDirectory(movementFunkDir);
        }
        if(Directory.Exists(speedometerDir)){
          string configPath = speedometerDir + presetName + ".cfg";
          MovementConfig configFile = new MovementConfig(new ConfigFile(configPath, false));
          MovementFunkPlugin.MovementSettings = configFile;
          if (MovementFunkPlugin.player != null)
          {
            //UpdateInitVars();
          } 
        }
        else{
          MovementFunkPlugin.PubLogger.LogWarning("SpeedometerPresets directory not found.");
          MovementFunkPlugin.PubLogger.LogWarning($"Creating new SpeedometerPresets directory at \"{speedometerDir}\"");
          CreateConfigDirectory(speedometerDir);
        }
      }

        public static void NoMovementPreset()
        {
            string movementFunkDir = MovementFunkPlugin.Instance.Config.ConfigFilePath.Replace(MovementFunkPlugin.MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
            MovementConfig MVConfig = new MovementConfig(movementCfgFile);
            MovementFunkPlugin.MovementSettings = MVConfig;
            MovementFunkPlugin.MovementSettings.Misc.MVPreset.Value = "None";
        }
        public static void NoSpeedometerPreset()
        {
            string movementFunkDir = MovementFunkPlugin.Instance.Config.ConfigFilePath.Replace(MovementFunkPlugin.MyGUID + ".cfg", string.Empty) + @"MovementFunk\";
            SpeedometerConfig configFile = new SpeedometerConfig(speedometerCfgFile);
            MovementFunkPlugin.SpeedometerSettings = configFile;
            MovementFunkPlugin.SpeedometerSettings.Misc.SPPreset.Value = "None";
        }
        public static List<string> GetAvailablePresets()
        {
            var Instance = MovementFunkPlugin.Instance;
            string movementPlusPath = Path.Combine(Path.GetDirectoryName(Instance.Config.ConfigFilePath), "MovementFunk/MovementPresets");
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
                MovementFunkPlugin.PubLogger.LogError("MovementFunk directory not found.");
            }

            return presetNames;
        }

        private static void UpdateInitVars()
        {
            if (MovementFunkPlugin.player != null)
            {
                Player player = MovementFunkPlugin.player;
                MovementConfig config = MovementFunkPlugin.MovementSettings;

                player.grindAbility.grindDeccAboveNormal = config.RailGeneral.Decc.Value;

                player.boostAbility.decc = config.BoostGeneral.decc.Value;

                player.motor.maxFallSpeed = config.Misc.maxFallSpeed.Value;

                player.wallrunAbility.minDurationBeforeJump = config.WallGeneral.minDurJump.Value;
                player.wallrunAbility.wallrunDecc = config.WallGeneral.decc.Value;
            }
        }
        private static void CreateConfigDirectory(string movementFunkDir){
          try{
            Directory.CreateDirectory(movementFunkDir);
          }
          catch(Exception e){
            if(e is DirectoryNotFoundException){
              MovementFunkPlugin.PubLogger.LogError($"Directory \"{movementFunkDir}\" does not exist!");
              MovementFunkPlugin.PubLogger.LogError("Check that above-level directories exist.");
            }
            if(e is UnauthorizedAccessException){
              MovementFunkPlugin.PubLogger.LogError($"You do not have permission to create directory \"{movementFunkDir}\"!");
              MovementFunkPlugin.PubLogger.LogError("Make sure you have the right permissions to write to BepInEx\\config.");
            }
            if(e is IOException || e is ArgumentException || e is ArgumentNullException || e is NotSupportedException){
              MovementFunkPlugin.PubLogger.LogError($"Bad directory path \"{movementFunkDir}\"!");
              MovementFunkPlugin.PubLogger.LogError("Make sure that the path does not contain invalid characters, is empty, or points to a file.");
            }
            if(e is PathTooLongException){
              MovementFunkPlugin.PubLogger.LogError($"MovementFunk yapped too hard. "+
                  $"(Directory ${movementFunkDir} exceeds the maximum path length set by your operating system)!");
            }
          }
        }
    }
}
