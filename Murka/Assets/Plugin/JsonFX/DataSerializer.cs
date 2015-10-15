using System.IO;
using System.Text;
using UnityEngine;
using JsonFx.Json;

public static class DataSerializer
{
  public static bool Serialize(object data, string path)
  {
    if (data == null)
      return false;

    string json = JsonFXSerializeData(data).ToString();

    if (json.Length <= 2 || !SaveTxtFile(path, json))
    {
      Debug.LogWarning(string.Format("DataSerializer::Serialize failed to serialize '{0}' data", path));
      return false;
    }

    return true;
  }

  public static T Deserialize<T>(string path)
  {
    string json = LoadTxtFile(path);
    if (string.IsNullOrEmpty(json))
    {
      Debug.LogWarning(string.Format("DataSerializer::Deserialize failed to deserialize '{0}' data", path));
      return default(T);
    }

    var data = JsonFXDeserializeData<T>(json);

    return data;
  }

  private static bool SaveTxtFile(string path, string text)
  {
    FileInfo fi = new FileInfo(path);
    if (!fi.Directory.Exists)
      fi.Directory.Create();

    using (StreamWriter stream = new StreamWriter(path, false, Encoding.Unicode))
    {
      stream.Write(text);
      stream.Close();
    }

    return true;
  }

  private static string LoadTxtFile(string path)
  {
    FileInfo fi = new FileInfo(path);
    if (!fi.Exists)
      return string.Empty;

    string text;
    using (StreamReader stream = new StreamReader(path, Encoding.Unicode))
    {
      text =  stream.ReadToEnd();
      stream.Close();
    }

    return text;
  }

  public static void RemoveFile(string path)
  {
    FileInfo fi = new FileInfo(path);
    if (fi.Exists)
      fi.Delete();
  }

  private static T JsonFXDeserializeData<T>(string json)
  {
    //Controls the deserialization settings for JsonReader
    JsonReaderSettings readSettings = new JsonReaderSettings();
    //Reader for consuming JSON data
    JsonReader reader = new JsonReader(json, readSettings);

    //Convert from JSON string to Object graph of specific Type
    T deserialized = reader.Deserialize<T>();

    return deserialized;
  }

  private static StringBuilder JsonFXSerializeData(object serializable)
  {
    //Controls the serialization settings for JsonWriter
    JsonWriterSettings writeSettings = new JsonWriterSettings();
    //Gets and sets if JSON will be formatted for human reading
    writeSettings.PrettyPrint = true;
    //Represents a mutable string of characters
    StringBuilder output = new StringBuilder();
    //Writer for producing JSON data
    JsonWriter writer = new JsonWriter(output, writeSettings);

    //Producing JSON data into StringBuilder
    writer.Write(serializable);

    return output;
  }
}
