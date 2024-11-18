using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMove1 : MonoBehaviour
{
    [SerializeField] private GameObject titleCam;
    [SerializeField] private Image fadeImage;

    private float fadeDuration = 0.5f;

    [SerializeField] private Dialogue dialoguePanel;
    [SerializeField] private CarMove carMove;

    [SerializeField] private GameObject target;
    private Animator targetAnimator;
    
    private Animator animator;
    private AudioSource audioSource;

    public bool isStart = false;
    private float speed = 0.5f;

    public int step = 0;

    private void Awake()
    {
        targetAnimator = target.GetComponent<Animator>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        fadeImage.color = new Color(0, 0, 0, 0);
    }
    
    private void LateUpdate()
    {
        if (!isStart) return;
        if (target == null) return;

        if (step == 0)
        {
            Move();
            // if (isDie && Vector3.Distance(transform.position, target.transform.position) <= 1f)
            if (Vector3.Distance(transform.position, target.transform.position) <= 1f)
            {
                audioSource.Play();
                targetAnimator.SetTrigger("OnPunch");
                animator.SetTrigger("Die");
                step++;
            }
        }
        else if (step == 1)
        {
            dialoguePanel.StartDialogue();
            step++;
        }
        else if (step == 2)
        {
            if (dialoguePanel.isEnd == true)
                step++;
        }
        else if (step == 3)
        {
            StartCoroutine(FadeSequence(1f));
            step++;
        }
    }

    private void Move()
    {
        Vector3 destin = target.transform.position;

        transform.position = Vector3.Lerp(transform.position, destin, speed * Time.deltaTime);
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0));
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0 ,0, targetAlpha);
    }

    IEnumerator FadeSequence(float stayDuration)
    {
        // fade in
        yield return StartCoroutine(Fade(1));
        titleCam.SetActive(false);

        yield return new WaitForSeconds(stayDuration);
        
        // fade out
        yield return StartCoroutine(Fade(0));
        carMove.isStart = true;

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("SampleScene");
    }
}
