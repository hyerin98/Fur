[System.Serializable]
public class ConfigData
{
    public int maxPlayerCount = 50;
    public string QRURL = "bit.ly/3sRSf27";
    public string serverURL = "https://joy.im-fine.dev";
    public int INT_SCREEN_RESOLUTION_INDEX;
    public Resolution[] RESOLUTION_RATIOS;
    public ObjectCounts OBJECT_COUNTS;
}

[System.Serializable]
public class Resolution
{
    public int width;
    public int height;
}

[System.Serializable]
public class ObjectCounts
{
    public int[] portrait;
    public int[] landscape;
}
