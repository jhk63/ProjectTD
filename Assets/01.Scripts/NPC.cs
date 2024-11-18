using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject DiaCam;
    [SerializeField] private GameObject[] enemies;

    private Animator animator;
    private Dialogue dialogue;

    private bool isTalk = false;

    private int enemyLength = 0;
    public int GetEnemyLength() { return enemyLength; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        dialogue = GetComponent<Dialogue>();
    }

    public void OnInit()
    {
        enemyLength = CountActiveEnemies();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isTalk) return;

            StageClear();
        }
    }

    public void OnClear()
    {
        animator.SetTrigger("talk");
        DiaCam.SetActive(true);
        dialogue.StartDialogue();
    }

    private void StageClear()
    {
        if (CountActiveEnemies() <= 0)
        {
            isTalk = true;
            OnClear();
        }
    }

    public int CountActiveEnemies()
    {
        int activeCount = 0;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                activeCount++;
            }
        }

        return activeCount;
    }

    public void OnDialogueEnd()
    {
        DiaCam.SetActive(false);
    }
}
