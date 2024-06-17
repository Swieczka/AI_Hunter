using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Inworld;
using Inworld.Interactions;
using Inworld.Sample;
using UnityEngine.SceneManagement;

public class DialogueManager : SingletonBehavior<DialogueManager>
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI QuestionPanel;
    [SerializeField] public TextMeshProUGUI ResponsePanel;
    [TextArea]
    public List<string> currentResponse = new();
    [SerializeField] private List<Button> questionButtons = new();
    [SerializeField] private List<TMP_Text> questionFields = new();
    [SerializeField] private Image PortaitImage = null;
    [SerializeField] private TMP_Text CharacterName = null, CharacterAge = null, CharacterStatus = null, CharacterJob = null, CharacterCharacter = null;
    [SerializeField] private TMP_Text Objective = null;
    [SerializeField] private GameObject FinishConversationButton = null;
    public Image BlackImage = null;
    public Animator anim = null;
    public GameObject LoadingObject = null;

    [Header("Characters")]
    [SerializeField] private CharacterData tutorialCharacter = null;
    [SerializeField] private List<CharacterData> characters = new();
    [SerializeField] private CharacterData currentCharacter = null;
    [SerializeField] private List<CharacterData> characterDatas = new();
    public DialogueLine currentDialogue;
    private Queue<IEnumerator> coroutineQueue = new();

    private int numberOfQuestionsAsked = 0;
    private int numberOfCharactersLoaded = 0;

    private void Awake()
    {
        foreach (CharacterData c in characters)
        {
            c.ConversationLogsQuestions.Clear();
            c.ConversationLogsResponses.Clear();
        }
        GameManager.Instance.AvailableCharacter = new List<CharacterData>(characters);
        tutorialCharacter.ConversationLogsQuestions.Clear();
        tutorialCharacter.ConversationLogsResponses.Clear();
        
    }

    void Start()
    {
        AudioManager.Instance.musicSource.Stop();
        StartCoroutine(CoroutineCoordinator());
        StartSetup();
    }
    private void StartSetup()
    {
        coroutineQueue.Enqueue(BootIn());
        coroutineQueue.Enqueue(FadeIn());
        LockButtons();
    }

    private IEnumerator BootIn()
    {
        LoadingObject.SetActive(true);
        AudioManager.Instance.PlaySFX("Boot2");
        while (AudioManager.Instance.sfxSource.isPlaying)
        {
            yield return null;
        }
        AudioManager.Instance.PlayMusic("Gameplay");
    }
    private void SelectCharacterToLoad()
    {
        //LoadCharacter(characters[0]);
         if (GameManager.Instance.IsTutorial)
         {
             LoadCharacter(tutorialCharacter);
         }
         else
         {
            //LoadCharacter(characters[0]);
              numberOfCharactersLoaded++;
            LoadCharacter(GameManager.Instance.AvailableCharacter[Random.Range(0, GameManager.Instance.AvailableCharacter.Count)]);
         }
         
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // Debug.Log(coroutineQueue.Count);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //  selectNewDialogueOption();
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
    public void Skip()
    {

    }

    public void ActivateTypeWriter(string text, TMP_Text panel, float delay = 0)
    {
        if (delay > 0) coroutineQueue.Enqueue(EffectTyping(delay));
        coroutineQueue.Enqueue(EffectTypeWriter(text, panel, 0));
    }

    public void ActivateTypeWriter(int index, bool final = false)
    {
        //if (isActivePlay) return;
        //isActivePlay = true;
        //StartCoroutine(EffectTypeWriter(currentResponse[index], responsePanel));

        coroutineQueue.Enqueue(EffectTypeWriter(currentResponse[index], ResponsePanel));

    }

    private IEnumerator EffectTypeWriter(string text, TMP_Text textField, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.PlaySFXOnLoop();
        foreach (char c in text.ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(0.01f);
        }
        AudioManager.Instance.StillSpeaking = false;
        //Debug.Log($"ITEMS IN QUEUE {coroutineQueue.Count}        Time: {Time.time}");
        if (coroutineQueue.Count == 0)
        {
            coroutineQueue.Enqueue(UnlockAvailableButtons());
        }
    }

    private IEnumerator EffectTyping(float delay)
    {
        float time = 0f;
        int index = 0;
        while (time < delay)
        {
            if (index == 0)
                ResponsePanel.text = "";
            else
                ResponsePanel.text += ".";
            index++;
            index %= 4;
            yield return new WaitForSeconds(0.35f);
            time += 0.35f;
        }
        ResponsePanel.text = "";
        yield return null;
    }

    private void LockButtons()
    {
        FinishConversationButton.SetActive(false);
        foreach (Button b in questionButtons)
        {
            b.interactable = false;
        }
    }

    private void UnlockButtons()
    {
        foreach (Button b in questionButtons)
        {
            b.interactable = true;
        }
    }

    private IEnumerator UnlockAvailableButtons()
    {
        currentCharacter.ConversationLogsQuestions.Add(QuestionPanel.text);
        currentCharacter.ConversationLogsResponses.Add(ResponsePanel.text);
        if (numberOfQuestionsAsked >= 3)
        {
            UnlockFinishConversationButton();
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < questionButtons.Count; i++)
        {
            if (currentCharacter.Dialogues.Count - 1 >= i && currentCharacter.Dialogues[i] != null && !currentCharacter.Dialogues[i].asked) questionButtons[i].interactable = true;
        }
        yield return null;
    }

    public void AskQuestion(int index)
    {
        LockButtons();
        AudioManager.Instance.PlaySFX("Button2");
        numberOfQuestionsAsked++;
        QuestionPanel.text = "";
        ResponsePanel.text = "";
        currentCharacter.Dialogues[index].asked = true;
        currentDialogue = currentCharacter.Dialogues[index];
        ActivateTypeWriter(currentDialogue.questionText, QuestionPanel,0.25f);
        //currentCharacter.ConversationLogs.Add(currentDialogue.questionText);

        if (!currentCharacter.IsAI)
        {
            ActivateTypeWriter(currentDialogue.response, ResponsePanel, 1.5f);
            // currentCharacter.ConversationLogs.Add(currentDialogue.response);
            // coroutineQueue.Enqueue(UnlockAvailableButtons());
        }
        else
        {
            if (!InworldController.CurrentCharacter)
            {
                Debug.Log("no character");
                return;
            }
            try
            {
                if (InworldController.CurrentCharacter)
                {
                    InworldController.CurrentCharacter.SendText(currentDialogue.questionText);
                    coroutineQueue.Enqueue(EffectTyping(1.5f));
                }
            }
            catch (InworldException e)
            {
                InworldAI.LogWarning($"Failed to send text: {e}");
            }
        }

    }

    private void LoadCharacter(CharacterData c)
    {
        numberOfQuestionsAsked = 0;
        currentCharacter = c;
        if(!GameManager.Instance.IsTutorial) characterDatas.Add(c);
        foreach (DialogueLine d in currentCharacter.Dialogues)
        {
            d.asked = false;
        }
        for (int i = 0; i < questionFields.Count; i++)
        {
            if (currentCharacter.Dialogues.Count - 1 >= i && currentCharacter.Dialogues[i] != null)
            {
                questionFields[i].text = currentCharacter.Dialogues[i].questionText;
                questionFields[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                questionFields[i].text = "";
                questionFields[i].GetComponent<Button>().interactable = false;
            }
        }
        foreach (Button b in questionButtons)
        {
            b.gameObject.SetActive(true);
        }
        PortaitImage.sprite = currentCharacter.Portait;
        CharacterName.text = "NAME: " + currentCharacter.characterData.CharacterName.ToUpper();
        CharacterAge.text = "AGE: " + currentCharacter.characterData.Age.ToUpper();
        CharacterStatus.text = "STATUS: " + currentCharacter.characterData.Status.ToUpper();
        CharacterJob.text = "JOB: " + currentCharacter.characterData.Job.ToUpper();
        CharacterCharacter.text = "CHARACTER: " + currentCharacter.characterData.Character.ToUpper();
        Objective.text = currentCharacter.Objective.ToUpper();
        QuestionPanel.text = "";
        ResponsePanel.text = "";
        //UnlockButtons();
        GameManager.Instance.AvailableCharacter.Remove(currentCharacter);
    }

    private void UnlockFinishConversationButton()
    {
        foreach (Button b in questionButtons)
        {
            b.gameObject.SetActive(false);
        }
        FinishConversationButton.SetActive(true);
    }

    public void FinishConversation()
    {
        AudioManager.Instance.PlaySFX("Button2");
        if (GameManager.Instance.IsTutorial)
        {
            GameManager.Instance.IsTutorial = false;
        }
        else
        {
            GameManager.Instance.LogsData.logsData.FillCharacterInfo(
                numberOfCharactersLoaded,
                currentCharacter.characterData.CharacterName,
                currentCharacter.ConversationLogsQuestions[0],
                currentCharacter.ConversationLogsResponses[0],
                currentCharacter.ConversationLogsQuestions[1],
                currentCharacter.ConversationLogsResponses[1],
                currentCharacter.ConversationLogsQuestions[2],
                currentCharacter.ConversationLogsResponses[2]
                );
            
        }
        if (numberOfCharactersLoaded < 3)
        {
            coroutineQueue.Enqueue(FadeOut());
            StartSetup();
        }
        else
        {
            GameManager.Instance.AvailableCharacter = new List<CharacterData>(characterDatas);
            coroutineQueue.Enqueue(FadeOut());
            coroutineQueue.Enqueue(LoadNextScene());
            // GameManager.Instance.LogsData.SubmitData();
            // Debug.Log("GAME OVER");
        }
        
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(4);
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
        SelectCharacterToLoad();
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("FadeIn");
        LoadingObject.SetActive(false);
        yield return new WaitUntil(() => BlackImage.color.a == 0);
    }
}
