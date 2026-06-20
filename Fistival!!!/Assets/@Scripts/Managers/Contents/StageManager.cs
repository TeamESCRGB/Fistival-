using Coordinator.Stages;
using Data;
using System;
using System.Collections.Generic;
using UI.Popup;
using UnityEngine;

namespace Manager.Contents
{
    public class StageManager
    {
        private int _stageIdx = -1;
        private StageData _data;

        private double _scaledTimeStart = 0;
        private double _unscaledTimeStart = 0;
        private List<int> _collection = new List<int>(5);
        private Dictionary<string, (StageSectionCoordinator chunk, string allocatedResources)> _spawnedChunks = new Dictionary<string, (StageSectionCoordinator chunk, string allocatedResources)>();

        private int _totalTakenDamage = 0;

        private int _life = 0;

        public void Init()
        {
            _stageIdx = -1;
            _data = null;
            _scaledTimeStart = 0;
            _unscaledTimeStart = 0;
            
            foreach(var chunk in _spawnedChunks.Values)
            {
                if(chunk.chunk != null)
                {
                    chunk.chunk.DeInitChunk();
                }
                Managers.Instance.ResourceManager.ReleaseIn(chunk.allocatedResources);
            }
            _life = 0;
            _totalTakenDamage = 0;
            _collection.Clear();
            _spawnedChunks.Clear();
        }

        public void CollectCollection(int collectionIdx)
        {
            if(Managers.Instance.DataManager.CollectionDataDict.ContainsKey(collectionIdx) && _collection.Contains(collectionIdx) == false)
            {
                _collection.Add(collectionIdx);
            }
        }

        public bool CanCollect(int collectionIdx)
        {
            return Managers.Instance.DataManager.CollectionDataDict.ContainsKey(collectionIdx) && _collection.Contains(collectionIdx) == false
                 && Managers.Instance.SaveDataManager.GetSaveFileData().StageSaveDatas[_stageIdx].CollectedCollections.Contains(collectionIdx) == false;
        }

        public void TrySyncToPlayerData(bool isCleared)
        {
            if(isCleared)
            {
                var save = Managers.Instance.SaveDataManager.GetSaveFileData();
                int money = 0;
                for(int i = 0; i < _collection.Count; i++)
                {
                    money += Managers.Instance.DataManager.CollectionDataDict[_collection[i]].Money;
                }

                save.StageSaveDatas[_stageIdx].TotalGainedDamage = _totalTakenDamage;
                save.PlayerSaveData.Money += money;
                save.StageSaveDatas[_stageIdx].CollectedCollections.AddRange(_collection);
                Managers.Instance.GameManager.GetClearedMapDictRef()[_stageIdx] = true;

                if(_scaledTimeStart < save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause)
                {
                    save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause = _scaledTimeStart;
                    save.StageSaveDatas[_stageIdx].ClearTimeWithPause = _unscaledTimeStart;
                }
            }
        }

        public void SaveCheckpoint()
        {
                
        }

        public void OnDead()
        {
            _life--;
            if(_life <= 0)
            {
                OnFail();
                return;
            }
            Respawn();
        }

        private void Respawn()
        {
            Debug.Log("리스폰");
        }

        private void OnFail()
        {
            //컷씬 넣어줘야됨
            ReturnToLobby();
        }

        public void OnClear()
        {
            TrySyncToPlayerData(true);
            Managers.Instance.UIManager.ShowPopupUI<BasicConfirmBox>("BasicConfirmBox").SetCallback(OnSaveYes, OnSaveNo).SetText("현 시점의 세이브를 저장하시겠습니까?");
        }



        private void OnSaveYes()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            Managers.Instance.SaveDataManager.SaveSaveData();
            ReturnToLobby();
        }

        private void OnSaveNo()
        {
            Managers.Instance.UIManager.ClosePopupUI();
            ReturnToLobby();
        }

        private void ReturnToLobby()
        {
            Init();
            Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (_, now, max) => {
                if (now == max)
                {
                    Managers.Instance.ResourceManager.ReleaseIn("GameSceneBasicLoaded");
                    Managers.Instance.SceneManagerEx.LoadScene(Defines.SceneType.LobbyScene);
                }
            });
        }


        public void TakeDamage(int damage)
        {
            _totalTakenDamage += damage;
        }

        public void StartStage(int life)
        {
            _scaledTimeStart = _unscaledTimeStart = Time.timeAsDouble;
            _life = life;
        }

        public StageSectionCoordinator TrySpawnChunk(string key,string resourceKey ,Vector3 spawnPos)
        {
            var chunk = Managers.Instance.ResourceManager.Instantiate(key);
            if(chunk == null)
            {
                return null;
            }

            chunk.transform.position = spawnPos;

            StageSectionCoordinator section = chunk.GetComponent<StageSectionCoordinator>();

            if(section == null)
            {
                Managers.Instance.ResourceManager.Destroy(chunk);
                return null;
            }

            section.InitChunk();

            _spawnedChunks[key] = (section, resourceKey);

            return section;
        }

        public void TryUnloadChunk(string key)
        {
            if(_spawnedChunks.TryGetValue(key,out var chunk) ==false)
            {
                return;
            }
            _spawnedChunks.Remove(key);
            chunk.chunk.DeInitChunk();
            Managers.Instance.ResourceManager.Destroy(chunk.chunk.gameObject);
            Managers.Instance.ResourceManager.ReleaseIn(chunk.allocatedResources);
        }

        public StageData GetStageData()
        {
            return _data;
        }

        public void SetStageIDX(int idx)
        {
            _stageIdx = idx;
            _data = Managers.Instance.DataManager.StageDataDict[idx];
        }
    }
}