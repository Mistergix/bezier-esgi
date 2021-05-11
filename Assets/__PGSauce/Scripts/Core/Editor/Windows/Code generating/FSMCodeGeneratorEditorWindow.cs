using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

namespace PGSauce.Core.PGEditor
{
    //! REFACTOR
    public class FSMCodeGeneratorEditorWindow : EditorWindow
    {
        private string objectName = "", gameName = "", actionName = "", decisionName = "";
        private string localTemplatePath = "Templates/FSM";
        private string path = "";
        private string localFolderPath = "";
        private static FSMCodeGeneratorEditorWindow window;

        [MenuItem("PG/Code Generator/FSM/New State Machine")]
        public static void ShowWindow()
        {
            window = GetWindow<FSMCodeGeneratorEditorWindow>("FSM code generator");
        }

        private void OnGUI()
        {
            objectName = EditorGUILayout.TextField("Name, case sensitive", objectName).Trim();
            gameName = PGSettings.Instance.GameNameInNamespace.Trim();
            actionName = EditorGUILayout.TextField("Action Name, case sensitive", actionName).Trim();
            decisionName = EditorGUILayout.TextField("Decision Name, case sensitive", decisionName).Trim();

            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            string templatesDirPath = Path.Combine(projectDirPath, localTemplatePath);

            string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (folderPath.Contains("."))
                folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
            folderPath = Path.Combine(projectDirPath, folderPath);

            GUI.enabled = false;
            EditorGUILayout.TextField("Path", folderPath);
            GUI.enabled = true;

            path = folderPath;

            if (GUILayout.Button("Generate new FSMcode"))
            {
                GenerateFSMCode();
            }

            if (GUILayout.Button("Generate new FSM Action"))
            {
                GenerateFSMAction(templatesDirPath, actionName, path);
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Generate new FSM Decision"))
            {
                GenerateFSMDecision(templatesDirPath, decisionName, path, "true || false");
                AssetDatabase.Refresh();
            }

            
        }

        private string GetSubNamespace()
        {
            return IsInsidePGSauce ? "Core.PGFiniteStateMachine" : $"Games.{gameName}";
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

        private bool IsInsidePGSauce => path.Contains("__PGSauce");

        private void GenerateFSMCode()
        {
            if (objectName.Length <= 0)
            {
                throw new UnityException("Name must be not empty");
            }
            
            if(! ConfirmCodeGeneration()) {return;}

            localFolderPath = "State Machine " + objectName;
            string assetsDirPath = Application.dataPath;
            string projectDirPath = Directory.GetParent(assetsDirPath).FullName;
            string templatesDirPath = Path.Combine(projectDirPath, localTemplatePath);

            GenerateFolders();

            GenerateStateController(templatesDirPath);
            GenerateState(templatesDirPath);
            GenerateTransition(templatesDirPath);

            GenerateFSMDecision(templatesDirPath, "True", baseScriptsPath, "true");
            GenerateFSMDecision(templatesDirPath, "False", baseScriptsPath, "false");

            AssetDatabase.Refresh();

            /*
            EditorPrefs.SetBool("ShouldCreateAsset", true);
            EditorPrefs.SetString("FalseDecisionAssetName", string.Format("{0}Decision{1}", "False", objectName));
            EditorPrefs.SetString("TrueDecisionAssetName", string.Format("{0}Decision{1}", "True", objectName));
            EditorPrefs.SetString("StateAssetName", string.Format("State{0}", objectName));
            */
        }

        /*
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }

            EditorApplication.delayCall += CreateAssetNow;
        }

        private static void CreateAssetNow()
        {
            bool generate = EditorPrefs.GetBool("ShouldCreateAsset", false);

            if (generate)
            {
                string fDecisionName = EditorPrefs.GetString("FalseDecisionAssetName");
                string tDecisionName = EditorPrefs.GetString("TrueDecisionAssetName");
                string stateName = EditorPrefs.GetString("StateAssetName");

                Type fDecisionType = Type.GetType(fDecisionName);
                Type tDecisionType = Type.GetType(tDecisionName);
                Type stateType = Type.GetType(stateName);

                Debug.Log(fDecisionName + " " + tDecisionName + " " + stateName);
                Debug.Log(fDecisionType +" " + tDecisionType + " " + stateType);
                Debug.Log(fDecisionType.FullName + " " + tDecisionType.Name + " " + stateType.Name);
            }


            EditorPrefs.SetBool("ShouldCreateAsset", false);
            EditorPrefs.SetString("FalseDecisionAssetName", string.Empty);
            EditorPrefs.SetString("TrueDecisionAssetName", string.Empty);
            EditorPrefs.SetString("StateAssetName", string.Empty);
        }
        */

        private void GenerateFSMAction(string templatesDirPath, string actionName, string pathToScript)
        {
            if(! ConfirmCodeGeneration()) {return;}
            string templatePath = Path.Combine(templatesDirPath, "ActionTemplate.txt");
            string template = File.ReadAllText(templatePath);

            string result = template
            .Replace("#STATECONTROLLERNAME#", StateControllerName())
            .Replace("#NAME#", objectName)
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#ACTIONNAME#", actionName);

            string intoPath = Path.Combine(pathToScript, string.Format("{0}Action{1}.cs", actionName, objectName));

            File.WriteAllText(intoPath, result);
        }

        private void GenerateFSMDecision(string templatesDirPath, string decisionName, string pathToScript, string defaultValue)
        {
            if(! ConfirmCodeGeneration()) {return;}
            string templatePath = Path.Combine(templatesDirPath, "DecisionTemplate.txt");
            string template = File.ReadAllText(templatePath);

            string result = template
            .Replace("#STATECONTROLLERNAME#", StateControllerName())
            .Replace("#NAME#", objectName)
            .Replace("#SUBNAMESPACE#", GetSubNamespace())
            .Replace("#DECISIONNAME#", decisionName)
            .Replace("#DEFAULTVALUE#", defaultValue);

            string intoPath = Path.Combine(pathToScript, string.Format("{0}Decision{1}.cs", decisionName, objectName));

            File.WriteAllText(intoPath, result);
        }

        private void GenerateTransition(string templatesDirPath)
        {
            string templatePath = Path.Combine(templatesDirPath, "TransitionTemplate.txt");
            string template = File.ReadAllText(templatePath);

            string result = template
            .Replace("#STATECONTROLLERNAME#", StateControllerName())
            .Replace("#NAME#", objectName)
            .Replace("#SUBNAMESPACE#", GetSubNamespace());

            string intoPath = Path.Combine(baseScriptsPath, string.Format("Transition{0}.cs", objectName));

            File.WriteAllText(intoPath, result);
        }

        private void GenerateState(string templatesDirPath)
        {
            string templatePath = Path.Combine(templatesDirPath, "StateTemplate.txt");
            string template = File.ReadAllText(templatePath);

            string result = template
            .Replace("#STATECONTROLLERNAME#", StateControllerName())
            .Replace("#NAME#", objectName)
            .Replace("#SUBNAMESPACE#", GetSubNamespace());

            string intoPath = Path.Combine(baseScriptsPath, string.Format("State{0}.cs", objectName));

            File.WriteAllText(intoPath, result);
        }

        private string baseScriptsPath = "";

        private void GenerateFolders()
        {
            string folderPath = Path.Combine(path, localFolderPath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            Directory.CreateDirectory(baseScriptsPath = Path.Combine(folderPath, "__BASE SCRIPTS"));

            Directory.CreateDirectory(Path.Combine(folderPath, "User Scripts"));

            Directory.CreateDirectory(Path.Combine(folderPath, "User Scripts", "Actions"));
            Directory.CreateDirectory(Path.Combine(folderPath, "User Scripts", "Decisions"));

            Directory.CreateDirectory(Path.Combine(folderPath, "Scriptable Objects"));

            Directory.CreateDirectory(Path.Combine(folderPath, "Scriptable Objects", "Actions"));
            Directory.CreateDirectory(Path.Combine(folderPath, "Scriptable Objects", "Decisions"));
            Directory.CreateDirectory(Path.Combine(folderPath, "Scriptable Objects", "States"));
        }

        private void GenerateStateController(string templatesDirPath)
        {
            string templatePath = Path.Combine(templatesDirPath, "StateControllerTemplate.txt");
            string template = File.ReadAllText(templatePath);

            string result = template
            .Replace("#STATECONTROLLERNAME#", StateControllerName())
            .Replace("#SUBNAMESPACE#", GetSubNamespace());

            string intoPath = Path.Combine(baseScriptsPath, string.Format("{0}.cs", StateControllerName()));

            File.WriteAllText(intoPath, result);
        }

        private string StateControllerName()
        {
            return "StateController" + char.ToUpper(objectName[0]) + objectName.Substring(1);
        }
    }
}
