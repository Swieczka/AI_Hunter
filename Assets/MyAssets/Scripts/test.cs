using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inworld.Entities;
using Inworld.Packet;
using Inworld.UI;
using System;
using System.Linq;
using Inworld.Sample;


public class test : ChatPanel
{
    

    protected override void DoSomethingWithText(string text, bool isNew = false, bool isFinal = false)
    {
        if (isNew) DialogueManager.Instance.ResponsePanel.text = "";
        DialogueManager.Instance.currentResponse.Add(text);
        int i = DialogueManager.Instance.currentResponse.IndexOf(text);
        //if (isFinal) Debug.Log("FINAL");
        DialogueManager.Instance.ActivateTypeWriter(i, isFinal);
       // if(DialogueManager.Instance.interactions.Count > 0) DialogueManager.Instance.interactions[0].SkipCurrentUtterance();
    }
}
