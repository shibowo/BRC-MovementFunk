using Reptile;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;
using System.Globalization;

namespace MovementFunk.SpeedDisplay
{ 

  public enum Representation {
    SpeedUnits,
    KilometersPerHour,
    MilesPerHour,
    MetersPerSecond 
  }

  public class Speedometer : MonoBehaviour{
    private static float speed = 0.0f;
    private static float goonSpeed = 0.0f;
    private static TextMeshProUGUI speedometerLabel;
    private static TextMeshProUGUI goonStorageLabel;
    private static TextMeshProUGUI trickComboLabel;
    private static string speedRep;
    private static string goonStorageText;
    private static StringBuilder spmStrBuilder;
    private static StringBuilder gstStrBuilder;
    private static Representation rep;
    private static SpeedometerConfig Config = MovementFunkPlugin.SpeedometerSettings;
    private static float colorShiftRate;
    private static float redlineThreshold; 
    private static float labelGap; 
    private static readonly CultureInfo labelCulture = CultureInfo.InvariantCulture;

    //these ratios are taken off SoftGoat's spedometer, not sure how these were calculated
    //followup: it appears that player speed is meters per second, at least I can assume so
    private const float MPH_ratio = 2.236936f; //do we need this to be *this* accurate?
    private const float KPH_ratio = 3.6f;

    private const string CloseMonoTag = "</mspace>";
    private const string StartMonoTag = "<mspace=1.133em>";

    public static void Init(TextMeshProUGUI someLabel){
      if(!Config.Representation.Enabled.Value) return;

      if(Config.Color.Enabled.Value){
        colorShiftRate = Config.Color.ColorShiftRate.Value;
      }

      if(Config.Formatting.OutlinesEnabled.Value){
        //stupid ahh localizer breaks the outline on the spedometer
        //thanks to SoftGoat for this one, again.
        var localizer = someLabel.GetComponent<TMProFontLocalizer>();
        localizer.enabled = false;

        someLabel.fontMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
        someLabel.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
        someLabel.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.075f);
      }

      labelGap = Config.Formatting.LabelGap.Value;
      redlineThreshold = Config.Color.RedlineThreshold.Value;
      speedometerLabel = Instantiate(someLabel, someLabel.transform.parent);

      if(Config.Representation.GoonStorageEnabled.Value){
        goonStorageLabel = Instantiate(someLabel, someLabel.transform.parent);
        gstStrBuilder = new StringBuilder();
        goonStorageLabel.transform.position += Vector3.up * labelGap;
        goonStorageText = Config.Representation.GoonStorageText.Value;

        someLabel.maxVisibleLines = 0; //I am not sure if it's a good idea to Destroy() this...
        goonStorageLabel.enableWordWrapping = false;
        goonStorageLabel.enableAutoSizing = false;
        labelGap *= 2;

        if(!Config.Representation.AltTrickComboCount.Value){
          someLabel.transform.position += Vector3.up * labelGap;
        }
      }
      if(Config.Formatting.UseMonospace.Value){
        speedometerLabel.richText = true;
      }
      someLabel.transform.position += Vector3.up * labelGap;
      spmStrBuilder = new StringBuilder();
      UpdateSpeedRep();
    }

