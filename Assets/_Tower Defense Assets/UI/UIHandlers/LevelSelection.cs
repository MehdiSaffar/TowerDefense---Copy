using System;
using UnityEngine;

namespace UI
{
    public class LevelSelection : UIElement
    {
        public delegate void OnLevelItemClick(int levelId, bool locked);
        public event OnLevelItemClick LevelItemClick;

        public void TriggerLevelItemClick(int levelId)
        {
            if (LevelItemClick != null)
            {
                LevelItemClick(levelId, levelItems[levelId - 1].locked);
            }
        }

        public LevelItem[] levelItems;

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
                levelItems[i].locked = i >= GameManager.Player.data.lastUnlockedLevelId;
            }
        }
    }
}