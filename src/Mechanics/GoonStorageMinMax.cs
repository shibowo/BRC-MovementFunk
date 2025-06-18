namespace MovementFunk.Mechanics
{
    internal class GoonStorageMinMax
    {
        public static void Update()
        {
            var player = MovementFunkPlugin.player;
            var MovementSettings = MovementFunkPlugin.MovementSettings;
            if (player.ability != player.wallrunAbility)
            {
                float minimum = MovementSettings.WallGeneral.goonStorageMin.Value;
                float maximum = MovementSettings.WallGeneral.goonStorageMax.Value;

                float clampedValue = MFMath.Clamp(player.wallrunAbility.lastSpeed, minimum, maximum);

                MFVariables.savedGoon = clampedValue;
                player.wallrunAbility.lastSpeed = clampedValue;
                player.wallrunAbility.customVelocity = clampedValue * player.wallrunAbility.customVelocity.normalized;
            }
        }
    }
}
