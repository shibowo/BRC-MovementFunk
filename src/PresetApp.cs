using CommonAPI;
using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MovementFunk.SpeedDisplay;

namespace MovementFunk
{
  public class MFCustomApp : CustomApp
  {
    protected static Sprite IconSprite = null;
    protected static List<string> presets;

    virtual protected List<string>GetAvailablePresets() {
      return new List<string>();
    }

    public override void OnAppInit()
    {
      base.OnAppInit();
      CreateIconlessTitleBar("Available Presets");

      ScrollView = PhoneScrollView.Create(this);
    }

  }
  internal class PresetApp : MFCustomApp {
    virtual public string TitleBarText => "Available Presets";
    protected void PopulateList()
    {
      ScrollView.AddButton(NoneButton());
      presets = GetAvailablePresets();
      if (presets.Count > 0)
      {
        foreach (string preset in presets)
        {
          if(preset == "None") continue;
          Console.WriteLine(preset);
          var button = CreatePresetButton(preset);
          ScrollView.AddButton(button);
        }
      }
    }
    public override void OnAppInit()
    {
      base.OnAppInit();
      CreateIconlessTitleBar(TitleBarText);

      ScrollView = PhoneScrollView.Create(this);
      PopulateList();
    }
    virtual protected SimplePhoneButton CreatePresetButton(string preset)
    {
      var button = PhoneUIUtility.CreateSimpleButton(preset);
      button.OnConfirm += () => {};
      return button;
    }

    virtual protected SimplePhoneButton NoneButton()
    {
      var button = PhoneUIUtility.CreateSimpleButton("None");
      button.OnConfirm += () => {};
      return button;
    }
  }
  internal class MovementPresetApp : PresetApp {
    public override bool Available => false;
    public override string TitleBarText => "Movement";
        
    public static void Init(string title)
    {
      PhoneAPI.RegisterApp<MovementPresetApp>(title);
    }

    protected override SimplePhoneButton CreatePresetButton(string preset)
    {
      var button = PhoneUIUtility.CreateSimpleButton(preset);
      button.OnConfirm += () =>
      {
        MFPresetManager.ApplyMovementPreset(preset);
      };
      return button;
    }

    protected override SimplePhoneButton NoneButton()
    {
      var button = PhoneUIUtility.CreateSimpleButton("None");
      button.OnConfirm += () =>
      {
        MFPresetManager.NoMovementPreset();
      };
      return button;
    }
    protected override List<string> GetAvailablePresets(){
      return MFPresetManager.GetAvailableMovementPresets();
    }
  }
  internal class SpeedometerPresetApp : PresetApp {
    public override bool Available => false;
    public override string TitleBarText => "Speedometer";

    public static void Init(string title)
    {
      PhoneAPI.RegisterApp<SpeedometerPresetApp>(title);
    }

    protected override SimplePhoneButton CreatePresetButton(string preset)
    {
      var button = PhoneUIUtility.CreateSimpleButton(preset);
      button.OnConfirm += () =>
      {
        MFPresetManager.ApplySpeedometerPreset(preset);
        Speedometer.UpdateSpeedRep();
      };
      return button;
    }

    protected override SimplePhoneButton NoneButton()
    {
      var button = PhoneUIUtility.CreateSimpleButton("None");
      button.OnConfirm += () =>
      {
        MFPresetManager.NoSpeedometerPreset();
        Speedometer.UpdateSpeedRep();
      };
      return button;
    }
    protected override List<string> GetAvailablePresets(){
      return MFPresetManager.GetAvailableSpeedometerPresets();
    }
  }
  public class MFMainApp : MFCustomApp {
    public static void Init(string title, string icon_filename)
    {
      string iconPath = Path.Combine(MovementFunkPlugin.Instance.Dir, icon_filename);
      try
      {
        IconSprite = TextureUtility.LoadSprite(iconPath);
        PhoneAPI.RegisterApp<MFMainApp>(title, IconSprite);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading icon sprite: {ex.Message}");
        PhoneAPI.RegisterApp<MFMainApp>(title, null);
      }
    }

    public override void OnAppInit()
    {
      base.OnAppInit();
      CreateIconlessTitleBar("Available Presets");

      ScrollView = PhoneScrollView.Create(this);
      SimplePhoneButton movementAppButton = PhoneUIUtility.CreateSimpleButton("Movement");

      movementAppButton.OnConfirm += () => {
        MyPhone.OpenApp(typeof(MovementPresetApp));
      };

      ScrollView.AddButton(movementAppButton);

      SimplePhoneButton speedometerAppButton = PhoneUIUtility.CreateSimpleButton("Speedometer");

      speedometerAppButton.OnConfirm += () => {
        MyPhone.OpenApp(typeof(SpeedometerPresetApp));
      };

      ScrollView.AddButton(speedometerAppButton);
    }
  }
}
