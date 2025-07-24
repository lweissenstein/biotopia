
using UnityEngine;
using UnityEngine.EventSystems;

//using UnityEngine.InputSystem;
//using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [SerializeField, Range(0.001f, 1f)] public float zoomModifier;
    [SerializeField, Range(0.001f, 0.01f)] public float movementModifier;
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(1f, 60f)] float distance = 5f;
    [SerializeField] Transform focus = default;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField] float minCamSize;
    [SerializeField] float maxCamSize;
    private float screenSizeMultiplier = (Screen.height + Screen.width) * 0.01f;
    public Camera cam;
    Vector3 focusPoint;
    //static float baseCamSize;
    //static Vector3 baseCamPos;

    public Transform sphereTransform;
    Vector2 orbitAngles = new Vector2(45f, 0f);
    private Quaternion lastRotation;
    private Vector3 lastPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();

        Application.targetFrameRate = 60;
        //    baseCamSize = cam.orthographicSize;
        //    baseCamPos = focus.position;
        //}
    }


    private void LateUpdate()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Blockiere Kameraeingaben über UI
        }

        // Aktuellen Fokuspunkt holen
        focusPoint = focus.position;

        // Kamera-Rotation manuell aktualisieren, falls nötig
        bool rotated = ManualRotation();
        if (rotated)
        {
            ConstrainAngles();
        }

        Quaternion lookRotation = rotated ? Quaternion.Euler(orbitAngles) : transform.localRotation;

        // Neue Kamera-Position berechnen
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        // Kamera nur bewegen, wenn nötig
        if (lookRotation != lastRotation || lookPosition != lastPosition)
        {
            transform.SetPositionAndRotation(lookPosition, lookRotation);
            lastRotation = lookRotation;
            lastPosition = lookPosition;
        }
    }
    private void Update()
    {
        // Bewegung + Zoom
        TwoFingerMovement();
    }

    // Update is called once per frame
    bool ManualRotation()
    {
        var activeTouches = Touch.activeTouches;
        if (activeTouches.Count == 1)
        {
            Vector2 input = new Vector2(
                -activeTouches[0].delta.y, activeTouches[0].delta.x
            );
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
                return true;
            }
        }
        return false;
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }


    public void TwoFingerMovement()
    {
        var activeTouches = Touch.activeTouches;
        if (activeTouches.Count < 2)
        {
            return;
        }

        Touch primary = Touch.activeTouches[0];
        Touch secondary = Touch.activeTouches[1];
        float x1 = primary.delta.x;
        float x2 = secondary.delta.x;
        float y1 = primary.delta.y;
        float y2 = secondary.delta.y;

        //Wenn Finger bewegt
        if (primary.phase == TouchPhase.Moved || secondary.phase == TouchPhase.Moved)
        {
            //Wenn Finger keine Historie haben, return
            if (primary.history.Count < 1 || secondary.history.Count < 1){
                return;
            }
            //Distanz berechnen
            float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
            float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);
            if ((x1 <= 0.0 && x2 <= 0.0) || (x1 >= 0.0 && x2 >= 0.0))
            {
                float angle = transform.rotation.eulerAngles.y;
                Vector3 temp = new Vector3(-activeTouches[0].delta.x, 0, -activeTouches[0].delta.y * 2);
                Vector3 rotVec = Quaternion.AngleAxis(angle, Vector3.up) * temp;
                focus.position = focus.position + rotVec * (movementModifier * screenSizeMultiplier);
                return;
            }            
            if ((x1 < 5.0 && x2 > 5.0) || (x1 > 5.0 && x2 < 5.0))
            {
                float pinchDistance = currentDistance - previousDistance;
                Zoom(pinchDistance);
            }
        }
    }
    public void Zoom(float distance)
    {
        distance = distance * (zoomModifier * screenSizeMultiplier);
        float temp = cam.orthographicSize - distance;
        if (temp < maxCamSize && temp > minCamSize) 
        {
            cam.orthographicSize = temp;            
        }
    }

    public Vector2 normalize(Vector3 vec)
    {
        float temp = Mathf.Sqrt(Mathf.Pow(vec.x, 2) + Mathf.Pow(vec.y, 2) + Mathf.Pow(vec.z, 2));
        vec = vec * (1 / temp);
        return vec;
    }

    //public void resetPos()
    //{
    //    var activeTouches = Touch.activeTouches;
    //    if (activeTouches.Count == 0) return;
    //    if (activeTouches[0].tapCount == 2)
    //    {
    //        cam.orthographicSize = baseCamSize;
    //        focus.position = baseCamPos;
    //    }
    //}
}