namespace QweCMS.Core.Settings;

public class MongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017/qwecms";
    public string Database { get; set; } = "qwecms";
}