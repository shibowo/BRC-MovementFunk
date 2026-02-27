using BepInEx.Configuration;

namespace MovementFunk;

public class MovementConfig(ConfigFile config)
{
    public ConfigRailGoon RailGoon = new(config, "Rail Goon");
    public ConfigRailFrameboost RailFrameboost = new(config, "Rail Frameboost");
    public ConfigWallFrameboost WallFrameboost = new(config, "Wall Frameboost");
    public ConfigWallBoostplant WallBoostplant = new(config, "Wall Boostplant");
    public ConfigWallGeneral WallGeneral = new(config, "Wall General");
    public ConfigSuperTrickJump SuperTrickJump = new(config, "Super Trick Jump");
    public ConfigPerfectManual PerfectManual = new(config, "Perfect Manual");
    public ConfigSuperSlide SuperSlide = new(config, "Super Slide");
    public ConfigFastFall FastFall = new(config, "Fast Fall");
    public ConfigVertGeneral VertGeneral = new(config, "Vert General");
    public ConfigBoostGeneral BoostGeneral = new(config, "Boost General");
    public ConfigRailGeneral RailGeneral = new(config, "Rail General");
    public ConfigRailSlope RailSlope = new(config, "Rail Slope");
    public ConfigComboGeneral ComboGeneral = new(config, "Combo General");
    public ConfigLedgeClimbGeneral LedgeClimbGeneral = new(config, "Ledge Climb General");
    public ConfigButtslap Buttslap = new(config, "Buttslap");
    public ConfigWaveDash WaveDash = new(config, "Wave Dash");
    public ConfigHandplant Handplant = new(config, "Handplant");
    public ConfigMisc Misc = new(config, "Misc");
    public ConfigNonStable NonStable = new(config, "NonStable");
    public ConfigPopJump PopJump = new(config, "Pop Jump");
    public ConfigMortarStrike MortarStrike = new(config, "Mortar Strike");

    public enum DoubleJumpType
    {
        Additive,
        Replace,
        Capped
    };

    public enum BoostReturnType
    {
        Once,
        Always,
        Disabled
    };

    public enum HardLandingType
    {
        Off,
        OnlyFeet,
        OnlyMovestyle,
        FeetAndMovestyle
    };

    public enum AverageSpeedMode
    {
        Average,
        Max,
        BlendMaxAverage
    };

    public class ConfigRailGoon(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Rail Goon Enabled",
            true,
            "Trigered by landing on a rail corner."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Rail Goon Amount",
           15f,
           "Amount of speed added when triggering a rail goon."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Rail Goon Cap",
           -1f,
           "Maximum amount of speed that can be added when triggering a rail goon."
       );

        public ConfigEntry<string> Name = config.Bind(
           category,
           "Rail Goon Trick Name",
           "Rail Goon",
           "Name of the trick when performing a rail goon."
       );

        public ConfigEntry<int> points = config.Bind(
           category,
           "Rail Goon Points",
           0,
           "Points given when performing a rail goon."
       );

