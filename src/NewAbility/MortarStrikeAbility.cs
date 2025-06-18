using Reptile;
using UnityEngine;
using System.Collections.Generic;

namespace MovementFunk.NewAbility
{
  public class MortarStrikeAbility : Ability {

    public MortarStrikeAbility(Player player) : base(player){}

    private bool canMortarStrike;
    private float fallSpeedCap;
    private float speed;
    private bool keybinds;
    private MFAbilityManager abilityManager;

    public override void Init(){
      canMortarStrike = true;
      fallSpeedCap = 0f;
      keybinds = false;
      abilityManager = MovementFunkPlugin.abilityManager;
    }

    public bool Activation(){
      List<string> keys = MFMisc.StringToList(MovementFunkPlugin.MovementSettings.MortarStrike.Keybinds.Value);
      keybinds = MFInputBuffer.WasPressedRecentlyOrIsHeld(keys, 0.1f);

      if(!p.TreatPlayerAsSortaGrounded()
          && canMortarStrike
          && keybinds 
          && MovementFunkPlugin.MovementSettings.MortarStrike.Enabled.Value
          && p.preAbility != MovementFunkPlugin.abilityManager.surfAbility){
        p.ActivateAbility(this);
        return true;
      }
      return false;
    }

    public override void OnStartAbility(){
      fallSpeedCap = p.motor.maxFallSpeed;
      p.motor.maxFallSpeed = float.PositiveInfinity;
      canMortarStrike = false;
      PerformMortarStrike();
    }

    public void PerformMortarStrike(){
      if(p.isAI) return;
      speed = p.GetTotalSpeed(); 
      DoTrick();
      p.SetVelocity(Vector3.down * speed);
    }

    public override void OnStopAbility(){
      p.motor.maxFallSpeed = fallSpeedCap;
      canMortarStrike = true;
    }

    public override void FixedUpdateAbility(){
      if(p.TreatPlayerAsSortaGrounded()){
        p.StopCurrentAbility();
        return;
      }
      if(p.boostAbility.CheckActivation()){
        return;
      }
    }

    private void DoTrick(){
      string name = MovementFunkPlugin.MovementSettings.MortarStrike.Name.Value;
      int pointsMin = MovementFunkPlugin.MovementSettings.MortarStrike.PointsMin.Value;
      int points = MovementFunkPlugin.MovementSettings.MortarStrike.Points.Value;
      int listLength = MovementFunkPlugin.MovementSettings.Misc.listLength.Value;
      int repsToMin = MovementFunkPlugin.MovementSettings.Misc.repsToMin.Value;

      MFTrickManager.AddTrick(name);
      points = MFTrickManager.CalculateTrickValue(name, points, pointsMin, listLength, repsToMin);
      MFTrickManager.DoTrick(name, points);

      p.ringParticles.Emit(1);
      p.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, 
          global::Reptile.AudioClipID.singleBoost,
          p.playerOneShotAudioSource,
          0f);
    }
  }
}
