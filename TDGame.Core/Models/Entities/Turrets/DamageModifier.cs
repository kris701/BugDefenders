using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models.Entities.Turrets
{
    public class DamageModifier
    {
        public Guid EnemyType { get; set; }
        public float Modifier { get; set; }
    }
}
