using UnityEngine;

public class BuildingPreviewManager : MonoBehaviour
{
    public Camera previewCamera;
    public RenderTexture previewTexture;
    public Transform previewAnchor; // leerer Pivot für Rotation

    private GameObject currentPreview;


    public void Start()
    {
        previewAnchor.position = new Vector3(999f, 999f, 999f);
    }
    public void ShowPreview(GameObject prefab, int level)
    {
        ClearPreview();

        currentPreview = Instantiate(prefab, previewAnchor);
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.layer = LayerMask.NameToLayer("PreviewLayer");
        SetLayerRecursive(currentPreview.transform, LayerMask.NameToLayer("PreviewLayer"));

        // Nur das Child für das richtige Level aktivieren
        SetActiveChild(currentPreview.transform, level - 1);

        // BuildingInstance Script deaktivieren, damit keine Logik läuft
        var buildingInstance = currentPreview.GetComponent<BuildingInstance>();
        if (buildingInstance != null)
        {
            buildingInstance.enabled = false; // Script deaktivieren
                                              // Oder falls du isPreview-Flag hast:
                                              // buildingInstance.isPreview = true;
        }
    }

    private void SetActiveChild(Transform moin, int activeID)
    {
        var buildinInstance = currentPreview.GetComponent<BuildingInstance>();

        if (buildinInstance.compartmentTypeHouse != 7)
        {
            for (int i = 0; i < 3; i++)
            {
                Transform child = moin.Find(i.ToString());
                if (child != null)
                    child.gameObject.SetActive(i == activeID);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Transform notChild = moin.Find(i.ToString());
                if (notChild != null)
                    notChild.gameObject.SetActive(false);
            }

            Transform child = transform.Find("7");
            if (child != null)
                child.gameObject.SetActive(true);

            Transform childIndicator = transform.Find("8");
            if (childIndicator != null)
                childIndicator.gameObject.SetActive(false);
        }
    }

    public void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        foreach (Transform child in t)
        {
            SetLayerRecursive(child, layer);
        }
    }
}




