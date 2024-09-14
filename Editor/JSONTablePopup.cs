using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace com.achieve.scripting.table.editor
{
    public class JSONTablePopup : EditorWindow
    {
        private string className = "NewClass";
        private List<ColumnData> columns = new List<ColumnData>();
        private Vector2 scrollPos;
        private string[] dataTypes = 
            { "int", "uint", "short", "ushort", "long", "ulong",
            "float", "double", "decimal",
            "string", "bool" };
        private float columnWidth = 150f;

        private class ColumnData
        {
            public string columnName;
            public int dataTypeIndex;
            public bool isArray;
        }

        [MenuItem("My Tools/Create C# Class and JSON")]
        public static void ShowWindow()
        {
            GetWindow<JSONTablePopup>("Class and JSON Creator");
        }

        private void OnEnable()
        {
            if(columns.Count == 0)
            {
                columns.Add(new ColumnData { columnName = "id", dataTypeIndex = 0 });
            }
        }
        
        void OnGUI()
        {
            GUILayout.Label("Create C# Class and JSON", EditorStyles.boldLabel);

            className = EditorGUILayout.TextField("Class Name", className);

            EditorGUILayout.Space();
            if (GUILayout.Button("Add Field"))
            {
                columns.Add(new ColumnData { columnName = "FieldName", dataTypeIndex = 0 });
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Data Type", EditorStyles.boldLabel, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("Field Name", EditorStyles.boldLabel, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("Array", EditorStyles.boldLabel, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < columns.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (columns[i].columnName.Contains("id"))
                    {
                        GUI.enabled = false;
                    }
                    columns[i].dataTypeIndex = EditorGUILayout.Popup(columns[i].dataTypeIndex, dataTypes, GUILayout.ExpandWidth(true));
                    columns[i].columnName = EditorGUILayout.TextField(columns[i].columnName, GUILayout.ExpandWidth(true));
                    GUILayout.Space(70);
                    columns[i].isArray = EditorGUILayout.Toggle(columns[i].isArray, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                    {
                        columns.RemoveAt(i);
                    }


                    if (columns[i].columnName.Contains("id"))
                    {
                        GUI.enabled = true;
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Create C# Class and JSON"))
            {
                CreateClassAndJSON();
            }
        }

        void CreateClassAndJSON()
        {
            string folderPath = "Assets/Resources/";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string classFilePath = folderPath + className + ".cs";
            using (StreamWriter writer = new StreamWriter(classFilePath))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("public class " + className);
                writer.WriteLine("{");

                // 클래스의 필드 생성
                foreach (var column in columns)
                {
                    string fieldType = dataTypes[column.dataTypeIndex];

                    if(column.isArray)
                    {
                        fieldType = $"{fieldType}[]";
                    }

                    writer.WriteLine("    public " + fieldType + " " + column.columnName + ";");
                }

                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("C# class and JSON file created at: " + folderPath);
        }

        [System.Serializable]
        private class ColumnJsonData
        {
            public string columnName;
            public string dataType;
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<ColumnJsonData> columns;

            public Wrapper(List<ColumnJsonData> data)
            {
                columns = data;
            }
        }
    }
}