using Reptile;
using UnityEngine;
using System.Collections.Generic;

namespace MovementFunk.NewAbility
{
  public class MortarStrikeAbility : Ability {

    public MortarStrikeAbility(Player player) : base(player){}

    public bool canMortarStrike;
    private float fallSpeedCap;
    private float speed;
    private MFAbilityManager manager;

    private bool keybindsPressed;
    private List<string> keybinds;
    private bool mortarStrikeEnabled;
    private bool noDirInput;

    public override void Init(){
      canMortarStrike = true;
      fallSpeedCap = 0f;
      keybindsPressed = false;
      manager = MovementFunkPlugin.abilityManager;
    }

    public bool Activation(){
      UpdateConfig();
      keybindsPressed = MFInputBuffer.WasPressedRecentlyOrIsHeld(keybinds, 0.1f);

      if(!p.TreatPlayerAsSortaGrounded()
          && canMortarStrike
          && keybindsPressed
          && mortarStrikeEnabled
          && p.preAbility != manager.surfAbility
          && p.preAbility != manager.mortarStrikeAbility
          && (noDirInput ? (p.moveInput.sqrMagnitude == 0f) : true))
      {
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
      canMortarStrike = false;
    }

    public override void FixedUpdateAbility(){
      if(p.TreatPlayerAsSortaGrounded()){
        p.StopCurrentAbility();
        return;
      }
      p.boostAbility.CheckActivation();
      p.airDashAbility.CheckActivation();
    }

    public void UpdateConfig(){
      keybinds = MFMisc.StringToList(MovementFunkPlugin.MovementSettings.MortarStrike.Keybinds.Value);
      mortarStrikeEnabled = MovementFunkPlugin.MovementSettings.MortarStrike.Enabled.Value;
      noDirInput = MovementFunkPlugin.MovementSettings.MortarStrike.NoDirInput.Value;
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
