using System.Text.Json;
using System.Xml.Linq;
using BugDefender.Core.Game;
using BugDefender.Core.Resources;
#if RELEASE
using BugDefender.Core.Users.Helpers;
#endif
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.SavedGames;

namespace BugDefender.Core.Users
{
    public class UserEngine<T> where T : new()
    {
        public string UsersPath { get; } = "Users";
        public List<UserDefinition<T>> Users { get; set; }
        public UserDefinition<T> CurrentUser { get; set; }

        public UserEngine()
        {
            if (!Directory.Exists(UsersPath))
                Directory.CreateDirectory(UsersPath);

            Users = new List<UserDefinition<T>>();
            if (Directory.Exists(UsersPath))
            {
                foreach (var file in new DirectoryInfo(UsersPath).GetFiles())
                {
                    var parsed = Deserialize(File.ReadAllText(file.FullName));
                    if (parsed != null && parsed.ID != Guid.Empty)
                        Users.Add(parsed);
                }
            }

            foreach (var user in Users)
            {
                if (user.IsPrimary)
                {
                    SwitchUser(user);
                    break;
                }
            }
            if (CurrentUser != null && Users.Count > 0)
                SwitchUser(Users[0]);
            else
            {
                var newUser = AddNewUser("Default");
                newUser.IsPrimary = true;
                CurrentUser = newUser;
                SaveUser();
            }
        }

        public void SwitchUser(UserDefinition<T> toUser)
        {
            if (!Users.Contains(toUser))
                throw new Exception("User not found!");
            if (CurrentUser != null)
            {
                CurrentUser.IsPrimary = false;
                SaveUser();
            }
            CurrentUser = toUser;
            CurrentUser.IsPrimary = true;
            ApplyBuffsToResources();
        }

        public void AddBuffUpgrade(Guid id)
        {
            var buff = ResourceManager.Buffs.GetResource(id);
            if (buff.IsValid(CurrentUser) && CurrentUser.Credits >= buff.Cost)
            {
                CurrentUser.Credits -= buff.Cost;
                CurrentUser.Buffs.Add(id);
                SaveUser();
                ApplyBuffsToResources();
            }
        }

        private void ApplyBuffsToResources()
        {
            ResourceManager.ReloadResources();

            // Need to load projectile buffs first, since the projectile turret modules contain a projectile module
            bool any = false;
            foreach (var id in CurrentUser.Buffs)
            {
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.Effect.TargetType == Models.Buffs.BuffEffectTypes.Projectile)
                {
                    buff.Apply();
                    any = true;
                }
            }
            if (any)
                ResourceManager.Turrets.Reload();

            foreach (var id in CurrentUser.Buffs)
            {
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.Effect.TargetType != Models.Buffs.BuffEffectTypes.Projectile)
                    buff.Apply();
            }
        }

        public void CheckAndApplyAchivements()
        {
            var achivements = ResourceManager.Achivements.GetResources();
            bool changed = false;
            foreach (var id in achivements)
            {
                if (!CurrentUser.Achivements.Contains(id))
                {
                    var achivement = ResourceManager.Achivements.GetResource(id);
                    if (achivement.IsValid(CurrentUser))
                    {
                        CurrentUser.Achivements.Add(id);
                        changed = true;
                    }
                }
            }
            if (changed)
                SaveUser();
        }

        public UserDefinition<T> AddNewUser(string name)
        {
            var user = new UserDefinition<T>(name);
            var target = GetUserPath(user.ID);
            if (File.Exists(target))
                throw new Exception("User already exists!");
            File.WriteAllText(target, Serialize(user));
            Users.Add(user);
            return user;
        }

        public void RemoveUser(UserDefinition<T> user)
        {
            var target = GetUserPath(user.ID);
            if (File.Exists(target))
                File.Delete(target);
            Users.Remove(user);
        }

        public void SaveUser()
        {
            var target = GetUserPath(CurrentUser.ID);
            if (File.Exists(target))
                File.Delete(target);
            File.WriteAllText(target, Serialize(CurrentUser));
        }

        public void SaveGame(ISavedGame save)
        {
            if (!save.Context.CanSave())
                throw new Exception("Game still running! Cant save");

            var target = CurrentUser.SavedGames.SingleOrDefault(x => x.Name == save.Name);
            if (target != null)
                target = save;
            else
                CurrentUser.SavedGames.Add(save);
            SaveUser();
        }

        private string GetUserPath(Guid id) => Path.Combine(UsersPath, $"{id}.json");

        private string Serialize(UserDefinition<T> user)
        {
#if DEBUG || DONTCOMPRESSUSERDATA
            return JsonSerializer.Serialize(user);
#else
            return StringCompressor.CompressString(JsonSerializer.Serialize(user));
#endif
        }

        private UserDefinition<T> Deserialize(string data)
        {
            UserDefinition<T>? parsed = null;
#if DEBUG || DONTCOMPRESSUSERDATA
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
