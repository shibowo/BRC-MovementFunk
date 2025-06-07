using BepInEx.Configuration;

namespace MovementFunk;
public class SpeedometerConfig(ConfigFile config){
  public RepresentationConfig Representation = new(config, "Representation");
  public ColorConfig Color = new(config, "Color");
  public MiscConfig Misc = new(config, "Misc");

  public class RepresentationConfig(ConfigFile config, string category)
  {
    public ConfigEntry<bool> Enabled = config.Bind(
        category,
        "Enable Speedometer",
        true,
        "Enables the built-in spedometer"
        );

    public ConfigEntry<MovementFunk.SpeedDisplay.Representation> SpeedRep = config.Bind(
        category,
        "Speed Representation Mode",
        MovementFunk.SpeedDisplay.Representation.SpeedUnits,
        "Sets the units your speed will be displayed in in-game."
        );

    public ConfigEntry<bool> UseTotalSpeed = config.Bind(
        category,
        "Use Total Speed",
        true,
        "Set to true to use the player's total speed, otherwise set to false to only use forward speed."
        );
    public ConfigEntry<bool> OutlinesEnabled = config.Bind(
        category,
        "Enable Outlines",
        true,
        "Outlines the text of your speedometer."
        );
  }

  public class ColorConfig(ConfigFile config, string category){
    public ConfigEntry<bool> Enabled = config.Bind(
        category,
        "Enable Colors",
        true,
        "Makes your spedometer colorful."
        );
    public ConfigEntry<float> RedlineThreshold = config.Bind(
        category,
        "Redline Threshold",
        85.0f,
        "The point (in m/s) at which the speedometer will go fully red."
        );
    public ConfigEntry<bool> Rainbow = config.Bind(
        category,
        "Swagometer",
        true,
        "Changes the color of your spedometer as your speed changes."
        );
  }
  public class MiscConfig(ConfigFile config, string category){
    public ConfigEntry<string> SPPreset = config.Bind(
        category,
        "Preset",
        "None",
        "Preset to use when launching the game"
        );
    public ConfigEntry<bool> SPPresetEnabled = config.Bind(
        category,
        "Preset Enabled",
        true,
        "Whether the preset is enabled on launch or not."
        );
  }
}
