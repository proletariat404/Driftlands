// Interactable.cs
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;
            OnInteract(collision.gameObject);
        }
    }

    protected abstract void OnInteract(GameObject player);
}
