using System.Text.Json;
using BugDefender.Core.Resources;
#if RELEASE
using BugDefender.Core.Users.Helpers;
#endif
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.Buffs.BuffEffects;

namespace BugDefender.Core.Users
{
    public class UserEngine<T>
    {
        public string UsersPath { get; } = "Users";

        public UserEngine()
        {
            if (!Directory.Exists(UsersPath))
                Directory.CreateDirectory(UsersPath);
        }

        public List<UserDefinition<T>> GetAllUsers()
        {
            var retList = new List<UserDefinition<T>>();
            if (Directory.Exists(UsersPath))
            {
                foreach (var file in new DirectoryInfo(UsersPath).GetFiles())
                {
                    var parsed = Deserialize(File.ReadAllText(file.FullName));
                    if (parsed != null && parsed.ID != Guid.Empty)
                        retList.Add(parsed);
                }
            }
            return retList;
        }

        public void AddBuffUpgrade(UserDefinition<T> user, Guid id)
        {
            var buff = ResourceManager.Buffs.GetResource(id);
            if (buff.IsValid(user))
            {
                user.Buffs.Add(id);
                SaveUser(user);
                ApplyBuffsToResources(user);
            }
        }

        public void ApplyBuffsToResources(UserDefinition<T> user)
        {
            ResourceManager.ReloadResources();

            // Need to load projectile buffs first, since the projectile turret modules contain a projectile module
            bool any = false;
            foreach (var id in user.Buffs)
            {
                var buff = ResourceManager.Buffs.GetResource(id).Effect;
                if (buff is ProjectileBuffEffect projectileBuff)
                {
                    var target = ResourceManager.Projectiles.GetResource(projectileBuff.ProjectileID);
                    if (target.ModuleInfo.GetType() == projectileBuff.Module.GetType())
                        target.ModuleInfo = projectileBuff.Module;
                    any = true;
                }
            }
            if (any)
                ResourceManager.Turrets.Reload();

            foreach (var id in user.Buffs)
            {
                var buff = ResourceManager.Buffs.GetResource(id).Effect;
                if (buff is EnemyBuffEffect enemyBuff)
                {
                    var target = ResourceManager.Enemies.GetResource(enemyBuff.EnemyID);
                    if (target.ModuleInfo.GetType() == enemyBuff.Module.GetType())
                        target.ModuleInfo = enemyBuff.Module;
                }
                else if (buff is TurretBuffEffect turretBuff)
                {
                    var target = ResourceManager.Turrets.GetResource(turretBuff.TurretID);
                    if (target.ModuleInfo.GetType() == turretBuff.Module.GetType())
                        target.ModuleInfo = turretBuff.Module;
                }
            }
        }

        public void CheckAndApplyAchivements(UserDefinition<T> user)
        {
            var achivements = ResourceManager.Achivements.GetResources();
            foreach (var id in achivements)
            {
                if (!user.Achivements.Contains(id))
                {
                    var achivement = ResourceManager.Achivements.GetResource(id);
                    if (achivement.IsValid(user))
                        user.Achivements.Add(id);
                }
            }
        }

        public void AddNewUser(UserDefinition<T> user)
        {
            var target = Path.Combine(UsersPath, $"{user.ID}.json");
            if (File.Exists(target))
                throw new Exception("User already exists!");
            File.WriteAllText(target, Serialize(user));
        }

        public void RemoveUser(UserDefinition<T> user)
        {
            var target = Path.Combine(UsersPath, $"{user.ID}.json");
            if (File.Exists(target))
                File.Delete(target);
        }

        public void SaveUser(UserDefinition<T> user)
        {
            var target = Path.Combine(UsersPath, $"{user.ID}.json");
            if (File.Exists(target))
                File.Delete(target);
            File.WriteAllText(target, Serialize(user));
        }

        private string Serialize(UserDefinition<T> user)
        {
#if DEBUG
            return JsonSerializer.Serialize(user);
#else
            return StringCompressor.CompressString(JsonSerializer.Serialize(user));
#endif
        }

        private UserDefinition<T> Deserialize(string data)
        {
            UserDefinition<T>? parsed = null;
#if DEBUG
            try
            {
                parsed = JsonSerializer.Deserialize<UserDefinition<T>>(data);
            }
            catch { }
            if (parsed == null)
                throw new Exception("Error deserializing user data!");
            return parsed;
#else
            try
            {
                parsed = JsonSerializer.Deserialize<UserDefinition<T>>(StringCompressor.DecompressString(data));
            }
            catch { }
            if (parsed == null)
                throw new Exception("Error deserializing user data!");
            return parsed;
#endif
        }
    }
}
