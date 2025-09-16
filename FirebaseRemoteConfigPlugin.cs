using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Collections.Generic;


namespace RCS.PluginMediation
{
    public class FirebaseRemoteConfigPlugin : IRemoteConfigPlugin
    {
        public bool IsReady { get; private set; } = false;

        public bool GetBool(string key)
        {
            if (!IsReady) return false;
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
            return value == "1";
        }


        public Dictionary<string, string> GetMultipleStrings(List<string> keys)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            if (!IsReady) return results;

            foreach (var key in keys)
            {
                string value = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
                results[key] = value;
            }

            return results;
        }

        public void SetDefaults(Dictionary<string, object> defaults)
        {
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        }

        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    Debug.Log("[RCS] Firebase Remote Config initialized.");
                    FetchAndActivate();
                }
                else
                {
                    Debug.LogError($"[RCS] Firebase dependencies not resolved: {status}");
                }
            });
        }

        private void FetchAndActivate()
        {
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
            {
                if (fetchTask.IsCompleted && !fetchTask.IsFaulted)
                {
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(activateTask =>
                    {
                        IsReady = true;
                        Debug.Log("[RCS] Remote Config fetched and activated.");
                    });
                }
                else
                {
                    Debug.LogWarning("[RCS] Failed to fetch Remote Config.");
                }
            });
        }
    }
}
