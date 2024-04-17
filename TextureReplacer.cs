using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

    public class TextureReplacer : MonoBehaviour
    {
        internal static TextureReplacer Instance { get; private set; }

        public static void Setup()
        {
        GameObject gameObject = new GameObject("TextureReplacer");
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        TextureReplacer.Instance = gameObject.AddComponent<TextureReplacer>();

        string pluginFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        TextureReplacer.imagesPath = Path.Combine(pluginFolderPath, TextureReplacer.imagesPath_str);
        TextureReplacer.imagesDumpPath = Path.Combine(pluginFolderPath, TextureReplacer.imagesDumpPath_str);

        TextureReplacer.getImages();
        TextureReplacer.Textures = new Texture2D[0];
        }
        public static string imagesPath { get; set; } = "";
        public static string imagesDumpPath { get; set; } = "";

        private static void getImages()
        {
            if (!Directory.Exists(TextureReplacer.imagesPath))
            {
                Directory.CreateDirectory(TextureReplacer.imagesPath);
            }
            try
            {
                TextureReplacer.images = new Dictionary<string, string>();
                TextureReplacer.unique_keys = new Dictionary<string, string>();
                TextureReplacer.doCheckMD5_keys = new List<string>();
                string[] files = Directory.GetFiles(TextureReplacer.imagesPath, "*.png", SearchOption.AllDirectories);
                Array.Sort<string>(files);
                string[] array = files;
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i];
                    string[] array2 = text.Split(new char[] { Path.DirectorySeparatorChar });
                    string select_str = string.Empty;
                    if (array2.Count<string>() > 5)
                    {
                        for (int j = 1; j < array2.Count<string>(); j++)
                        {
                            if (array2[j - 1] == TextureReplacer.imagesPath_str && array2[j].ToLower() == TextureReplacer.select_folder && j + 2 < array2.Count<string>())
                            {
                                select_str = array2[j + 1];
                                break;
                            }
                        }
                    }
                    bool flag = false;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
                    string text2 = fileNameWithoutExtension;
                    if (!TextureReplacer.images.ContainsKey(fileNameWithoutExtension))
                    {
                        TextureReplacer.images.Add(fileNameWithoutExtension, text);
                    }
                    else
                    {
                        flag = true;
                    }
                    int num = fileNameWithoutExtension.LastIndexOf('_');
                    if (num > 0)
                    {
                        text2 = fileNameWithoutExtension.Substring(0, num);
                        bool flag2 = true;
                        if (fileNameWithoutExtension.Length > num + 1 && fileNameWithoutExtension.Substring(num + 1).Length == 32)
                        {
                            flag2 = false;
                            if (!TextureReplacer.doCheckMD5_keys.Contains(text2))
                            {
                                TextureReplacer.doCheckMD5_keys.Add(text2);
                            }
                        }
                        if (flag2)
                        {
                            if (select_str == string.Empty)
                            {
                                if (!TextureReplacer.doCheckMD5_keys.Contains(text2))
                                {
                                    if (TextureReplacer.unique_keys.ContainsKey(text2))
                                    {
                                        TextureReplacer.unique_keys.Remove(text2);
                                        TextureReplacer.doCheckMD5_keys.Add(text2);
                                    }
                                    else
                                    {
                                        TextureReplacer.unique_keys.Add(text2, text);
                                    }
                                }
                            }
                            else if (!TextureReplacer.unique_keys.ContainsKey(text2))
                            {
                                TextureReplacer.unique_keys.Add(text2, text);
                            }
                        }
                    }
                    else if (select_str == string.Empty)
                    {
                        if (!TextureReplacer.doCheckMD5_keys.Contains(text2))
                        {
                            if (TextureReplacer.unique_keys.ContainsKey(text2))
                            {
                                TextureReplacer.unique_keys.Remove(text2);
                                TextureReplacer.doCheckMD5_keys.Add(text2);
                            }
                            else
                            {
                                TextureReplacer.unique_keys.Add(text2, text);
                            }
                        }
                    }
                    else if (!TextureReplacer.unique_keys.ContainsKey(text2))
                    {
                        TextureReplacer.unique_keys.Add(text2, text);
                    }
                    if (select_str != string.Empty)
                    {
                        if (fileNameWithoutExtension.Length > num + 1 && fileNameWithoutExtension.Substring(num + 1).Length == 32)
                        {
                            text2 = fileNameWithoutExtension;
                        }
                        TextureReplacer.SelectTexture selectTexture = TextureReplacer.select_textures.Find((TextureReplacer.SelectTexture x) => x.SelectName == select_str);
                        if (selectTexture == null)
                        {
                            TextureReplacer.select_textures.Add(new TextureReplacer.SelectTexture(select_str));
                            selectTexture = TextureReplacer.select_textures.Find((TextureReplacer.SelectTexture x) => x.SelectName == select_str);
                        }
                        if (selectTexture != null && !selectTexture.IsExistItemKey(text2))
                        {
                            selectTexture.items[text2] = text;
                            flag = false;
                        }
                    }
                }
                if (TextureReplacer.select_textures.Count<TextureReplacer.SelectTexture>() > 0)
                {
                    TextureReplacer.select_textures_index = 0;
                    TextureReplacer.ChangeTextures(TextureReplacer.select_textures_index);
                }
            }catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            RenderTexture active = RenderTexture.active;
            Graphics.Blit(source, temporary);
            RenderTexture.active = temporary;
            Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }

        private bool DumpFile(Texture2D tex, string tex_name)
        {
            bool flag = false;
            string text = Path.Combine(TextureReplacer.imagesDumpPath, Application.loadedLevel.ToString());
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            bool flag2 = TextureReplacer.IsReadable(tex);
            string text2 = Path.Combine(text, tex_name + "_" + TextureReplacer.MakeCheckSum(tex, new bool?(flag2)) + ".png");
            if (!File.Exists(text2))
            {
                byte[] array;
                if (flag2)
                {
                    array = tex.EncodeToPNG();
                }
                else
                {
                    Texture2D texture2D = TextureReplacer.duplicateTexture(tex);
                    array = texture2D.EncodeToPNG();
                    UnityEngine.Object.DestroyImmediate(texture2D);
                }
                File.WriteAllBytes(text2, array);
                flag = true;
            }
            return flag;
        }

        private static string MakeCheckSum(Texture2D source, bool? Readable = null)
        {
            MD5 md = MD5.Create();
            if (Readable == null)
            {
                Readable = new bool?(TextureReplacer.IsReadable(source));
            }
            bool? flag = Readable;
            bool flag2 = true;
            if ((flag.GetValueOrDefault() == flag2) & (flag != null))
            {
                byte[] rawTextureData = source.GetRawTextureData();
                if (rawTextureData != null)
                {
                    byte[] array = new byte[4];
                    for (int i = 0; i < rawTextureData.Length; i += 16)
                    {
                        md.TransformBlock(rawTextureData, i, 4, array, 0);
                    }
                    md.TransformFinalBlock(array, 0, 0);
                }
            }
            flag = Readable;
            flag2 = false;
            if ((flag.GetValueOrDefault() == flag2) & (flag != null))
            {
                RenderTexture temporary = RenderTexture.GetTemporary(64, 64, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
                temporary.DiscardContents();
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = temporary;
                temporary.filterMode = FilterMode.Point;
                Graphics.Blit(source, temporary);
                Texture2D texture2D = new Texture2D(64, 64, TextureFormat.ARGB32, false, false);
                texture2D.filterMode = FilterMode.Point;
                texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
                texture2D.Apply(true);
                RenderTexture.active = active;
                float[] array2 = new float[4];
                int[,] array3 = new int[3, 4096];
                float num = 6f;
                float[] array4 = new float[3];
                for (int j = 0; j < 64; j++)
                {
                    for (int k = 0; k < 64; k++)
                    {
                        array4[0] = num * texture2D.GetPixel(k, j).r;
                        array4[1] = num * texture2D.GetPixel(k, j).g;
                        array4[2] = num * texture2D.GetPixel(k, j).b;
                        for (int l = 0; l < 3; l++)
                        {
                            array3[l, k * j] = (int)Math.Round((double)array4[l]);
                            array2[l] += array4[l];
                        }
                    }
                }
                for (int m = 0; m < 3; m++)
                {
                    if (array2[m] > 0f)
                    {
                        array2[m] /= 4096f;
                    }
                }
                byte[] array5 = new byte[4096];
                for (int n = 0; n < 64; n++)
                {
                    for (int num2 = 0; num2 < 64; num2++)
                    {
                        byte b = 0;
                        if (array2[0] < (float)array3[0, num2 * n])
                        {
                            b |= 1;
                        }
                        if (array2[1] < (float)array3[1, num2 * n])
                        {
                            b |= 2;
                        }
                        if (array2[2] < (float)array3[2, num2 * n])
                        {
                            b |= 4;
                        }
                        array5[num2 * n] = b;
                    }
                }
                md.ComputeHash(array5);
                UnityEngine.Object.Destroy(texture2D);
                RenderTexture.ReleaseTemporary(temporary);
            }
            string text = "";
            if (md.Hash != null)
            {
                text = BitConverter.ToString(md.Hash).Replace("-", "").ToLower();
            }
            return text;
        }

        private static bool IsReadable(Texture2D tex)
        {
            return false;
        }

        private static void ReplaceFile(Texture2D tex, string path)
        {
            tex.LoadImage(File.ReadAllBytes(path));
        }

        private static string toValidFilename(string fileName)
        {
            foreach (char c in TextureReplacer.invalid)
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private static void ChangeTextures(int index)
        {
            foreach (KeyValuePair<string, string> keyValuePair in TextureReplacer.select_textures[index].items)
            {
                if (TextureReplacer.images.ContainsKey(keyValuePair.Key))
                {
                    TextureReplacer.images[keyValuePair.Key] = keyValuePair.Value;
                }
                else if (TextureReplacer.unique_keys.ContainsKey(keyValuePair.Key))
                {
                    TextureReplacer.unique_keys[keyValuePair.Key] = keyValuePair.Value;
                }
            }
            TextureReplacer.TextureIndex = -1;
            TextureReplacer.need_clear_checked_instanceID = true;
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                array = memoryStream.ToArray();
            }
            return array;
        }

        public void OnGUI()
        {
            if (TextureReplacer.DoingReplace)
            {
                return;
            }
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                if (Keyboard.current.f11Key.wasPressedThisFrame && Keyboard.current.altKey.wasPressedThisFrame)
                {
                    TextureReplacer.bOnDumping = !TextureReplacer.bOnDumping;
                    TextureReplacer.toast_txt = "Dumping mode " + (TextureReplacer.bOnDumping ? "ON" : "OFF");
                    TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                    TextureReplacer.lastTextureCount = 0;
                    TextureReplacer.TextureIndex = -1;
                }
                else if (TextureReplacer.select_textures_index > -1 && Keyboard.current.f12Key.wasPressedThisFrame)
                {
                    if (TextureReplacer.DoingReplace)
                    {
                        TextureReplacer.toast_txt = "In changing textures. please try later.";
                    }
                    else
                    {
                        TextureReplacer.select_textures_index = ((TextureReplacer.select_textures_index < TextureReplacer.select_textures.Count<TextureReplacer.SelectTexture>() - 1) ? (TextureReplacer.select_textures_index + 1) : 0);
                        TextureReplacer.ChangeTextures(TextureReplacer.select_textures_index);
                        TextureReplacer.toast_txt = "Change texture set to " + TextureReplacer.select_textures[TextureReplacer.select_textures_index].SelectName;
                    }
                    TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                }
            }
            if (TextureReplacer.dtStartToast != null)
            {
                //GUI.Button(new Rect(10f, 10f, (float)Screen.width - 20f, 20f), "\n" + TextureReplacer.toast_txt + "\n");
                if (DateTime.Now - TextureReplacer.dtStartToast > new TimeSpan(0, 0, 0, 2))
                {
                    TextureReplacer.dtStartToast = null;
                }
            }
            int num = TextureReplacer.TextureIndex + 1;
            if (TextureReplacer.need_clear_checked_instanceID)
            {
                TextureReplacer.checked_instanceID.Clear();
                TextureReplacer.need_clear_checked_instanceID = false;
            }
            if (TextureReplacer.OldSceneId != null)
            {
                int? oldSceneId = TextureReplacer.OldSceneId;
                int loadedLevel = Application.loadedLevel;
                if ((oldSceneId.GetValueOrDefault() == loadedLevel) & (oldSceneId != null))
                {
                    if (TextureReplacer.next_check != null && TextureReplacer.TextureCount > 0 && TextureReplacer.TextureIndex >= 0 && TextureReplacer.TextureCount > TextureReplacer.TextureIndex)
                    {
                        DateTime now = DateTime.Now;
                        if (TextureReplacer.TextureIndex >= TextureReplacer.TextureCount)
                        {
                            TextureReplacer.TextureIndex = TextureReplacer.TextureCount;
                            return;
                        }
                        if (!TextureReplacer.bOnDumping)
                        {
                            for (; ; )
                            {
                                try
                                {
                                    string text = TextureReplacer.toValidFilename(TextureReplacer.Textures[TextureReplacer.TextureIndex].name);
                                    if (text.Length > 0 && TextureReplacer.Textures[TextureReplacer.TextureIndex].width > 16 && TextureReplacer.Textures[TextureReplacer.TextureIndex].height > 16)
                                    {
                                        int instanceID = TextureReplacer.Textures[TextureReplacer.TextureIndex].GetInstanceID();
                                        if (text.Length > 0 && TextureReplacer.checked_instanceID.IndexOf(instanceID) < 0)
                                        {
                                            TextureReplacer.checked_instanceID.Add(instanceID);
                                            if (TextureReplacer.unique_keys.ContainsKey(text))
                                            {
                                                TextureReplacer.ReplaceFile(TextureReplacer.Textures[TextureReplacer.TextureIndex], TextureReplacer.unique_keys[text]);
                                            }
                                            else if (TextureReplacer.doCheckMD5_keys.Contains(text))
                                            {
                                                string text2 = text + "_" + TextureReplacer.MakeCheckSum(TextureReplacer.Textures[TextureReplacer.TextureIndex], null);
                                                if (TextureReplacer.select_textures_index > -1)
                                                {
                                                    while (TextureReplacer.changed.ContainsKey(text2) && !(text2 == TextureReplacer.changed[text2]))
                                                    {
                                                        text2 = TextureReplacer.changed[text2];
                                                    }
                                                }
                                                
                                                if (TextureReplacer.images.ContainsKey(text2))
                                                {
                                                    TextureReplacer.ReplaceFile(TextureReplacer.Textures[TextureReplacer.TextureIndex], TextureReplacer.images[text2]);
                                                    if (TextureReplacer.select_textures_index > -1)
                                                    {
                                                        TextureReplacer.changed.Add(text + "_" + TextureReplacer.MakeCheckSum(TextureReplacer.Textures[TextureReplacer.TextureIndex], null), text2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    TextureReplacer.TextureIndex = -2;
                                }
                                catch (Exception)
                                {
                                }
                                TextureReplacer.TextureIndex++;
                                if (TextureReplacer.TextureIndex <= -1 || TextureReplacer.TextureIndex >= TextureReplacer.TextureCount)
                                {
                                    break;
                                }
                                if (!(DateTime.Now - now < this.LoopTimeLimit))
                                {
                                    return;
                                }
                            }
                            return;
                        }
                        if (!TextureReplacer.IsDoingDump)
                        {
                            if (TextureReplacer.lastTextureCount != TextureReplacer.TextureCount)
                            {
                                TextureReplacer.lastTextureCount = TextureReplacer.TextureCount;
                                TextureReplacer.DoDump = true;
                            }
                            if (TextureReplacer.DoDump)
                            {
                                TextureReplacer.IsDoingDump = true;
                                do
                                {
                                    try
                                    {
                                        string text3 = TextureReplacer.toValidFilename(TextureReplacer.Textures[TextureReplacer.TextureIndex].name);
                                        if (TextureReplacer.TextureIndex < TextureReplacer.TextureCount && text3.Length > 0 && TextureReplacer.Textures[TextureReplacer.TextureIndex].width > 16 && TextureReplacer.Textures[TextureReplacer.TextureIndex].height > 16)
                                        {
                                            int num2 = num * 100 / TextureReplacer.TextureCount;
                                            TextureReplacer.toast_txt = string.Format("Checking {0} / {1} {2} [{3}%]", new object[]
                                            {
                                                num,
                                                TextureReplacer.TextureCount,
                                                text3,
                                                num2
                                            });
                                            TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                                            if (this.DumpFile(TextureReplacer.Textures[TextureReplacer.TextureIndex], text3))
                                            {
                                                TextureReplacer.toast_txt = string.Format("Dumped: {0} [{1}%]", text3, num2);
                                                TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                                            }
                                            else
                                            {
                                                TextureReplacer.toast_txt = string.Format("Passed: {0} [{1}%]", text3, num2);
                                                TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                                            }
                                        }
                                    }
                                    catch (NullReferenceException)
                                    {
                                        TextureReplacer.TextureIndex = -2;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.LogError(ex);
                                    }
                                    TextureReplacer.TextureIndex++;
                                    num = TextureReplacer.TextureIndex + 1;
                                }
                                while (TextureReplacer.TextureIndex > -1 && TextureReplacer.TextureIndex < TextureReplacer.TextureCount && DateTime.Now - now < this.LoopTimeLimit);
                                if (TextureReplacer.TextureIndex >= TextureReplacer.TextureCount)
                                {
                                    TextureReplacer.DoDump = false;
                                    TextureReplacer.toast_txt = "Scene dumping completed.";
                                    TextureReplacer.dtStartToast = new DateTime?(DateTime.Now);
                                }
                                TextureReplacer.IsDoingDump = false;
                                return;
                            }
                        }
                    }
                    else if (TextureReplacer.next_check == null || TextureReplacer.next_check < DateTime.Now)
                    {
                        TextureReplacer.Textures = Resources.FindObjectsOfTypeAll<Texture2D>();
                        if (TextureReplacer.Textures == null)
                        {
                            TextureReplacer.Textures = new Texture2D[0];
                        }
                        TextureReplacer.TextureCount = TextureReplacer.Textures.Count<Texture2D>();
                        if (TextureReplacer.TextureCount > 0)
                        {
                            TextureReplacer.TextureIndex = 0;
                        }
                        TextureReplacer.next_check = new DateTime?(DateTime.Now + TextureReplacer.check_span);
                    }
                    return;
                }
            }
            TextureReplacer.lastTextureCount = 0;
            TextureReplacer.need_clear_checked_instanceID = true;
            TextureReplacer.next_check = null;
            TextureReplacer.OldSceneId = new int?(Application.loadedLevel);
        }
        public static Dictionary<string, string> images = new Dictionary<string, string>();
        public static Dictionary<string, string> unique_keys = new Dictionary<string, string>();
        public static List<string> doCheckMD5_keys = new List<string>();
        public static List<int> checked_instanceID = new List<int>();
        public static bool need_clear_checked_instanceID = false;
        public static Dictionary<string, string> changed = new Dictionary<string, string>();
        public static int select_textures_index = -1;
        public static int? OldSceneId = null;
        public static int lastTextureCount = 0;
        public static bool DoDump = false;
        public static bool IsDoingDump = false;
        public TimeSpan LoopTimeLimit = new TimeSpan(0, 0, 0, 0, 1);
        public static DateTime? next_check = null;
        public static TimeSpan check_span = new TimeSpan(0, 0, 0, 2);
        public static List<TextureReplacer.SelectTexture> select_textures = new List<TextureReplacer.SelectTexture>();
        public static string imagesPath_str = "";
        public static string imagesDumpPath_str = "LCKR_Tex_Dump";
        public static string select_folder = "select";
        public static Texture2D[] Textures = new Texture2D[0];
        public static int TextureIndex = -1;
        public static int TextureCount = -1;
        public static bool bOnDumping = false;
        public static char[] invalid = Path.GetInvalidFileNameChars();
        public static bool CanUseThread = true;
        public static bool DoingReplace = false;
        public static string toast_txt = "";

        public static DateTime? dtStartToast = null;

        public class SelectTexture
        {
            public SelectTexture(string name = "")
            {
                this.SelectName = name;
                this.items = new Dictionary<string, string>();
            }

            public bool IsExistItemKey(string itemkey)
            {
                return this.items.ContainsKey(itemkey);
            }

            public string SelectName;
            public Dictionary<string, string> items;
        }
    }