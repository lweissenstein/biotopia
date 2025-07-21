using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//using UnityEngine.InputSystem;
//using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [SerializeField] public float zoomModifier;
    [SerializeField] public float movementModifier;
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(1f, 60f)] float distance = 5f;
    [SerializeField] Transform focus = default;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    public Camera cam;
    Vector3 focusPoint;

    public Transform sphereTransform;
    Vector2 orbitAngles = new Vector2(45f, 0f);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }


    private void LateUpdate()
    {
        ManualRotation();
        
        Vector3 focusPoint = focus.position;
        Vector3 lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }

    // Update is called once per frame
    void Update()
    {
        //focus.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up);
        Debug.Log(transform.rotation.eulerAngles.y);

        var activeTouches = Touch.activeTouches;
        

        Quaternion lookRotation;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (ManualRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        TwoFingerMovement();
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
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
        // get the finger inputs
        Touch primary = Touch.activeTouches[0];
        Touch secondary = Touch.activeTouches[1];
        float x1 = primary.delta.x;
        float x2 = secondary.delta.x;
        float y1 = primary.delta.y;
        float y2 = secondary.delta.y;

        // check if none of the fingers moved, return
        if (primary.phase == TouchPhase.Moved || secondary.phase == TouchPhase.Moved)
        {
            // if fingers have no history, then return
            if (primary.history.Count < 1 || secondary.history.Count < 1)
                return;

            // calculate distance before and after touch movement
            float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
            float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);

            // the zoom distance is the difference between the previous distance and the current distance
            if ((x1 <= 0.0 && x2 >= 0.0) || (x1 >= 0.0 && x2 <= 0.0))
            {
                float pinchDistance = currentDistance - previousDistance;
                Zoom(pinchDistance);

            }
            //if ((x1 <= 0.0 && x2 <= 0.0) || (x1 >= 0.0 && x2 >= 0.0))
            //{
            //    //Vector3 movement = new Vector3(x1, 0, y1);
            //    //float angle = transform.rotation.eulerAngles.y;
            //    //Vector3 rotVec = new Vector3(movement.x * Mathf.Cos(angle) - movement.z * Mathf.Sin(angle), 0, movement.x * Mathf.Sin(angle) + movement.z * Mathf.Cos(angle));

            //    float angle = transform.rotation.eulerAngles.y;
            //    Vector3 temp = new Vector3(-activeTouches[0].delta.x, 0, -activeTouches[0].delta.y);
            //    //Debug.Log(temp);
            //    Vector3 rotVec = Quaternion.AngleAxis(angle, Vector3.up) * temp;
            //    //Debug.Log(rotVec);
            //    focus.position = focus.position + rotVec * movementModifier;
            //}
        }
    }
    public void Zoom(float distance)
    {
        distance = distance * zoomModifier;
        cam.orthographicSize -= distance;
    }
    public Vector2 normalize(Vector3 vec)
    {
        float temp = Mathf.Sqrt(Mathf.Pow(vec.x, 2) + Mathf.Pow(vec.y, 2) + Mathf.Pow(vec.z, 2));
        vec = vec * (1 / temp);
        return vec;
    }
}using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//using UnityEngine.InputSystem;
//using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [SerializeField] public float zoomModifier;
    [SerializeField] public float movementModifier;
    [SerializeField, Range(0.1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(1f, 60f)] float distance = 5f;
    [SerializeField] Transform focus = default;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    public Camera cam;
    Vector3 focusPoint;

    public Transform sphereTransform;
    Vector2 orbitAngles = new Vector2(45f, 0f);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!EnhancedTouchSupport.enabled)
            EnhancedTouchSupport.Enable();
    }


    private void LateUpdate()
    {
        ManualRotation();
        
        Vector3 focusPoint = focus.position;
        Vector3 lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }

    // Update is called once per frame
    void Update()
    {
        //focus.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up);
        Debug.Log(transform.rotation.eulerAngles.y);

        var activeTouches = Touch.activeTouches;
        

        Quaternion lookRotation;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (ManualRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        TwoFingerMovement();
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
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
        // get the finger inputs
        Touch primary = Touch.activeTouches[0];
        Touch secondary = Touch.activeTouches[1];
        float x1 = primary.delta.x;
        float x2 = secondary.delta.x;
        float y1 = primary.delta.y;
        float y2 = secondary.delta.y;

        // check if none of the fingers moved, return
        if (primary.phase == TouchPhase.Moved || secondary.phase == TouchPhase.Moved)
        {
            // if fingers have no history, then return
            if (primary.history.Count < 1 || secondary.history.Count < 1)
                return;

            // calculate distance before and after touch movement
            float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
            float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);

            // the zoom distance is the difference between the previous distance and the current distance
            if ((x1 <= 0.0 && x2 >= 0.0) || (x1 >= 0.0 && x2 <= 0.0))
            {
                float pinchDistance = currentDistance - previousDistance;
                Zoom(pinchDistance);

            }
            //if ((x1 <= 0.0 && x2 <= 0.0) || (x1 >= 0.0 && x2 >= 0.0))
            //{
            //    //Vector3 movement = new Vector3(x1, 0, y1);
            //    //float angle = transform.rotation.eulerAngles.y;
            //    //Vector3 rotVec = new Vector3(movement.x * Mathf.Cos(angle) - movement.z * Mathf.Sin(angle), 0, movement.x * Mathf.Sin(angle) + movement.z * Mathf.Cos(angle));

            //    float angle = transform.rotation.eulerAngles.y;
            //    Vector3 temp = new Vector3(-activeTouches[0].delta.x, 0, -activeTouches[0].delta.y);
            //    //Debug.Log(temp);
            //    Vector3 rotVec = Quaternion.AngleAxis(angle, Vector3.up) * temp;
            //    //Debug.Log(rotVec);
            //    focus.position = focus.position + rotVec * movementModifier;
            //}
        }
    }
    public void Zoom(float distance)
    {
        distance = distance * zoomModifier;
        cam.orthographicSize -= distance;
    }
    public Vector2 normalize(Vector3 vec)
    {
        float temp = Mathf.Sqrt(Mathf.Pow(vec.x, 2) + Mathf.Pow(vec.y, 2) + Mathf.Pow(vec.z, 2));
        vec = vec * (1 / temp);
        return vec;
    }
}