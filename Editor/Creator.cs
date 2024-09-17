using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Achieve.TableCraft
{
    public static class Creator
    {
        public static void SaveClass(string className, List<ColumnData> columns)
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
                    string fieldType = DataTypes[column.dataTypeIndex];

                    if (column.isArray)
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

        public static List<RowData> LoadJson(string className)
        {
            string filePath = $"Assets/Resources/{className}.json";

            if (File.Exists(filePath))
            {
#if ENABLE_ENCRYPT
                var jsonFile = EncryptionUtility.LoadDecryptedJson(filePath, "ejkrqiwebmvl1kry");
#else
                var jsonFile = string.Empty;
#endif
                var wrapper = JsonUtility.FromJson<TableWrapper>(jsonFile);
                return wrapper.table;
            }

            return null;
        }
        public static void SaveJson(List<RowData> tableData, string path)
        {
            string json = JsonUtility.ToJson(new TableWrapper(tableData), true);

#if ENABLE_ENCRYPT
            EncryptionUtility.SaveEncryptedJson(path, json, "ejkrqiwebmvl1kry");
#else

#endif
        }

        public static string[] DataTypes =
        {
            "int", 
            "uint", 
            "short", 
            "ushort", 
            "long", 
            "ulong",
            "float", 
            "double", 
            "decimal",
            "string", 
            "bool" 
        };

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

        #region TableEditorWindow DTO

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
        #endregion


        #region JSONTablePopup DTO
        [System.Serializable]
        public class TableWrapper
        {
            public List<RowData> table;

            public TableWrapper(List<RowData> data)
            {
                table = data;
            }
        }

        public class ColumnData
        {
            public string columnName;
            public int dataTypeIndex;
            public bool isArray;
        }

        [System.Serializable]
        private class ColumnJsonData
        {
            public string columnName;
            public string dataType;
        }

        [System.Serializable]
        private class ClassWrapper
        {
            public List<ColumnJsonData> columns;

            public ClassWrapper(List<ColumnJsonData> data)
            {
                columns = data;
            }
        }
        #endregion
    }
}