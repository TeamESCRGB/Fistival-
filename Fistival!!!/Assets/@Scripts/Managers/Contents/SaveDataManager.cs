using Data.NonLodable;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace Manager.Contents
{
    public class SaveDataManager
    {
        private string _saveDataPath;
        private string _settingDataPath;
        private GameSaveData[] _gameData;
        private GameSetting _setting;
        private int _selectedGameFile=0;
        private int _selectedSaveFile=0;

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

            SelectGameFile(0);
            SelectSaveFile(0);
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

        public bool SelectGameFile(int idx)
        {
            if(idx < 0 || _gameData.Length <= idx)
            {
                return false;
            }

            if (_gameData[idx] is null)
            {
                _gameData[idx] = new GameSaveData();
            }

            _selectedGameFile = idx;

            return true;
        }

        public bool SelectSaveFile(int idx)
        {
            if (_gameData[_selectedGameFile] is null)
            {
                return false;
            }

            if(idx < 0 || _gameData[_selectedGameFile].SaveData.Length <= idx)
            {
                return false;
            }

            if (_gameData[_selectedGameFile].SaveData[idx] is null)
            {
                _gameData[_selectedGameFile].SaveData[idx] = new SaveFile();
            }

            _selectedSaveFile = idx;

            return true;
        }

        public GameSetting GetGameSettingRef()
        {
            return _setting;
        }

        public int GetSelectedFileIDX()
        {
            return _selectedGameFile;
        }

        public int GetSelectedSaveIDX()
        {
            return _selectedSaveFile;
        }

        public SaveFile GetSaveFileData()
        {
            return _gameData[_selectedGameFile].SaveData[_selectedSaveFile]; //null이 나오는 경우는 없도록 함. 선택할 때 null이면 값을 생성해주니까
        }
    }
}