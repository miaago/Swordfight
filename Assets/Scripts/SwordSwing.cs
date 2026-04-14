using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    // public Animator swordAnimator;
    public GameObject sword;    
    public KeyCode rightDownDiag = KeyCode.Mouse1;
    public float swingSpeed;
    public GameObject swordHitbox;

    void Start()
    {
        swordHitbox.SetActive(false);
    }
    void Update()
    {    
        if (Input.GetKey(rightDownDiag))
        {
            StartCoroutine(RDDSlash());
        }
    }

    IEnumerator RDDSlash()
    {
        enableHitbox();
        sword.GetComponent<Animator>().Play("RDDSlash");
        yield return new WaitForSeconds(swingSpeed);
        sword.GetComponent<Animator>().Play("SwordIdle");
        disableHitbox();
    }

    public void enableHitbox()
    {
        swordHitbox.SetActive(true);
    }

    public void disableHitbox()
    {
        swordHitbox.SetActive(false);
    }

    // void Update()
    // {
    //     if (swordSwingCooldown == 0f && Input.GetKey(rightDownDiagSlash))
    //     {
    //         MyAnimation();
    //         Invoke(nameof(ResetAnimation), 0.5f);

    //     }

    //     // if (swordSwingCooldown > 0f)
    //     // {
    //     //     swordSwingCooldown -= Time.deltaTime * 1f;
    //     // }
    // }

    // private void MyAnimation()
    // {
    //     if (Input.GetKey(rightDownDiagSlash))
    //         swordAnimator.SetBool("RightDownwardDiagonalSlash", true);
    //         // swordSwingCooldown = 1f;
    // }

    // private void ResetAnimation()
    // {
    //     swordAnimator.SetBool("RightDownwardDiagonalSlash", false);
    // }

}
