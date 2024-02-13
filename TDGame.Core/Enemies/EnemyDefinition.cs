using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Enemies
{
    public class EnemyDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public float Health { get; set; }
        public float Speed { get; set; }
        public int Reward { get; set; }
        public int WaveSize { get; set; }
        public bool IsBoss { get; set; }
        public Guid EnemyType { get; set; }
    }
}
