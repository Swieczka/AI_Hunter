using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    public TMP_Text textField = null;
    [TextArea]
    public List<string> texts_1 = new();
    [TextArea]
    public List<string> texts_2 = new();

    private Queue<IEnumerator> coroutineQueue = new();

    public Image BlackImage = null;
    public Animator anim = null;

    private bool canGoNext = false;
    private bool canGoNextScene = false;
    private int index = 0;

    private void Start()
    {
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(FadeIn(1f));
        coroutineQueue.Enqueue(EffectTypeWriter(texts_1[0]));

        coroutineQueue.Enqueue(EffectTypeWriter(texts_1[1]));

        coroutineQueue.Enqueue(EffectTypeWriter(texts_1[2], true));

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
        anim.SetTrigger("FadeOut");
        yield return new WaitUntil(() => BlackImage.color.a == 1);
    }

    private IEnumerator FadeIn(float delay)
    {
        // yield return new WaitForSeconds(delay);
        textField.text = "";
        anim.SetTrigger("FadeIn");
        yield return new WaitUntil(() => BlackImage.color.a == 0);

    }

    private IEnumerator EffectTypeWriter(string text, bool next = false)
    {
        textField.text = "";
        //textField.text = text;
        foreach (char c in text.ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        if (next)
        {
            if (index == 0) canGoNext = true;
            else if (index == 1) canGoNextScene = true;
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator NextScene()
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitUntil(() => BlackImage.color.a == 1);
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if (canGoNext && Input.anyKeyDown)
        {
            canGoNext = false;
            index++;
            coroutineQueue.Enqueue(FadeOut());
            coroutineQueue.Enqueue(FadeIn(1f));

            coroutineQueue.Enqueue(EffectTypeWriter(texts_2[0]));

            coroutineQueue.Enqueue(EffectTypeWriter(texts_2[1]));

            coroutineQueue.Enqueue(EffectTypeWriter(texts_2[2], true));
        }
        else if (canGoNextScene && Input.anyKeyDown)
        {
            canGoNextScene = false;
            coroutineQueue.Enqueue(NextScene());
        }
    }
}
