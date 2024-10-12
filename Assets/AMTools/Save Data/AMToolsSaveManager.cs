using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System;

namespace AMTools.AMToolsSaveManager
{
    public static class AMToolsSaveManager
    {
        private static string _saveDirectoryName = "Saves/";
        private static string _saveFileName = "save.lsd";
        private static bool _prettyJson = false;
        private static bool _useEncryption = true;
        private static SaveDataClass _saveDataClassScript;
        private static string _saveFolderPath;
        private static FileStream _dataStream;
        private static readonly byte[] _savedKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };

        private static String SaveFilePath()
        {
            string _saveFilePath;
            
            if(Application.isEditor)
            {
                //Debug.Log("In Unity Editor");
                _saveFolderPath = Path.Combine(Application.dataPath, _saveDirectoryName);
            }
            else
            {
                //Debug.Log("In Standalone Build");
                _saveFolderPath = Path.Combine(Application.persistentDataPath, _saveDirectoryName);
            }

            return _saveFilePath = Path.Combine(_saveFolderPath, _saveFileName);
        }

        private static void InitializeSaveDataClass()
        {
            _saveDataClassScript = new SaveDataClass();
            _saveDataClassScript._saveDataIntList = new List<SaveDataClass.SaveDataInt>();
            _saveDataClassScript._saveDataFloatList = new List<SaveDataClass.SaveDataFloat>();
            _saveDataClassScript._saveDataBoolList = new List<SaveDataClass.SaveDataBool>();
            _saveDataClassScript._saveDataStringList = new List<SaveDataClass.SaveDataString>();
        }

        private static void CheckCreateDirectoryFile()
        {
            string _saveFilePath = SaveFilePath();

            if(!Directory.Exists(_saveFolderPath))
            {
                Debug.LogWarning("No save directory found, creating one!");
                Directory.CreateDirectory(_saveFolderPath);
            }

            if(!File.Exists(_saveFilePath))
            {
                Debug.LogWarning("No save file found, creating one!");
                //DISPOSING IS VERY IMPORTANT!!!!!!
                File.Create(_saveFilePath).Dispose();
            }
        }

        public static void SaveDataToJson()
        {
            CheckCreateDirectoryFile();

            if(_saveDataClassScript == null)
            {
                Debug.Log("Save data class is null, initializing so it can be saved to.");
                InitializeSaveDataClass();
            }

            if(_useEncryption == true)
            {
                //Save encrypted.
                
                //Get the save data path.
                string _saveFilePath = SaveFilePath();

                //Create a new AES instance.
                Aes _iAes = Aes.Create();

                //Create a FileStream for creating files.
                _dataStream = new FileStream(_saveFilePath, FileMode.Create);

                //Save the new generated IV.
                byte[] _inputIV = _iAes.IV;

                //Write the IV to the FileStream unencrypted.
                _dataStream.Write(_inputIV, 0, _inputIV.Length);

                //Create a CryptoStream, wrapping FileStream.
                CryptoStream _iStream = new CryptoStream
                (
                    _dataStream,
                    _iAes.CreateEncryptor(_savedKey, _iAes.IV),
                    CryptoStreamMode.Write
                );

                //Create a StreamWriter, wrapping CryptoStream.
                StreamWriter _sWriter = new StreamWriter(_iStream);

                string _sOToJsonEncrypted = JsonUtility.ToJson(_saveDataClassScript, _prettyJson);

                //Write to the innermost stream which will encrypt.
                _sWriter.Write(_sOToJsonEncrypted);

                Debug.Log("Saved encrypted to " + _saveFilePath + "!");

                //Close StreamWriter.
                _sWriter.Close();

                //Close CryptoStream.
                _iStream.Close();

                //Close FileSteam.
                _dataStream.Close();
            }
            else
            {
                //Save Unencrypted
                string _saveFilePath = SaveFilePath();
                
                string _sOToJson = JsonUtility.ToJson(_saveDataClassScript, _prettyJson);

                File.WriteAllText(_saveFilePath, _sOToJson);

                Debug.Log("Saved to " + _saveFilePath + "!");
            }
        }

