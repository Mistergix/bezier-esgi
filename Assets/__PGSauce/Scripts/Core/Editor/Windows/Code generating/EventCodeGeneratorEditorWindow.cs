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
    public class EventCodeGeneratorEditorWindow : EditorWindow
    {
        private string _types = "";
        private string _localPath = "";
        private string localPathEvents = "__PGSauce/Events";
        private string localPathTemplate = "Templates";

        [MenuItem("PG/Code Generator/New Event Type")]
        public static void ShowWindow()
        {
            var window = GetWindow<EventCodeGeneratorEditorWindow>("Event code generator");
        }

        private void OnGUI()
        {
            _types = EditorGUILayout.TextField("Comma separated types, case sensitive", _types);
            
            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (folderPath.Contains("."))
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            //folderPath = Path.Combine(projectDirPath, folderPath);
            folderPath = folderPath.Replace("Assets/", "");

            _localPath = folderPath;
            
            GUI.enabled = false;
            EditorGUILayout.TextField("Local Path", _localPath);
            GUI.enabled = true;

            if(GUILayout.Button("Generate code"))
            {
                GenerateEventCode();
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

        private bool IsInsidePGSauce => _localPath.Contains("__PGSauce");
        
        private string GetSubNamespace()
        {
            return IsInsidePGSauce ? "Core.PGEvents" : $"Games.{PGSettings.Instance.GameNameInNamespace.Trim()}";
        }

        private void GenerateEventCode()
        {
            if (_types.Length <= 0)
            {
                throw new UnityException("Types must be not empty");
            }
            if(! ConfirmCodeGeneration()) {return;}
            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            string templatesDirPath = Path.Combine(projectDirPath, localPathTemplate);
            string unityEventsTemplatePath = Path.Combine(templatesDirPath, "UnityEventTemplate.txt");
            string pgEventGenericTemplatePath = Path.Combine(templatesDirPath, "PGEventGenericTemplate.txt");
            string pgEventListenerGenericTemplatePath = Path.Combine(templatesDirPath, "PGEventListenerGenericTemplate.txt");
            string pgEventTemplatePath = Path.Combine(templatesDirPath, "PGEventTemplate.txt");
            string pgEventListenerTemplatePath = Path.Combine(templatesDirPath, "PGEventListenerTemplate.txt");

            List<string> typesList = _types.Trim().Split(',').ToList();

            for (int i = 0; i < typesList.Count; i++)
            {
                typesList[i] = typesList[i].Trim();
            }

            GenerateUnityEventCode(assetsDirPath, unityEventsTemplatePath, typesList);
            GeneratePGEventCodeGeneric(assetsDirPath, pgEventGenericTemplatePath, typesList);
            GeneratePGEventListenerCodeGeneric(assetsDirPath, pgEventListenerGenericTemplatePath, typesList);
            GeneratePGEventCode(assetsDirPath, pgEventTemplatePath, typesList);
            GeneratePGEventListenerCode(assetsDirPath, pgEventListenerTemplatePath, typesList);

            if (IsInsidePGSauce)
            {
                string eventsPath = Path.Combine(assetsDirPath, localPathEvents, FormatSpaceTypes(typesList));

                if (!Directory.Exists(eventsPath))
                {
                    Directory.CreateDirectory(eventsPath);
                }
            }

            

            // Refresh the asset database to show the (potentially) new file
            AssetDatabase.Refresh();
        }

        private void GeneratePGEventListenerCode(string assetsDirPath, string pgEventListenerTemplatePath, List<string> typesList)
        {
            string template = File.ReadAllText(pgEventListenerTemplatePath);

            string result = template
            .Replace("#NUMBERARG#", typesList.Count.ToString())
            .Replace("#TYPES#", _types)
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#FORMATTEDTYPES#", FormatTypes(typesList));

            string intoPath = Path.Combine(assetsDirPath, _localPath, string.Format("{0} args", typesList.Count), FormatSpaceTypes(typesList));

            if (!Directory.Exists(intoPath))
            {
                Directory.CreateDirectory(intoPath);
            }

            intoPath = Path.Combine(intoPath, string.Format("PGEventListener{0}.cs", FormatTypes(typesList)));

            File.WriteAllText(intoPath, result);
        }

        private void GeneratePGEventCode(string assetsDirPath, string pgEventTemplatePath, List<string> typesList)
        {
            string template = File.ReadAllText(pgEventTemplatePath);

            string result = template
            .Replace("#TYPES#", _types)
            .Replace("#NUMBERARG#", typesList.Count.ToString())
            .Replace("#FORMATTEDTYPES#", FormatTypes(typesList))
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#FORMATTEDSPACEDTYPES#", FormatSpaceTypes(typesList));

            string intoPath = Path.Combine(assetsDirPath, _localPath, string.Format("{0} args", typesList.Count), FormatSpaceTypes(typesList));

            if (!Directory.Exists(intoPath))
            {
                Directory.CreateDirectory(intoPath);
            }

            intoPath = Path.Combine(intoPath, string.Format("PGEvent{0}.cs", FormatTypes(typesList)));

            File.WriteAllText(intoPath, result);
        }


        private void GeneratePGEventListenerCodeGeneric(string assetsDirPath, string pgEventTemplatePath, List<string> typesList)
        {
            string template = File.ReadAllText(pgEventTemplatePath);

            string result = template
            .Replace("#NUMBERARG#", typesList.Count.ToString())
            .Replace("#GENERICTYPES#", GetGenericTypes(typesList.Count))
            .Replace("#GENERICARGUMENTS#", GetGenericArguments(typesList.Count))
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#GENERICVALUES#", GetGenericValues(typesList.Count));

            string intoPath = Path.Combine(assetsDirPath, _localPath, string.Format("{0} args", typesList.Count));

            if (!Directory.Exists(intoPath))
            {
                Directory.CreateDirectory(intoPath);
            }

            intoPath = Path.Combine(intoPath, string.Format("PGEventListener{0}Args.cs", typesList.Count));

            File.WriteAllText(intoPath, result);
        }

        private void GeneratePGEventCodeGeneric(string assetsDirPath, string pgEventTemplatePath, List<string> typesList)
        {
            string template = File.ReadAllText(pgEventTemplatePath);

            string result = template
            .Replace("#NUMBERARG#", typesList.Count.ToString())
            .Replace("#GENERICTYPES#", GetGenericTypes(typesList.Count))
            .Replace("#GENERICARGUMENTS#", GetGenericArguments(typesList.Count))
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#GENERICVALUES#", GetGenericValues(typesList.Count));

            string intoPath = Path.Combine(assetsDirPath, _localPath, string.Format("{0} args", typesList.Count));

            if (!Directory.Exists(intoPath))
            {
                Directory.CreateDirectory(intoPath);
            }

            intoPath = Path.Combine(intoPath, string.Format("PGEvent{0}Args.cs", typesList.Count));

            File.WriteAllText(intoPath, result);
        }

        private string GetGenericValues(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count - 1; i++)
            {
                sb.Append(string.Format("value{0}, ", i));
            }

            sb.Append(string.Format("value{0}", count - 1));

            return sb.ToString();
        }

        private string GetGenericArguments(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count - 1; i++)
            {
                sb.Append(string.Format("T{0} value{0}, ", i));
            }

            sb.Append(string.Format("T{0} value{0}", count - 1));

            return sb.ToString();
        }

        private string GetGenericTypes(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count - 1; i++)
            {
                sb.Append(string.Format("T{0}, ", i));
            }

            sb.Append(string.Format("T{0}", count - 1));

            return sb.ToString();
        }

        private void GenerateUnityEventCode(string assetsDirPath, string unityEventsTemplatePath, List<string> typesList)
        {
            string template = File.ReadAllText(unityEventsTemplatePath);


            string formattedTypes = FormatTypes(typesList);
            string result = template
            .Replace("#TYPES#", _types)
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#FORMATTEDTYPES#", formattedTypes);

            string intoPath = Path.Combine(assetsDirPath, _localPath, string.Format("UnityEvent{0}", formattedTypes));

            intoPath += ".cs";
            File.WriteAllText(intoPath, result);
        }

        private string FormatTypes(List<string> typesList)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var type in typesList)
            {
                sb.Append(char.ToUpper(type[0]) + type.Substring(1).ToLower()) ;
            }

            return sb.ToString();
        }

        private string FormatSpaceTypes(List<string> typesList)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < typesList.Count - 1; i++)
            {
                string type = typesList[i];
                sb.Append(char.ToUpper(type[0]) + type.Substring(1).ToLower() + " ");
            }

            sb.Append(char.ToUpper(typesList[typesList.Count - 1][0]) + typesList[typesList.Count - 1].Substring(1).ToLower());

            return sb.ToString();
        }
    }
}
