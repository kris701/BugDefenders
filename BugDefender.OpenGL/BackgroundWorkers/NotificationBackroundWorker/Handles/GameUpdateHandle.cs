﻿using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker.Handles
{
    public class GameUpdateHandle : INotificationHandle
    {
        public NotificationBackroundWorker Parent { get; }

        private bool _checked = false;
        public GameUpdateHandle(NotificationBackroundWorker parent)
        {
            Parent = parent;
        }

        public NotificationItem? GetNewNotification()
        {
            if (!_checked)
            {
                _checked = true;
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

                        if (thisVersionStr != content)
                            return new NotificationItem("", new ManualDefinition($"Update Available", $"Version {content} is available! Click here to go to the download page."), false, (s) => { OpenUrl("https://github.com/kris701/BugDefenders.Public/releases"); });
                    }
                }
                catch
                {
                    return new NotificationItem("", new ManualDefinition($"Update Check Failed", $"An error occured while checking for updates...."), false);
                }
            }
            return null;
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}