        public ConfigEntry<int> pointsMin = config.Bind(
           category,
           "Rail Goon Points",
           0,
           "Minimum amount of points given when performing a rail goon."
       );
    }

    public class ConfigRailFrameboost(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Rail Frameboost Enabled",
           true,
           "Trigered by jumping shortly after landing on a rail."
       );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Rail Frameboost Amount",
          5f,
          "Amount of speed added when triggering a rail frameboost."
      );

        public ConfigEntry<float> Grace = config.Bind(
          category,
          "Rail Frameboost Grace",
          0.1f,
          "Amount of time for a rail frameboost."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Rail Frameboost Cap",
          -1f,
          "Maximum speed that can be obtained from rail frameboosts."
      );
    }

    public class ConfigWallFrameboost(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Wall Frameboost Enabled",
           true,
           "Trigered by jumping after landing on a wallride."
       );

        public ConfigEntry<bool> RunoffEnabled = config.Bind(
           category,
           "Wall Frameboost Runoff Enabled",
           false,
           "Trigered by running off a wallride"
       );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Wall Frameboost Amount",
           5.0f,
           "Amount of speed added when triggering a wallride frameboost."
       );

        public ConfigEntry<float> Grace = config.Bind(
           category,
           "Wall Frameboost Grace",
           0.1f,
           "Amount of time for a wallride frameboost."
       );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Wall Frameboost Cap",
          -1f,
          "Maximum speed that can be obtained from wallride frameboosts."
       );
    }

    public class ConfigWallBoostplant(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Wall Boostplant Enabled",
           true,
           "Trigered by jumping after landing on a wallride and while holding trick 2 and trick 3."
       );

        public ConfigEntry<string> Buttons = config.Bind(
           category,
           "Wall Boostplant Buttons",
           "anyTwoTricks",
           "Buttons that need to be held or pressed within the buffer window to trigger a boostplant. Valid buttons are boost, dance, jump, slide, switchstyle, trick1, trick2, trick3, walk, anyTrick, anyTwoTricks"
       );

        public ConfigEntry<float> Grace = config.Bind(
           category,
           "Wall Boostplant Grace",
           0.1f,
           "Amount of time for a boostplant."
       );

        public ConfigEntry<float> Strength = config.Bind(
           category,
           "Wall Boostplant Jump Strength",
           0.5f,
           "Amount of total speed that gets converted to height."
       );

        public ConfigEntry<float> SpeedStrength = config.Bind(
           category,
           "Wall Boostplant Speed Reduction Strength",
           0.85f,
           "Amount of forward speed to remove when performing a boostplant."
       );

        public ConfigEntry<float> Buffer = config.Bind(
           category,
           "Tricks Buffer Window",
           0.25f,
           "The buffer window for tricks 2 and 3."
       );
    }

    public class ConfigWallGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<float> wallTotalSpeedCap = config.Bind(
          category,
          "Wall Total Speed Cap",
          -1f,
          "Maximum amount of speed that can be added when landing on a wallride."
       );

        public ConfigEntry<float> goonStorageMin = config.Bind(
          category,
          "Goon Storage Minimum",
          40f,
          "Minimum speed your stored goonride can be."
       );

        public ConfigEntry<float> goonStorageMax = config.Bind(
          category,
          "Goon Storage Maximum",
          -1f,
          "Maximum speed your stored goonride can be."
       );

        public ConfigEntry<float> wallCD = config.Bind(
          category,
          "Wallrun Cooldown",
          0.2f,
          "Time that needs to pass before you can perform another wallrun."
       );

        public ConfigEntry<float> decc = config.Bind(
          category,
          "Wallrun Deceleration",
          0f,
          ""
       );

        public ConfigEntry<float> minDurJump = config.Bind(
          category,
          "Minimum Duration Before Jump",
          0f,
          ""
       );
    }

    public class ConfigSuperTrickJump(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Super Trick Jump Enabled",
           true,
           "On foot trick jump height increases with forward speed."
       );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Super Trick Jump Amount",
           0.2f,
           "Amount of height gained with forward speed."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Super Trick Jump Cap",
           30f,
           "Maximum height gained from a super trick jump."
       );

        public ConfigEntry<float> Threshold = config.Bind(
           category,
           "Super Trick Jump Threshold",
           21f,
           "Minimum Speed required to add any jump height."
       );

        public ConfigEntry<bool> EarlySuperTrick = config.Bind(
           category,
           "Allow Early Super Trick Jump",
           false,
           "Allows the super trick jump to be performed early if you're allowed to jump."
       );

        public ConfigEntry<bool> MSSuperTrick = config.Bind(
           category,
           "Allow Movestyle Super Trick Jump",
           false,
           "Allows the super trick jump to be performed with movestyles."
       );
    }

    public class ConfigPerfectManual(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Perfect Manual Enabled",
          true,
          "Pressing manual just before landing."
      );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Perfect Manual Amount",
          5f,
          "Amount of speed added when performing a perfect manual."
      );

        public ConfigEntry<float> Grace = config.Bind(
          category,
          "Perfect Manual Grace",
          0.1f,
          "Amount of time for a perfect manual."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Perfect Manual Cap",
          45f,
          "Maximum speed that can be obtained from perfect manuals."
      );

        public ConfigEntry<string> Prefix = config.Bind(
          category,
          "Trick Name Prefix",
          "Perfect",
          "Text added before the trick name when performing a perfect manual."
      );
    }

    public class ConfigSuperSlide(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
         category,
         "Super Slide Enabled",
         true,
         "Carpet sliding changes."
     );

        public ConfigEntry<float> Speed = config.Bind(
          category,
          "Super Slide Base Speed",
          35f,
          "Base speed of the super slide, this speed is reached very quickly."
      );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Super Slide Amount",
          0.1f,
          "Amount of speed added while super sliding."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Super Slide Cap",
          55f,
          "Maximum speed that can be obtained from super sliding."
      );
    }

    public class ConfigFastFall(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
        category,
        "Fast Fall Enabled",
        true,
        "Pressing manual while falling."
    );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Fast Fall Amount",
          -13f,
          "Amount of speed added when triggering a fast fall."
      );

        public ConfigEntry<bool> FallEnabled = config.Bind(
          category,
          "Fast Fall Requires Falling",
          true,
          "Falling is required for triggering a fast fall."
      );

        public ConfigEntry<bool> ResetOnDash = config.Bind(
          category,
          "Reset Fast Fall On Air Dash",
          true,
          "Resets fast fall when using air dash or double jump."
      );

        public ConfigEntry<bool> CancelBoost = config.Bind(
         category,
         "Fast Fall Cancel Boost Ability",
         true,
         "If you can fast while in a boost and you do so the boost ability will cancel"
     );
    }

    public class ConfigVertGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Vert Change Enabled",
          true,
          "Vert ramp speed is adjusted to your total speed."
      );

        public ConfigEntry<bool> JumpEnabled = config.Bind(
          category,
          "Vert Jump Change Enabled",
          true,
          "Jump height from vert ramps scales with your speed."
      );

        public ConfigEntry<float> JumpStrength = config.Bind(
          category,
          "Vert Jump Strength",
          0.4f,
          "Multiplier applied to your total speed to calculate vert jump height."
      );

        public ConfigEntry<float> JumpCap = config.Bind(
          category,
          "Vert Jump Cap",
          75f,
          "Maximum jump height from vert ramps."
      );

        public ConfigEntry<float> ExitSpeed = config.Bind(
          category,
          "Vert Bonus Bottom Exit Speed",
          7f,
          "Bonus speed added when exiting a vert ramp."
      );

        public ConfigEntry<float> ExitSpeedCap = config.Bind(
          category,
          "Vert Bonus Bottom Exit Speed Cap",
          75f,
          "Maximum speed that can be obtained from exiting a vert ramp."
      );
    }

    public class ConfigBoostGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> StartEnabled = config.Bind(
         category,
         "Boost Start Change Enabled",
         true,
         "Bonus speed when using a boost outside of a grind or wallride."
     );

        public ConfigEntry<bool> SpeedScale = config.Bind(
         category,
         "Boost Speed Set To Forward Speed",
         true,
         "Boost speed is set to the players forward speed instead of a flat speed."
     );

        public ConfigEntry<float> StartAmount = config.Bind(
         category,
         "Boost Start Amount",
         2.75f,
         "Amount of speed added when starting a boost."
     );

        public ConfigEntry<float> StartCap = config.Bind(
         category,
         "Boost Start Cap",
         60f,
         "Maximum speed that can be obtained from starting a boost."
     );

        public ConfigEntry<float> RailAmount = config.Bind(
         category,
         "Boost On Rail Speed Over Time",
         4f,
         "Amount of speed added over time while boosting on a rail."
     );

        public ConfigEntry<float> WallAmount = config.Bind(
         category,
         "Boost On Wallride Speed Over Time",
         15f,
         "Amount of speed added over time while boosting on a wallride."
     );

        public ConfigEntry<float> RailCap = config.Bind(
         category,
         "Boost Rail Cap",
         55f,
         "Maximum amount of speed that can be obtained from boosting while on a rail."
     );

        public ConfigEntry<float> WallCap = config.Bind(
         category,
         "Boost Wallride Cap",
         70f,
         "Maximum amount of speed that can be obtained from boosting while on a wallride."
     );

        public ConfigEntry<bool> TotalSpeedEnabled = config.Bind(
         category,
         "Total Speed Change Enabled",
         true,
         "Boost speed scales with total speed."
     );

        public ConfigEntry<float> TotalSpeedCap = config.Bind(
         category,
         "Total Speed Change Cap",
         -1f,
         "Maximum amount of speed obtainable from the total speed boost change."
     );

        public ConfigEntry<bool> NoBoostLossTrick = config.Bind(
         category,
         "Preserve Air Boost After Boost Trick",
         true,
         "Retains the ability to use air boost if you don't consume it while using an air boost trick."
     );

        public ConfigEntry<BoostReturnType> BoostReturnType = config.Bind(
           category,
           "Boost Trick Returns Air Boost",
           MovementConfig.BoostReturnType.Once,
           "Restores air boost after using an air boost trick. Options: Once per jump, every boost trick, or disabled."
       );

        public ConfigEntry<BoostReturnType> BoostDashReturnType = config.Bind(
           category,
           "Boost Trick Returns Air Dash",
           MovementConfig.BoostReturnType.Once,
           "Restores air dash after using an air boost trick. Options: Once per jump, every boost trick, or disabled."
       );

        public ConfigEntry<BoostReturnType> BoostMortarStrikeReturnType = config.Bind(
           category,
           "Boost Trick Returns Mortar Strike",
           MovementConfig.BoostReturnType.Once,
           "Restores mortar strike after using an air boost trick. Options: Once per jump, every boost trick, or disabled."
           );

        public ConfigEntry<bool> InfiniteBoost = config.Bind(
         category,
         "Infinite Boost",
         false,
         "Boost meter is always full."
     );

        public ConfigEntry<bool> RetainY = config.Bind(
         category,
         "Retain Y velocity on air boost",
         false,
         "Retains vertical velocity when performing a boost in the air instead of setting it to 0."
     );

        public ConfigEntry<bool> Force0 = config.Bind(
         category,
         "Force 0 Y Velocity On Start Boost",
         false,
         "Forces vertical velocity to 0 when starting an air boost, this is seperate and happens sooner than the vanilla one."
     );

        public ConfigEntry<float> decc = config.Bind(
         category,
         "Boost Deceleration",
         0f,
         ""
     );
    }

    public class ConfigRailGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<float> HardAmount = config.Bind(
         category,
         "Rail Hard Corner Amount",
         1.5f,
         "Amount of speed per hard corner."
     );

        public ConfigEntry<float> HardCap = config.Bind(
         category,
         "Rail Hard Corner Cap",
         45f,
         "Maximum amount of speed that can be obtained from rail hard corners."
     );

        public ConfigEntry<float> Decc = config.Bind(
         category,
         "Rail Deceleration",
         1f,
         "Deceleration while grinding."
     );

        public ConfigEntry<bool> ChangeEnabled = config.Bind(
         category,
         "Hard Corner Change Enabled",
         true,
         "Changes how hard corners are calculated, this should stop them from being tied to the framerate."
     );

        public ConfigEntry<float> HCThresh = config.Bind(
         category,
         "Hard Corner Threshold",
         5f,
         "How strict the hard corner detection is. A lower value means more corners will be considered hard corners."
     );

        public ConfigEntry<bool> BoostCornerEnabled = config.Bind(
         category,
         "Boost Hard Corner",
         true,
         "Boost to hit hard corners, no tilting required."
     );

        public ConfigEntry<bool> Detection = config.Bind(
         category,
         "Extra Rail Detection",
         true,
         "Extra detection for rails, can make grinding at higher speeds more consistent."
     );

        public ConfigEntry<bool> ChangeDirectionEnabled = config.Bind(
         category,
         "Change Grind Direction On Start Grind Enabled",
         false,
         "(WARNING: This feature is obsolete and breaks vertical grinds. Use at your own risk.)Allows the player to choose where they want to grind when first starting a grind."
     );

        public ConfigEntry<float> ChangeDirectionAngle = config.Bind(
         category,
         "Grind Direction Change Max Angle",
         95f,
         "The maximum angle the player can change from their forward direction when starting a grind."
     );

        public ConfigEntry<bool> RailReversalEnabled = config.Bind(
         category,
         "Rail Reversal Enabled",
         true,
         "Holding the boost button, slide button and down (by default) will allow the player to reverse their grind direction."
     );

        public ConfigEntry<string> RailReversalButtons = config.Bind(
         category,
         "Rail Reversal Buttons",
         "slide, boost, down",
         "The buttons used to trigger a rail reversal."
     );

        public ConfigEntry<float> RailReversalCD = config.Bind(
         category,
         "Rail Reversal Cooldown",
         0.55f,
         "Time needed to pass before you can perform another rail reversal."
     );

        public ConfigEntry<float> railCD = config.Bind(
         category,
         "Rail Cooldown",
         0.2f,
         "Time needed to pass before you can grind on any rail."
     );

        public ConfigEntry<bool> GrindStartSpeed = config.Bind(
         category,
         "Rail Start Average Speed",
         true,
         "Speed is set to your average when starting a grind."
     );

        public ConfigEntry<bool> ModifyJump = config.Bind(
         category,
         "Modify Grind Jump",
         true,
         ""
     );

        public ConfigEntry<bool> ModifyFlipout = config.Bind(
         category,
         "Modify Grind Jump flipout",
         true,
         ""
     );

        public ConfigEntry<bool> ModifyUpdateSpeed = config.Bind(
         category,
         "Modify Update Speed Adjustment",
         true,
         ""
     );

        public ConfigEntry<bool> ModifyUpdateSpeedBoost = config.Bind(
         category,
         "Modify Update Speed Boost Adjustment",
         true,
         ""
     );

        public ConfigEntry<bool> ModifyUpdateSpeedBrake = config.Bind(
         category,
         "Modify Update Speed Brake Adjustment",
         true,
         ""
     );

        public ConfigEntry<bool> KeepVelOnExit = config.Bind(
         category,
         "Keep Grind Velocity On End",
         true,
         "When grinding off of a rail you will keep the velocity you had while grinding."
     );
    }

    public class ConfigRailSlope(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
         category,
         "Rail Slope Change Enabled",
         true,
         "Jumping while on an upwards sloped rail gives a larger jump and a downwards slope gives bonus speed."
     );

        public ConfigEntry<float> SlopeJumpAmount = config.Bind(
         category,
         "Rail Slope Jump Height Amount",
         4f,
         "Bonus height amount for sloped rail jump. This is affected by your speed and the slope of the rail."
     );

        public ConfigEntry<float> SlopeSpeedAmount = config.Bind(
         category,
         "Rail Slope Jump Speed Amount",
         6f,
         "Bonus speed amount for sloped rail jump. This is affected by your speed and the slope of the rail."
     );

        public ConfigEntry<float> SlopeJumpMax = config.Bind(
         category,
         "Rail Slope Jump Height Max",
         20f,
         "Maximum jump height you can gain from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedCap = config.Bind(
         category,
         "Rail Slope Jump Speed Cap",
         -1f,
         "Maximum Speed you can gain from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeJumpMin = config.Bind(
         category,
         "Rail Slope Jump Height Minimum",
         -7f,
         "Minimum jump height from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedMin = config.Bind(
         category,
         "Rail Slope Jump Speed Minimum",
         -10f,
         "Minimum speed from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedMax = config.Bind(
         category,
         "Rail Slope Jump Speed Max",
         7f,
         "Maximum speed gained from a single sloped rail jump."
     );
    }

    public class ConfigComboGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> BoostEnabled = config.Bind(
         category,
         "Combo During Boost Enabled",
         true,
         "Allows you to maintain combo while boosting."
     );

        public ConfigEntry<bool> NoAbilityEnabled = config.Bind(
         category,
         "Combo No Ability Enabled",
         true,
         "Forces combo meter when touching the ground even without a manual."
     );

        public ConfigEntry<float> BoostTimeout = config.Bind(
         category,
         "Combo Boost Timer",
         0.5f,
         "How fast the combo timer ticks while boosting."
     );

        public ConfigEntry<float> BoostJumpAmount = config.Bind(
         category,
         "Combo Boost Jump",
         0.1f,
         "Amount of the combo timer is removed when jumping while boosting."
     );

        public ConfigEntry<float> NoAbilityTimeout = config.Bind(
         category,
         "Combo No Ability Timer",
         5f,
         "How fast the combo timer ticks down while not manualing."
     );
    }

    public class ConfigLedgeClimbGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Ledge Climb Cancel Enabled",
            true,
            "Allows you to cancel the ledge climb animation with a jump."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Ledge Climb Cancel Amount",
           15f,
           "Amount of speed added when you cancel a ledge climb."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Ledge Climb Cancel Cap",
           -1f,
           "Maximum amount of speed that can be added when canceling a ledge climb."
       );
    }

    public class ConfigButtslap(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Buttslap Enabled",
            true,
            "Allows a jump during a ground trick animation while in the air."
        );

        public ConfigEntry<bool> MultiEnabled = config.Bind(
            category,
            "Buttslap Multi Enabled",
            true,
            "Allows multiple jumps during a buttslap."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Buttslap Amount",
           2f,
           "Forward speed added when performing a buttslap."
       );

        public ConfigEntry<float> ComboAmount = config.Bind(
          category,
          "Buttslap Combo Amount",
          -0.1f,
          "Amount of combo meter added when performing a buttslap."
      );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Buttslap Cap",
           -1f,
           "Maximum amount of speed that can be added when performing a buttslap."
       );

        public ConfigEntry<float> JumpAmount = config.Bind(
          category,
          "Buttslap Jump Amount",
          5f,
          "Jump height per buttslap."
      );

        public ConfigEntry<float> Timer = config.Bind(
          category,
          "Buttslap Multi Time",
          0.45f,
          "Amount of time after the initial buttslap to perform multiple."
      );

        public ConfigEntry<string> Name = config.Bind(
          category,
          "Buttslap Trick Name",
          "Buttslap",
          "The trick name when performing a buttslap."
      );

        public ConfigEntry<int> Points = config.Bind(
          category,
          "Buttslap Trick Points",
          100,
          "Amount of points given when performing a buttslap."
      );

        public ConfigEntry<int> PointsMin = config.Bind(
          category,
          "Buttslap Trick Points Minimum",
          10,
          "Minimum amount of points given when performing a buttslap."
      );

        public ConfigEntry<string> BoostName = config.Bind(
          category,
          "Boosted Buttslap Trick Name",
          "Boosted Buttslap",
          "The trick name when performing a buttslap from a boost trick."
      );

        public ConfigEntry<int> BoostPoints = config.Bind(
          category,
          "Boosted Buttslap Trick Points",
          500,
          "Amount of points given when performing a buttslap from a boost trick."
      );

        public ConfigEntry<int> BoostPointsMin = config.Bind(
          category,
          "Boosted Buttslap Trick Points Minimum",
          50,
          "Minimum amount of points given when performing a buttslap from a boost trick."
      );

        public ConfigEntry<float> PoleAmount = config.Bind(
           category,
           "Pole Buttslap Amount",
           2f,
           "Forward speed added when performing a polevault."
       );

        public ConfigEntry<float> PoleComboAmount = config.Bind(
          category,
          "Pole Buttslap Combo Amount",
          -0.1f,
          "Amount of combo meter added when performing a polevault."
      );

        public ConfigEntry<float> PoleCap = config.Bind(
           category,
           "Pole Buttslap Cap",
           -1f,
           "Maximum amount of speed that can be added when performing a polevault."
       );

        public ConfigEntry<float> PoleJumpAmount = config.Bind(
          category,
          "Pole Buttslap Jump Amount",
          5f,
          "Jump height per buttslap."
      );

        public ConfigEntry<string> PoleName = config.Bind(
          category,
          "Pole Buttslap Trick Name",
          "Polevault",
          "The trick name when performing a buttslap from a pole."
      );

        public ConfigEntry<int> PolePoints = config.Bind(
          category,
          "Pole Buttslap Trick Points",
          100,
          "Amount of points given when performing a polevault."
      );

        public ConfigEntry<int> PolePointsMin = config.Bind(
          category,
          "Pole Buttslap Trick Points Minimum",
          10,
          "Minimum amount of points given when performing a polevault."
      );

        public ConfigEntry<string> PoleBoostName = config.Bind(
          category,
          "Pole Boosted Buttslap Trick Name",
          "Boosted Polevault",
          "The trick name when performing a polevault from a boost trick."
      );

        public ConfigEntry<int> PoleBoostPoints = config.Bind(
          category,
          "Pole Boosted Buttslap Trick Points",
          500,
          "Amount of points given when performing a polevault from a boost trick."
      );

        public ConfigEntry<int> PoleBoostPointsMin = config.Bind(
          category,
          "Pole Boosted Buttslap Trick Points Minimum",
          50,
          "Minimum amount of points given when performing a polevault from a boost trick."
      );

        public ConfigEntry<float> SurfAmount = config.Bind(
           category,
           "Surf Buttslap Amount",
           2f,
           "Forward speed added when performing a wavejump."
       );

        public ConfigEntry<float> SurfComboAmount = config.Bind(
          category,
          "Buttslap Combo Amount",
          -0.1f,
          "Amount of combo meter added when performing a wavejump."
      );

        public ConfigEntry<float> SurfCap = config.Bind(
           category,
           "Buttslap Cap",
           -1f,
           "Maximum amount of speed that can be added when performing a wavejump."
       );

        public ConfigEntry<float> SurfJumpAmount = config.Bind(
          category,
          "Surf Buttslap Jump Amount",
          5f,
          "Jump height per wavejump."
      );

        public ConfigEntry<string> SurfName = config.Bind(
          category,
          "Surf Buttslap Trick Name",
          "Wavejump",
          "The trick name when performing a buttslap from a surf."
      );

        public ConfigEntry<int> SurfPoints = config.Bind(
          category,
          "Surf Buttslap Trick Points",
          100,
          "Amount of points given when performing a wavejump."
      );

        public ConfigEntry<int> SurfPointsMin = config.Bind(
          category,
          "Surf Buttslap Trick Points Minimum",
          10,
          "Minimum amount of points given when performing a wavejump."
      );

        public ConfigEntry<string> SurfBoostName = config.Bind(
          category,
          "Surf Boosted Buttslap Trick Name",
          "Boosted Wavejump",
          "The trick name when performing a wavejump from a boost trick."
      );

        public ConfigEntry<int> SurfBoostPoints = config.Bind(
          category,
          "Surf Boosted Buttslap Trick Points",
          500,
          "Amount of points given when performing a wavejump from a boost trick."
      );

        public ConfigEntry<int> SurfBoostPointsMin = config.Bind(
          category,
          "Surf Boosted Buttslap Trick Points Minimum",
          50,
          "Minimum amount of points given when performing a wavejump from a boost trick."
      );
    }

    public class ConfigWaveDash(ConfigFile config, string category)
    {
        public ConfigEntry<float> grace = config.Bind(
          category,
          "Wave Dash Grace",
          0.3f,
          "Amount of time after a Fast Fall to start a Wave Dash."
      );

        public ConfigEntry<float> NormalSpeed = config.Bind(
          category,
          "Normal Wave Dash Amount",
          7f,
          "Amount of forward speed given when performing a Wave Dash."
      );

        public ConfigEntry<float> BoostSpeed = config.Bind(
          category,
          "Boost Wave Dash Amount",
          17f,
          "Amount of forward speed given when performing a Boost Wave Dash."
      );
        public ConfigEntry<float> BoostCost = config.Bind(
          category,
          "Boost Wave Dash Cost",
          13f,
          "Amount of boost performing a single Boost Wave Dash costs."
      );
        public ConfigEntry<int> NormalPoints = config.Bind(
          category,
          "Normal Wave Dash Point Amount",
          100,
          "Amount of points given when performing a Wave Dash."
      );

        public ConfigEntry<int> NormalPointsMin = config.Bind(
          category,
          "Normal Wave Dash Point Amount Minimum",
          10,
          "Minimum amount of points given when performing a Wave Dash."
      );

        public ConfigEntry<int> BoostPoints = config.Bind(
          category,
          "Boost Wave Dash Point Amount",
          250,
          "Amount of points given when performing a Boost Wave Dash."
      );

        public ConfigEntry<int> BoostPointsMin = config.Bind(
          category,
          "Boost Wave Dash Point Amount Minimum",
          50,
          "Minimum amount of points given when performing a Boost Wave Dash."
      );

        public ConfigEntry<string> NormalName = config.Bind(
          category,
          "Normal Wave Dash Trick Name",
          "Wave Dash",
          "Trick name when performing a Wave Dash."
      );

        public ConfigEntry<string> BoostName = config.Bind(
          category,
          "Boosted Wave Dash Trick Name",
          "Boost Wave Dash",
          "Trick name when performing a Boost Wave Dash."
      );
    }

    public class ConfigHandplant(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Handplant Trick Jump Enabled",
           true,
           "Trigered by tricking during a handplant."
       );

        public ConfigEntry<float> Strength = config.Bind(
          category,
          "Handplant Trick Jump Strength",
          0.5f,
          "Amount of forward speed converted to height when performing a handplant trick jump."
      );
    }

    public class ConfigMisc(ConfigFile config, string category)
    {
        public ConfigEntry<float> groundTrickDecc = config.Bind(
            category,
            "Ground Trick Deceleration",
            1f,
            "Deceleration while performing a ground trick."
        );

        public ConfigEntry<bool> collisionChangeEnabled = config.Bind(
            category,
            "Collision Change Enabled",
            true,
            "Changes the collision fixing almost all instances of clipping through objects."
        );

        public ConfigEntry<float> speedLimit = config.Bind(
            category,
            "Speed Limit",
            -1f,
            "Soft speed limit, if moving faster than this speed your speed will be reduced over time."
        );

        public ConfigEntry<float> speedLimitAmount = config.Bind(
            category,
            "Speed Limit Penalty Amount",
            5f,
            "Amount of speed to remove over time while above the speed limit."
        );

        public ConfigEntry<float> maxFallSpeed = config.Bind(
            category,
            "Max Fall Speed",
            40f,
            "Maximum speed you're allowed to fall."
        );

        public ConfigEntry<bool> airDashChangeEnabled = config.Bind(
            category,
            "Air Dash Change Enabled",
            true,
            "Allows adjustment to the speed loss when changing direction with an air dash."
        );

        public ConfigEntry<float> airDashStrength = config.Bind(
            category,
            "Air Dash Strength",
            0.3f,
            "How much speed you lose when changing direction with the air dash."
        );

        public ConfigEntry<bool> airDashDoubleJumpEnabled = config.Bind(
            category,
            "Air Dash Double Jump Enabled",
            true,
            "Not holding a direction when performing an air dash will work like a double jump instead."
        );

        public ConfigEntry<float> airDashDoubleJumpAmount = config.Bind(
            category,
            "Air Dash Double Jump Amount",
            7f,
            "Amount of vertical veloctiy to add when performing an air dash double jump."
        );

        public ConfigEntry<DoubleJumpType> airDashDoubleJumpType = config.Bind(
            category,
            "Air Dash Double Jump Type",
            DoubleJumpType.Additive,
            "How the double jump behaves. Additive will add the amount to your vertical velocity. Replace will replace your vertical velocity so if you're going faster it will slow you down. Soft cap will only set your vertical veloctiy if it's lower than the amount."
        );

        public ConfigEntry<string> airDashDoubleJumpAnim = config.Bind(
            category,
            "Air Dash Double Jump Animation",
            "airTrick1",
            ""
        );

        public ConfigEntry<float> averageSpeedTimer = config.Bind(
            category,
            "Average Speed Timer",
            0.4f,
            "Many mechanics use your average speed over a period of time this is that period of time, a lower time will be more responsive but less forgiving and might feel less smooth."
        );

        public ConfigEntry<AverageSpeedMode> averageSpeedMode = config.Bind(
            category,
            "Average Speed Mode",
            AverageSpeedMode.BlendMaxAverage,
            "The mode used for any mechanic that uses the players average speed. Average is the players average speed over a period of time, max chooses the players max speed in that period of time, and BlendMaxAverage blends the max speed and the average."
        );

        public ConfigEntry<float> averageSpeedBias = config.Bind(
            category,
            "Blend Speed Bias",
            0.5f,
            "The bias towards using the average or max speed when using the BlendMaxAverage mode. 0 lean more towards the average, 1 will lean more towards the max"
        );

        public ConfigEntry<string> MVPreset = config.Bind(
            category,
            "Movement Preset",
            "None",
            "Movement preset to use when launching the game."
        );

        public ConfigEntry<bool> MVPresetEnabled = config.Bind(
           category,
           "Movement Preset Enabled",
           true,
           "Whether the preset is enabled on launch or not."
       );

        public ConfigEntry<int> listLength = config.Bind(
           category,
           "Trick List Total Length",
           15,
           "Total size of trick point degradation list."
       );

        public ConfigEntry<int> repsToMin = config.Bind(
           category,
           "Repetitions To Minimum Points",
           3,
           "Amount of repetitions of the same trick in the trick list to reach the minimum point value."
       );

        public ConfigEntry<bool> ReturnSpeed = config.Bind(
           category,
           "Return Speed On No Collision",
           true,
           "Attempts to return speed if no collision was detected, this attempts to help with small lips stealing player speed."
       );

        public ConfigEntry<bool> ReturnSpeedLoading = config.Bind(
           category,
           "Return Speed On Exiting Loading",
           true,
           "Retains player speed when changing stages."
       );

        public ConfigEntry<bool> HardLandingEnabled = config.Bind(
           category,
           "Hard landing change enabled",
           true,
           "being in the air for a period of time and not sliding when landing will remove all of your speed."
       );

        public ConfigEntry<HardLandingType> HardLandingMode = config.Bind(
            category,
            "Hard Landing Mode",
            HardLandingType.OnlyFeet,
            "What instances the hard landing can trigger."
        );

        public ConfigEntry<float> HardFallTime = config.Bind(
           category,
           "Hard landing Air Time",
           1.5f,
           "How long you need to be in the air to trigger a hard landing."
       );

        public ConfigEntry<bool> JumpGroundTrickFoot = config.Bind(
           category,
           "Allow Jump During On Foot Ground Tricks",
           false,
           "Allows jump during on foot ground tricks, this will not trigger a super ground trick if you jump early."
       );

        public ConfigEntry<bool> JumpGroundTrickMove = config.Bind(
           category,
           "Allow Jump During Movestyle Ground Tricks",
           true,
           "Allows jump during movestyle ground tricks."
       );

        public ConfigEntry<bool> SlopeSlideSpeedChange = config.Bind(
           category,
           "Slope Slide Speed Enabled",
           true,
           "New calculation for sliding while going up or down a slope."
       );

        public ConfigEntry<float> SlopeSlideSpeedDown = config.Bind(
           category,
           "Slope Slide Speed Strength Down",
           0.1f,
           "Multiplier used when adding speed while going down a slope while sliding."
       );

        public ConfigEntry<float> SlopeSlideSpeedUp = config.Bind(
           category,
           "Slope Slide Speed Strength Up",
           0.3f,
           "Multiplier used when subtracting speed while going up a slope while sliding."
       );

        public ConfigEntry<bool> DisablePatch = config.Bind(
           category,
           "Disable All Harmony Patches",
           false,
           "Disables all harmony patches, this is used for the vanilla preset."
       );
    }

    public class ConfigNonStable(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Non Stable Changes Enabled",
            true,
            "General changes to non stable surfaces, this allows for more control on surfaces you can't stand on."
        );

        public ConfigEntry<bool> SurfEnabled = config.Bind(
            category,
            "Surf Enabled",
            true,
            "Allows the player to surf on non stable surfaces by holding the slide button."
        );
    }
    public class ConfigPopJump(ConfigFile config, string category)
    {
      public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Pop Jump Enabled",
          true,
          "When enabled, jumping after getting bounced back by a vending machine/dumpster/etc will convert all of your forward momentum into upward momentum"
          );
      public ConfigEntry<string> Name = config.Bind(
          category,
          "Pop Jump Name",
          "Pop Jump",
          "Trick name that appears when performing a Pop Jump"
          );
      public ConfigEntry<float> GracePeriod = config.Bind(
          category,
          "Pop Jump Grace Period",
          1.0f,
          "The time window you can perform a Pop Jump in starting from when you hit a dumpster/vending machine."
          );
      public ConfigEntry<float> SpeedMultiplier = config.Bind(
          category,
          "Pop Jump Speed Multiplier",
          1.0f,
          "The multiplier by which your total speed gets converted to upwards velocity."
          );
      public ConfigEntry<float> PointsPerSpeed = config.Bind(
           category,
           "Pop Jump Base Points",
           1.0f,
           "Points given per one speed unit when performing a Pop Jump. The exact points given are points=max(float(Base Points * Speed Units), Minimum Pop Jump Points)."
       );
        public ConfigEntry<int> PointsMin = config.Bind(
           category,
           "Minimum Pop Jump Points",
           200,
           "Minimum amount of points(flat, not per speed unit) given when performing a Pop Jump."
       );
    }

    public class ConfigMortarStrike(ConfigFile config, string category){
      public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Mortar Strike Enabled",
          true,
          "This is basically Meteor Drop."
          );
      public ConfigEntry<string> Name = config.Bind(
          category,
          "Mortar Strike Name",
          "Mortar Strike",
          "Trick name that appears when performing a Mortar Strike."
          );
      public ConfigEntry<int> Points = config.Bind(
          category,
          "Maximum Mortar Strike Points",
          500,
          "Maximum amount of points that can be gained from performing a Mortar Strike"
          );

      public ConfigEntry<int> PointsMin = config.Bind(
          category,
          "Minimum Mortar Strike Points",
          100,
          "Minimum amount of points that can be gained from performing a Mortar Strike"
          );
      public ConfigEntry<string> Keybinds = config.Bind(
          category,
          "Mortar Strike Keybinds",
          "anyTrick, slide",
          "The buttons used to trigger a mortar strike.\n" +
          "Button list:\n"+
          "boost, dance, jump, phone, slide, switchStyle, trick1, trick2, trick3,  walk" +
          "anyTrick, anyTwoTricks"
          );
      public ConfigEntry<bool> NoDirInput = config.Bind(
          category,
          "Require No Directional Input",
          true,
          "This setting makes it so that you have to not be using any directional inputs before doing a mortar stike. Basically you have to \"Let go of your stick\" or not press WASD before a mortar strike."
          );
    }
}
