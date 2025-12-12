using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SimplePCCameraFixedFocus : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 10f;       // WASD movement speed (Panning)
    public float orbitSpeed = 0.2f;    // Mouse rotation sensitivity (Orbiting)
    public float fastMultiplier = 3f;  // Hold Shift to move faster

    // --- Removed PanDragDelay ---

    [Header("Zoom Limits")]
    public float minZoomSize = 0.5f;
    public float maxZoomSize = 200f;

    // --- Hard-coded strong zoom value ---
    private const float ScrollZoomStrength = 0.5f;

    [Header("Orbit State")]
    public Transform focusPoint;

    // Internal State
    private float _currentDistance;
    private float _currentYaw;
    private float _currentPitch;
    private bool _hasMovedThisFrame = false;

    // --- REMOVED: Time tracking for click vs. drag (_lmbHoldStartTime) ---

    private Mouse _mouse;
    private Keyboard _keyboard;
    private Camera _cam;

    void Start()
    {
        _mouse = Mouse.current;
        _keyboard = Keyboard.current;
        _cam = GetComponent<Camera>();

        if (!_cam.orthographic)
        {
            _cam.orthographic = true;
        }

        if (focusPoint == null)
        {
            focusPoint = new GameObject("CameraFocusPoint").transform;
            focusPoint.position = Vector3.zero;
        }

        Vector3 directionToFocus = transform.position - focusPoint.position;
        _currentDistance = directionToFocus.magnitude;

        Quaternion lookRotation = Quaternion.LookRotation(-directionToFocus.normalized);
        _currentYaw = lookRotation.eulerAngles.y;
        _currentPitch = lookRotation.eulerAngles.x;

        _currentDistance = Mathf.Clamp(_currentDistance, minZoomSize, maxZoomSize);
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoomSize, maxZoomSize);

        UpdateCameraTransform();
    }

    void LateUpdate()
    {
        if (_mouse == null || _keyboard == null) return;

        float deltaPanSpeed = panSpeed * Time.deltaTime;

        if (_keyboard.leftShiftKey.isPressed)
        {
            deltaPanSpeed *= fastMultiplier;
        }

        _hasMovedThisFrame = false;

        // --- REMOVED: All LMB time tracking logic here ---

        // Handle all inputs
        HandleOrbit();
        HandlePanning(deltaPanSpeed); // <-- Now only uses WASD
        HandleDolly();

        if (_hasMovedThisFrame)
        {
            UpdateCameraTransform();
        }
    }

    // --- 1. ORBITING (Right Mouse Button) ---
    void HandleOrbit()
    {
        if (_mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta = _mouse.delta.ReadValue();
            _currentYaw += mouseDelta.x * orbitSpeed;
            _currentPitch -= mouseDelta.y * orbitSpeed;
            _currentPitch = Mathf.Clamp(_currentPitch, 5f, 85f);
            _hasMovedThisFrame = true;
        }
    }

    // --- 2. PANNING (WASD ONLY) ---
    void HandlePanning(float panSpeed)
    {
        Vector3 panVector = Vector3.zero;

        // WASD Panning (Only use keyboard inputs)
        if (_keyboard.aKey.isPressed) panVector -= transform.right;
        if (_keyboard.dKey.isPressed) panVector += transform.right;
        if (_keyboard.wKey.isPressed) panVector += transform.up;
        if (_keyboard.sKey.isPressed) panVector -= transform.up;

        // --- REMOVED: All LMB/Drag panning logic here ---

        if (panVector.sqrMagnitude > 0)
        {
            // Apply movement based on panSpeed and time
            Vector3 movement = panVector.normalized * panSpeed * Time.deltaTime; // Re-introduced Time.deltaTime scaling here for proper speed control
            transform.position += movement;
            focusPoint.position += movement;
            _hasMovedThisFrame = true;
        }
    }

    // --- 3. DOLLY/ZOOM (Scroll Wheel) ---
    void HandleDolly()
    {
        float scrollDelta = _mouse.scroll.ReadValue().y;
        float zoomChange = 0f;

        if (Mathf.Abs(scrollDelta) > 0.001f)
        {
            zoomChange -= Mathf.Sign(scrollDelta) * ScrollZoomStrength;
            _hasMovedThisFrame = true;
        }

        if (Mathf.Abs(zoomChange) > 0f)
        {
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize + zoomChange,
                minZoomSize,
                maxZoomSize
            );
        }

        _currentDistance = Mathf.Clamp(_currentDistance, minZoomSize, maxZoomSize);
    }

    // --- FINAL TRANSFORM UPDATE ---
    void UpdateCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        Vector3 position = focusPoint.position + (rotation * Vector3.back * _currentDistance);
        transform.position = position;
        transform.rotation = rotation;
    }
}
