using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Inworld;
using Inworld.Interactions;
using Inworld.Sample;

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

    [Header("Characters")]
    [SerializeField] private CharacterData tutorialCharacter = null;
    [SerializeField] private List<CharacterData> characters = new();
    [SerializeField] private CharacterData currentCharacter = null;
    public DialogueLine currentDialogue;
    private Queue<IEnumerator> coroutineQueue = new();

    private void Awake()
    {
        GameManager.Instance.AvailableCharacter = characters;
        tutorialCharacter.ConversationLogsQuestions.Clear();
        tutorialCharacter.ConversationLogsResponses.Clear();
        foreach(CharacterData c in characters)
        {
            c.ConversationLogsQuestions.Clear();
            c.ConversationLogsResponses.Clear();
        }
    }

    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
        LockButtons();
        SelectCharacterToLoad();
    }

    private void SelectCharacterToLoad()
    {
        if (GameManager.Instance.IsTutorial)
        {
            LoadCharacter(tutorialCharacter);
        }
        else
        {
            LoadCharacter(characters[0]);
            // LoadCharacter(characters[Random.Range(0, GameManager.Instance.AvailableCharacter.Count)]);
            // GameManager.Instance.AvailableCharacter.Remove(currentCharacter);
        }
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(coroutineQueue.Count);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
          //  selectNewDialogueOption();
        }
    }

    void nextDialoguePlay()
    {
        //questionPanel.text = currentDialogue.dialogueText;
        QuestionPanel.text = "";
        ActivateTypeWriter(currentDialogue.questionText, QuestionPanel);
        //responsePanel.text = currentDialogue.response;
        ResponsePanel.text = "";
        ActivateTypeWriter(currentDialogue.response, ResponsePanel,3f);
    }

    public void selectNewDialogueOption()
    {
        coroutineQueue.Clear();
        StopAllCoroutines();
        StartCoroutine(CoroutineCoordinator());

       // currentDialogue = currentDialogue.Dialogues[0];
        nextDialoguePlay();
    }

    

    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while(coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }
    public void Skip()
    {

    }

    public void ActivateTypeWriter(string text, TMP_Text panel, float delay=0)
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
        foreach(char c in text.ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(0.01f);
        }
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
        while(time < delay)
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
        foreach(Button b in questionButtons)
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
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < questionButtons.Count; i++)
        {
            if (currentCharacter.Dialogues.Count-1 >= i && currentCharacter.Dialogues[i] !=null && !currentCharacter.Dialogues[i].asked) questionButtons[i].interactable = true;
        }
        yield return null;
    }

    public void AskQuestion(int index)
    {
        LockButtons();
        QuestionPanel.text = "";
        ResponsePanel.text = "";
        currentCharacter.Dialogues[index].asked = true;
        currentDialogue = currentCharacter.Dialogues[index];
        ActivateTypeWriter(currentDialogue.questionText, QuestionPanel);
        //currentCharacter.ConversationLogs.Add(currentDialogue.questionText);

        if (!currentCharacter.IsAI)
        {
            ActivateTypeWriter(currentDialogue.response, ResponsePanel, 3f);
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
                    coroutineQueue.Enqueue(EffectTyping(3f));
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
        currentCharacter = c;
        foreach(DialogueLine d in c.Dialogues)
        {
            d.asked = false;
        }
        for(int i=0; i<questionFields.Count;i++)
        {
            if (c.Dialogues.Count-1 >= i && c.Dialogues[i] != null)
            {
                questionFields[i].text = c.Dialogues[i].questionText;
                questionFields[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                questionFields[i].text = "";
                questionFields[i].GetComponent<Button>().interactable = false;
            }
        }
        PortaitImage.sprite = c.Portait;
        CharacterName.text = "NAME: "+ c.characterData.CharacterName.ToUpper();
        CharacterAge.text = "AGE: " + c.characterData.Age.ToUpper();
        CharacterStatus.text = "STATUS: " + c.characterData.Status.ToUpper();
        CharacterJob.text = "JOB: " + c.characterData.Job.ToUpper();
        CharacterCharacter.text = "CHARACTER: " + c.characterData.Character.ToUpper();
        Objective.text = c.Objective.ToUpper();
        QuestionPanel.text = "";
        ResponsePanel.text = "";
        //UnlockButtons();
    }

    private void UnlockFinishConversationButton()
    {
        FinishConversationButton.SetActive(true);
    }

    public void FinishConversation()
    {

    }
}
