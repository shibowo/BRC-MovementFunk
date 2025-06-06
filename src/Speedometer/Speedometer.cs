using Reptile;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

namespace MovementPlus.SpeedDisplay
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
    private static MyConfig Config = MovementPlusPlugin.ConfigSettings;

      
    //these ratios are taken off SoftGoat's spedometer, not sure how these were calculated
    //followup: it appears that player speed is meters per second, at least I can assume so
    private const float MPH_ratio = 2.236936f;
    private const float KPH_ratio = 3.6f;
    

    public static void Init(TextMeshProUGUI someLabel){
      if(!Config.Speedometer.Enabled.Value) return;

      speedometerText = Instantiate(someLabel, someLabel.transform.parent);
      someLabel.transform.position += Vector3.up * 20.0f;
      spmStrBuilder = new StringBuilder();

      UpdateSpeedRep();
    }
    public static void Update(){
      if(!Config.Speedometer.Enabled.Value) return;

      if(Config.Speedometer.UseTotalSpeed.Value){
        speed = MovementPlusPlugin.player.GetTotalSpeed(); 
      }
      else {
        speed = MovementPlusPlugin.player.GetForwardSpeed();
      }

      if(Config.Speedometer.Rainbow.Value){
        speedometerText.color = ShiftHueBySpeed(speedometerText.color, speed);
      }

      spmStrBuilder.Clear();
      switch(rep){
        case Representation.SpeedUnits:
          speed *= 10;
          spmStrBuilder.AppendFormat("{0:0} {1}", speed, speedRep);
          break;
        case Representation.KilometersPerHour:
          speed *= KPH_ratio;
          spmStrBuilder.AppendFormat("{0:0.0#} {1}", speed, speedRep);
          break;
        case Representation.MilesPerHour:
          speed *= MPH_ratio;
          spmStrBuilder.AppendFormat("{0:0.0#} {1}", speed, speedRep);
          break;
        default:
          spmStrBuilder.AppendFormat("{0:0.0#} {1}", speed, speedRep);
          break;
      }
      speedometerText.SetText(spmStrBuilder);
    }

    public static void UpdateSpeedRep(){
      if(!MovementPlusPlugin.ConfigSettings.Speedometer.Enabled.Value) return;

      rep = MovementPlusPlugin.ConfigSettings.Speedometer.SpeedRep.Value;

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
        case Representation.None:
        default:
          speedRep = "";
          break;
      }  
    }

    private static Color ShiftHueBySpeed(Color color, float speed){
      float hue, saturation, value;
      Color.RGBToHSV(color, out hue, out saturation, out value);
      hue = Mathf.PingPong(speed, 1.0f);
      return Color.HSVToRGB(hue, saturation, value);
    }
  }
}
