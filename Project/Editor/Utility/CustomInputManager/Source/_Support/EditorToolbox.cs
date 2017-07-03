#region [Copyright (c) 2015 Cristian Alexandru Geambasu]

//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2015 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//	and associated documentation files (the "Software"), to deal in the Software without restriction,
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion [Copyright (c) 2015 Cristian Alexandru Geambasu]

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using TeamUtility.IO;
using UnityInputConverter;
using AxisConfiguration = TeamUtility.IO.AxisConfiguration;
using InputSaverXML = TeamUtility.IO.InputSaverXML;

namespace TeamUtilityEditor.IO.InputManager
{
    /// <summary>
    /// Class EditorToolbox.
    /// </summary>
    public static class EditorToolbox
    {
        private static string _snapshotFile;
        private static string[] _axisNames;
        private static string[] _joystickNames;

        /// <summary>
        /// Generates the joystick axis names.
        /// </summary>
        /// <returns>System.String[].</returns>
        public static string[] GenerateJoystickAxisNames()
        {
            if (_axisNames == null || _axisNames.Length != AxisConfiguration.MaxJoystickAxes)
            {
                _axisNames = new string[AxisConfiguration.MaxJoystickAxes];
                for (int i = 0; i < AxisConfiguration.MaxJoystickAxes; i++)
                {
                    if (i == 0)
                        _axisNames[i] = "X";
                    else if (i == 1)
                        _axisNames[i] = "Y";
                    else if (i == 2)
                        _axisNames[i] = "3rd axis (Joysticks and Scrollwheel)";
                    else if (i == 21)
                        _axisNames[i] = "21st axis (Joysticks)";
                    else if (i == 22)
                        _axisNames[i] = "22nd axis (Joysticks)";
                    else if (i == 23)
                        _axisNames[i] = "23rd axis (Joysticks)";
                    else
                        _axisNames[i] = string.Format("{0}th axis (Joysticks)", i + 1);
                }
            }

            return _axisNames;
        }

        /// <summary>
        /// Generates the joystick names.
        /// </summary>
        /// <returns>System.String[].</returns>
        public static string[] GenerateJoystickNames()
        {
            if (_joystickNames == null || _joystickNames.Length != AxisConfiguration.MaxJoysticks)
            {
                _joystickNames = new string[AxisConfiguration.MaxJoysticks];
                for (int i = 0; i < AxisConfiguration.MaxJoysticks; i++)
                {
                    _joystickNames[i] = string.Format("Joystick {0}", i + 1);
                }
            }

            return _joystickNames;
        }

        /// <summary>
        /// Determines whether this instance [can load snapshot].
        /// </summary>
        /// <returns><c>true</c> if this instance [can load snapshot]; otherwise, <c>false</c>.</returns>
        public static bool CanLoadSnapshot()
        {
            if (_snapshotFile == null)
            {
                _snapshotFile = Path.Combine(Application.temporaryCachePath, "input_config.xml");
            }

            return File.Exists(_snapshotFile);
        }

        /// <summary>
        /// Creates the snapshot.
        /// </summary>
        /// <param name="inputManager">The input manager.</param>
        public static void CreateSnapshot(TeamUtility.IO.InputManager inputManager)
        {
            if (_snapshotFile == null)
            {
                _snapshotFile = Path.Combine(Application.temporaryCachePath, "input_config.xml");
            }

            InputSaverXML inputSaver = new InputSaverXML(_snapshotFile);
            inputSaver.Save(inputManager.GetSaveParameters());
        }

        /// <summary>
        /// Loads the snapshot.
        /// </summary>
        /// <param name="inputManager">The input manager.</param>
        public static void LoadSnapshot(TeamUtility.IO.InputManager inputManager)
        {
            if (!CanLoadSnapshot())
                return;

            InputLoaderXML inputLoader = new InputLoaderXML(_snapshotFile);
            inputManager.Load(inputLoader.Load());
        }

        /// <summary>
        /// Shows the startup warning.
        /// </summary>
        public static void ShowStartupWarning()
        {
            string key = string.Concat(PlayerSettings.companyName, ".", PlayerSettings.productName, ".InputManager.StartupWarning");

            if (!EditorPrefs.GetBool(key, false))
            {
                string message = "In order to use the InputManager plugin you need to overwrite your project's input settings. Your old input axes will be exported to a file which can be imported at a later time from the File menu.\n\nDo you want to overwrite the input settings now?\nYou can always do it later from the File menu.";
                if (EditorUtility.DisplayDialog("Warning", message, "Yes", "No"))
                {
                    if (OverwriteInputSettings())
                        EditorPrefs.SetBool(key, true);
                }
            }
        }

