using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

namespace com.achieve.scripting.table.editor
{
    public class TableEditorWindow : EditorWindow
    {
        private List<string> classFiles = new List<string>();
        private string selectedClassFile = "";
        private List<FieldInfo> classFields = new List<FieldInfo>();
        private List<RowData> tableData = new List<RowData>();
        private Vector2 scrollPos;
        private float columnWidth = 150f;

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

            if (classFields.Count > 0)
            {
                DisplayTable();
            }

            EditorGUILayout.Space();

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
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var field in classFields)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.LabelField(field.Name, EditorStyles.boldLabel, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField(GetFieldType(field), EditorStyles.miniLabel, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                    GUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));

            for (int i = 0; i < tableData.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = 0; j < classFields.Count; j++)
                    {
                        var field = classFields[j];
                        string fieldType = GetFieldType(field);

                        switch(fieldType)
                        {
                            case "int":
                                if (int.TryParse(EditorGUILayout.TextField(tableData[i].intData[j].ToString(), GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true)), out var intResult))
                                {
                                    tableData[i].intData[j] = intResult;
                                }
                                break;
                            case "float":
                                if (float.TryParse(EditorGUILayout.TextField(tableData[i].floatData[j].ToString(), GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true)), out var floatResult))
                                {
                                    tableData[i].floatData[j] = floatResult;
                                }
                                break;
                                case "double":
                                if (double.TryParse(EditorGUILayout.TextField(tableData[i].floatData[j].ToString(), GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true)), out var doubleResult))
                                {
                                    tableData[i].doubleData[j] = doubleResult;
                                }
                                break;
                        }

                        if (fieldType == "int")
                        {

                        }
                        else if (fieldType == "Single")
                        {

                        }
                        else if (fieldType == "String")
                        {
                            tableData[i].stringData[j] = EditorGUILayout.TextField(tableData[i].stringData[j], GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                        }
                        else if (fieldType == "Int32[]")
                        {
                            tableData[i].intArrayStringData[j] = EditorGUILayout.TextField(tableData[i].intArrayStringData[j], GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                            tableData[i].intArrayData[j] = ConvertStringToIntArray(tableData[i].intArrayStringData[j]);
                        }
                        else if (fieldType == "Single[]")
                        {
                            tableData[i].floatArrayStringData[j] = EditorGUILayout.TextField(tableData[i].floatArrayStringData[j], GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                            tableData[i].floatArrayData[j] = ConvertStringToFloatArray(tableData[i].floatArrayStringData[j]);
                        }
                        else if (fieldType == "String[]")
                        {
                            tableData[i].stringArrayStringData[j] = EditorGUILayout.TextField(tableData[i].stringArrayStringData[j], GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                            tableData[i].stringArrayData[j] = ConvertStringToStringArray(tableData[i].stringArrayStringData[j]);
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
            RowData newRow = new RowData(classFields.Count);
            tableData.Add(newRow);
        }

        void SaveJsonData()
        {
            string path = "Assets/Resources/" + selectedClassFile + ".json";
            string json = JsonUtility.ToJson(new TableWrapper(tableData), true);
            EncryptionUtility.SaveEncryptedJson(path, json);

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
                var jsonFile = EncryptionUtility.LoadDecryptedJson(filePath);
                var wrapper = JsonUtility.FromJson<TableWrapper>(jsonFile);
                tableData = wrapper.table;
            }

            Type selectedClassType = Type.GetType(className + ", Assembly-CSharp");

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

        string GetFieldType(FieldInfo field)
        {
            var name = field.FieldType.Name switch
            {
                "Int32" => "int",
                "String" => "string",
                "Int64" => "long",
                "UInt64" => "ulong",
                "Single" => "float",
                _ => ""
            };

            if (field.FieldType.IsArray)
            {
                return field.FieldType.GetElementType().Name + "[]";
            }
            return field.FieldType.Name;
        }

        [System.Serializable]
        public class RowData
        {
            public List<int> intData = new List<int>();
            public List<float> floatData = new List<float>();
            public List<double> doubleData = new List<double>();
            public List<string> stringData = new List<string>();

            public List<List<int>> intArrayData = new List<List<int>>();
            public List<List<float>> floatArrayData = new List<List<float>>();
            public List<List<string>> stringArrayData = new List<List<string>>();

            public List<string> intArrayStringData = new List<string>();
            public List<string> floatArrayStringData = new List<string>();
            public List<string> stringArrayStringData = new List<string>();

            public RowData(int fieldCount)
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    intData.Add(0);
                    floatData.Add(0f);
                    stringData.Add("");
                    intArrayData.Add(new List<int>());
                    floatArrayData.Add(new List<float>());
                    stringArrayData.Add(new List<string>());
                    intArrayStringData.Add("");
                    floatArrayStringData.Add("");
                    stringArrayStringData.Add("");
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
