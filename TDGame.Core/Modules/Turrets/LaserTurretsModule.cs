﻿using System;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Enemies.Modules;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Modules.Turrets
{
    public class LaserTurretsModule : IGameModule
    {
        public Game Game { get; }

        public LaserTurretsModule(Game game)
        {
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
            {
                if (turret.TurretInfo is LaserTurretDefinition def)
                {
                    def.CoolingFor -= passed;
                    if (def.CoolingFor <= TimeSpan.Zero)
                        UpdateTurret(turret, def);
                }
            }
        }

        private void UpdateTurret(TurretInstance turret, LaserTurretDefinition def)
        {
            var best = Game.GetBestEnemy(turret, def.Range);
            if (best != null)
            {
                if (Game.OnTurretShooting != null && turret.Targeting == null)
                    Game.OnTurretShooting.Invoke(turret);
                turret.Targeting = best;

                if (best.ModuleInfo is ISlowable slow)
                {
                    if (def.SlowingFactor <= slow.SlowingFactor)
                    {
                        slow.SlowingFactor = def.SlowingFactor;
                        slow.SlowingDuration = def.SlowingDuration;
                    }
                }
                if (!Game.DamageEnemy(best, GetModifiedDamage(best.GetDefinition(), def)))
                    turret.Angle = Game.GetAngle(best, turret);
                else
                    turret.Kills++;
                def.CoolingFor = TimeSpan.FromMilliseconds(def.Cooldown);
            }
            else
            {
                if (Game.OnTurretIdle != null && turret.Targeting != null)
                    Game.OnTurretIdle.Invoke(turret);
                turret.Targeting = null;
            }
        }

        private float GetModifiedDamage(EnemyDefinition enemyDef, LaserTurretDefinition def)
        {
            var damage = def.Damage;
            foreach (var modifier in def.DamageModifiers)
                if (modifier.EnemyType == enemyDef.EnemyType)
                    damage = damage * modifier.Modifier;
            return damage;
        }
    }
}
