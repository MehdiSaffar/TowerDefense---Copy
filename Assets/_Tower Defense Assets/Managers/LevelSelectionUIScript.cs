using System;
using UnityEngine;

public class LevelSelectionUIScript : MonoBehaviour
{
    public delegate void OnLevelItemClick(int levelId, bool locked);
    public event OnLevelItemClick LevelItemClick;

    public void TriggerLevelItemClick(int levelId)
    {
        if (LevelItemClick != null)
        {
            LevelItemClick(levelId, levelItems[levelId - 1].Locked);
        }
    }

    public LevelItemUIScript[] levelItems;

    public void SetUsingLevelSelectionData(LevelSelectionData data)
    {
        for (int i = 0; i < Mathf.Min(data.levels.Count, levelItems.Length); i++)
        {
            bool locked = i >= GameManager.Player.data.lastUnlockedLevelId;
            levelItems[i].Set(data.levels[i].name, data.levels[i].thumbnailTexture, locked);
        }
    }

    public void UpdateLocked()
    {
        for (int i = 0; i < levelItems.Length; i++)
        {
            levelItems[i].Locked = i >= GameManager.Player.data.lastUnlockedLevelId;
        }
    }
}