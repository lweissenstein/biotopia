//using UnityEngine;

//public class CameraMovement : MonoBehaviour
//{
//    public float moveSpeed = 5f;
//    public float verticalSpeedFactor = 1.5f; // Skalierungsfaktor für die vertikale Geschwindigkeit

//    void Update()
//    {
//        // Input lesen (WASD)
//        float horizontal = Input.GetAxisRaw("Horizontal");
//        float vertical = Input.GetAxisRaw("Vertical");

//        // XZ-Anteil der Blickrichtung berechnen
//        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
//        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z).normalized;

//        // Bewegung berechnen
//        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

//        // Vertikale Bewegung schneller skalieren
//        if (vertical != 0)
//        {
//            moveDirection *= verticalSpeedFactor;  // Erhöht die Geschwindigkeit nach oben und unten
//        }

//        // Bewegen
//        transform.position += moveDirection * moveSpeed * Time.deltaTime;
//    }
//}