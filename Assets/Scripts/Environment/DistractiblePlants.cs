using System;
using UnityEngine;

public class DistractiblePlants : MonoBehaviour
{


    public event EventHandler OnDistractibleTakeDamage;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            OnDistractibleTakeDamage?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);

            NavMeshSurfaceManagement.Instance.RebakeNavMeshSurface();
        }
    }
}
