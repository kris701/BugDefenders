﻿using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Challenges;
using System.Text.Json;

namespace BugDefender.Core.Game
{
    public enum GameResult { None, Lost, ChallengeSuccess }
    public class GameContext
    {
        public MapDefinition Map { get; set; }
        public GameStyleDefinition GameStyle { get; set; }
        public List<List<Guid>> EnemiesToSpawn { get; set; } = new List<List<Guid>>();
        public bool AutoSpawn { get; set; } = false;
        public float Evolution { get; set; } = 1;
        public StatsDefinition Stats { get; set; } = new StatsDefinition();
        public HashSet<EnemyInstance> CurrentEnemies { get; set; } = new HashSet<EnemyInstance>();
        public HashSet<TurretInstance> Turrets { get; set; } = new HashSet<TurretInstance>();
        public HashSet<ProjectileInstance> Projectiles { get; set; } = new HashSet<ProjectileInstance>();
        public int HP { get; set; } = 0;
        public int Money { get; set; } = 0;
        public int Score { get; set; } = 0;
        public ChallengeDefinition? Challenge { get; set; }

        public int Wave { get; set; } = 0;
        public TimeSpan GameTime { get; set; }

        public bool CanSave()
        {
            if (CurrentEnemies.Count > 0)
                return false;
            if (Projectiles.Count > 0)
                return false;

            return true;
        }

        public void Save(FileInfo file)
        {
            if (file.Exists)
                file.Delete();
            var content = JsonSerializer.Serialize(this);
            File.WriteAllText(file.FullName, content);
        }
    }
}
