using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    public UnityEvent EndTeleport;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null && targetTransform != null)
            {
                controller.enabled = false;
                other.transform.position = targetTransform.position;
                controller.enabled = true;
                EndTeleport.Invoke();
                gameObject.SetActive(false);
            }
        }
    }

    public void OnActivate()
    {
        gameObject.SetActive(true);
    }
}
