﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    public static void SavePlayer(mainHandler handler)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ScrambledSaves.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(handler);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerShop(shopHandler handler)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ScrambledSaves.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(handler);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveOptions(optionsHandler handler)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ScrambledSettings.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerOptions data = new PlayerOptions(handler);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/ScrambledSaves.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            print(path);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            print("save file didn't exist");
            return new PlayerData();
        }
    }

    public static PlayerOptions LoadOptions()
    {
        string path = Application.persistentDataPath + "/ScrambledSettings.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            print(path);

            PlayerOptions data = formatter.Deserialize(stream) as PlayerOptions;
            stream.Close();

            return data;
        }
        else
        {
            print("options file didn't exist");
            return new PlayerOptions();
        }
    }

}
