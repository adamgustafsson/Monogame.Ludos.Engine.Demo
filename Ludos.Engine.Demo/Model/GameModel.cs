namespace LudosEngineDemo.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Ludos.Engine.Level;
    using Microsoft.Xna.Framework;

    public class GameModel
    {
        private readonly LudosPlayer _player;

        public GameModel(GameServiceContainer services, LudosPlayer player)
        {
            _player = player;
            LoadCrates();
        }

        public List<GameObject> Crates { get; set; } = new List<GameObject>();

        public void Update(float elapsedTime)
        {
            foreach (var crate in Crates)
            {
                crate.Update(elapsedTime);
            }
        }

        private void LoadCrates()
        {
            Crates = new List<GameObject>();

            foreach (var mapObject in LevelManager.GetAllLayerObjects(TMXDefaultLayerInfo.ObjectLayerInteractableObjects).Where(x => x.Type == "crate"))
            {
                Crates.Add(new GameObject(100, mapObject.Bounds.Location.ToVector2(), new Point(16, 16)));
            }

            foreach (var crate in Crates)
            {
                crate.AdditionalCollisionObjects = Crates.Where(x => x.Position != crate.Position).ToList();
            }
        }
    }
}
