namespace sql2crud.net;

#nullable disable

public class Config
{
    public string ConnectionString { get; set; }
    public List<string> SearchEntities { get; set; }
    public string SqlScriptsDirPath { get; set; }
    public string EntitiesDirPath { get; set; }
    public string DTOsDirPath { get; set; }
    public string ServicesDirPath { get; set; }
    public string ControllersDirPath { get; set; }
    public string EntitiesNamespace { get; set; }
    public string DTOsNamespace { get; set; }
    public string ServicesNamespace { get; set; }
    public string ControllersNamespace { get; set; }
}