        public static void JsonToSaveData()
        {
            InitializeSaveDataClass();
            
            string _saveFilePath = SaveFilePath();

            if(File.Exists(_saveFilePath))
            {
                string _saveFileContents = File.ReadAllText(_saveFilePath);

                if(_saveFileContents.Length == 0)
                {
                    Debug.Log("Save file is empty, if read it will break everything :).");
                    return;
                }
                else
                {  
                    if(_useEncryption == true)
                    {
                        //Debug.Log("Reading Encrypted Save File");
                        //Create a new filestream for opening the save file in the path.
                        _dataStream = new FileStream(_saveFilePath, FileMode.Open);

                        //Crease a new AES instance.
                        Aes _oAes = Aes.Create();

                        //Create an array of bytes the length of the AES IV.
                        byte[] _outputIV = new byte[_oAes.IV.Length];

                        //Read the iV from the file.
                        _dataStream.Read(_outputIV, 0, _outputIV.Length);

                        //Create a CryptoStream, wrappuing FileStream
                        CryptoStream _oStream = new CryptoStream
                        (
                            _dataStream,
                            _oAes.CreateDecryptor(_savedKey, _outputIV),
                            CryptoStreamMode.Read
                        );

                        //Create a StreamReader, wrapping CryptoStream
                        StreamReader _reader = new StreamReader(_oStream);

                        //Read the entire file into a string.
                        string _readStream = _reader.ReadToEnd();

                        //Always close a stream after usage.
                        _reader.Close();

                        //Clost the filestream
                        _dataStream.Close();

                        JsonUtility.FromJsonOverwrite(_readStream, _saveDataClassScript);

                        Debug.Log("Loaded encrypted from " + _saveFilePath + "!");
                    }
                    else
                    {
                        //Debug.Log("Reading Unencrypted Save File");
                        string _jsonToSO = File.ReadAllText(_saveFilePath);

                        JsonUtility.FromJsonOverwrite(_jsonToSO, _saveDataClassScript);

                        Debug.Log("Loaded from " + _saveFilePath + "!");
                    }
                }
            }
            else
            {
                Debug.Log("Cannot read a file that does not exist");
            }
        }

        public static void DeleteAll()
        {
            string _saveFilePath = SaveFilePath();

            _saveDataClassScript._saveDataIntList.Clear();
            _saveDataClassScript._saveDataFloatList.Clear();
            _saveDataClassScript._saveDataBoolList.Clear();
            _saveDataClassScript._saveDataStringList.Clear();
            File.Delete(_saveFilePath);

            InitializeSaveDataClass();
        }

