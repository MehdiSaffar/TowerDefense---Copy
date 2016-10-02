using UnityEngine;
using FullSerializer;
using System;
using System.IO;

[Serializable]
public class PlayerData
{
    public int lastUnlockedLevelId = 1;
}

public class PlayerController : MonoBehaviour {
    public PlayerData data;
    private int money = 300;
    public int Money {
        get {
            return money;
        }
        set {
            if (value < 0) money = 0;
            else money = value;
            EventManager.TriggerMoneyUpdate(money);
        }
    }

    //------------- Player data Load/Save -------------//
    public bool Load(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("You have not specified a player data filename.");
            return false;
        }
        string path = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(path))
        {
            Debug.LogError("The player data file @" + path + " does not exist!");
            return false;
        }
        StreamReader reader = new StreamReader(path);
        fsData fsData = fsJsonParser.Parse(reader.ReadToEnd());
        fsSerializer fs = new fsSerializer();
        fsResult result = fs.TryDeserialize(fsData, ref data);
        if (result.Failed)
        {
            Debug.LogError("Failed to load player data. Reason:");
            Debug.LogError(result.RawMessages);
            return false;
        }
        reader.Close();

        Debug.Log("Successfully loaded player data!");
        return true;
    }
    public bool Save(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("You have not specified a player data filename to save to.");
            return false;
        }
        string path = Application.persistentDataPath + "/" + filename;

        fsData fsData = new fsData();
        fsSerializer fs = new fsSerializer();
        fsResult result = fs.TrySerialize(data, out fsData);
        if (result.Failed)
        {
            Debug.LogError("Failed to serialize player data. Reason:");
            Debug.LogError(result.RawMessages);
            return false;
        }
        StreamWriter writer = new StreamWriter(path);
        writer.Write(fsJsonPrinter.PrettyJson(fsData));
        writer.Close();

        Debug.Log("Successfully loaded player data!");
        return true;
    }

    public bool CanBuy(int cost) {
        return (Money - cost) >= 0;
    }
    public void Buy(int cost) {
        Money -= cost;
    }
}
