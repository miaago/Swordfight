using UnityEngine;

public class CameraFolllow : MonoBehaviour
{
    public Transform cameraPosition;
    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
