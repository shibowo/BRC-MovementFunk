using Reptile;
using UnityEngine;

namespace MovementPlus.Mechanics
{
  internal class DumpsterKick{

    public static void Update(){
      PerformDumpsterKick();
    }

    private static void PerformDumpsterKick(){ 
     if (MovementPlusPlugin.player.isAI || !MovementPlusPlugin.ConfigSettings.DumpsterKick.Enabled.Value) {return;}
     Player player = MovementPlusPlugin.player;
     HitBounceAbility hitBounceAbility = player.hitBounceAbility;

     bool bouncingOut = hitBounceAbility.state == HitBounceAbility.State.BOUNCE_OUT; //is this even counted anymore
     bool is_hitbouncing = player.ability == player.hitBounceAbility;
     bool timerExpired = player.abilityTimer > 1.0f; //config var when 
     bool sortaGrounded = player.TreatPlayerAsSortaGrounded();
     bool shouldDumpsterKick = sortaGrounded && bouncingOut && !timerExpired && is_hitbouncing;
     
     if(hitBounceAbility.p.jumpButtonNew && shouldDumpsterKick){
       string name = MovementPlusPlugin.ConfigSettings.DumpsterKick.Name.Value;
       MPTrickManager.AddTrick(name);
       
       int trickMinPoints = MovementPlusPlugin.ConfigSettings.DumpsterKick.pointsMin.Value;
       int trickPoints = Mathf.Max((int)(MovementPlusPlugin.ConfigSettings.DumpsterKick.pointsPerSpeed.Value * MPVariables.savedSpeedBeforeHitBounce), trickMinPoints);
       int trickListLength = MovementPlusPlugin.ConfigSettings.Misc.listLength.Value;
       int trickRepsToMin = MovementPlusPlugin.ConfigSettings.Misc.repsToMin.Value;

       int points = MPTrickManager.CalculateTrickValue(name, trickPoints, trickMinPoints, trickListLength, trickRepsToMin); 
       player.StopCurrentAbility();
       MPTrickManager.DoTrick(name, points);

       player.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.MoveStyle_0_Default, global::Reptile.AudioClipID.LegSweep, player.playerOneShotAudioSource, 0f);
       
       player.PlayAnim(player.jumpHash, true, true, -1f);
       float speed = MPVariables.savedSpeedBeforeHitBounce * MovementPlusPlugin.ConfigSettings.DumpsterKick.SpeedMultiplier.Value;
       player.motor.SetVelocityYOneTime(MPVariables.savedSpeedBeforeHitBounce); 
     } 
    }
  }
}
