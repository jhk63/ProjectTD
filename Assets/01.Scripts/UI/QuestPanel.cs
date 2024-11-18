using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public GameObject textObject;
    private TMP_Text questText;

    private NPC npc;
    private int enemyLength;
    private bool isBattle = false;

    private void Awake()
    {
        questText = textObject.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!isBattle) return;

        questText.SetText("좀비 {0}/{1}마리 처치하기", enemyLength - npc.CountActiveEnemies(), enemyLength);
    }

    public void SetText(string text)
    {
        isBattle = false;
        questText.text = text;
    }

    public void SetBattleText(GameObject gameObject)
    {
        npc = gameObject.GetComponent<NPC>();

        enemyLength = npc.GetEnemyLength();
        isBattle = true;
    }

    public void SetActivate(bool active)
    {
        gameObject.SetActive(active);
    }
}
