using System;
using UnityEngine;

public class DistractablePlantsVisual : MonoBehaviour
{


    [SerializeField] private DistractablePlants _distractablePlants;
    [SerializeField] private GameObject _bushDeathVFXPrefab;


    private void Start()
    {
        _distractablePlants.OnDistractableTakeDamage += _distractablePlants_OnDistractableTakeDamage;
    }

    private void _distractablePlants_OnDistractableTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(_bushDeathVFXPrefab, _distractablePlants.transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        _distractablePlants.OnDistractableTakeDamage -= _distractablePlants_OnDistractableTakeDamage;
    }
}
