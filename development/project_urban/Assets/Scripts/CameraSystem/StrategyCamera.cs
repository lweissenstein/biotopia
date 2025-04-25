using UnityEngine;

public class CameraRigController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraRigTransform; 
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField, Tooltip("1 für RMB und 2 für MMB")] private int mouseButton = 1;
    private float currentYRotation = 0f; // startrotation Y
    private float rotationStep = 90f;     
    private float rotationSpeed = 360f; // drehgeschwindigkeit pro sekunde

    private Camera cam;

    void Start()
    {
        cam = cameraTransform.GetComponent<Camera>();
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleRotation();
        ApplyRotation();
    }

    void HandleZoom()  // ändert die orthographic size der camera => dadurch, dass orthographic die entfernung der kamera ignoriert, muss man die size ändern für einen zoom effekt
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // clamp auf max und min zoom aus den werten oben
        }
    }

    void HandlePan()
    {
        Vector3 move = Vector3.zero;

        // bewegung der kamera
        if (Input.GetKey(KeyCode.W)) move += cameraTransform.up;
        if (Input.GetKey(KeyCode.S)) move -= cameraTransform.up;
        if (Input.GetKey(KeyCode.A)) move -= cameraTransform.right;
        if (Input.GetKey(KeyCode.D)) move += cameraTransform.right;

        // mausdrag mit rechter oder mittlerer Maustaste
        if (Input.GetMouseButton(mouseButton))
        {
            float mouseX = -Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");
            float dragSpeed = cam.orthographicSize * 0.4f; // abhängig vom Zoom-Level
            move += (cameraTransform.right * mouseX + cameraTransform.up * mouseY) * dragSpeed;
        }

        // bewegung anwenden auf das parent rig object
        transform.position += move * moveSpeed * Time.deltaTime;

        Vector3 clampedPosition = transform.position;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -10f, 10f);  //hier noch clampen, dasmit man nicht ins unendliche raus-pannen kann
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -10f, 10f);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -10f, 10f);

        transform.position = clampedPosition;

    }

    void ApplyRotation() // smooth rotation -> von chat gibidi
    {
        Quaternion targetRotation = Quaternion.Euler(cameraRigTransform.rotation.eulerAngles.x, currentYRotation, cameraRigTransform.rotation.eulerAngles.z);
        cameraRigTransform.rotation = Quaternion.RotateTowards(
            cameraRigTransform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleRotation() // rotation mit E und Q 
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentYRotation -= rotationStep;
            currentYRotation = NormalizeAngle(currentYRotation);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentYRotation += rotationStep;
            currentYRotation = NormalizeAngle(currentYRotation);
        }
    }

    float NormalizeAngle(float angle) // damit man nicht irgendwann unendliche winkel hat wie 329864938247983260 grad
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
}