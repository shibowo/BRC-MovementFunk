using BepInEx;
using HarmonyLib;
using MovementPlus.Mechanics;
using Reptile;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MovementPlus
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class MovementPlusPlugin : BaseUnityPlugin
    {
        public const string MyGUID = "shibowo.MovementFunk";
        private const string PluginName = "MovementFunk";
        private const string VersionString = "0.1.0";

        public static MovementPlusPlugin Instance { get; private set; }
        public string Dir => Path.GetDirectoryName(Info.Location);

        private Harmony harmony;
        public static Player player;
        public static MyConfig ConfigSettings;

        private void Awake()
        {
            harmony = new Harmony(MyGUID);
            ConfigSettings = new MyConfig(Config);
            Instance = this;

            PatchAllInNamespace(harmony, "MovementPlus.Patches");

            MPMovementMetrics.Init();
            SpeedReturn.Init();
            PresetApp.Init();
            MPAnimation.Init();
            MPPresetManager.LaunchPreset();

            Logger.LogInfo($"{PluginName} has been loaded!");
        }

        private void FixedUpdate()
        {
            if (player != null)
            {
                Fastfall.Update();
                VertChanges.Update();
                BoostChanges.Update();
                MPMovementMetrics.Update();
                MPVariables.Update();
                PerfectManual.Update();
                SpeedLimit.Update();
                NonStableChanges.Update();
                SpeedReturn.Update();
                MPInputBuffer.Update();
                MPTrickManager.Update();
                RailCollision.Update();
                GoonStorageMinMax.Update();
                BoostHardCorner.Update();
                DumpsterKick.Update();
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
