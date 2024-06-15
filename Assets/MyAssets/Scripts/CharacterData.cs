using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    [System.Serializable]
    public class MetricData
    {
        public string CharacterName, Age, Status, Job, Character;
    } 
    
    public MetricData characterData = new();
    public List<DialogueLine> Dialogues = new();
    public Sprite Portait = null;
    public string Objective = "";
    public bool IsAI = false;
    public bool IsSelected = false;

    [TextArea]
    public List<string> ConversationLogsQuestions = new();

    [TextArea]
    public List<string> ConversationLogsResponses = new();
}

[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string questionText;
    [TextArea]
    public string response;
    public bool asked = false;
}