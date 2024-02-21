using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TDGame.Core.Resources.Integrity
{
    public class ResourceIntegrityChecker
    {
        public List<IntegrityError> Errors { get; set; }
        public ResourceIntegrityChecker()
        {
            Errors = new List<IntegrityError>();
        }

        public void CheckGameIntegrity()
        {
            Errors.Clear();

            var enemyTypes = ResourceManager.EnemyTypes.GetResources();
            foreach (var id in ResourceManager.Enemies.GetResources())
            {
                var enemy = ResourceManager.Enemies.GetResource(id);
                if (!enemyTypes.Contains(enemy.EnemyType))
                    Errors.Add(new IntegrityError(
                        $"Enemy ({id}) has an unknown enemy type: {enemy.EnemyType}",
                        IntegrityError.SeverityLevel.Critical));
            }
            var projectiles = ResourceManager.Projectiles.GetResources();
            foreach (var id in ResourceManager.Turrets.GetResources())
            {
                var turret = ResourceManager.Turrets.GetResource(id);
                foreach (var upgrade in turret.Upgrades)
                    if (upgrade.Requires != null)
                        if (!turret.Upgrades.Any(x => x.ID == upgrade.Requires))
                            Errors.Add(new IntegrityError(
                                $"Turret ({id}) has an upgrade ({upgrade.ID}) that requires another upgrade ({upgrade.Requires}) but it does not exist!",
                                IntegrityError.SeverityLevel.Critical));
            }
        }
    }
}
