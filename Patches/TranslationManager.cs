using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using static UnityEngine.UI.Selectable;
using System.Text.RegularExpressions;

namespace LCKorean.Patches
{
    public class TranslationManager : MonoBehaviour
    {
        public static string translationPath = "";
        public static string translationPath_str = "LCKR_Translation";

        public static Dictionary<string, string[]> tipTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> scanNodeTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string> itemTranslations = new Dictionary<string, string>();
        public static Dictionary<string, string> dialogueTranslations = new Dictionary<string, string>();
        public static Dictionary<string, string> controlTipTranslations = new Dictionary<string, string>();
        public static Dictionary<string, string> cursorTipTranslations = new Dictionary<string, string>();

        public static void Setup()
        {
            string pluginFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //translationPath = Path.Combine(pluginFolderPath, translationPath_str);
            translationPath = Plugin.TranslationFilePath;
            LoadArrayDictionary(tipTranslations, "Tip.txt");
            LoadDictionary(itemTranslations, "Item.txt");
            LoadDictionary(dialogueTranslations, "Dialogue.txt");
            LoadDictionary(controlTipTranslations, "ControlTip.txt");
            LoadDictionary(cursorTipTranslations, "CursorTip.txt");
        }

        private static void LoadDictionary(Dictionary<string, string> dict, string fileName)
        {
            string filePath = Path.Combine(translationPath, fileName);
            if (!File.Exists(filePath)) return;

            try
            {
                string text = File.ReadAllText(filePath);
                text = RemoveComments(text);

                foreach (var pair in ParseEntries(text))
                {
                    if (string.IsNullOrEmpty(pair.Key)) continue;

                    // Dialogue is single string
                    string value = pair.Value;
                    if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    else if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        // Fallback if they use {"..."} for dialogue? Take first element
                        string content = value.Substring(1, value.Length - 2);
                        Match match = Regex.Match(content, "\"(.*?)\"");
                        if (match.Success) value = match.Groups[1].Value;
                    }

                    if (!dict.ContainsKey(pair.Key))
                    {
                        dict.Add(pair.Key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"LCKR이 대사 번역을 불러오는 도중 오류가 발생했습니다: {ex}");
            }
        }

        private static void LoadArrayDictionary(Dictionary<string, string[]> dict, string fileName)
        {
            string filePath = Path.Combine(translationPath, fileName);
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(translationPath))
                {
                    Directory.CreateDirectory(translationPath);
                }
                return;
            }

            try
            {
                string text = File.ReadAllText(filePath);
                text = RemoveComments(text);

                foreach (var pair in ParseEntries(text))
                {
                    string key = pair.Key;
                    string value = pair.Value;

                    if (string.IsNullOrEmpty(key) || dict.ContainsKey(key)) continue;

                    if (value.StartsWith("{") && value.EndsWith("}"))
                    {
                        // Array format
                        string content = value.Substring(1, value.Length - 2);
                        MatchCollection matches = Regex.Matches(content, "\"(.*?)\"");
                        List<string> values = new List<string>();
                        foreach (Match match in matches)
                        {
                            values.Add(match.Groups[1].Value);
                        }
                        dict.Add(key, values.ToArray());
                    }
                    else
                    {
                        // Single string
                        string cleanValue = value;
                        if (cleanValue.StartsWith("\"") && cleanValue.EndsWith("\""))
                            cleanValue = cleanValue.Substring(1, cleanValue.Length - 2);
                            
                        dict.Add(key, new string[] { cleanValue });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"LCKR이 {fileName} 번역을 불러오는 도중 오류가 발생했습니다: {ex}");
            }
        }

        private static string RemoveComments(string text)
        {
            // Remove // comments
            return Regex.Replace(text, @"//.*?$", "", RegexOptions.Multiline);
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseEntries(string text)
        {
            List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
            
            bool inQuote = false;
            int braceLevel = 0;
            int lastIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '"' && (i == 0 || text[i - 1] != '\\')) inQuote = !inQuote;
                else if (c == '{' && !inQuote) braceLevel++;
                else if (c == '}' && !inQuote) braceLevel--;
                else if (c == ',' && !inQuote && braceLevel == 0)
                {
                    string entry = text.Substring(lastIndex, i - lastIndex).Trim();
                    if (!string.IsNullOrEmpty(entry))
                    {
                        AddEntry(entries, entry);
                    }
                    lastIndex = i + 1;
                }
            }

            if (lastIndex < text.Length)
            {
                string entry = text.Substring(lastIndex).Trim();
                if (!string.IsNullOrEmpty(entry))
                {
                    AddEntry(entries, entry);
                }
            }

            return entries;
        }

        private static void AddEntry(List<KeyValuePair<string, string>> entries, string entry)
        {
            int splitIndex = entry.IndexOf('=');
            if (splitIndex > 0)
            {
                string key = entry.Substring(0, splitIndex).Trim();
                if (key.StartsWith("\"") && key.EndsWith("\"") && key.Length >= 2)
                {
                    key = key.Substring(1, key.Length - 2);
                }

                string value = entry.Substring(splitIndex + 1).Trim();
                entries.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        public static string GetArrayTranslation(string type, int index, string key, bool partialMatch = false)
        {
            if (string.IsNullOrEmpty(key)) return key;

            Dictionary<string, string[]> targetDict = null;

            switch (type)
            {
                case "Tip":
                    targetDict = tipTranslations;
                    break;
                case "ScanNode": // Just in case, though usually not partial matched
                    targetDict = scanNodeTranslations;
                    break;
            }

            if (targetDict != null)
            {
                if (partialMatch)
                {
                    string modifiedKey = key;
                    // Sort by length descending to prioritize longer matches (e.g. "Open : " before "Open")
                    var sortedKeys = targetDict.Keys.OrderByDescending(k => k.Length);
                    
                    foreach (var dictKey in sortedKeys)
                    {
                        string[] values = targetDict[dictKey];
                        if (index >= 0 && index < values.Length && modifiedKey.Contains(dictKey))
                        {
                            modifiedKey = modifiedKey.Replace(dictKey, values[index]);
                        }
                    }
                    return modifiedKey;
                }
                else
                {
                    if (targetDict.TryGetValue(key, out string[] translatedValues))
                    {
                        if (index >= 0 && index < translatedValues.Length)
                        {
                            return translatedValues[index];
                        }
                    }
                }
            }
            return key;
        }

        public static string GetStringTranslation(string type, string key, bool partialMatch = false)
        {
            if (string.IsNullOrEmpty(key)) return key;

            Dictionary<string, string> targetDict = null;

            switch (type)
            {
                case "Item":
                    targetDict = itemTranslations;
                    break;
                case "Dialogue":
                    targetDict = dialogueTranslations;
                    break;
                case "ControlTip":
                    targetDict = controlTipTranslations;
                    break;
                case "CursorTip":
                    targetDict = cursorTipTranslations;
                    break;
            }

            if (targetDict != null)
            {
                if (partialMatch)
                {
                    string modifiedKey = key;
                    // Sort by length descending to prioritize longer matches
                     var sortedKeys = targetDict.Keys.OrderByDescending(k => k.Length);

                    foreach (var dictKey in sortedKeys)
                    {
                        string value = targetDict[dictKey];
                        if (modifiedKey.Contains(dictKey))
                        {
                            modifiedKey = modifiedKey.Replace(dictKey, value);
                        }
                    }
                    return modifiedKey;
                }
                else
                {
                    if (targetDict.TryGetValue(key, out string translatedValue))
                    {
                        return translatedValue;
                    }
                }
            }
            return key;
        }
    }
}
