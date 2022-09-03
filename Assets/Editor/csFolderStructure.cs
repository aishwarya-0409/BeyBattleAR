/* 
    Cerenity Studio Folder Structure C# script for Unity Projects 
    Copyrighted ©2020 - 2022
*/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class csFolderStructure : EditorWindow
{
    private static string projectName = "_MAIN / _DEV ?";
    [MenuItem("Asset/Create Default Folders")]
    private static void SetUpFolders()
    {
        csFolderStructure window = ScriptableObject.CreateInstance<csFolderStructure>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
        window.ShowPopup();
    }

    private static void CreateAllFolders()
    {
        List<string> folders = new List<string>
        {
            "_Animations",
            "_Audio",
            "_Editor",
            "_Materials",
            "_Meshes",
            "_Prefabs",
            "_Scripts",
            "_Scenes",
            "_Shaders",
            "_Textures",
            "_UI"
        };

        foreach (string folder in folders)
        {
            if (!Directory.Exists("Assets/" + folder))
            {
                Directory.CreateDirectory("Assets/" + projectName + "/" + folder);
            }
        }

        List<string> uiFolders = new List<string>
        {
            "_Assets",
            "_Fonts",
            "_Icon"
        };

        foreach (string subfolder in uiFolders)
        {
            if(!Directory.Exists("Assets/" + projectName + "/_UI/" + subfolder))
            {
                Directory.CreateDirectory("Assets/" + projectName + "/_UI/" + subfolder);
            }
        }

        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Insert the Project name used as the root folder");
        projectName = EditorGUILayout.TextField("Project Name: ", projectName);
        this.Repaint();
        GUILayout.Space(70);
        if (GUILayout.Button("Generate!"))
        {
            CreateAllFolders();
            this.Close();
        }
    }
}
