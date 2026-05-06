using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public Slider sensSlider;
    public TextMeshProUGUI sensValueText;

    void Start()
    {
        GameSettings.Load();

        sensSlider.minValue = 0.1f;
        sensSlider.maxValue = 10f;
        sensSlider.value = GameSettings.MouseSensitivity;

        UpdateText(sensSlider.value);

        // Listen for when the player moves the slider
        sensSlider.onValueChanged.AddListener(OnSensChanged);
    }

    public void OnSensChanged(float newSens)
    {
        GameSettings.MouseSensitivity = newSens;
        GameSettings.Save();
        UpdateText(newSens);
    }

    void UpdateText(float value)
    {
        if (sensValueText != null)
            sensValueText.text = $"Sensitivity: {value:0.0}";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
