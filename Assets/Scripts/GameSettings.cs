using UnityEngine;

public static class GameSettings
{
    public static float MouseSensitivity = 0.5f;

    private const string SensKey = "MouseSensitivity";

    public static void Load()
    {
        //1.5 is default
        MouseSensitivity = PlayerPrefs.GetFloat(SensKey, 0.5f);
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat(SensKey, MouseSensitivity);
        PlayerPrefs.Save();
    }
}
