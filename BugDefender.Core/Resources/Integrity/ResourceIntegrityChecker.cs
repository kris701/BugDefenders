using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;

namespace BugDefender.Core.Resources.Integrity
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

            CheckEnemyTypes();
            CheckEnemies();
            CheckTurrets();
            CheckProjectiles();
            CheckMaps();
            CheckBuffs();
            CheckAchivements();

            Errors = Errors.OrderByDescending(x => x.Severity).ToList();
        }

        private void CheckEnemyTypes()
        {
            var enemyTypes = ResourceManager.EnemyTypes.GetResources();
            if (enemyTypes.Distinct().Count() != enemyTypes.Count)
                Errors.Add(new IntegrityError(
                    $"Some enemy type IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
        }

        private void CheckEnemies()
        {
            var enemyTypes = ResourceManager.EnemyTypes.GetResources();
            var enemyIDs = ResourceManager.Enemies.GetResources();
            if (enemyIDs.Distinct().Count() != enemyIDs.Count)
                Errors.Add(new IntegrityError(
                    $"Some enemy IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
            var usedEnemyTypes = new List<Guid>();
            foreach (var id in enemyIDs)
            {
                var enemy = ResourceManager.Enemies.GetResource(id);
                if (!enemyTypes.Contains(enemy.EnemyType))
                    Errors.Add(new IntegrityError(
                        $"Enemy ({id}) has an unknown enemy type: {enemy.EnemyType}",
                        IntegrityError.SeverityLevel.Critical));
                else
                    usedEnemyTypes.Add(enemy.EnemyType);

                switch (enemy.ModuleInfo)
                {
                    case SingleEnemyDefinition def:

                        if (def.Speed == 0)
                            Errors.Add(new IntegrityError(
                                $"Enemy ({id}) has speed set to zero so it wont move!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case WaveEnemyDefinition def:

                        if (def.Speed == 0)
                            Errors.Add(new IntegrityError(
                                $"Enemy ({id}) has speed set to zero so it wont move!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.WaveSize == 0)
                            Errors.Add(new IntegrityError(
                                $"Enemy ({id}) has a wave size set to zero!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.SpawnDelay == 0)
                            Errors.Add(new IntegrityError(
                                $"Enemy ({id}) has a spawn delay set to zero, so the entire wave will spawn inside eachother!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                }
            }
            foreach (var type in enemyTypes)
                if (!usedEnemyTypes.Contains(type))
                    Errors.Add(new IntegrityError(
                        $"Unused enemy type: {type}",
                        IntegrityError.SeverityLevel.Message));
        }

        private void CheckTurrets()
        {
            var projectiles = ResourceManager.Projectiles.GetResources();
            var turrets = ResourceManager.Turrets.GetResources();
            if (turrets.Distinct().Count() != turrets.Count)
                Errors.Add(new IntegrityError(
                    $"Some turret IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
            if (projectiles.Distinct().Count() != projectiles.Count)
                Errors.Add(new IntegrityError(
                    $"Some projectile IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
            var usedProjectiles = new List<Guid>();
            foreach (var id in ResourceManager.Turrets.GetResources())
            {
                var turret = ResourceManager.Turrets.GetResource(id);
                switch (turret.ModuleInfo)
                {
                    case ProjectileTurretDefinition def:
                        if (!projectiles.Contains(def.ProjectileID))
                            Errors.Add(new IntegrityError(
                                $"Projectile turret ({id}) has an unknown projectile ID: {def.ProjectileID}",
                                IntegrityError.SeverityLevel.Critical));
                        else
                            usedProjectiles.Add(def.ProjectileID);
                        if (def.Range == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile turret ({id}) has zero range!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case AOETurretDefinition def:
                        if (def.Range == 0)
                            Errors.Add(new IntegrityError(
                                $"AOE turret ({id}) has zero range!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.Damage == 0)
                            Errors.Add(new IntegrityError(
                                $"AOE turret ({id}) has zero damage!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case LaserTurretDefinition def:
                        if (def.Range == 0)
                            Errors.Add(new IntegrityError(
                                $"Laser turret ({id}) has zero range!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.Damage == 0)
                            Errors.Add(new IntegrityError(
                                $"Laser turret ({id}) has zero damage!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case PassiveTurretDefinition def:
                        if (def.Range == 0)
                            Errors.Add(new IntegrityError(
                                $"Passive turret ({id}) has zero range!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case InvestmentTurretDefinition def:
                        if (def.MoneyPrWave == 0)
                            Errors.Add(new IntegrityError(
                                $"Investment turret ({id}) gives no money pr wave!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                }

                if (turret.CanDamage.Count == 0 && turret.ModuleInfo is not PassiveTurretDefinition && turret.ModuleInfo is not InvestmentTurretDefinition)
                    Errors.Add(new IntegrityError(
                        $"Turret ({id}) is set to be unable to target anything!",
                        IntegrityError.SeverityLevel.Message));

                foreach (var upgrade in turret.Upgrades)
                {
                    if (upgrade.Requires != null)
                    {
                        if (!turret.Upgrades.Any(x => x.ID == upgrade.Requires))
                            Errors.Add(new IntegrityError(
                                $"Turret ({id}) has an upgrade ({upgrade.ID}) that requires another upgrade ({upgrade.Requires}) but it does not exist!",
                                IntegrityError.SeverityLevel.Critical));
                        if (upgrade.Requires == id)
                            Errors.Add(new IntegrityError(
                                $"Turret upgrade ({id}) requires itself to be enabled!",
                                IntegrityError.SeverityLevel.Critical));
                    }
                }

                if (turret.Upgrades.All(x => x.Requires != null))
                    Errors.Add(new IntegrityError(
                        $"Turret ({id}) only has upgrades that require another upgrade so the chain has nowhere to start!",
                        IntegrityError.SeverityLevel.Message));
            }

            foreach (var type in projectiles)
                if (!usedProjectiles.Contains(type))
                    Errors.Add(new IntegrityError(
                        $"Unused projectile: {type}",
                        IntegrityError.SeverityLevel.Message));
        }

        private void CheckProjectiles()
        {
            var projectiles = ResourceManager.Projectiles.GetResources();
            foreach (var id in projectiles)
            {
                var projectile = ResourceManager.Projectiles.GetResource(id);
                if (projectile.CanDamage.Count == 0)
                    Errors.Add(new IntegrityError(
                        $"Projectile ({id}) is set to be unable to target anything!",
                        IntegrityError.SeverityLevel.Message));

                switch (projectile.ModuleInfo)
                {
                    case DirectProjectileDefinition def:
                        if (def.Speed == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has speed set to zero so it wont move!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.Damage == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has damage set to zero, so it wont be able to hurt anything!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                    case ExplosiveProjectileDefinition def:
                        if (def.Speed == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has speed set to zero so it wont move!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.Damage == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has damage set to zero, so it wont be able to hurt anything!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.TriggerRange == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has trigger range set to zero so it will never trigger!",
                                IntegrityError.SeverityLevel.Message));
                        if (def.SplashRange == 0)
                            Errors.Add(new IntegrityError(
                                $"Projectile ({id}) has splash range set to zero, so it wont hurt anything!",
                                IntegrityError.SeverityLevel.Message));
                        break;
                }
            }
        }

        private void CheckMaps()
        {
            var maps = ResourceManager.Maps.GetResources();
            if (maps.Distinct().Count() != maps.Count)
                Errors.Add(new IntegrityError(
                    $"Some map IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
            foreach (var id in maps)
            {
                var map = ResourceManager.Maps.GetResource(id);
                if (map.Paths.Count == 0)
                    Errors.Add(new IntegrityError(
                        $"Map ({id}) has zero paths!",
                        IntegrityError.SeverityLevel.Critical));
            }
        }

        private void CheckBuffs()
        {
            var buffs = ResourceManager.Buffs.GetResources();
            if (buffs.Distinct().Count() != buffs.Count)
                Errors.Add(new IntegrityError(
                    $"Some buff IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
        }

        private void CheckAchivements()
        {
            var achivements = ResourceManager.Achivements.GetResources();
            if (achivements.Distinct().Count() != achivements.Count)
                Errors.Add(new IntegrityError(
                    $"Some achivement IDs are not unique!",
                    IntegrityError.SeverityLevel.Critical));
        }
    }
}
