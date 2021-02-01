using Ludos.Engine.Managers;
using Ludos.Engine.Model;
using Microsoft.Xna.Framework;

namespace LudosEngineDemo.Model
{
    public class Player : LudosPlayer  
    {
        public Player(Vector2 position, Point size, TMXManager tmxManager, InputManager inputManager)
            : base(position, size, tmxManager, inputManager)
        {

        }
    }
}
