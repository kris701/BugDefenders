using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Enemies
{
    public class Enemy
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public double Speed { get; set; }

        public Enemy(string name, int health, double speed)
        {
            Name = name;
            Health = health;
            Speed = speed;
        }
    }
}