        /// <summary>
        /// Overwrites the input settings.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool OverwriteInputSettings()
        {
            int length = Application.dataPath.LastIndexOf('/');
            string projectSettingsFolder = string.Concat(Application.dataPath.Substring(0, length), "/ProjectSettings");
            string inputManagerPath = string.Concat(projectSettingsFolder, "/InputManager.asset");

            if (!Directory.Exists(projectSettingsFolder))
            {
                EditorUtility.DisplayDialog("Error", "Unable to get the correct path to the ProjectSetting folder.", "OK");
                return false;
            }

            if (!File.Exists(inputManagerPath))
            {
                EditorUtility.DisplayDialog("Error", "Unable to get the correct path to the InputManager file from the ProjectSettings folder.", "OK");
                return false;
            }

            int option = EditorUtility.DisplayDialogComplex("Warning", "Do you want to export your old input settings?\n\nYour project needs to have text serialization enabled. If text serialization is not enabled press the Abort button.\n\nYou can resume this process after text serialization is enabled from the File menu.", "Yes", "No", "Abort");
            string exportPath = null;

            if (option == 0)
            {
                exportPath = EditorUtility.SaveFilePanel("Export old input axes", "", "unity_input_export", "xml");
                if (string.IsNullOrEmpty(exportPath))
                {
                    if (!EditorUtility.DisplayDialog("Warning", "You chose not to export your old input settings. They will be lost forever. Are you sure you want to continue?", "Yes", "No"))
                        return false;
                }
            }
            else
            {
                if (option == 1)
                {
                    if (!EditorUtility.DisplayDialog("Warning", "You chose not to export your old input settings. They will be lost forever. Are you sure you want to continue?", "Yes", "No"))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            InputConverter inputConverter = new InputConverter();
            if (!string.IsNullOrEmpty(exportPath))
            {
                try
                {
                    inputConverter.ConvertUnityInputManager(inputManagerPath, exportPath);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    if (!EditorUtility.DisplayDialog("Error", "Failed to export your old input settings. Do you want to continue? If you continue your old input settings will be lost forever.", "Yes", "No"))
                        return false;
                }
            }

            inputConverter.GenerateDefaultUnityInputManager(inputManagerPath);

            EditorUtility.DisplayDialog("Success", "The input settings have been successfully replaced.\n\nYou might need to minimize and restore Unity to reimport the new settings.", "OK");

            return true;
        }

        /// <summary>
        /// Keys the code field.
        /// </summary>
        /// <param name="keyString">The key string.</param>
        /// <param name="isEditing">if set to <c>true</c> [is editing].</param>
        /// <param name="label">The label.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <param name="currentKey">The current key.</param>
        public static void KeyCodeField(ref string keyString, ref bool isEditing, string label, string controlName, KeyCode currentKey)
        {
            GUI.SetNextControlName(controlName);
            bool hasFocus = (GUI.GetNameOfFocusedControl() == controlName);
            if (!isEditing && hasFocus)
            {
                keyString = currentKey == KeyCode.None ? string.Empty : currentKey.ToString();
            }

            isEditing = hasFocus;
            if (isEditing)
            {
                keyString = EditorGUILayout.TextField(label, keyString);
            }
            else
            {
                EditorGUILayout.TextField(label, currentKey == KeyCode.None ? string.Empty : currentKey.ToString());
            }
        }

        /// <summary>
        /// Determines whether [has joystick mapping addon].
        /// </summary>
        /// <returns><c>true</c> if [has joystick mapping addon]; otherwise, <c>false</c>.</returns>
        public static bool HasJoystickMappingAddon()
        {
            return GetMappingImporterWindowType() != null;
        }

        /// <summary>
        /// Opens the import joystick mapping window.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        public static void OpenImportJoystickMappingWindow(AdvancedInputEditor configurator)
        {
            Type type = GetMappingImporterWindowType();
            if (type == null)
                return;

            MethodInfo methodInfo = type.GetMethod("Open", BindingFlags.Static | BindingFlags.Public);
            if (methodInfo == null)
            {
                Debug.LogError("Unable to open joystick mapping import window");
            }

            methodInfo.Invoke(null, new object[] { configurator });
        }

        private static Type GetMappingImporterWindowType()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return Array.Find<Type>(assembly.GetTypes(), (type) => { return type.Name == "MappingImportWindow"; });
        }

        /// <summary>
        /// Determines whether [has input adapter addon].
        /// </summary>
        /// <returns><c>true</c> if [has input adapter addon]; otherwise, <c>false</c>.</returns>
        public static bool HasInputAdapterAddon()
        {
            Assembly assembly = typeof(TeamUtility.IO.InputManager).Assembly;
            Type inputAdapterType = Array.Find<Type>(assembly.GetTypes(), (type) => { return type.Name == "InputAdapter"; });
            return inputAdapterType != null;
        }

        /// <summary>
        /// Used to get access to the hidden toolbar search field.
        /// Credits go to the user TowerOfBricks for finding the way to do it.
        /// </summary>
        public static string SearchField(string searchString, params GUILayoutOption[] layoutOptions)
        {
            Type type = typeof(EditorGUILayout);
            string methodName = "ToolbarSearchField";
            System.Object[] parameters = new System.Object[] { searchString, layoutOptions };
            string result = null;

            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo method = type.GetMethod(methodName, (BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public),
                                                null, types, null);

            if (method.IsStatic)
            {
                result = (string)method.Invoke(null, parameters);
            }
            else
            {
                var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                                    BindingFlags.Instance | BindingFlags.CreateInstance;
                System.Object obj = type.InvokeMember(null, bindingFlags, null, null, new System.Object[0]);

                result = (string)method.Invoke(obj, parameters);
            }

            return (result != null) ? result : "";
        }
    }
}