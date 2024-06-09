using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private Queue<IEnumerator> coroutineQueue = new();

    [SerializeField] private TMP_Text textField = null;
    public Image BlackImage = null;
    public Animator anim = null;

    public GameObject Button1 = null, Button2 = null;
    [TextArea]
    public List<string> Conversations = new();
    [TextArea]
    public List<string> ButtonTexts = new();

    [SerializeField] List<Image> UI_Frames = new();
    [SerializeField] List<Sprite> defaultSprites = new(), highlightSprites = new();

    [SerializeField] Color colorFadeIn, colorFadeOut;

    private int frameIndex = 0;
    private int textIndex = 0;

    private void Start()
    {
        DisableButtons();
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(FadeIn());
        coroutineQueue.Enqueue(WaitTime(1f));
        coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
        textIndex++;
    }

    private IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private IEnumerator WaitTimeColor(float time, bool fadeIn)
    {
        if (fadeIn) BlackImage.color = new Color(0,0,0,1);
        else BlackImage.color = new Color(0,0,0,0);
        yield return new WaitForSeconds(time);
    }

    private IEnumerator EffectTypeWriter(string text, string textButton1="", string textButton2="")
    {
         textField.text = "";
        //textField.text = text;
        foreach (char c in text.ToCharArray())
         {
             textField.text += c;
             yield return new WaitForSeconds(0.02f);
         }

        yield return new WaitForSeconds(0.25f);
        if(textButton1 != "")
        {
            SetButtonText(Button1, textButton1);
        }
        if(textButton2 != "")
        {
            SetButtonText(Button2, textButton2);
        }
    }

    private void HighlightFrames(int i)
    {
        UI_Frames[i].sprite = highlightSprites[i];
    }

    private void DefaultFrames(int i)
    {
        UI_Frames[i].sprite = defaultSprites[i];
    }

    private void Update()
    {
    }

    public void Next()
    {
        DisableButtons();
        switch(textIndex)
        {
            case 1:
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
                textIndex++;
                break;
            case 2:
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[1], ButtonTexts[2]));
                textIndex++;
                break;
            case 3:
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
                HighlightFrames(frameIndex);
                textIndex++;
                frameIndex++;
                break;
            case 4:
                DefaultFrames(frameIndex - 1);
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
                HighlightFrames(frameIndex);
                textIndex++;
                frameIndex++;
                break;
            case 5:
                DefaultFrames(frameIndex - 1);
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
                HighlightFrames(frameIndex);
                frameIndex++;
                HighlightFrames(frameIndex);
                frameIndex++;
                textIndex++;
                break;
            case 6:
                DefaultFrames(frameIndex - 2);
                DefaultFrames(frameIndex - 1);
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[0]));
                HighlightFrames(frameIndex);
                textIndex++;
                break;
            case 7:
                DefaultFrames(frameIndex);
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[3], ButtonTexts[4]));
                textIndex++;
                break;
            case 8:
                StartTutorial();
                break;
        }
    }

    public void Skip()
    {
        DisableButtons();
        switch (textIndex)
        {
            case 3:
                textIndex = 8;
                coroutineQueue.Enqueue(EffectTypeWriter(Conversations[textIndex], ButtonTexts[3], ButtonTexts[4]));
                break;
            case 8:
                StartGame();
                break;
        }
    }

    private void SetButtonText(GameObject btn, string text)
    {
        btn.SetActive(true);
        btn.GetComponent<TMP_Text>().text = text;
    }

    private void DisableButtons()
    {
        Button1.SetActive(false);
        Button2.SetActive(false);
    }

    private void StartTutorial()
    {
        coroutineQueue.Enqueue(FadeOut());
        coroutineQueue.Enqueue(LoadScene(true));
    }

    private IEnumerator LoadScene(bool isTutorial)
    {   
        GameManager.Instance.IsTutorial = isTutorial;
        SceneManager.LoadScene(3);
        yield return null;
    }

    private void StartGame()
    {
        coroutineQueue.Enqueue(FadeOut());
        coroutineQueue.Enqueue(LoadScene(false));
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
        textField.text = "";
        anim.SetTrigger("IdleBlack");
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("FadeIn");
        yield return new WaitUntil(() => BlackImage.color.a == 0);

    }
}
