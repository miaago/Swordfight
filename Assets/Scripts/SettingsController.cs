using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("UI References")]
    public Slider sensSlider;
    public TextMeshProUGUI sensValueText;

    [Header("Sensitivity Settings")]
    [SerializeField] private float minSensitivity = 1f;
    [SerializeField] private float maxSensitivity = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameSettings.Load();

        sensSlider.minValue = minSensitivity;
        sensSlider.maxValue = maxSensitivity;
        sensSlider.value = GameSettings.MouseSensitivity;

        UpdateText(sensSlider.value);
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
            sensValueText.text = $"{value:0}";
    }

    public void BackToMenu()
    {
        if (PauseManager.FromGameScene)
        {
            PauseManager.FromGameScene = false;
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
