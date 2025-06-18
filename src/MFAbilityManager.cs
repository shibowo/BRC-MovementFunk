using Reptile;
using MovementFunk.NewAbility;

namespace MovementFunk{
  public class MFAbilityManager{
    public ButtslapAbility buttslapAbility;
    public SurfAbility surfAbility;
    public MortarStrikeAbility mortarStrikeAbility;
    private Player player;

    public MFAbilityManager(Player p) {
      player = p;
    }

    public void InitCustomAbilities(){
      buttslapAbility = new ButtslapAbility(player);
      surfAbility = new SurfAbility(player);
      mortarStrikeAbility = new MortarStrikeAbility(player); 
    }
    public void ActivateCustomAbilities(){
      //CheckActivation gets called internally and for some reason causes insane lag.
      //So, at least for now, overriding CheckActivation isn't a good idea for custom abilities.
      buttslapAbility.Activation();
      surfAbility.Activation();
      mortarStrikeAbility.Activation();
    }
  }
}
