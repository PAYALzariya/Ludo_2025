using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat", FileMode.Open);
            LocalSaveData game = new LocalSaveData();
            game.Username = LocalSaveData.current.Username;
            game.password = LocalSaveData.current.password;
            game.isRememberMe = LocalSaveData.current.isRememberMe;
            bf.Serialize(file, game);
            file.Close();
        }
        PlayerPrefs.SetString("USERNAME", LocalSaveData.current.Username);
        PlayerPrefs.SetString("PASSWORD", LocalSaveData.current.password);
        PlayerPrefs.SetInt("REMEMBER_ME", LocalSaveData.current.isRememberMe == true ? 1 : 0);
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat", FileMode.Open);
            LocalSaveData game = (LocalSaveData)bf.Deserialize(file);
            file.Close();
            LocalSaveData.current.Username = game.Username;
            LocalSaveData.current.password = game.password;
            LocalSaveData.current.isRememberMe = game.isRememberMe;

            /* Ludo_UIManager.instance.assetOfGame.SavedLoginData.Username = LocalSaveData.current.Username;
            Ludo_UIManager.instance.assetOfGame.SavedLoginData.password = game.password;
            Ludo_UIManager.instance.assetOfGame.SavedLoginData.isRememberMe = game.isRememberMe; */
        }
        else
        {
            Debug.Log("File Does Not Exist");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat");
            LocalSaveData game = new LocalSaveData();
            game.Username = LocalSaveData.current.Username;
            game.password = LocalSaveData.current.password;
            game.isRememberMe = LocalSaveData.current.isRememberMe;

            bf.Serialize(file, game);
            Debug.Log("Create file from asset of data" + game.Username);
        }
        /*         Ludo_UIManager.instance.assetOfGame.SavedLoginData.Username = PlayerPrefs.GetString("USERNAME", "");
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.password = PlayerPrefs.GetString("PASSWORD", "");
                Ludo_UIManager.instance.assetOfGame.SavedLoginData.isRememberMe = PlayerPrefs.GetInt("REMEMBER_ME", 0) == 1 ? true : false; */
    }
    public static void DeleteFile()
    {
        PlayerPrefs.SetInt("PokerBetAutoLogin", 0);
        PlayerPrefs.SetString("USERNAME", "");
        PlayerPrefs.SetString("PASSWORD", "");
        PlayerPrefs.DeleteKey("REMEMBER_ME");

        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + Application.productName + "SavedData.dat";

        if (File.Exists(Application.persistentDataPath + "/" + Application.productName + "SavedData.dat"))
        {
            File.Delete(path);
        }
    }
}