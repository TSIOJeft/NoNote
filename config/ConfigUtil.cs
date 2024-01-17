using System.IO;
using Newtonsoft.Json;
using NoNote.util;

namespace NoNote.config;

public class ConfigUtil
{
    public static ConfigArray configArray;
    public static string myFolder;

    public ConfigArray getConfig()
    {
        if (configArray != null) return configArray;
        string config_json = FileUtil.getTextFile(myFolder + "\\data\\config.json");
        if (string.IsNullOrEmpty(config_json))
        {
            configArray = new ConfigArray();
        }
        else
        {
            configArray = JsonConvert.DeserializeObject<ConfigArray>(config_json);
        }

        return configArray;
    }

    public void saveConfig()
    {
        FileUtil.saveTextFile(myFolder + "\\data\\config.json", JsonConvert.SerializeObject(configArray));
    }

    public class ConfigArray
    {
        public string workFolder = Path.Combine(myFolder, "assets\\editor");
    }
}