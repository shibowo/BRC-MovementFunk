using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MovementFunk.Mechanics;
using MovementFunk.SpeedDisplay;
using Reptile;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MovementFunk
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class MovementFunkPlugin : BaseUnityPlugin
    {
        public const string MyGUID = "shibowo.MovementFunk";
        private const string PluginName = "MovementFunk";
        private const string VersionString = "0.3.3";

        public static MovementFunkPlugin Instance { get; private set; }
        public string Dir => Path.GetDirectoryName(Info.Location);

        private Harmony harmony;
        public static Player player;
        public static ManualLogSource PubLogger; //plugin's logger instance is protected.
        public static MovementConfig MovementSettings;
        public static SpeedometerConfig SpeedometerSettings;
        public static MFAbilityManager abilityManager;
        public static MFMainApp mainApp;

        private void Awake()
        {
            harmony = new Harmony(MyGUID);
            Instance = this;
            PubLogger = BepInEx.Logging.Logger.CreateLogSource(PluginName);
            
            PatchAllInNamespace(harmony, "MovementFunk.Patches");

            MFMovementMetrics.Init();
            SpeedReturn.Init();
            MFAnimation.Init();
            MFPresetManager.LaunchPreset();
            
            MFMainApp.Init("MF Presets", "MF_icon.png");
            MovementPresetApp.Init("Movement Presets");
            SpeedometerPresetApp.Init("Speedometer Presets");
            Logger.LogInfo($"{PluginName} v{VersionString} has been loaded!");
        }

        private void FixedUpdate()
        {
            if (player != null)
            {
                Fastfall.Update();
                VertChanges.Update();
                BoostChanges.Update();
                MFMovementMetrics.Update();
                MFVariables.Update();
                PerfectManual.Update();
                SpeedLimit.Update();
                NonStableChanges.Update();
                SpeedReturn.Update();
                MFInputBuffer.Update();
                MFTrickManager.Update();
                RailCollision.Update();
                GoonStorageMinMax.Update();
                //BoostHardCorner.Update();
                PopJump.Update();
            }
        }

        public static void PatchAllInNamespace(Harmony harmony, string namespaceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.Namespace == namespaceName);
            foreach (var type in types)
            {
                harmony.PatchAll(type);
            }
        }
    }
}
