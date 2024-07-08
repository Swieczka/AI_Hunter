using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendLogsData : MonoBehaviour
{
    public LogsDataInfo logsData = new();

    private string formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScfMryEyAwTVameTnp_phdEvrdWPg38XpFSswtsfpyDcGNwAA/formResponse";

    private void Start()
    {
        logsData.sessionID = System.Guid.NewGuid().ToString();
    }
    public void SubmitData()
    {
        StartCoroutine(PostData(logsData));
    }

    public void ClearData()
    {
        logsData = new();
    }
    private IEnumerator PostData(LogsDataInfo dataInfo)
    {
        WWWForm form = new WWWForm();
        /* id = entry.542467327 -
         * 
        name1 = entry.1445918763 -
        q1_1 = entry.1190943548 -
        r1_1 = entry.53480203 -
        q1_2 = entry.1308521743 -
        r1_2 = entry.96753211 -
        q1_3 = entry.1992644914 -
        r1_3 = entry.2056168861 -

        name2 = entry.532510113 -
        q2_1 = entry.1778083957 -
        r2_1 = entry.1968659028 -
        q2_2 = entry.1958257050 -
        r2_2 = entry.76728523 -
        q2_3 = entry.1565940047 -
        r2_3 = entry.2108117064 -

        name3 = entry.2064632221 -
        q3_1 = entry.1295338612 -
        r3_1 = entry.1694449279 -
        q3_2 = entry.1727114526 -
        r3_2 = entry.1236335991 -
        q3_3 = entry.1908367869 -
        r3_3 = entry.1279056349 -

        order - entry.1700068582

        david selection - entry.445937372
        beth selection - entry.663104300
        mark selection - entry.913910474
        score - entry.87966159
        feedback_1 - entry.1461599095
        feedback_2 - entry.1682600286
        feedback_3 - entry.156359541
        */
        form.AddField("entry.542467327", dataInfo.sessionID);
        #region c1
        form.AddField("entry.1445918763", dataInfo.c1.name);
        form.AddField("entry.1190943548", dataInfo.c1.q1);
        form.AddField("entry.53480203", dataInfo.c1.r1);
        form.AddField("entry.1308521743", dataInfo.c1.q2);
        form.AddField("entry.96753211", dataInfo.c1.r2);
        form.AddField("entry.1992644914", dataInfo.c1.q3);
        form.AddField("entry.2056168861", dataInfo.c1.r3);
        #endregion

        #region c2
        form.AddField("entry.532510113", dataInfo.c2.name);
        form.AddField("entry.1778083957", dataInfo.c2.q1);
        form.AddField("entry.1968659028", dataInfo.c2.r1);
        form.AddField("entry.1958257050", dataInfo.c2.q2);
        form.AddField("entry.76728523", dataInfo.c2.r2);
        form.AddField("entry.1565940047", dataInfo.c2.q3);
        form.AddField("entry.2108117064", dataInfo.c2.r3);
        #endregion

        #region c3
        form.AddField("entry.2064632221", dataInfo.c3.name);
        form.AddField("entry.1295338612", dataInfo.c3.q1);
        form.AddField("entry.1694449279", dataInfo.c3.r1);
        form.AddField("entry.1727114526", dataInfo.c3.q2);
        form.AddField("entry.1236335991", dataInfo.c3.r2);
        form.AddField("entry.1908367869", dataInfo.c3.q3);
        form.AddField("entry.1279056349", dataInfo.c3.r3);
        #endregion

        #region data
        form.AddField("entry.1700068582", dataInfo.CharacterOrder);
        form.AddField("entry.445937372", dataInfo.David_Selection);
        form.AddField("entry.663104300", dataInfo.Beth_Selection);
        form.AddField("entry.913910474", dataInfo.Mark_Selection);
        form.AddField("entry.87966159", dataInfo.Score);
        form.AddField("entry.1461599095", dataInfo.Feedback_1);
        form.AddField("entry.1682600286", dataInfo.Feedback_2);
        form.AddField("entry.156359541", dataInfo.Feedback_3);
        #endregion

        using ( UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();

            if(www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Submit success");
            }
            else
            {
                Debug.LogError("Error in submission: " + www.error);
            }
        }
    }
}
[System.Serializable]
public class LogsDataInfo
{
    [System.Serializable]
    public class Character
    {
        public string name="", q1="", r1 = "", q2 = "", r2 = "", q3 = "", r3 = "";
    }
    public string sessionID = "";
    public Character c1, c2, c3; //David, Beth, Mark
    public string CharacterOrder = "";
    public string David_Selection = "", Beth_Selection = "", Mark_Selection = "";
    public string Score = "";
    public string Feedback_1 = "";
    public string Feedback_2 = "";
    public string Feedback_3 = "";
    public string SelectedName = "";

    public void FillCharacterInfo(int i, string name, string q1, string r1, string q2, string r2, string q3, string r3)
    {
        switch(name)
        {
            case "David":
                break;
                CharacterOrder += "D";
                c1.name = name;
                c1.q1 = q1;
                c1.r1 = r1;
                c1.q2 = q2;
                c1.r2 = r2;
                c1.q3 = q3;
                c1.r3 = r3;
                
            case "Beth":
                break;
                CharacterOrder += "B";
                c2.name = name;
                c2.q1 = q1;
                c2.r1 = r1;
                c2.q2 = q2;
                c2.r2 = r2;
                c2.q3 = q3;
                c2.r3 = r3;
                //break;
            case "Mark":
                CharacterOrder += "M";
                c3.name = name;
                c3.q1 = q1;
                c3.r1 = r1;
                c3.q2 = q2;
                c3.r2 = r2;
                c3.q3 = q3;
                c3.r3 = r3;
                break;
        }
    }
}