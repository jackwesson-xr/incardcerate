using UnityEngine;

public class ShootScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("BeepBoop");
        }
    }
}
