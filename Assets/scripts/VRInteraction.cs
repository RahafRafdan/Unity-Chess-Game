using UnityEngine;

public class VRInteraction : MonoBehaviour
{
    public Material hoverMaterial;
    public Material selectMaterial;
    private Material originalMaterial;
    private MaterialSetter materialSetter;

    private void Start()
    {
        materialSetter = GetComponent<MaterialSetter>();
        originalMaterial = materialSetter.GetComponent<MeshRenderer>().material;
        Debug.Log("VRInteraction script started.");
    }

    public void OnHoverEnter()
    {
        Debug.Log("OnHoverEnter called.");
        materialSetter.SetSingleMaterial(hoverMaterial);
    }

    public void OnHoverExit()
    {
        Debug.Log("OnHoverExit called.");
        materialSetter.SetSingleMaterial(originalMaterial);
    }

    public void OnSelect()
    {
        Debug.Log("OnSelect called.");
        materialSetter.SetSingleMaterial(selectMaterial);
    }

    public void OnDeselect()
    {
        Debug.Log("OnDeselect called.");
        materialSetter.SetSingleMaterial(originalMaterial);
    }
}
