using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game manager is NULL");
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerPrefs.DeleteAll();
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
    }

    public bool IsTutorial = true;
    public List<CharacterData> AvailableCharacter = new();
    public SendLogsData LogsData = null;
    public Texture2D cursorTexture=null;

}
