using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class SaveSystem : MonoBehaviour
{

    private static string optionsFileName = "/gameOptions.txt";

    public static void SaveOptions(SaveOptions options)
    {

        //Binary 
        BinaryFormatter formatter = new();
        string path = Application.persistentDataPath + optionsFileName;
        FileStream stream = new(path, FileMode.Create);
        //SaveOptions data = new(options); //is this needed? could I just pass options into the serializer?
        //formatter.Serialize(stream, data);

        formatter.Serialize(stream,options);
        stream.Close();


        //Start of JSON code
        // string path = Application.persistentDataPath + optionsFileName;
        // string json = JsonUtility.ToJson(options);

        

        Debug.Log("File saved to: " + path);


    }


    public static SaveOptions LoadOptions()
    {
        string path = Application.persistentDataPath + optionsFileName;
        if (File.Exists(path))
        {
            //Binary 
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);
            SaveOptions data = formatter.Deserialize(stream) as SaveOptions;
            stream.Close();
            return data;

        }
        else
        {
            //File not found

            //Create a default settings object instead
            SaveOptions data = new(.50f, .50f);

            return data;
        }
    }


}
