using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Helpers
{
    public class BaseBuilder<T> where T : IDefinition
    {
        private string _resourceDir;
        private Dictionary<Guid, T> _resources = new Dictionary<Guid, T>();
        private Assembly _assembly;

        public BaseBuilder(string resourceDir, Assembly assembly)
        {
            _resourceDir = resourceDir;
            _assembly = assembly;
            Load();
        }

        private void Load()
        {
            _resources.Clear();

            var names = _assembly.GetManifestResourceNames().ToList();
            var source = $"{_assembly.GetName().Name}.{_resourceDir}";
            var resources = names.Where(str => str.StartsWith(source)).ToList();
            foreach (var resource in resources)
            {
                var item = ParseResource(resource);
                _resources.Add(item.ID, item);
            }
        }

        public void Reload() => Load();

        public void LoadModResources(List<T> items)
        {
            foreach(var item in items)
            {
                if (_resources.ContainsKey(item.ID))
                    _resources[item.ID] = item;
                else
                    _resources.Add(item.ID, item);
            }
        }

        public List<Guid> GetResources() => _resources.Keys.ToList();
        public T GetResource(Guid id) => _resources[id];

        private T ParseResource(string resource)
        {
            using (Stream? stream = _assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                    throw new Exception($"Resource not found: {resource}");
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    var map = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (map == null)
                        throw new Exception($"Resource not found: {resource}");
                    return map;
                }
            }
            throw new Exception($"Resource not found: {resource}");
        }
    }
}
