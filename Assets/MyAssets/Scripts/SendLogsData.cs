using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendLogsData : MonoBehaviour
{
    public LogsDataInfo logsData;

    private string formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScfMryEyAwTVameTnp_phdEvrdWPg38XpFSswtsfpyDcGNwAA/formResponse";

    public void SubmitData()
    {
        StartCoroutine(PostData(logsData));

    }

    private IEnumerator PostData(LogsDataInfo dataInfo)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.542467327", dataInfo.question1 + "\n " + dataInfo.response1);
        form.AddField("entry.1425005152", dataInfo.question2 + "\n " + dataInfo.response2);
        form.AddField("entry.67789171", dataInfo.question3 + "\n " + dataInfo.response3);

        using( UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
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
    public string question1="", question2 = "", question3 = "";
    public string response1="", response2="", response3 = "";
}