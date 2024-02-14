using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.EnemyTypes;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.GameStyles;
using TDGame.Core.Maps;
using TDGame.Core.Models;

namespace TDGame.Core.Mods
{
    public class ModPackDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<EnemyTypeDefinition>? EnemyTypes { get; set; }
        public List<GameStyleDefinition>? GameStyles { get; set; }
        public List<MapDefinition>? Maps { get; set; }

        public List<EnemyDefinition>? Enemies { get; set; }
        public List<ProjectileDefinition>? Projectiles { get; set; }
        public List<TurretDefinition>? Turrets { get; set; }
    }
}
