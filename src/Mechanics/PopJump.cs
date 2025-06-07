using Reptile;
using UnityEngine;

namespace MovementFunk.Mechanics
{
  internal class PopJump{

    public static void Update(){
      PerformPopJump();
    }

    private static void PerformPopJump(){ 
     if (MovementFunkPlugin.player.isAI || !MovementFunkPlugin.ConfigSettings.PopJump.Enabled.Value) {return;}
     Player player = MovementFunkPlugin.player;
     HitBounceAbility hitBounceAbility = player.hitBounceAbility;

     bool bouncingOut = hitBounceAbility.state == HitBounceAbility.State.BOUNCE_OUT; //is this even counted anymore
     bool is_hitbouncing = player.ability == player.hitBounceAbility;
     bool timerExpired = player.abilityTimer > MovementFunkPlugin.ConfigSettings.PopJump.GracePeriod.Value; 
     bool sortaGrounded = player.TreatPlayerAsSortaGrounded();
     bool shouldPopJump = sortaGrounded && bouncingOut && !timerExpired && is_hitbouncing;
     
     if(hitBounceAbility.p.jumpButtonNew && shouldPopJump){
       string name = MovementFunkPlugin.ConfigSettings.PopJump.Name.Value;
       MFTrickManager.AddTrick(name);
       
       int trickMinPoints = MovementFunkPlugin.ConfigSettings.PopJump.pointsMin.Value;
       int trickPoints = Mathf.Max((int)(MovementFunkPlugin.ConfigSettings.PopJump.pointsPerSpeed.Value * MFVariables.savedSpeedBeforeHitBounce), trickMinPoints);
       int trickListLength = MovementFunkPlugin.ConfigSettings.Misc.listLength.Value;
       int trickRepsToMin = MovementFunkPlugin.ConfigSettings.Misc.repsToMin.Value;

       int points = MFTrickManager.CalculateTrickValue(name, trickPoints, trickMinPoints, trickListLength, trickRepsToMin); 
       player.StopCurrentAbility();
       MFTrickManager.DoTrick(name, points);

       player.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.MoveStyle_0_Default, global::Reptile.AudioClipID.LegSweep, player.playerOneShotAudioSource, 0f);
       
       player.PlayAnim(player.jumpHash, true, true, -1f);
       float speed = MFVariables.savedSpeedBeforeHitBounce * MovementFunkPlugin.ConfigSettings.PopJump.SpeedMultiplier.Value;
       player.motor.SetVelocityYOneTime(MFVariables.savedSpeedBeforeHitBounce); 
     } 
    }
  }
}
