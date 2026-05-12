using System;
using UnityEngine;

public class DistractablePlants : MonoBehaviour
{


    public event EventHandler OnDistractableTakeDamage;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            OnDistractableTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);

            NavMeshSurfaceManagment.Instance.RebakeNavMeshSurface();
        }
    }
}
