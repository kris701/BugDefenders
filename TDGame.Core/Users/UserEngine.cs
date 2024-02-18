using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Resources;
using TDGame.Core.Users.Models;
using TDGame.Core.Users.Models.Buffs;

namespace TDGame.Core.Users
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
                foreach(var file in new DirectoryInfo(UsersPath).GetFiles())
                {
                    var parsed = JsonSerializer.Deserialize<UserDefinition<T>>(File.ReadAllText(file.FullName));
                    if (parsed != null && parsed.ID != Guid.Empty)
                        retList.Add(parsed);
                }
            }
            return retList;
        }

        public void ApplyBuffsToResources(UserDefinition<T> user)
        {
            ResourceManager.ReloadResources();

            foreach (var buff in user.Buffs)
            {
                if (buff is EnemyBuff enemyBuff)
                {
                    var target = ResourceManager.Enemies.GetResource(enemyBuff.EnemyID);
                    if (target.ModuleInfo.GetType() == enemyBuff.Module.GetType())
                        target.ModuleInfo = enemyBuff.Module;
                } else if (buff is TurretBuff turretBuff)
                {
                    var target = ResourceManager.Turrets.GetResource(turretBuff.TurretID);
                    if (target.ModuleInfo.GetType() == turretBuff.Module.GetType())
                        target.ModuleInfo = turretBuff.Module;
                }
            }
        }

        public void AddNewUser(UserDefinition<T> user)
        {
            var target = Path.Combine(UsersPath, $"{user.ID}.json");
            if (File.Exists(target))
                throw new Exception("User already exists!");
            File.WriteAllText(target, JsonSerializer.Serialize(user));
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
            File.WriteAllText(target, JsonSerializer.Serialize(user));
        }
    }
}
