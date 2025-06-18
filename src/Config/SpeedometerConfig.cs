using BepInEx.Configuration;

namespace MovementFunk;
public class SpeedometerConfig(ConfigFile config){
  public RepresentationConfig Representation = new(config, "Representation");
  public ColorConfig Color = new(config, "Color");
  public MiscConfig Misc = new(config, "Misc");
  public FormatConfig Formatting = new(config, "Text Formatting");

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
    public ConfigEntry<bool> SpeedUnitsEnabled = config.Bind(
        category,
        "Display Speed Unit Text",
        true,
        "Enables/Disables the speed unit text(e.g. KM/H, MPH etc)."
        );
    public ConfigEntry<bool> UseTotalSpeed = config.Bind(
        category,
        "Use Total Speed",
        true,
        "Set to true to use the player's total speed, otherwise set to false to only use forward speed."
        );
    public ConfigEntry<bool> GoonStorageEnabled = config.Bind(
        category,
        "Display Goon Storage",
        true,
        "Shows goon storage."
        );
    public ConfigEntry<string> GoonStorageText = config.Bind(
        category,
        "Goon Storage Text",
        "STRG:",
        "A piece of text that displays right before your goon storage count, i.e. STRG: {goonStorageCount}."
        );
    public ConfigEntry<bool> AltTrickComboCount = config.Bind(
        category,
        "Alternative Trick Combo Count",
        true,
        "Puts the trick count near the trick names rather than above the boost meter."
        );
  }
  public class FormatConfig(ConfigFile config, string category){
        public ConfigEntry<bool> OutlinesEnabled = config.Bind(
        category,
        "Enable Outlines",
        true,
        "Outlines the text of your speedometer."
        );

    public ConfigEntry<float> LabelGap = config.Bind(
        category,
        "Label Gap",
        30.0f,
        "Sets the gap between the speedometer text and the \"x tricks combo\" label."
        );

    public ConfigEntry<bool> UseMonospace = config.Bind(
        category,
        "Use Monospace Font",
        true,
        "Turns the speedometer font into a monospace font. This eliminates the jitter that the original font might have at times."
        );
    public ConfigEntry<bool> MonospacedDot = config.Bind(
        category,
        "Monospaced Dot",
        false,
        "Makes the decimal dot also monospaced."
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
    public ConfigEntry<float> ColorShiftRate = config.Bind(
        category,
        "Color Shift Rate",
        0.2f,
        "How quickly the colors shift in swagometer mode."
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
