using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace LCKR.Patches
{
    public class TranslationManager : MonoBehaviour
    {
        public static string translationPath = "";
        public static string defTranslationPath = "";

        public static bool isDownloadingFiles;
        public static string downloadingFileText;

        public static Dictionary<string, string[]> TipTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> ScanNodeTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> OtherHudTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> PlanetsTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> ItemTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> DialogueTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> ControlTipTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> CursorTipTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> PlayerLevelsTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> IngameTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> DeathReasonsTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> UnlockableItemTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> TerminalTranslations = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> SigurdTranslations = new Dictionary<string, string[]>();
        
        public static Dictionary<string, string[]> ColdOpenTranslations = new Dictionary<string, string[]>();

        private static readonly string[] TranslationFiles =
        {
            "ColdOpen.txt",
            "ControlTip.txt",
            "CursorTip.txt",
            "DeathReasons.txt",
            "Dialogue.txt",
            "HUD_Others.txt",
            "Ingame.txt",
            "Item.txt",
            "Planets.txt",
            "PlayerLevel.txt",
            "ScanNode.txt",
            "Sigurd.txt",
            "Terminal.txt",
            "Tip.txt",
            "UnlockableItem.txt",
        };
        private static string[] translationFileLink =
        {
            "https://drive.google.com/uc?export=download&id=1feKiEAyQH0174MPwhyQf3FuWk0WzfTe8", //ColdOpen
            "https://drive.google.com/uc?export=download&id=1d6BYFC8sIdamPGJx5RPLeGgqP8LCDFd3", //ControlTip
            "https://drive.google.com/uc?export=download&id=1ZH59zRmXEV8wEgLSKt6dcjhQiMOo7sV2", //CursorTip
            "https://drive.google.com/uc?export=download&id=147nKj6q_CAsjFjM496epWH4531CFpLxS", //DeathReasons
            "https://drive.google.com/uc?export=download&id=1_EROnz5l15jWUxJoTQmxWNbTTJWZB3zi", //Dialogue
            "https://drive.google.com/uc?export=download&id=1kVNZpQDf-sDwRtfBUbsXSJO-zsxWTOSq", //HUD_Others
            "https://drive.google.com/uc?export=download&id=1x5P4_WBp0h7IJCWW4dF3DNcUgVlJcBrg", //Ingame
            "https://drive.google.com/uc?export=download&id=1sMxKzaT4VI9ON5DcyULRKN3L-vo4YlUG", //Item
            "https://drive.google.com/uc?export=download&id=1HbEWYrtkECHLSdOPQ4OFSdGe7a28BPeV", //Planets
            "https://drive.google.com/uc?export=download&id=1s_NOPyG3KQeQcweS5xP8zVGS8Sgl13am", //PlayerLevel
            "https://drive.google.com/uc?export=download&id=1hUjXj2CD4HEGUXsGmD9ctNt221xIKm7t", //ScanNode
            "https://drive.google.com/uc?export=download&id=1J8a21DRbKktY2VyjQMSnkFh4W2N5aH5l", //Sigurd
            "https://drive.google.com/uc?export=download&id=1IX3r6fh6Rw4yVovB07dT_sbldIjYDEUv", //Terminal
            "https://drive.google.com/uc?export=download&id=1eE-IalX3ApXf7lNdSVZW00qJOVAyL_Rb", //Tip
            "https://drive.google.com/uc?export=download&id=1F9NQ6r0yIgDz2UgGUPfs3Tfw4Izya1tf" //UnlockableItem
        };
        
        private static int downloadedFileCount;
        
        public static async void DownloadDefaultTranslation()
        {
            if (isDownloadingFiles)
                return;

            isDownloadingFiles = true;

            if (string.IsNullOrEmpty(defTranslationPath))
                defTranslationPath = Plugin.DefTranslationFilePath;

            if (!Directory.Exists(defTranslationPath))
                Directory.CreateDirectory(defTranslationPath);

            downloadedFileCount = 0;
            Plugin.mls.LogInfo("기본 번역 파일을 다운로드하는 중...");

            try
            {
                for (int i = 0; i < translationFileLink.Length; i++)
                {
                    string url = translationFileLink[i];
                    string fileName = TranslationFiles[i];

                    await DownloadSingleFileAsync(fileName, url);
                }
            }
            finally
            {
                isDownloadingFiles = false;
                Plugin.mls.LogInfo("번역 파일 다운로드가 완료되었습니다.");
            }
        }

        private static async Task DownloadSingleFileAsync(string fileName, string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var op = request.SendWebRequest();

                while (!op.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Plugin.mls.LogError(
                        $"번역 기본값 파일을 다운로드하는데 실패했습니다: {url}\n{request.error}"
                    );
                    return;
                }

                try
                {
                    string filePath = Path.Combine(defTranslationPath, fileName);
                    File.WriteAllBytes(filePath, request.downloadHandler.data);

                    downloadedFileCount++;

                    downloadingFileText = $"<size=14>다운로드하는 중: {fileName} {GetDownloadProgressText()}</size>";
                    Plugin.mls.LogInfo(
                        $"번역 파일을 다운로드하는 중: {fileName}, {GetDownloadProgressText()}"
                    );
                }
                catch (Exception e)
                {
                    Plugin.mls.LogError($"파일 저장 실패: {defTranslationPath}\n{e}");
                }
            }
        }

        private static string GetDownloadProgressText(bool withPercent = false)
        {
            int totalCount = TranslationFiles.Length;
            int currentCount = downloadedFileCount;

            int percent = totalCount <= 0
                ? 0
                : Mathf.FloorToInt((currentCount / (float)totalCount) * 100f);

            if (!withPercent)
                return $"({currentCount}/{totalCount})";

            return $"{percent}% ({currentCount}/{totalCount})";
        }

        public static void Setup()
        {
            string pluginFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DownloadDefaultTranslation();
            //translationPath = Path.Combine(pluginFolderPath, translationPath_str);
            TipTranslations = new Dictionary<string, string[]>();
            ColdOpenTranslations = new Dictionary<string, string[]>();
            ScanNodeTranslations = new Dictionary<string, string[]>();
            OtherHudTranslations = new Dictionary<string, string[]>();
            PlanetsTranslations = new Dictionary<string, string[]>();
            ItemTranslations = new Dictionary<string, string[]>();
            DialogueTranslations = new Dictionary<string, string[]>();
            ControlTipTranslations = new Dictionary<string, string[]>();
            CursorTipTranslations = new Dictionary<string, string[]>();
            PlayerLevelsTranslations = new Dictionary<string, string[]>();
            IngameTranslations = new Dictionary<string, string[]>();
            DeathReasonsTranslations = new Dictionary<string, string[]>();
            UnlockableItemTranslations = new Dictionary<string, string[]>();
            TerminalTranslations = new Dictionary<string, string[]>();
            SigurdTranslations = new Dictionary<string, string[]>();
            
            translationPath = Plugin.TranslationFilePath;
            defTranslationPath = Plugin.DefTranslationFilePath;
            LoadArrayDictionary(ColdOpenTranslations, "ColdOpen.txt");
            LoadArrayDictionary(TipTranslations, "Tip.txt");
            LoadArrayDictionary(ScanNodeTranslations, "ScanNode.txt");
            LoadArrayDictionary(OtherHudTranslations, "HUD_Others.txt");
            LoadArrayDictionary(PlanetsTranslations, "Planets.txt");
            LoadArrayDictionary(ItemTranslations, "Item.txt");
            LoadArrayDictionary(DialogueTranslations, "Dialogue.txt");
            LoadArrayDictionary(ControlTipTranslations, "ControlTip.txt");
            LoadArrayDictionary(CursorTipTranslations, "CursorTip.txt");
            LoadArrayDictionary(PlayerLevelsTranslations, "PlayerLevel.txt");
            LoadArrayDictionary(IngameTranslations, "Ingame.txt");
            LoadArrayDictionary(DeathReasonsTranslations, "DeathReasons.txt");
            LoadArrayDictionary(UnlockableItemTranslations, "UnlockableItem.txt");
            LoadArrayDictionary(TerminalTranslations, "Terminal.txt");
            LoadArrayDictionary(SigurdTranslations, "Sigurd.txt");
        }

        public static void ResetTranslation()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();

            try
            {
                if (string.IsNullOrEmpty(translationPath))
                    translationPath = Plugin.TranslationFilePath;

                if (string.IsNullOrEmpty(defTranslationPath))
                    defTranslationPath = Plugin.DefTranslationFilePath;

                string progressText = GetDownloadProgressText();

                OverwriteTranslationsWithDefaults();
                Setup();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ResetTranslation: {ex}");
                menuManager?.DisplayMenuNotification($"번역 재설정 중 오류가 발생했습니다. {ex}", "[ 뒤로 ]");
            }
        }
        
        private static int GetDownloadedDefaultTranslationFileCount()
        {
            if (string.IsNullOrEmpty(defTranslationPath) || !Directory.Exists(defTranslationPath))
                return 0;

            int count = 0;

            foreach (string fileName in TranslationFiles)
            {
                string filePath = Path.Combine(defTranslationPath, fileName);
                if (File.Exists(filePath))
                    count++;
            }

            return count;
        }

        private static bool AreAllDefaultTranslationFilesDownloaded(out List<string> missingFiles)
        {
            missingFiles = new List<string>();

            if (string.IsNullOrEmpty(defTranslationPath) || !Directory.Exists(defTranslationPath))
            {
                foreach (string fileName in TranslationFiles)
                    missingFiles.Add(fileName);

                return false;
            }

            foreach (string fileName in TranslationFiles)
            {
                string filePath = Path.Combine(defTranslationPath, fileName);
                if (!File.Exists(filePath))
                {
                    missingFiles.Add(fileName);
                }
            }

            return missingFiles.Count == 0;
        }
        
        private static void OverwriteTranslationsWithDefaults()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (string.IsNullOrEmpty(defTranslationPath) || !Directory.Exists(defTranslationPath))
            {
                menuManager.DisplayMenuNotification("번역 기본값 폴더를 찾을 수 없습니다!", "[ 뒤로 ]");
                Debug.LogError($"LCKR 기본 번역 폴더가 존재하지 않습니다: {defTranslationPath}");
                return;
            }

            if (string.IsNullOrEmpty(translationPath))
            {
                menuManager.DisplayMenuNotification("번역 폴더를 찾을 수 없습니다!", "[ 뒤로 ]");
                return;
            }

            
            if (!Directory.Exists(translationPath))
                Directory.CreateDirectory(translationPath);

            List<string> missingFiles = new List<string>();

            foreach (var fileName in TranslationFiles)
            {
                string src = Path.Combine(defTranslationPath, fileName);
                string dst = Path.Combine(translationPath, fileName);

                if (!File.Exists(src))
                {
                    missingFiles.Add(fileName);
                    continue;
                }

                File.Copy(src, dst, overwrite: true);
            }

            // 🔹 누락 파일이 있을 때만 한 번에 로깅
            if (missingFiles.Count > 0)
            {
                menuManager.DisplayMenuNotification($"누락된 기본 번역 파일 목록:\n{string.Join(", ", missingFiles)}", "[ 뒤로 ]");
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

        /// <summary>
        /// fullText 안에 있는 특정 key를 번역된 문자열로 치환합니다.
        /// </summary>
        /// <param name="fullText"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ReplaceArrayText(string fullText, string type, string key, int index = 0)
        {
            return fullText.Replace(key, GetArrayTranslation(type, key, index));
        }
        
        /// <summary>
        /// 입력 텍스트 전체에서 해당 타입 딕셔너리에 있는 모든 매칭 가능한 키를 찾아 번역합니다.
        /// </summary>
        /// <param name="fullText"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ReplaceArrayTextAll(string fullText, string type, int index = 0)
        {
            if (string.IsNullOrEmpty(fullText))
                return fullText;
            return GetArrayTranslation(type, fullText, index, partialMatch: true);
        }
        
        /// <summary>
        /// 번역 딕셔너리에서 번역값을 찾아 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <param name="partialMatch"></param>
        /// <param name="orgText"></param>
        /// <returns></returns>
        public static string GetArrayTranslation(string type, string key, int index = 0, bool partialMatch = false, string orgText = "")
        {
            if (string.IsNullOrEmpty(key)) return key;

            Dictionary<string, string[]> targetDict = null;

            switch (type)
            {
                case "ColdOpen":
                    targetDict = ColdOpenTranslations;
                    break;
                case "Tip":
                    targetDict = TipTranslations;
                    break;
                case "ScanNode": // Just in case, though usually not partial matched
                    targetDict = ScanNodeTranslations;
                    break;
                case "HUD":
                    targetDict = OtherHudTranslations;
                    break;
                case "Planets":
                    targetDict = PlanetsTranslations;
                    break;
                
                case "Item":
                    targetDict = ItemTranslations;
                    break;
                case "Dialogue":
                    targetDict = DialogueTranslations;
                    break;
                case "ControlTip":
                    targetDict = ControlTipTranslations;
                    break;
                case "CursorTip":
                    targetDict = CursorTipTranslations;
                    break;
                case "PlayerLevel":
                    targetDict = PlayerLevelsTranslations;
                    break;
                case "Ingame":
                    targetDict = IngameTranslations;
                    break;
                case "DeathReasons":
                    targetDict = DeathReasonsTranslations;
                    break;
                case "UnlockableItem":
                    targetDict = UnlockableItemTranslations;
                    break;
                case "Sigurd":
                    targetDict = SigurdTranslations;
                    break;
                case "Terminal":
                    targetDict = TerminalTranslations;
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
                            return Regex.Unescape(translatedValues[index]);
                            //return translatedValues[index];
                        }
                    }
                }
            }

            if (type == "Terminal" || type == "Sigurd")
            {
                return orgText;
            }
            return key;
        }
    }
}
