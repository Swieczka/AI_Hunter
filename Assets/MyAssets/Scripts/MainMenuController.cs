using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Inworld;

public class MainMenuController : MonoBehaviour
{
    public Image BlackImage = null;
    public Animator anim = null;

    private void Start()
    {
        if(InworldController.Instance!=null) InworldController.Instance.Disconnect();
        AudioManager.Instance.PlayMusic("Menu");
        GameManager.Instance.AvailableCharacter.Clear();
        GameManager.Instance.LogsData.ClearData();
        GameManager.Instance.LogsData.logsData.sessionID = System.Guid.NewGuid().ToString();
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
