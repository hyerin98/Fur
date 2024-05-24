using System.Collections.Generic;

[System.Serializable]
public class Resolution
{
    public int width;
    public int height;
}

[System.Serializable]
public class ConfigData
{
    public int maxPlayerCount = 50;
    public string QRURL = "bit.ly/3sRSf27";
    public string serverURL = "https://joy.im-fine.dev";
     public int INT_SCREEN_RESOLUTION_INDEX;
    
}