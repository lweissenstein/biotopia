using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraInputController : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController inputAxisController;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private GameObject cameraRig;

    private void Update()
    {
        // 
        if (!GameState.isTutorial)
        {
            EnableCameraControl();
        }
        else
        {
            DisableCameraControl();
        }
    }

    public void StartCameraMoveSmooth(Vector3 pos, float orthographicSize, float horizontalAxis, float verticalAxis, float time)
    {
        StartCoroutine(CameraMoveCoroutine(pos, orthographicSize, horizontalAxis, verticalAxis, time));
    }

    private IEnumerator CameraMoveCoroutine(Vector3 targetPos, float targetSize, float targetHoriz, float targetVert, float duration)
    {
        Vector3 startPos = cameraRig.transform.position;
        float startSize = cam.Lens.OrthographicSize;
        float startHoriz = orbitalFollow.HorizontalAxis.Value;
        float startVert = orbitalFollow.VerticalAxis.Value;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            t = t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

            cameraRig.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t);
            orbitalFollow.HorizontalAxis.Value = Mathf.LerpAngle(startHoriz, targetHoriz, t);
            orbitalFollow.VerticalAxis.Value = Mathf.Lerp(startVert, targetVert, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalwerte setzen, um Rundungsfehler zu vermeiden
        cameraRig.transform.position = targetPos;
        cam.Lens.OrthographicSize = targetSize;
        orbitalFollow.HorizontalAxis.Value = targetHoriz;
        orbitalFollow.VerticalAxis.Value = targetVert;
    }

    public void StartCameraSpinSmooth(Vector3 pos, float orthographicSize, float verticalAxis, float duration)
    {
        float startHoriz = orbitalFollow.HorizontalAxis.Value;
        float targetHoriz = startHoriz + 360f;

        StartCoroutine(CameraSpinCoroutine(pos, orthographicSize, startHoriz, targetHoriz, verticalAxis, duration));
    }

    private IEnumerator CameraSpinCoroutine(Vector3 targetPos, float targetSize, float startHoriz, float targetHoriz, float targetVert, float duration)
    {
        Vector3 startPos = cameraRig.transform.position;
        float startSize = cam.Lens.OrthographicSize;
        float startVert = orbitalFollow.VerticalAxis.Value;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t; // InOutQuad easing

            cameraRig.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t);
            orbitalFollow.HorizontalAxis.Value = Mathf.Lerp(startHoriz, targetHoriz, t); // keine LerpAngle!
            orbitalFollow.VerticalAxis.Value = Mathf.Lerp(startVert, targetVert, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Wrap around auf 0°
        orbitalFollow.HorizontalAxis.Value = startHoriz % 360f;
        cameraRig.transform.position = targetPos;
        cam.Lens.OrthographicSize = targetSize;
        orbitalFollow.VerticalAxis.Value = targetVert;
    }

    public void StartCameraMoveInstant(Vector3 pos, float orthographicSize, float horizontalAxis, float verticalAxis)
    {
        cameraRig.transform.position = pos;
        orbitalFollow.HorizontalAxis.Value = horizontalAxis;
        orbitalFollow.VerticalAxis.Value = verticalAxis;
        cam.Lens.OrthographicSize = orthographicSize;
    }

    public void EnableCameraControl()
    {
        if (inputAxisController != null)
        {
            inputAxisController.enabled = true;
        }
    }

    public void DisableCameraControl()
    {
        if (inputAxisController != null)
        {
            inputAxisController.enabled = false;
        }
    }
}

