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
    MetersPerSecond,
    None
  }

  public class Speedometer : MonoBehaviour{
    private static float speed = 0.0f;
    private static TextMeshProUGUI speedometerText;
    private static string speedRep;
    private static StringBuilder spmStrBuilder;
    private static Representation rep;
    private static SpeedometerConfig Config = MovementFunkPlugin.SpeedometerSettings;
    private static float colorShiftRate = 0.2f;
    private static float redlineThreshold; 
    private static bool enabledOnce = false;
    private static float labelGap; 
    private static readonly CultureInfo labelCulture = CultureInfo.InvariantCulture;

    //these ratios are taken off SoftGoat's spedometer, not sure how these were calculated
    //followup: it appears that player speed is meters per second, at least I can assume so
    private const float MPH_ratio = 2.236936f;
    private const float KPH_ratio = 3.6f;
    
    private const string CloseMonoTag = "</mspace>";
    private const string StartMonoTag = "<mspace=1.133em>";

    public static void Init(TextMeshProUGUI someLabel){
      if(!Config.Representation.Enabled.Value) return;
      
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
      enabledOnce = true;

      speedometerText = Instantiate(someLabel, someLabel.transform.parent);
      someLabel.transform.position += Vector3.up * labelGap;
      if(Config.Formatting.UseMonospace.Value){
        speedometerText.richText = true;
      }
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
          speedometerText.color = SetSaturationTo(Color.red, speed / redlineThreshold);
        }
        else if(Config.Color.Rainbow.Value){
          speedometerText.color = ShiftHueWithSpeed(speedometerText.color, speed);
        }
      }

      spmStrBuilder.Clear();
      switch(rep){
        case Representation.SpeedUnits:
          speed *= 10;
          spmStrBuilder.AppendFormat(labelCulture, "{0:0} {1}", speed, speedRep);
          break;
        case Representation.KilometersPerHour:
          speed *= KPH_ratio;
          spmStrBuilder.AppendFormat(labelCulture, "{0:0.0} {1}", speed, speedRep);
          break;
        case Representation.MilesPerHour:
          speed *= MPH_ratio;
          spmStrBuilder.AppendFormat(labelCulture, "{0:0.0} {1}", speed, speedRep);
          break;
        default:
          spmStrBuilder.AppendFormat(labelCulture, "{0:0.0} {1}", speed, speedRep);
          break;
      }
      if(Config.Formatting.UseMonospace.Value){
        if(!Config.Formatting.MonospacedDot.Value && rep != Representation.SpeedUnits){
          int sepIndex = spmStrBuilder.ToString().IndexOf(labelCulture.NumberFormat.NumberDecimalSeparator);
          spmStrBuilder.Insert(sepIndex, CloseMonoTag);
          spmStrBuilder.Insert(sepIndex + CloseMonoTag.Length + 1, StartMonoTag);
        }
        spmStrBuilder.Insert(0, CloseMonoTag);
        spmStrBuilder.Insert(CloseMonoTag.Length + 1, StartMonoTag);
      }
      speedometerText.SetText(spmStrBuilder);
    }

    public static void UpdateSpeedRep(){
      if(!Config.Representation.Enabled.Value){
        speedometerText.text = ""; 
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
          speedRep = "MFH";
          break;
        case Representation.MetersPerSecond:
          speedRep = "M/S";
          break;
        case Representation.None:
        default:
          speedRep = "";
          break;
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
