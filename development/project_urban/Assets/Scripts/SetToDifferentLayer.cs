using UnityEngine;

public class SetToInteractable : MonoBehaviour
{
    public GameObject objectToChangeLayer;
    public int layerNumber;
    
    void Start()
    {
        objectToChangeLayer.layer = layerNumber;
    }
}
