using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace LCKorean.Patches
{
    public class TextureReplacer : MonoBehaviour
    {
        private static TextureReplacer _instance;
        public static string imagesPath = "";
        public static string imagesPath_str = "LCKR_Tex";

        // Logic Variables
        public static Dictionary<string, string> images = new Dictionary<string, string>();
        public static Dictionary<string, string> unique_keys = new Dictionary<string, string>();
        public static List<string> doCheckMD5_keys = new List<string>();
        public static HashSet<int> checked_instanceID = new HashSet<int>(); // List -> HashSet 권장

        // Optimization
        public TimeSpan LoopTimeLimit = new TimeSpan(0, 0, 0, 0, 1);
        public static DateTime? next_check = null;
        public static TimeSpan check_span = new TimeSpan(0, 0, 0, 2);

        public static Texture2D[] Textures = new Texture2D[0];
        public static int TextureIndex = -1;
        public static int TextureCount = -1;
        public static char[] invalid = Path.GetInvalidFileNameChars();

        private static bool scanRequested = true; // 최초 1회 스캔

        public static void Setup()
        {
            if (_instance != null) return;

            GameObject gameObject = new GameObject("TextureReplacer");
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            _instance = gameObject.AddComponent<TextureReplacer>();

            string pluginFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            imagesPath = Path.Combine(pluginFolderPath, imagesPath_str);

            getImages();

            // 씬 로드 때마다 “스캔 요청”만 걸어두고, 실제 처리는 Update에서 시간분할 처리
            SceneManager.sceneLoaded += (_, __) =>
            {
                scanRequested = true;
                next_check = null;
                // 여기서 checked_instanceID를 Clear하지 않음 (핵심)
            };
        }

        private static void getImages()
        {
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            images = new Dictionary<string, string>();
            unique_keys = new Dictionary<string, string>();
            doCheckMD5_keys = new List<string>();

            string[] files = Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories);
            Array.Sort(files);

            foreach (string path in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string key = fileNameWithoutExtension;

                if (!images.ContainsKey(fileNameWithoutExtension))
                    images.Add(fileNameWithoutExtension, path);

                int num = fileNameWithoutExtension.LastIndexOf('_');
                if (num > 0)
                {
                    string baseName = fileNameWithoutExtension.Substring(0, num);
                    string suffix = (fileNameWithoutExtension.Length > num + 1)
                        ? fileNameWithoutExtension.Substring(num + 1)
                        : "";

                    // MD5-like (32 chars)
                    if (suffix.Length == 32)
                    {
                        if (!doCheckMD5_keys.Contains(baseName))
                            doCheckMD5_keys.Add(baseName);

                        if (!unique_keys.ContainsKey(baseName))
                            unique_keys.Add(baseName, path);
                    }
                    else if (!unique_keys.ContainsKey(baseName))
                    {
                        unique_keys.Add(baseName, path);
                    }
                }
                else if (!unique_keys.ContainsKey(key))
                {
                    unique_keys.Add(key, path);
                }
            }
        }

        private static string toValidFilename(string fileName)
        {
            foreach (char c in invalid)
                fileName = fileName.Replace(c, '_');
            return fileName;
        }

        private static void ReplaceFile(Texture2D tex, string path)
        {
            tex.LoadImage(File.ReadAllBytes(path));
        }

        private static string MakeCheckSum(Texture2D source)
        {
            using (MD5 md = MD5.Create())
            {
                RenderTexture temporary = RenderTexture.GetTemporary(64, 64, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
                temporary.DiscardContents();

                RenderTexture active = RenderTexture.active;
                RenderTexture.active = temporary;

                temporary.filterMode = FilterMode.Point;
                Graphics.Blit(source, temporary);

                Texture2D texture2D = new Texture2D(64, 64, TextureFormat.ARGB32, false, false);
                texture2D.filterMode = FilterMode.Point;
                texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
                texture2D.Apply(true);

                RenderTexture.active = active;

                float[] avg = new float[3];
                int[] q = new int[64 * 64 * 3];
                float num = 6f;

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        int idx = x + y * 64;
                        Color pixel = texture2D.GetPixel(x, y);

                        float r = num * pixel.r;
                        float g = num * pixel.g;
                        float b = num * pixel.b;

                        q[idx * 3 + 0] = (int)Math.Round(r);
                        q[idx * 3 + 1] = (int)Math.Round(g);
                        q[idx * 3 + 2] = (int)Math.Round(b);

                        avg[0] += r;
                        avg[1] += g;
                        avg[2] += b;
                    }
                }

                avg[0] /= 4096f;
                avg[1] /= 4096f;
                avg[2] /= 4096f;

                byte[] bits = new byte[4096];
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        int idx = x + y * 64;
                        byte b = 0;

                        if (avg[0] < q[idx * 3 + 0]) b |= 1;
                        if (avg[1] < q[idx * 3 + 1]) b |= 2;
                        if (avg[2] < q[idx * 3 + 2]) b |= 4;

                        bits[idx] = b;
                    }
                }

                byte[] hash = md.ComputeHash(bits);

                UnityEngine.Object.Destroy(texture2D);
                RenderTexture.ReleaseTemporary(temporary);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private void Update()
        {
            // 스캔 요청이 없으면 아무 것도 안 함
            if (!scanRequested) return;

            // Initialize Search (주기 제한)
            if (next_check == null || next_check < DateTime.Now)
            {
                if (TextureIndex == -1 || TextureIndex >= TextureCount)
                {
                    Textures = Resources.FindObjectsOfTypeAll<Texture2D>() ?? Array.Empty<Texture2D>();
                    TextureCount = Textures.Length;
                    TextureIndex = (TextureCount > 0) ? 0 : -1;

                    next_check = DateTime.Now + check_span;
                }
            }

            // Process Batch
            if (TextureCount <= 0 || TextureIndex < 0 || TextureIndex >= TextureCount)
            {
                // 이번 라운드 스캔이 끝났으면 요청 해제
                scanRequested = false;
                return;
            }

            DateTime start = DateTime.Now;

            while (TextureIndex < TextureCount)
            {
                try
                {
                    Texture2D tex = Textures[TextureIndex];
                    if (tex != null && tex.width > 16 && tex.height > 16)
                    {
                        int instanceID = tex.GetInstanceID();
                        if (!checked_instanceID.Contains(instanceID))
                        {
                            checked_instanceID.Add(instanceID);

                            string safeName = toValidFilename(tex.name);
                            if (!string.IsNullOrEmpty(safeName))
                            {
                                if (unique_keys.TryGetValue(safeName, out var path))
                                {
                                    ReplaceFile(tex, path);
                                }
                                else if (doCheckMD5_keys.Contains(safeName))
                                {
                                    string checksumName = safeName + "_" + MakeCheckSum(tex);
                                    if (images.TryGetValue(checksumName, out var md5Path))
                                        ReplaceFile(tex, md5Path);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore errors during iteration
                }

                TextureIndex++;

                // Time Slicing
                if ((DateTime.Now - start) >= LoopTimeLimit)
                    return;
            }

            // 스캔 완료
            scanRequested = false;
        }
    }
}
