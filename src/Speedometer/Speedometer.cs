using Reptile;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

namespace MovementPlus.SpeedDisplay
{ 
  internal class Speedometer : MonoBehaviour{
    private static float speed = 0.0f;
    private static TextMeshProUGUI speedometerText;
    private static string speedRep;
    private static StringBuilder spmStrBuilder;
    private static Representation rep;

    private enum Representation {
      SpeedUnits,
      KilometersPerHour,
      MilesPerHour,
      MetersPerSecond,
      None
    }

    //these ratios are taken off SoftGoat's spedometer, not sure how these were calculated
    //followup: it appears that player speed is meters per second, at least I can assume so
    private const float MPH_ratio = 2.236936f;
    private const float KPH_ratio = 3.6f;
    

    public static void Init(TextMeshProUGUI someLabel){ 
      speedometerText = Instantiate(someLabel, someLabel.transform.parent);
      someLabel.transform.position += Vector3.up * 20.0f;
      spmStrBuilder = new StringBuilder();
      
      rep = Representation.SpeedUnits;
      switch(rep){
        case Representation.SpeedUnits:
          speedRep = "SPD";
          break;
        case Representation.KilometersPerHour:
          speedRep = "km/h";
          break;
        case Representation.MilesPerHour:
          speedRep = "MPH";
          break;
        case Representation.MetersPerSecond:
          speedRep = "m/s";
          break;
        case Representation.None:
        default:
          speedRep = "";
          break;
      }
    }
    public static void Update(){
      speed = MovementPlusPlugin.player.GetTotalSpeed(); // .GetForwardSpeed();
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
  }
}
