namespace MovementFunk.Mechanics
{
    internal class GoonStorageMinMax
    {
        public static void Update()
        {
            var player = MovementFunkPlugin.player;
            var ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (player.ability != player.wallrunAbility)
            {
                float minimum = ConfigSettings.WallGeneral.goonStorageMin.Value;
                float maximum = ConfigSettings.WallGeneral.goonStorageMax.Value;

                float clampedValue = MFMath.Clamp(player.wallrunAbility.lastSpeed, minimum, maximum);

                MFVariables.savedGoon = clampedValue;
                player.wallrunAbility.lastSpeed = clampedValue;
                player.wallrunAbility.customVelocity = clampedValue * player.wallrunAbility.customVelocity.normalized;
            }
        }
    }
}
