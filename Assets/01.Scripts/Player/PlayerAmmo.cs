using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void SetText(int maxAmmo, int currenAmmo)
    {
        text.SetText("{0}/{1}", currenAmmo, maxAmmo);
    }
}