    public static void Update(){
      if(!Config.Representation.Enabled.Value) return;

      if(Config.Representation.UseTotalSpeed.Value){
        speed = MovementFunkPlugin.player.GetTotalSpeed(); 
      }
      else {
        speed = MovementFunkPlugin.player.GetForwardSpeed();
      }

      if(Config.Color.Enabled.Value){
        if(speed < redlineThreshold){
          speedometerLabel.color = SetSaturationTo(Color.red, speed / redlineThreshold);
        }
        else if(Config.Color.Rainbow.Value){
          speedometerLabel.color = ShiftHueWithSpeed(speedometerLabel.color, speed);
        }
      }

      if(Config.Representation.GoonStorageEnabled.Value){
        gstStrBuilder.Clear();
      }
      spmStrBuilder.Clear();

      switch(rep){
        case Representation.SpeedUnits:
          speed *= 10;
          goonSpeed = MFVariables.savedGoon * 10;
          break;
        case Representation.KilometersPerHour:
          speed *= KPH_ratio;
          goonSpeed = MFVariables.savedGoon * KPH_ratio;
          break;
        case Representation.MilesPerHour:
          speed *= MPH_ratio;
          goonSpeed = MFVariables.savedGoon * MPH_ratio;
          break;
        default:
          goonSpeed = MFVariables.savedGoon;
          break;
      }
      if(rep == Representation.SpeedUnits){
        spmStrBuilder.AppendFormat(labelCulture, "{0:0} {1}", speed, speedRep);
        gstStrBuilder.AppendFormat(labelCulture, "{0} {1:0} {2}", goonStorageText, goonSpeed, speedRep);
      }
      else{
        spmStrBuilder.AppendFormat(labelCulture, "{0:0.0} {1}", speed, speedRep);
        gstStrBuilder.AppendFormat(labelCulture, "{0} {1:0.0} {2}", goonStorageText, goonSpeed, speedRep);
      }
      if(Config.Formatting.UseMonospace.Value){
        if(!Config.Formatting.MonospacedDot.Value && rep != Representation.SpeedUnits){
          int sepIndex = spmStrBuilder.ToString().IndexOf(labelCulture.NumberFormat.NumberDecimalSeparator);
          spmStrBuilder.Insert(sepIndex, CloseMonoTag);
          spmStrBuilder.Insert(sepIndex + CloseMonoTag.Length + 1, StartMonoTag);
          if(Config.Representation.GoonStorageEnabled.Value){  
            int sepIndexGst = spmStrBuilder.ToString().IndexOf(labelCulture.NumberFormat.NumberDecimalSeparator);
            gstStrBuilder.Insert(sepIndexGst, CloseMonoTag);
            gstStrBuilder.Insert(sepIndexGst + CloseMonoTag.Length + 1, StartMonoTag);
          }
        }
        spmStrBuilder.Insert(0, CloseMonoTag);
        spmStrBuilder.Insert(CloseMonoTag.Length, StartMonoTag);
        if(Config.Representation.GoonStorageEnabled.Value){  
          gstStrBuilder.Insert(0, CloseMonoTag);
          gstStrBuilder.Insert(CloseMonoTag.Length, StartMonoTag);
        }
      }
      speedometerLabel.SetText(spmStrBuilder);
      if(Config.Representation.GoonStorageEnabled.Value){
        goonStorageLabel.SetText(gstStrBuilder);
        if(goonStorageLabel.fontSize != speedometerLabel.fontSize){
          goonStorageLabel.fontSize = speedometerLabel.fontSize;
        }
      }
    }

    public static void UpdateSpeedRep(){
      Config = MovementFunkPlugin.SpeedometerSettings;
      if(!Config.Representation.SpeedUnitsEnabled.Value){
        speedRep = string.Empty;
        return;
      }
      rep = Config.Representation.SpeedRep.Value;

      switch(rep){
        case Representation.SpeedUnits:
          speedRep = "SPD";
          break;
        case Representation.KilometersPerHour:
          speedRep = "KM/H";
          break;
        case Representation.MilesPerHour:
          speedRep = "MPH";
          break;
        case Representation.MetersPerSecond:
          speedRep = "M/S";
          break;
        default:
          speedRep = string.Empty;
          break;
      }
      if(Config.Representation.GoonStorageEnabled.Value){
        goonStorageLabel.fontSize = speedometerLabel.fontSize;
      }
    }
    public static void InitAltTrickLabel(TextMeshProUGUI someLabel){
      if(!Config.Representation.AltTrickComboCount.Value) return;
      trickComboLabel = Instantiate(someLabel, someLabel.transform.parent);
      LayoutElement layoutElement = trickComboLabel.GetComponent<LayoutElement>();
      layoutElement.minWidth = 200;
    }
    public static void UpdateAltTrickLabel(){
      if(!Config.Representation.AltTrickComboCount.Value) return;
      if(MovementFunkPlugin.player.IsComboing()){
        trickComboLabel.SetText("(tricks: x{0})", MovementFunkPlugin.player.tricksInCombo);
      }
      else{
        trickComboLabel.text = string.Empty;
      }
    }
    private static Color ShiftHueWithSpeed(Color color, float speed){
      float hue, saturation, value;
      Color.RGBToHSV(color, out hue, out saturation, out value);
      hue = Mathf.PingPong(colorShiftRate * speed, 1.0f);
      saturation = 1.0f;
      return Color.HSVToRGB(hue, saturation, value);
    }
    private static Color SetSaturationTo(Color color, float sat){
      float hue, saturation, value;
      Color.RGBToHSV(color, out hue, out saturation, out value);
      saturation = sat;
      return Color.HSVToRGB(hue, saturation, value);
    }
  }
}
