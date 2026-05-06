using UnityEngine;

public static class GameSettings
{
    public static float MouseSensitivity = 1.5f; // Default starting value

    private const string SensKey = "MouseSensitivity";

    public static void Load()
    {
        //1.5 is default
        MouseSensitivity = PlayerPrefs.GetFloat(SensKey, 1.5f);
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat(SensKey, MouseSensitivity);
        PlayerPrefs.Save();
    }
}
