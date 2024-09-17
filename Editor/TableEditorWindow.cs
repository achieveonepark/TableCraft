using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

namespace Achieve.TableCraft.Editor
{
    public class TableEditorWindow : EditorWindow
    {
        private List<string> classFiles = new List<string>();
        private string selectedClassFile = "";
        private List<FieldInfo> classFields = new List<FieldInfo>();
        private List<Creator.RowData> tableData = new List<Creator.RowData>();
        private Vector2 scrollPos;
        private Type selectedClassType;
        private GUILayoutOption[] options = { GUILayout.Width(100), GUILayout.ExpandWidth(false) };

        [MenuItem("My Tools/Manage C# Tables")]
        public static void ShowWindow()
        {
            GetWindow<TableEditorWindow>("Manage C# Tables");
        }

        void OnEnable()
        {
            string[] files = Directory.GetFiles("Assets/Resources", "*.cs");
            classFiles.Clear();
            foreach (var file in files)
            {
                classFiles.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        void OnGUI()
        {
            DisplayTop();

            if (classFields.Count > 0)
            {
                DisplayTable();
                DisplayBottom();
            }
        }

        void DisplayTop()
        {
            GUILayout.Label("Manage C# Classes", EditorStyles.boldLabel);

            int selectedIndex = classFiles.IndexOf(selectedClassFile);
            selectedIndex = EditorGUILayout.Popup("Select Class", selectedIndex, classFiles.ToArray());

            if (selectedIndex >= 0 && classFiles.Count > 0)
            {
                if (selectedClassFile != classFiles[selectedIndex])
                {
                    selectedClassFile = classFiles[selectedIndex];
                    LoadClassFields();
                }
            }

        }

        void DisplayBottom()
        {
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Row", GUILayout.Width(100)))
                {
                    AddRow();
                }

                if (GUILayout.Button("Save to JSON", GUILayout.Width(100)))
                {
                    SaveJsonData();
                }
            }
        }

        void DisplayTable()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var field in classFields)
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField(field.Name, EditorStyles.boldLabel, options);
                        EditorGUILayout.LabelField(Creator.GetFieldType(field), EditorStyles.miniLabel, options);
                    }
                }
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(700));

            for (int i = 0; i < tableData.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = 0; j < tableData[i].Key.Count; j++)
                    {
                        var field = classFields[j];
                        string fieldType = Creator.GetFieldType(field);
                        var dataStr = tableData[i].Value[j] != null ? tableData[i].Value[j].ToString() : string.Empty;
                        switch (fieldType)
                        {
                            case "int": if (int.TryParse(EditorGUILayout.TextField(dataStr, options), out var intResult)) tableData[i].Value[j] = intResult; break;
                            case "short": if (short.TryParse(EditorGUILayout.TextField(dataStr, options), out var shortResult)) tableData[i].Value[j] = shortResult; break;
                            case "ushort": if (ushort.TryParse(EditorGUILayout.TextField(dataStr, options), out var ushortResult)) tableData[i].Value[j] = ushortResult; break;
                            case "bool": if (bool.TryParse(EditorGUILayout.TextField(dataStr, options), out var boolResult)) tableData[i].Value[j] = boolResult; break;
                            case "decimal": if (decimal.TryParse(EditorGUILayout.TextField(dataStr, options), out var decimalResult)) tableData[i].Value[j] = decimalResult; break;
                            case "ulong": if (ulong.TryParse(EditorGUILayout.TextField(dataStr, options), out var ulongResult)) tableData[i].Value[j] = ulongResult; break;
                            case "long": if (long.TryParse(EditorGUILayout.TextField(dataStr, options), out var longResult)) tableData[i].Value[j] = longResult; break;
                            case "float": if (float.TryParse(EditorGUILayout.TextField(dataStr, options), out var floatResult)) tableData[i].Value[j] = floatResult; break;
                            case "double": if (double.TryParse(EditorGUILayout.TextField(dataStr, options), out var doubleResult)) tableData[i].Value[j] = doubleResult; break;
                            case "string": tableData[i].Value[j] = EditorGUILayout.TextField(dataStr, options); break;
                            case "int[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<int>(EditorGUILayout.TextField(StringToListConverter.ToString((int[])tableData[i].Value[j]), options)); break;
                            case "short[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<short>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "ushort[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<ushort>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "bool[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<bool>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "decimal[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<decimal>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "ulong[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<ulong>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "long[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<long>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "float[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<float>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "double[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<double>(EditorGUILayout.TextField(dataStr, options)); break;
                            case "string[]": tableData[i].Value[j] = StringToListConverter.ConvertStringToArray<string>(EditorGUILayout.TextField(dataStr, options)); break;
                        }
                    }


                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        tableData.RemoveAt(i);
                        break;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void AddRow()
        {
            Creator.RowData newRow = new Creator.RowData(classFields);
            tableData.Add(newRow);
        }

        void SaveJsonData()
        {
            string path = "Assets/Resources/" + selectedClassFile + ".json";
            Creator.SaveJson(tableData, path);
        }

        void LoadClassFields()
        {
            classFields.Clear();
            tableData.Clear();
            string className = selectedClassFile;

            tableData = Creator.LoadJson(className);
            selectedClassType = Type.GetType(className + ", Assembly-CSharp");

            if (selectedClassType != null)
            {
                FieldInfo[] fields = selectedClassType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                classFields.AddRange(fields);
            }
            else
            {
                Debug.LogError("Class not found: " + className);
            }
        }
    }
}
