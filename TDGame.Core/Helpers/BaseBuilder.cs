using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TDGame.Core.Helpers
{
    public class BaseBuilder<T>
    {
        private string _resourceDir;
        private Dictionary<string, string> _resources = new Dictionary<string, string>();
        private Assembly _assembly;

        public BaseBuilder(string resourceDir, Assembly assembly)
        {
            _resourceDir = resourceDir;
            _assembly = assembly;
            Load();
        }

        private void Load()
        {
            var names = _assembly.GetManifestResourceNames().ToList();
            var source = $"{_assembly.GetName().Name}.{_resourceDir}";
            var resources = names.Where(str => str.StartsWith(source)).ToList();
            foreach (var resource in resources)
            {
                var name = resource.Substring(0, resource.LastIndexOf("."));
                name = name.Substring(name.LastIndexOf(".") + 1);
                _resources.Add(name, resource);
            }
        }
        public List<string> GetResources() => _resources.Keys.ToList();
        public T GetResource(string name)
        {
            if (!_resources.ContainsKey(name))
                throw new Exception($"Resource not found: {name}");

            using (Stream? stream = _assembly.GetManifestResourceStream(_resources[name]))
            {
                if (stream == null)
                    throw new Exception($"Resource not found: {name}");
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    var map = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    if (map == null)
                        throw new Exception($"Resource not found: {name}");
                    return map;
                }
            }
            throw new Exception($"Resource not found: {name}");
        }
    }
}
