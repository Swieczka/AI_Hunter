using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Image BlackImage = null;
    public Animator anim = null;

    private void Start()
    {
        AudioManager.Instance.PlayMusic("Menu");
    }
    public void StartGame()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitUntil(() => BlackImage.color.a == 1);
        SceneManager.LoadScene(1);
    }

    public void ButtonSound()
    {
        AudioManager.Instance.PlaySFX("Button1");
    }
}
