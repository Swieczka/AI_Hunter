using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inworld.UI;
using TMPro;
public class testChatBubble : ChatBubble
{


    public override void SetBubbleWithPacketInfo(string charName, string interactionID, string correlationID, Texture2D thumbnail = null, string text = null, bool isFinal = false)
    {
        base.SetBubbleWithPacketInfo(charName, interactionID, correlationID, thumbnail, text);
    }

    public override void AttachBubble(string text)
    {
        base.AttachBubble(text);
        
    }


    
}
