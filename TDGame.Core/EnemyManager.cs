using System.Linq;
using TDGame.Core.Helpers;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Maps;
using TDGame.Core.Resources;

namespace TDGame.Core
{
    public partial class Game
    {
        private void UpdateEnemiesToSpawnList()
        {
            for (int i = EnemiesToSpawn.Count; i < 5; i++)
            {
                if (Spawned++ % GameStyle.BossEveryNWave == 0 && SingleEnemiesModule.EnemyOptions.Count > 0)
                    EnemiesToSpawn.Add(SingleEnemiesModule.EnemyOptions[_rnd.Next(0, SingleEnemiesModule.EnemyOptions.Count)]);
                else if (WaveEnemiesModule.EnemyOptions.Count > 0)
                    EnemiesToSpawn.Add(WaveEnemiesModule.EnemyOptions[_rnd.Next(0, WaveEnemiesModule.EnemyOptions.Count)]);
            }
        }

        public void QueueEnemies()
        {
            Money += GameStyle.MoneyPrWave;

            if (WaveEnemiesModule.EnemyOptions.Contains(EnemiesToSpawn[0]))
                _spawnQueue.AddRange(WaveEnemiesModule.QueueEnemies(EnemiesToSpawn[0]));
            else if (SingleEnemiesModule.EnemyOptions.Contains(EnemiesToSpawn[0]))
                _spawnQueue.AddRange(SingleEnemiesModule.QueueEnemies(EnemiesToSpawn[0]));
            EnemiesToSpawn.RemoveAt(0);
            UpdateEnemiesToSpawnList();
        }

        private void UpdateSpawnQueue()
        {
            var newToAdd = WaveEnemiesModule.UpdateSpawnQueue(_spawnQueue);
            newToAdd.AddRange(SingleEnemiesModule.UpdateSpawnQueue(_spawnQueue));
            foreach (var enemy in newToAdd)
            {
                CurrentEnemies.Add(enemy);
                if (OnEnemySpawned != null)
                    OnEnemySpawned.Invoke(enemy);
            }
            foreach(var remove in newToAdd)
                _spawnQueue.Remove(remove);
        }
    }
}
