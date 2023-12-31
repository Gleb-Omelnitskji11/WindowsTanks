using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Game.SavingService
{
    public static class JsonDataService
    {
        private const string Key = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg6l1DVdwN8=";
        private const string Iv = "JZuM0HQsWSBVpRHTeRZMYQ==";

        public static void SaveData<T>(string relativePath, T data)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing file for the first time!");
                }

                using FileStream stream = File.Create(path);
                WriteEncryptedData(data, stream);
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            }
        }

        private static void WriteEncryptedData<T>(T data, FileStream stream)
        {
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(Key);
            aesProvider.IV = Convert.FromBase64String(Iv);
            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

            string json = JsonUtility.ToJson(data, true);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            cryptoStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        public static T LoadData<T>(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            if (!File.Exists(path))
            {
                Debug.LogError($"Cannot load file at {path}. File does not exist!");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try
            {
                T data = ReadEncryptedData<T>(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        private static T ReadEncryptedData<T>(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            using Aes aesProvider = Aes.Create();

            aesProvider.Key = Convert.FromBase64String(Key);
            aesProvider.IV = Convert.FromBase64String(Iv);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            using MemoryStream decryptionStream = new MemoryStream(fileBytes);

            using CryptoStream cryptoStream = new CryptoStream(
                decryptionStream, cryptoTransform, CryptoStreamMode.Read);

            using StreamReader reader = new StreamReader(cryptoStream);

            string result = reader.ReadToEnd();

            Debug.Log($"Decrypted result (if the following is not legible, probably wrong key or iv): {result}");
            return JsonUtility.FromJson<T>(result);
        }
    }
}