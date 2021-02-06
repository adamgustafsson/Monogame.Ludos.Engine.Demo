namespace LudosEngineDemo.Model
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Input;
    using Ludos.Engine.Tmx;
    using Microsoft.Xna.Framework;

    public class Player : LudosPlayer
    {
        public Player(Vector2 position, Point size, TMXManager tmxManager, InputManager inputManager)
            : base(position, size, tmxManager, inputManager)
        {
        }
    }
}
