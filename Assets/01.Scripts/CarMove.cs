using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    public Transform target;
    private AudioSource audioSource;

    public bool isStart = false;
    private float speed = 0.3f;

    private bool isPlay = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isStart) return;

        if (!isPlay)
        {
            audioSource.Play();
            isPlay = true;
        }

        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
    }
}
