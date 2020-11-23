
namespace Model.Actors.Abilities
{
    public interface IAbility
    {
        bool AbilityEnabled
        {
            get;
            set;
        }

        void ResetAbility();
    }
}
