namespace MovementPlus.Mechanics
{
    internal class GoonStorageMinMax
    {
        public static void Update()
        {
            var player = MovementPlusPlugin.player;
            var ConfigSettings = MovementPlusPlugin.ConfigSettings;
            if (player.ability != player.wallrunAbility)
            {
                float minimum = ConfigSettings.WallGeneral.goonStorageMin.Value;
                float maximum = ConfigSettings.WallGeneral.goonStorageMax.Value;

                float clampedValue = MPMath.Clamp(player.wallrunAbility.lastSpeed, minimum, maximum);

                MPVariables.savedGoon = clampedValue;
                player.wallrunAbility.lastSpeed = clampedValue;
                player.wallrunAbility.customVelocity = clampedValue * player.wallrunAbility.customVelocity.normalized;
            }
        }
    }
}