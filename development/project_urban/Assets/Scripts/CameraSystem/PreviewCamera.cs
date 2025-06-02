using System;
using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraRigTransform;
    private float currentYRotation = 0f; // startrotation Y
    [SerializeField] private float rotationSpeedperSecond = 45f; // drehgeschwindigkeit pro sekunde
  

    private Camera cam;

    void Update()
    {
        currentYRotation += rotationSpeedperSecond * Time.deltaTime;
        currentYRotation = NormalizeAngle(currentYRotation);
        ApplyRotation();
    }

    void ApplyRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(
            cameraRigTransform.rotation.eulerAngles.x,
            currentYRotation,
            cameraRigTransform.rotation.eulerAngles.z
        );

        cameraRigTransform.rotation = Quaternion.RotateTowards(
            cameraRigTransform.rotation,
            targetRotation,
            rotationSpeedperSecond * Time.deltaTime
        );
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
}