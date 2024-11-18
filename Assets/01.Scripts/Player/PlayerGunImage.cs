using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGunImage : MonoBehaviour
{
    private RawImage rawImage;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    public void SetImage(Texture2D texture2D)
    {
        rawImage.texture = texture2D;
    }
}
