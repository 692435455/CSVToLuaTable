using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

public class ExcelToLuaTable : EditorWindow
{
    public string tableDirectoryName = "Table";
    public string luaDirectoryName = "LuaTable";
    public int startDataLine = 1;


    [MenuItem("Tools/ExcelToLuaTable")]
    static void Init()
    {
        ExcelToLuaTable window = (ExcelToLuaTable)GetWindow(typeof(ExcelToLuaTable));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        tableDirectoryName = EditorGUILayout.TextField("TableDirectory Name", tableDirectoryName);
        luaDirectoryName = EditorGUILayout.TextField("LuaDirectory Name", luaDirectoryName);
        startDataLine = EditorGUILayout.IntField("StartDataLine", startDataLine);

        if (GUILayout.Button("ExcelToLuaTable"))
            Change();
    }



    public void Change()
    {
        foreach (var item in Directory.GetFiles(Application.streamingAssetsPath + "/" + tableDirectoryName))
        {
            if (item.Contains(".meta"))
                continue;

            CreatLuaTable(item);
        }
    }

    void CreatLuaTable(string filePath)
    {
        CreatDirectory();
        Save(GetSavePath(filePath), GetSaveContext(filePath));
    }

    void CreatDirectory()
    {
        string path = Application.dataPath + "/" + luaDirectoryName;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }

    string GetSavePath(string filePath)
    {
        string fileName = GetFileName(filePath);
        return Application.dataPath + "/" + luaDirectoryName + "/" + fileName + ".lua";
    }

    string GetFileName(string filePath)
    {
        string targetPath = filePath.Replace(@"\", "/");
        string[] strs = targetPath.Split('/');
        targetPath = strs[strs.Length - 1];
        strs = targetPath.Split('.');
        string fileName = strs[0];

        return fileName + "Table";
    }

    string GetSaveContext(string filePath)
    {
        string fileName = GetFileName(filePath);

        List<string> context = GetContext(filePath);
        string[] propertyName = GetPropertyName(context[0]);

        StringBuilder sb = new StringBuilder();
        sb.Append(fileName + " = {" + "\n");

        for (int i = startDataLine; i < context.Count; i++)
        {
            string[] details = context[i].Split(',');

            sb.Append("[" + details[0] + "]={" + "\n");

            for (int j = 1; j < propertyName.Length; j++)
            {
                sb.Append("['" + propertyName[j] + "']=" + "'" + details[j] + "';" + "\n");
            }

            sb.Append("};" + "\n");
        }

        sb.Append("}");
        return sb.ToString();
    }

    List<string> GetContext(string filePath)
    {
        StreamReader sr = new StreamReader(filePath);
        List<string> contexts = new List<string>();
        string line;
        while ((line = sr.ReadLine()) != null)
            contexts.Add(line);

        return contexts;
    }

    string[] GetPropertyName(string line)
    {
        string[] names = line.Split(',');
        return names;
    }

    void Save(string path, string information)
    {
        if (File.Exists(path))
            File.Delete(path);

        FileStream aFile = new FileStream(@"" + path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(aFile);
        sw.Write(information);
        sw.Close();
        sw.Dispose();

        AssetDatabase.Refresh();
    }
}
