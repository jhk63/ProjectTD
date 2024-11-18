using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    private Slider slider;

    private Transform cam;
    public Transform target;

    public bool isBoss = false;

    void Awake()
    {
        slider = GetComponent<Slider>();

        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        transform.position = target.position + new Vector3(0, 1.5f, 0);

        if (isBoss)
        {
            transform.position = target.position + new Vector3(0, 3.5f, 0);
        }
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }
}
