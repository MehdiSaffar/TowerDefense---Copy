using UnityEngine;
using System;
using System.Collections.Generic;
using FullSerializer;
using System.IO;

[Serializable]
public class LevelInfo
{
    public int id = -1;
    public string name = "NO NAME ASSIGNED";
    public string levelPath = "NO LEVEL PATH ASSIGNED.dat";
    public string thumbnailPath = "NO THUMBNAIL ASSIGNED.png";
    [NonSerialized] public Texture2D thumbnailTexture = null;
}
[Serializable]
public class LevelSelectionData
{
    public List<LevelInfo> levels;
}

public class LevelSelectState : MonoBehaviour
{
    public string levelSelectionDataFilename;
    public LevelSelectionData data;

    private UI.LevelSelection UIScript;
    [SerializeField] private AudioClip onLockedLevelClick;

    void Awake()
    {
        data = null;
    }
    void Start()
    {
        data = null;
    }

    private void OnLevelItemClick(int levelId, bool locked)
    {
        LevelInfo levelInfo = data.levels.Find(level => level.id == levelId);
        if(locked)
        {
            Debug.Log("You have not unlocked level " + levelInfo.name + " yet!");
            GameManager.SoundManager.RandomizeFx(onLockedLevelClick);
            return;
        }
        GameManager.LevelManager.LoadLevel(levelInfo.levelPath);
        GameManager.LevelManager.BuildLevel();
        GameManager.Fsm.ChangeState(GameManager.States.Edit);
    }

    public void Enter()
    {
        if (data == null)
        {
            if (!LoadData())
            {
                Debug.LogError("Level selection could not be set up... going back to main menu..");
                GameManager.Fsm.ChangeState(GameManager.States.MainMenu);
                return;
            }
            UIScript.SetUsingLevelSelectionData(data);
        }
        else
        {
            UIScript.UpdateLocked();
        }
        UIScript.LevelItemClick += OnLevelItemClick;
        UIScript.gameObject.SetActive(true);
    }
    public void Exit()
    {
        UIScript.LevelItemClick -= OnLevelItemClick;
        UIScript.gameObject.SetActive(false);
        GameManager.SoundManager.StopMusic();
    }
    public bool LoadData()
    {
        Debug.Log("Sup");
        if (string.IsNullOrEmpty(levelSelectionDataFilename))
        {
            Debug.LogError("You have not specified a level selection data filename.");
            return false;
        }
        string path = Application.persistentDataPath + "/" + levelSelectionDataFilename;
        if(!File.Exists(path))
        {
            Debug.LogError("The level selection file @" + path + " does not exist!");
            return false;
        }
        StreamReader reader = new StreamReader(path);
        fsData fsData = fsJsonParser.Parse(reader.ReadToEnd());
        fsSerializer fs = new fsSerializer();
        fsResult result = fs.TryDeserialize(fsData, ref data);
        if(result.Failed)
        {
            Debug.LogError("Failed to load level selection data. Reason:");
            Debug.LogError(result.RawMessages);
            return false;
        }
        Debug.Log("Successfully loaded level selection data!");
        reader.Close();

        foreach (var level in data.levels)
        {
            string thumbnailPath = Application.persistentDataPath + "/" + level.thumbnailPath;
            if (File.Exists(thumbnailPath))
            {
                level.thumbnailTexture = new Texture2D(512, 512);
                level.thumbnailTexture.LoadImage(File.ReadAllBytes(thumbnailPath));
            }
            else
            {
                Debug.LogError("Could not find thumbnail of level " + level.id + " at " + thumbnailPath);
            }
        }
        return true;
    }
}