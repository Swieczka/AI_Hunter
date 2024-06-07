using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [TextArea]
    public string questionText;
    [TextArea]
    public string response;
    public bool asked = false;
}
