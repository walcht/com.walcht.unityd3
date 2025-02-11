using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityD3
{
    public static class Importers
    {
        public static List<RecordType> FromResourcesCSV<RecordType>(string filepath,
            Func<string, RecordType> record_parser)
        {
            List<RecordType> result = new();
            var dataset = Resources.Load<TextAsset>(filepath);
            string[] lines = dataset.text.Split("\n");
            for (int i = 1; i < lines.Length; ++i)
            {
                if ((lines[i].Length == 0) || string.IsNullOrWhiteSpace(lines[i])) continue;
                try
                {
                    result.Add(record_parser(lines[i]));
                }
                catch (FormatException)
                {
                    Debug.LogWarning($"ignored entry: {lines[i]} at line: {i} due to FormatException");
                }
            }
            return result;
        }

        public static List<RecordType> FromPathCSV<RecordType>(string filepath, Func<string, RecordType> record_parser)
        {
            List<RecordType> result = new();
            using (var reader = new StreamReader(filepath))
            {
                string line;
                reader.ReadLine();  // ignore header line
                while ((line = reader.ReadLine()) != null)
                {
                    if ((line.Length == 0) || string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        result.Add(record_parser(line));
                    }
                    catch (FormatException)
                    {
                        Debug.LogWarning($"ignored entry: {line} due to FormatException");
                    }
                }
            }
            return result;
        }
    }
}