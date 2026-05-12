using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public int healthPoints;
    public GameObject sword;
    void Start()
    {
        sword.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthPoints == 0)
        {
            Destroy(gameObject);
            sword.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            healthPoints--;
        }
        if (collision.gameObject.CompareTag("Health"))
        {
            healthPoints++;
            Destroy(collision.gameObject);
        }
    }

    public int getHealth()
    {
        return healthPoints;
    }
}
