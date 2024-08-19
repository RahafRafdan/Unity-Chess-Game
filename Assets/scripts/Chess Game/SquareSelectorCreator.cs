using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] private Material freesquareMaterial;
    [SerializeField] private Material opponentsquareMaterial;
    [SerializeField] private GameObject selectorPrefab;
    private List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3, bool> squareData)
    {
        ClearSelection();
        foreach (var data in squareData)
        {
            Vector3 selectorPosition = data.Key + new Vector3(0, 0.01f, 0); // Slightly above the board
            GameObject selector = Instantiate(selectorPrefab, selectorPosition, Quaternion.identity);
            instantiatedSelectors.Add(selector);
            MaterialSetter[] materialSetters = selector.GetComponentsInChildren<MaterialSetter>();
            foreach (var setter in materialSetters)
            {
                setter.SetSingleMaterial(data.Value ? freesquareMaterial : opponentsquareMaterial);
            }
        }
    }


    public void ClearSelection()
    {
        foreach (var selector in instantiatedSelectors)
        {
            Destroy(selector.gameObject);
        }
        instantiatedSelectors.Clear();
    }
}
