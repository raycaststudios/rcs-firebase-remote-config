using System.Collections.Generic;

namespace RCS.PluginMediation
{
    public interface IRemoteConfigPlugin
    {
        Dictionary<string, string> GetMultipleStrings(List<string> keys);
        void SetDefaults(Dictionary<string, object> defaults);
        bool GetBool(string key); // ? Add this line
        bool IsReady { get; }
    }
}
