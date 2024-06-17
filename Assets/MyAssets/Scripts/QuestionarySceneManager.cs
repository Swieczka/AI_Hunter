using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionarySceneManager : MonoBehaviour
{
    public Image BlackImage = null;
    public Animator anim = null;
    private Queue<IEnumerator> coroutineQueue = new();

    public Button ContinueButton = null, SkipButton = null;
    public TMP_Text AnswerField = null, TextPanel = null, ButtonText = null, ButtonSkipText = null;
    public TMP_InputField inputField = null;

    [TextArea]
    public string IntroText, Question1, Question2, Question3;

    private int questionCounter = 0;

    private void Start()
    {
        ContinueButton.interactable = false;
        ButtonText.enabled = false;
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(LockButtons());
        coroutineQueue.Enqueue(FadeIn());
        coroutineQueue.Enqueue(EffectTypeWriter(IntroText));
        coroutineQueue.Enqueue(UnlockButtons(true));
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
        yield return new WaitForSeconds(0.25f);
        AudioManager.Instance.PlaySFXOnLoop();
        foreach (char c in text.ToCharArray())
        {
            TextPanel.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        AudioManager.Instance.StillSpeaking = false;
        ContinueButton.interactable = true;
        ButtonText.enabled = true;
    }

    public void Continue()
    {
        AudioManager.Instance.PlaySFX("Button2");
        questionCounter++;
        if(questionCounter > 3)
        {
            GameManager.Instance.LogsData.logsData.Feedback_3 = AnswerField.text;
            Skip();
        }
        else
        {
            switch(questionCounter)
            {
                case 1:
                    AnswerField.text = "";
                    inputField.text = "";
                    Question1 = Question1.Replace("{character_name}", GameManager.Instance.LogsData.logsData.SelectedName);
                    coroutineQueue.Enqueue(LockButtons());
                    coroutineQueue.Enqueue(EffectTypeWriter(Question1));
                    coroutineQueue.Enqueue(UnlockButtons());
                    break;
                case 2:
                    GameManager.Instance.LogsData.logsData.Feedback_1 = AnswerField.text;
                    AnswerField.text = "";
                    inputField.text = "";
                    
                    coroutineQueue.Enqueue(LockButtons());
                    coroutineQueue.Enqueue(EffectTypeWriter(Question2));
                    coroutineQueue.Enqueue(UnlockButtons());
                    break;
                case 3:
                    GameManager.Instance.LogsData.logsData.Feedback_2 = AnswerField.text;
                    AnswerField.text = "";
                    inputField.text = "";
                    
                    coroutineQueue.Enqueue(LockButtons());
                    coroutineQueue.Enqueue(EffectTypeWriter(Question3));
                    coroutineQueue.Enqueue(UnlockButtons());
                    ButtonText.text = "SEND & FINISH";
                    break;
            }
        }
    }

    public void Skip()
    {
        GameManager.Instance.LogsData.SubmitData();
        coroutineQueue.Enqueue(FadeOut());
        coroutineQueue.Enqueue(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator LockButtons()
    {
        ContinueButton.gameObject.SetActive(false);
        SkipButton.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator UnlockButtons(bool firstTime = false)
    {
        if(firstTime)
        {
            ContinueButton.gameObject.SetActive(true);
            SkipButton.gameObject.SetActive(true);
        }
        else
        {
            ContinueButton.gameObject.SetActive(true);
        }
        yield return null;
    }
}
