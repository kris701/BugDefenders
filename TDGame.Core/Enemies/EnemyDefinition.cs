using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Enemies
{
    public class EnemyDefinition : BasePositionModel, ITextured
    {
        public enum EnemyTypes { None, Ice, Water, Fire, Stone }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override float Size { get; set; }
        public int Health { get; set; }
        public double Speed { get; set; }
        public int Reward { get; set; }
        public double WaveSize { get; set; }
        public bool IsBoss { get; set; }
        public EnemyTypes Type { get; set; }

        [JsonIgnore]
        public int WayPointID { get; set; } = 0;
        [JsonIgnore]
        public override float Angle { get; set; } = 0;
        [JsonIgnore]
        public Guid GroupID { get; set; }

        public static string GetEnemyTypeName(EnemyTypes type)
        {
            var name = Enum.GetName(typeof(EnemyTypes), type);
            if (name != null)
                return name;
            throw new Exception("Unknown enemy type!");
        }
    }
}
