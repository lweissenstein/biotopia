
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSequence", menuName = "Scriptable Objects/TutorialSequence")]
public class TutorialSequence : ScriptableObject
{
    public List<TutorialStepData> steps;
}

public enum TutorialStepType
{
    TextPopup,
    CameraMovement,
    WaitForClick,
    Cutscene,
    SpectateForSeconds,
    UntenText,
}


[System.Serializable]
public class TutorialStepData
{
    public TutorialStepType stepType;
    public string stepName;

    [TextArea] public string titleText;
    [TextArea] public string tutorialText;
    public GameObject targetObject;

    [Header("Allow Updates?")]
    public bool allowUpdates;

    [Header("Wich Updates?")]
    public bool allowProduction;
    public bool allowSaturation;
    public bool allowConsumption;

    [Header("Cinema Settings + Kamerafahrten + Player Input")]
    public bool allowPlayerInput;
    public bool allowBuildingSpawn;
    public float cameraMoveTime = 0f;
    public float secondsToWait = 0f;

    [Header("Waiting for click on specific Building?")]
    public bool waitForClick;
    public bool waitForButton;

    [Header("Camera Settings")]
    public Vector3 cameraPosition;
    public float cameraHorizontalAxis;
    public float cameraVerticalAxis;
    public float cameraOrthogrpahicSize;

    public bool spinCam360;

    [Header("Left/Right Panel Visibility")]
    public bool isLeft;
    public bool isRight;
    public bool buttonLeft;
    public bool buttonRight;

    [Header("Compartimente")]
    public bool alge;
    public bool supermarkt;
    public bool algePowder;

    [Header("UIs")]
    public int grayOutIndex;

    public bool grayOutOpen;
    public bool menuOpen;
    public bool showMoney;
    public bool processClose;
    public bool upgradeClose;
    public bool menuClose;
}