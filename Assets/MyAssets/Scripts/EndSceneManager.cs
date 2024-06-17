using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public Image BlackImage = null;
    public Animator anim = null;
    private Queue<IEnumerator> coroutineQueue = new();

    public Button ContinueButton = null;
    public TMP_Text EvaluationText = null, TextPanel = null, ButtonText = null;

    [TextArea]
    public string FailureHeadline, FailureText, SuccessHeadline, SuccessText;

    private void Start()
    {
        ContinueButton.interactable = false;
        ButtonText.enabled = false;
        EvaluationText.text = GameManager.Instance.LogsData.logsData.Score == "Success" ? SuccessHeadline : FailureHeadline;
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(FadeIn());
        if(GameManager.Instance.LogsData.logsData.Score == "Success")
        {
            Success();
        }
        else
        {
            Failure();
        }
    }

    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        anim.SetTrigger("Idle");
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("FadeOut");
        yield return new WaitUntil(() => BlackImage.color.a == 1);
    }

    private IEnumerator FadeIn()
    {
        anim.SetTrigger("IdleBlack");
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("FadeIn");
        yield return new WaitUntil(() => BlackImage.color.a == 0);

    }

    private IEnumerator EffectTypeWriter(string text)
    {
        TextPanel.text = "";
        AudioManager.Instance.PlaySFXOnLoop();
        foreach (char c in text.ToCharArray())
        {
            TextPanel.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        AudioManager.Instance.StillSpeaking = false;
        ContinueButton.interactable = true;
        ButtonText.enabled = true;
    }

    private void Success()
    {
        coroutineQueue.Enqueue(EffectTypeWriter(SuccessText));
    }

    private void Failure()
    {
        coroutineQueue.Enqueue(EffectTypeWriter(FailureText));
    }

    public void Continue()
    {
        AudioManager.Instance.PlaySFX("Button2");
        coroutineQueue.Enqueue(FadeOut());
        coroutineQueue.Enqueue(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(6);
    }

}
