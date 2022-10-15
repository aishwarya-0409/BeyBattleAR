using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 3600;
    public bool doSpin = false;

    private Rigidbody rb;

    public GameObject playerGraphics;

    private void FixedUpdate()
    {
        if (doSpin)
        {
            playerGraphics.transform.Rotate(new Vector3(0, spinSpeed * Time.fixedDeltaTime, 0));
        }
    }
}
