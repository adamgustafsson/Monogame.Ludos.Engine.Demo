namespace LudosEngineDemo.Model
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Input;
    using Ludos.Engine.Level;
    using Microsoft.Xna.Framework;

    public class Player : LudosPlayer
    {
        public Player(Vector2 position, Point size, LevelManager tmxManager, InputManager inputManager)
            : base(position, size, tmxManager, inputManager)
        {
        }
    }
}
