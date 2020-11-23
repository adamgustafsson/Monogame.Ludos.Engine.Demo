
namespace Model.Actors.Abilities
{
    interface IAbility
    {
        bool AbilityEnabled
        {
            get;
            set;
        }

        void ResetAbility();
    }
}
