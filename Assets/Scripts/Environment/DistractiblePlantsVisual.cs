using UnityEngine;

public class DistractiblePlantsVisual : MonoBehaviour
{


    [SerializeField] private DistractiblePlants distractiblePlants;
    [SerializeField] private GameObject bushDeathVFXPrefab;


    private void Start()
    {
        distractiblePlants.OnDistractibleTakeDamage += DistractiblePlantsOnDistractibleTakeDamage;
    }

    private void DistractiblePlantsOnDistractibleTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(bushDeathVFXPrefab, distractiblePlants.transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        distractiblePlants.OnDistractibleTakeDamage -= DistractiblePlantsOnDistractibleTakeDamage;
    }
}
