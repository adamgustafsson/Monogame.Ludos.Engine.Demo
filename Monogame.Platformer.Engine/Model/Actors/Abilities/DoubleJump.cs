
namespace Model.Actors.Abilities
{
    internal class DoubleJump : IAbility
    {
        public bool AbilityEnabled { get; set; }
        public bool DoubleJumpAvailable { get; set; } = true;
        public bool DoubleJumpUsed { get; set; } = false;

        public DoubleJump()
        {
            AbilityEnabled = true;
        }

        public void ResetAbility()
        {
            DoubleJumpAvailable = true;
            DoubleJumpUsed = false;
        }
    }
}