using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

namespace TDGame.Core
{
    public partial class Game
    {
        private void UpdateEnemiesToSpawnList()
        {
            for (int i = EnemiesToSpawn.Count; i < 5; i++)
            {
                if (_spawned++ % GameStyle.BossEveryNWave == 0)
                    EnemiesToSpawn.Add(_bossEnemies[_rnd.Next(0, _bossEnemies.Count)]);
                else
                    EnemiesToSpawn.Add(_normalEnemies[_rnd.Next(0, _normalEnemies.Count)]);
            }
        }

        public void QueueEnemies()
        {
            var group = Guid.NewGuid();
            var template = EnemyBuilder.GetEnemy(EnemiesToSpawn[0]);
            for (int i = 0; i < template.WaveSize * GameStyle.EnemyWaveMultiplier; i++)
            {
                var enemy = new EnemyInstance(EnemiesToSpawn[0], Evolution);
                enemy.X = Map.WayPoints[0].X - enemy.Size / 2;
                enemy.Y = Map.WayPoints[0].Y - enemy.Size / 2;
                enemy.GroupID = group;
                _spawnQueue.Add(enemy);
            }
            EnemiesToSpawn.RemoveAt(0);
            UpdateEnemiesToSpawnList();
        }

        private FloatPoint GetEnemyLocationChange(float angle, float speed)
        {
            var xMod = Math.Cos(angle);
            var yMod = Math.Sin(angle);
            return new FloatPoint(
                (float)xMod * speed * (float)GameStyle.EnemySpeedMultiplier,
                (float)yMod * speed * (float)GameStyle.EnemySpeedMultiplier);
        }

        private void UpdateSpawnQueue()
        {
            if (_spawnQueue.Count > 0)
            {
                var vistedGroups = new List<Guid>();
                var enemiesToAdd = new List<EnemyInstance>();
                foreach (var enemy in _spawnQueue)
                {
                    if (vistedGroups.Contains(enemy.GroupID))
                        continue;
                    vistedGroups.Add(enemy.GroupID);
                    var minDist = double.MaxValue;
                    foreach (var CurrentEnemy in CurrentEnemies)
                        if (CurrentEnemy.GroupID == enemy.GroupID)
                            minDist = Math.Min(MathHelpers.Distance(enemy, CurrentEnemy), minDist);
                    if (minDist > enemy.Size)
                        enemiesToAdd.Add(enemy);
                }
                foreach (var enemy in enemiesToAdd)
                {
                    CurrentEnemies.Add(enemy);
                    _spawnQueue.Remove(enemy);
                    if (OnEnemySpawned != null)
                        OnEnemySpawned.Invoke(enemy);
                }
            }
        }

        private void UpdateEnemySlowDuration(TimeSpan passed)
        {
            foreach (var enemy in CurrentEnemies)
                if (enemy.SlowingDuration > 0)
                    enemy.SlowingDuration -= passed.Milliseconds;
        }

        private void UpdateEnemyPositions()
        {
            var toRemove = new List<EnemyInstance>();
            foreach (var enemy in CurrentEnemies)
            {
                var target = Map.WayPoints[enemy.WayPointID];
                if (MathHelpers.Distance(enemy, target) < 5)
                {
                    enemy.WayPointID++;
                    if (enemy.WayPointID >= Map.WayPoints.Count)
                    {
                        DamagePlayer();
                        toRemove.Add(enemy);
                        continue;
                    }
                    target = Map.WayPoints[enemy.WayPointID];
                }
                enemy.Angle = GetAngle(target, enemy);
                var change = GetEnemyLocationChange(enemy.Angle, enemy.GetSpeed());
                enemy.X += change.X;
                enemy.Y += change.Y;
            }
            foreach (var enemy in toRemove)
                CurrentEnemies.Remove(enemy);
        }
    }
}
