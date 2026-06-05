using Data.NonLodable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Manager.Contents
{
    public class SaveDataManager
    {
        private string _saveDataPath;
        private string _settingDataPath;
        private GameSaveData[] _gameData;
        private GameSetting _setting;

        public void Init()
        {
            _saveDataPath = Path.Combine(Application.persistentDataPath, "SaveFile.json");
            _settingDataPath = Path.Combine(Application.persistentDataPath, "SettingSaveFile.json");

            if(File.Exists(_settingDataPath) == false)
            {
                _setting = new GameSetting();
                SaveSettings();
            }

            if (File.Exists(_saveDataPath) == false)
            {
                _gameData = new GameSaveData[6];
                SaveSaveData();
            }

            string raw = File.ReadAllText(_settingDataPath);
            _setting = JsonConvert.DeserializeObject<GameSetting>(raw);

            raw = File.ReadAllText(_saveDataPath);
            _gameData = JsonConvert.DeserializeObject<GameSaveData[]>(raw);

        }

        public void SaveSettings()
        {
            string path = _settingDataPath + ".tmp";
            File.WriteAllText(path, JsonConvert.SerializeObject(_setting));
            if(File.Exists(_settingDataPath))
            {
                File.Replace(path, _settingDataPath, _settingDataPath + ".backup");
            }
            else
            {
                File.Move(path, _settingDataPath);
            }
        }

        public void SaveSaveData()
        {
            string path = _saveDataPath + ".tmp";
            File.WriteAllText(path, JsonConvert.SerializeObject(_gameData));
            if (File.Exists(_saveDataPath))
            {
                File.Replace(path, _saveDataPath, _saveDataPath + ".backup");
            }
            else
            {
                File.Move(path, _saveDataPath);
            }
        }

    }
}
