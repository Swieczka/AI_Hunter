using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DesicionSceneManager : MonoBehaviour
{
    [System.Serializable]
    public class Metrics
    {
        public Image PortraitImage = null;
        [SerializeField] public TMP_Text CharacterName = null, CharacterAge = null, CharacterStatus = null, CharacterJob = null, CharacterCharacter = null;
    }

    [SerializeField] private GameObject SummaryPanel = null, ChoicePanel = null;

    [SerializeField] private List<TMP_Text> questionFields = new();
    public Metrics SummaryMetrics;
    public List<Metrics> ChoicesMetrics = new();
    public Image BlackImage = null;
    public Animator anim = null;
    private Queue<IEnumerator> coroutineQueue = new();
    public List<Button> DecisionButtons = new();
    public Button FinishButton = null;
    public Sprite SelectedButtonSprite=null, DefaultButtonSprite=null;

    [SerializeField] private int charIndex = 0;

    private void Start()
    {
        LoadChoices();
        StartCoroutine(CoroutineCoordinator());
        coroutineQueue.Enqueue(FadeIn());
    }
    public void NextCharacter()
    {
        charIndex++;
        if (charIndex > 2) charIndex = 0;
        LoadCharacter(GameManager.Instance.AvailableCharacter[charIndex]);
    }

    public void PrevCharacter()
    {
        charIndex--;
        if (charIndex < 0) charIndex = 2;
        LoadCharacter(GameManager.Instance.AvailableCharacter[charIndex]);
    }

    private void LoadCharacter(CharacterData c)
    {
        for (int i = 0; i < questionFields.Count; i++)
        {
            questionFields[i].text = c.ConversationLogsQuestions[i] + "\n\n" + c.ConversationLogsResponses[i];
        }
        SummaryMetrics.PortraitImage.sprite = c.Portait;
        SummaryMetrics.CharacterName.text = "NAME: " + c.characterData.CharacterName.ToUpper();
        SummaryMetrics.CharacterAge.text = "AGE: " + c.characterData.Age.ToUpper();
        SummaryMetrics.CharacterStatus.text = "STATUS: " + c.characterData.Status.ToUpper();
        SummaryMetrics.CharacterJob.text = "JOB: " + c.characterData.Job.ToUpper();
        SummaryMetrics.CharacterCharacter.text = "CHARACTER: " + c.characterData.Character.ToUpper();
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
        LoadCharacter(GameManager.Instance.AvailableCharacter[charIndex]);
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("FadeIn");
        yield return new WaitUntil(() => BlackImage.color.a == 0);

    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(5);
    }

    public void CloseSummary()
    {
        SummaryPanel.SetActive(false);
        ChoicePanel.SetActive(true);
    }

    private void LoadChoices()
    {
        FinishButton.interactable = false;
        for(int i=0; i<=2;i++)
        {
            CharacterData.MetricData m = GameManager.Instance.AvailableCharacter[i].characterData;
            ChoicesMetrics[i].CharacterName.text = "NAME: " + m.CharacterName.ToUpper();
            ChoicesMetrics[i].CharacterJob.text = "JOB: " + m.Job.ToUpper();
            ChoicesMetrics[i].CharacterStatus.text = "STATUS: " + m.Status.ToUpper();
            ChoicesMetrics[i].CharacterAge.text = "AGE: " + m.Age.ToUpper();
            ChoicesMetrics[i].CharacterCharacter.text = "CHARACTER: " + m.Character.ToUpper();
            ChoicesMetrics[i].PortraitImage.sprite = GameManager.Instance.AvailableCharacter[i].Portait;
        }
    }

    public void SelectRogueAI(int index)
    {
        foreach(CharacterData c in GameManager.Instance.AvailableCharacter)
        {
            c.IsSelected = false;
        }
        GameManager.Instance.AvailableCharacter[index].IsSelected = true;
        foreach (Button b in DecisionButtons)
        {
            b.interactable = false;
            b.GetComponent<Image>().sprite = DefaultButtonSprite;
        }
        DecisionButtons[index].GetComponent<Image>().sprite = SelectedButtonSprite;
        FinishButton.interactable = true;
    }

    public void Deselect()
    {
        foreach (CharacterData c in GameManager.Instance.AvailableCharacter)
        {
            c.IsSelected = false;
        }
        foreach(Button b in DecisionButtons)
        {
            b.interactable = true;
            b.GetComponent<Image>().sprite = DefaultButtonSprite;
        }
        FinishButton.interactable = false;
    }

    public void FinishGame()
    {
        foreach (CharacterData c in GameManager.Instance.AvailableCharacter)
        {
            if(c.IsSelected)
            {
                if (c.IsAI)
                    GameManager.Instance.LogsData.logsData.Score = "Success";
                else
                    GameManager.Instance.LogsData.logsData.Score = "Failure";

                switch(c.characterData.CharacterName)
                {
                    case "Mark":
                        GameManager.Instance.LogsData.logsData.Mark_Selection = "Selected";
                        GameManager.Instance.LogsData.logsData.SelectedName = "Mark";
                        break;
                    case "Beth":
                        GameManager.Instance.LogsData.logsData.Beth_Selection = "Selected";
                        GameManager.Instance.LogsData.logsData.SelectedName = "Beth";
                        break;
                    case "David":
                        GameManager.Instance.LogsData.logsData.David_Selection = "Selected";
                        GameManager.Instance.LogsData.logsData.SelectedName = "David";
                        break;
                }
            }
        }
        coroutineQueue.Enqueue(FadeOut());
        coroutineQueue.Enqueue(LoadNextScene());
    }
}
 