        public static void SaveInt(string _funcKey, int _funcValue)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            if(_saveDataClassScript._saveDataIntList.Count == 0)
            {
                SaveDataClass.SaveDataInt _newIntData = new SaveDataClass.SaveDataInt { _key = _funcKey, _int = _funcValue };
                _saveDataClassScript._saveDataIntList.Add(_newIntData);
                //Debug.Log("Nothing in save data list, added new int.");
            }
            else
            {
                foreach(SaveDataClass.SaveDataInt _saveDataInt in _saveDataClassScript._saveDataIntList)
                {
                    if(_saveDataInt._key == _funcKey)
                    {
                        _saveDataInt._int = _funcValue;
                        //Debug.Log("Updated existing int");
                        //Return stops the code here.
                        return;
                    }
                }

                SaveDataClass.SaveDataInt _newIntData = new SaveDataClass.SaveDataInt { _key = _funcKey, _int = _funcValue };
                _saveDataClassScript._saveDataIntList.Add(_newIntData);
                //Debug.Log("Nothing in save data list, added new int.");
            }
        }

        public static int GetInt(string _fucnKey)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            foreach(SaveDataClass.SaveDataInt _saveDataInt in _saveDataClassScript._saveDataIntList)
            {
                if(_saveDataInt._key == _fucnKey)
                {
                    return _saveDataInt._int;
                }
            }

            //Debug.Log("Key ' " + _fucnKey + " ' does not exist, returning default int value.");
            return 0;
        }

        public static void SaveFloat(string _funcKey, float _funcValue)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            if(_saveDataClassScript._saveDataFloatList.Count == 0)
            {
                SaveDataClass.SaveDataFloat _newFloatData = new SaveDataClass.SaveDataFloat { _key = _funcKey, _float = _funcValue };
                _saveDataClassScript._saveDataFloatList.Add(_newFloatData);
                //Debug.Log("Nothing in save data list, added new float.");
            }
            else
            {
                foreach(SaveDataClass.SaveDataFloat _saveDataFloat in _saveDataClassScript._saveDataFloatList)
                {
                    if(_saveDataFloat._key == _funcKey)
                    {
                        _saveDataFloat._float = _funcValue;
                        //Debug.Log("Updated existing float");
                        return;
                    }
                }

                SaveDataClass.SaveDataFloat _newFloatData = new SaveDataClass.SaveDataFloat { _key = _funcKey, _float = _funcValue };
                _saveDataClassScript._saveDataFloatList.Add(_newFloatData);
                //Debug.Log("Nothing in save data list, added new float.");
            }
        }

        public static float GetFloat(string _fucnKey)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            foreach(SaveDataClass.SaveDataFloat _saveDataFloat in _saveDataClassScript._saveDataFloatList)
            {
                if(_saveDataFloat._key == _fucnKey)
                {
                    return _saveDataFloat._float;
                }
            }

            //Debug.Log("Key ' " + _fucnKey + " ' does not exist, returning default float value.");
            return 0;
        }

        public static void SaveBool(string _funcKey, bool _funcValue)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }
            
            if(_saveDataClassScript._saveDataBoolList.Count == 0)
            {
                SaveDataClass.SaveDataBool _newBoolData = new SaveDataClass.SaveDataBool { _key = _funcKey, _bool = _funcValue };
                _saveDataClassScript._saveDataBoolList.Add(_newBoolData);
                //Debug.Log("Nothing in save data list, added new bool.");
            }
            else
            {
                foreach(SaveDataClass.SaveDataBool _saveDataBool in _saveDataClassScript._saveDataBoolList)
                {
                    if(_saveDataBool._key == _funcKey)
                    {
                        _saveDataBool._bool = _funcValue;
                        //Debug.Log("Updated existing bool");
                        return;
                    }
                }

                SaveDataClass.SaveDataBool _newBoolData = new SaveDataClass.SaveDataBool { _key = _funcKey, _bool = _funcValue };
                _saveDataClassScript._saveDataBoolList.Add(_newBoolData);
                //Debug.Log("Nothing in save data list, added new bool.");
            }
        }

        public static bool GetBool(string _fucnKey)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            foreach(SaveDataClass.SaveDataBool _saveDataBool in _saveDataClassScript._saveDataBoolList)
            {
                if(_saveDataBool._key == _fucnKey)
                {
                    return _saveDataBool._bool;
                }
            }

            //Debug.Log("Key ' " + _fucnKey + " ' does not exist, returning false.");
            return false;
        }

        public static void SaveString(string _funcKey, string _funcValue)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }

            if(_saveDataClassScript._saveDataStringList.Count == 0)
            {
                SaveDataClass.SaveDataString _newStringData = new SaveDataClass.SaveDataString { _key = _funcKey, _string = _funcValue };
                _saveDataClassScript._saveDataStringList.Add(_newStringData);
                //Debug.Log("Nothing in save data list, added new string.");
            }
            else
            {
                foreach(SaveDataClass.SaveDataString _saveDataString in _saveDataClassScript._saveDataStringList)
                {
                    if(_saveDataString._key == _funcKey)
                    {
                        _saveDataString._string = _funcValue;
                        //Debug.Log("Updated existing string");
                        return;
                    }
                }

                SaveDataClass.SaveDataString _newStringData = new SaveDataClass.SaveDataString { _key = _funcKey, _string = _funcValue };
                _saveDataClassScript._saveDataStringList.Add(_newStringData);
                //Debug.Log("Nothing in save data list, added new string.");
            }
        }

        public static string GetString(string _fucnKey)
        {
            if(_saveDataClassScript == null)
            {
                InitializeSaveDataClass();
            }
            
            foreach(SaveDataClass.SaveDataString _saveDataString in _saveDataClassScript._saveDataStringList)
            {
                if(_saveDataString._key == _fucnKey)
                {
                    return _saveDataString._string;
                }
            }

            //Debug.Log("Key ' " + _fucnKey + " ' does not exist, returning ''.");
            return "";
        }
    }
}