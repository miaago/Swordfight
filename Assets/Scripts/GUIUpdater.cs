using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GUIUpdater : MonoBehaviour
{
    public TextMeshProUGUI HP;
    public TextMeshProUGUI EnemyCount;

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject HUD;
    public GameObject pauseScreen;
    public Player player;
    private int currentEnemies;
    void Start()
    {
        HUD.SetActive(true);        
        currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        HP.text = "Lives: " + player.getHealth();
        EnemyCount.text = "Enemies Left: " + currentEnemies;
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        HP.text = "Lives: " + player.getHealth();
        EnemyCount.text = "Enemies Left: " + currentEnemies;
        GameState();
    }

    void GameState()
    {
        if (player.getHealth() == 0) // lose
        {
            pauseScreen.SetActive(true);
            loseScreen.SetActive(true);
            HUD.SetActive(false);
        }

        if (currentEnemies == 0) // win
        {
            pauseScreen.SetActive(true);
            winScreen.SetActive(true);
            HUD.SetActive(false);
        }
    }
}
