using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace PGSauce.Core.PGEditor
{
    //! REFACTOR
    public class GlobalVariableCodeGeneratorWindow : EditorWindow
    {
        private string type = "";
        private string localPath = "";
        private string localPathGV = "__PGSauce/Global Variables";
        private string localPathTemplate = "Templates";

        [MenuItem("PG/Code Generator/New Global Variable Type")]
        public static void ShowWindow()
        {
            var window = GetWindow<GlobalVariableCodeGeneratorWindow>("Global Variable code generator");
        }

        private void OnGUI()
        {
            type = EditorGUILayout.TextField("Type, case sensitive", type).Trim();
            
            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (folderPath.Contains("."))
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            //folderPath = Path.Combine(projectDirPath, folderPath);
            folderPath = folderPath.Replace("Assets/", "");

            localPath = folderPath;

            GUI.enabled = false;
            EditorGUILayout.TextField("Local Path", localPath);
            GUI.enabled = true;

            if (GUILayout.Button("Generate code"))
            {
                GenerateGVCode();
            }
        }
        
        private bool ConfirmCodeGeneration()
        {
            var sure = true;
            if (IsInsidePGSauce)
            {
                sure = EditorUtility.DisplayDialog("Inside PGSauce",
                    "Are you sure you want to generate the script inside PG Sauce", "Yes", "No");
            }

            return sure;
        }

        private bool IsInsidePGSauce => localPath.Contains("__PGSauce");
        
        private string GetSubNamespace()
        {
            return IsInsidePGSauce ? "Core" : $"Games.{PGSettings.Instance.GameNameInNamespace.Trim()}";
        }

        private void GenerateGVCode()
        {
            if (type.Length <= 0)
            {
                throw new UnityException("Type must be not empty");
            }
            if(! ConfirmCodeGeneration()) {return;}
            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            string templatesDirPath = Path.Combine(projectDirPath, localPathTemplate);
            string templatePath = Path.Combine(templatesDirPath, "GlobalVariableTemplate.txt");

            string template = File.ReadAllText(templatePath);

            string formatType = FormatType(type);

            string result = template
            .Replace("#TYPE#", type)
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#FORMATTEDTYPE#", formatType);

            string intoPath = Path.Combine(assetsDirPath, localPath, string.Format("Global{0}.cs", formatType));

            File.WriteAllText(intoPath, result);

            if (IsInsidePGSauce)
            {
                string gvPath = Path.Combine(assetsDirPath, localPathGV, formatType);

                if (!Directory.Exists(gvPath))
                {
                    Directory.CreateDirectory(gvPath);
                }
            }
            
            

            // Refresh the asset database to show the (potentially) new file
            AssetDatabase.Refresh();
        }

        private string FormatType(string type)
        {
            string result = type.Trim().Replace("<", "")
                .Replace(">", "");


            return char.ToUpper(result[0]) + result.Substring(1);
        }
    }
}
