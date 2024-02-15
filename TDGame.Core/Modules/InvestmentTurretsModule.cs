﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Turrets;

namespace TDGame.Core.Modules
{
    public class InvestmentTurretsModule : IGameModule
    {
        public Game Game { get; }
        private int _currentWave = 0;

        public InvestmentTurretsModule(Game game)
        {
            Game = game;
        }

        public void Update(TimeSpan passed)
        {
            foreach (var turret in Game.Turrets)
            {
                if (turret.TurretInfo is InvestmentTurretDefinition def)
                {
                    if (Game.Spawned != _currentWave)
                        Game.Money += def.MoneyPrWave;
                }
            }
            _currentWave = Game.Spawned;
        }
    }
}
