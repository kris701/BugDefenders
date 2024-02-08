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
        private bool _loaded = false;
        private Dictionary<string, string> _resources = new Dictionary<string, string>();

        public BaseBuilder(string resourceDir)
        {
            _resourceDir = resourceDir;
            Load();
        }

        private void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames().ToList();
            var resources = names.Where(str => str.StartsWith($"{assembly.GetName().Name}.{_resourceDir}")).ToList();
            foreach (var resource in resources)
            {
                var name = resource.Substring(0, resource.LastIndexOf("."));
                name = name.Substring(name.LastIndexOf(".") + 1);
                _resources.Add(name, resource);
            }
            _loaded = true;
        }
        public List<string> GetResources()
        {
            if (!_loaded)
                Load();
            return _resources.Keys.ToList();
        }
        public T GetResource(string name)
        {
            if (!_loaded)
                Load();

            if (!_resources.ContainsKey(name))
                throw new Exception($"Resource not found: {name}");

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream? stream = assembly.GetManifestResourceStream(_resources[name]))
            {
                if (stream == null)
                    throw new Exception($"Resource not found: {name}");
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    var map = JsonSerializer.Deserialize<T>(result);
                    if (map == null)
                        throw new Exception($"Resource not found: {name}");
                    return map;
                }
            }
            throw new Exception($"Resource not found: {name}");
        }
    }
}
