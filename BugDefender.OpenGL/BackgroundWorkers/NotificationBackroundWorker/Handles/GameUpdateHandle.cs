using BugDefender.OpenGL.Helpers;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class GameUpdateHandle : INotificationHandle
    {
        public NotificationBackroundWorker Parent { get; }

        private bool _checked = false;
        private bool _isDoneCheckingForUpdate = false;
        private bool _hasUpdate = false;
        private string _availableUpdateVersion = "";
        public GameUpdateHandle(NotificationBackroundWorker parent)
        {
            Parent = parent;
            CheckForUpdate();
        }

        private void CheckForUpdate()
        {
            Task.Run(() =>
            {
                try
                {
                    var thisVersion = Assembly.GetEntryAssembly()?.GetName().Version!;
                    var thisVersionStr = $"v{thisVersion.Major}.{thisVersion.Minor}.{thisVersion.Build}";
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BugDefenderHTTPClient", "1"));
                    var response = client.GetAsync("https://api.github.com/repos/kris701/BugDefenders.Public/releases/latest").Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        content = content.Substring(content.IndexOf("\"tag_name\":") + "\"tag_name\":".Length);
                        content = content.Substring(0, content.IndexOf(','));
                        content = content.Replace("\"", "");

                        _availableUpdateVersion = content;
                        if (thisVersionStr != content)
                            _hasUpdate = true;
                    }
                }
                catch
                {
                    _hasUpdate = false;
                }
            });
            _isDoneCheckingForUpdate = true;
        }

        public NotificationItem? GetNewNotification()
        {
            if (!_checked && _isDoneCheckingForUpdate)
            {
                _checked = true;
                if (_hasUpdate)
                    return new NotificationItem("", new ManualDefinition($"Update Available", $"Version {_availableUpdateVersion} is available! Click here to go to the download page."), false, (s) => { LinkHelper.OpenUrl("https://kris701.itch.io/bug-defenders"); });
            }
            return null;
        }
    }
}
