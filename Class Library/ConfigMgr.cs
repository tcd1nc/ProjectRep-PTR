using System.IO;
using Newtonsoft.Json.Linq;

namespace PTR
{
    public static class ConfigMgr
    {
        public static string ReadJsonValue(string filepath, string key)
        {
            string result = string.Empty;
            try
            {
                if (File.Exists(filepath))
                {
                    using (StreamReader r = new StreamReader(filepath))
                    {
                        var json = r.ReadToEnd();
                        var jobj = JObject.Parse(json);
                        result = jobj[key].ToString();
                    }
                }
            }
            catch { }
            return result;
        }

        public static string GetLocalJSONPath()
        {
            try
            {
                System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
                //To get the Directory path
                return Path.GetDirectoryName(asmly.Location);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
