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
        private List<RowData> tableData = new List<RowData>();
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
                        EditorGUILayout.LabelField(GetFieldType(field), EditorStyles.miniLabel, options);
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
                        string fieldType = GetFieldType(field);
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

        List<int> ConvertStringToIntArray(string input)
        {
            List<int> result = new List<int>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] parts = input.Split(',');
                foreach (string part in parts)
                {
                    if (int.TryParse(part.Trim(), out int value))
                    {
                        result.Add(value);
                    }
                }
            }
            return result;
        }

        List<float> ConvertStringToFloatArray(string input)
        {
            List<float> result = new List<float>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] parts = input.Split(',');
                foreach (string part in parts)
                {
                    if (float.TryParse(part.Trim(), out float value))
                    {
                        result.Add(value);
                    }
                }
            }
            return result;
        }

        List<string> ConvertStringToStringArray(string input)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(input))
            {
                string[] parts = input.Split(',');
                foreach (string part in parts)
                {
                    result.Add(part.Trim());
                }
            }
            return result;
        }

        void AddRow()
        {
            RowData newRow = new RowData(classFields);
            tableData.Add(newRow);
        }

        void SaveJsonData()
        {
            string path = "Assets/Resources/" + selectedClassFile + ".json";
            string json = JsonUtility.ToJson(new TableWrapper(tableData), true);

#if ENABLE_ENCRYPT
            EncryptionUtility.SaveEncryptedJson(path, json, "ejkrqiwebmvl1kry");
#else

#endif
            Debug.Log("JSON saved to: " + path);
        }

        void LoadClassFields()
        {
            classFields.Clear();
            tableData.Clear();
            string className = selectedClassFile;

            string filePath = $"Assets/Resources/{className}.json";

            if(File.Exists(filePath))
            {
#if ENABLE_ENCRYPT
                var jsonFile = EncryptionUtility.LoadDecryptedJson(filePath, "ejkrqiwebmvl1kry");
#else
                var jsonFile = string.Empty;
#endif
                var wrapper = JsonUtility.FromJson<TableWrapper>(jsonFile);
                tableData = wrapper.table;
            }

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

        public static string GetFieldType(FieldInfo field)
        {
            return field.FieldType.Name switch
            {
                "UInt32" => "uint",
                "Int32" => "int",
                "Int16" => "short",
                "UInt16" => "ushort",
                "String" => "string",
                "Int64" => "long",
                "UInt64" => "ulong",
                "Single" => "float",
                "Double" => "double",
                "Decimal" => "decimal",
                "Boolean" => "boolean",
                "UInt32[]" => "uint[]",
                "Int32[]" => "int[]",
                "Int16[]" => "short[]",
                "UInt16[]" => "ushort[]",
                "String[]" => "string[]",
                "Int64[]" => "long[]",
                "UInt64[]" => "ulong[]",
                "Single[]" => "float[]",
                "Double[]" => "double[]",
                "Decimal[]" => "decimal[]",
                "Boolean[]" => "boolean[]",
                _ => ""
            };
        }

        [System.Serializable]
        public class RowData
        {
            public List<string> Key = new List<string>();
            public List<object> Value = new List<object>();

            public RowData(List<FieldInfo> classFields)
            {
                for (int i = 0; i < classFields.Count; i++)
                {
                    Key.Add(classFields[i].Name);
                    Value.Add(null);
                }
            }
        }


        [System.Serializable]
        public class TableWrapper
        {
            public List<RowData> table;

            public TableWrapper(List<RowData> data)
            {
                table = data;
            }
        }
    }
}
