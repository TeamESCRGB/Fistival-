using Coordinator;
using Coordinator.Stages;
using Coordinator.Victims;
using Data;
using Data.NonLodable;
using System;
using System.Collections.Generic;
using UI;
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

        private HashSet<string> _clearedBossList = new HashSet<string>(4);

        private int _totalTakenDamage = 0;

        private int _life = 0;


        private double _clearTimeWithPause = 0;

        private CheckPointSaveData _checkPointData;
        private SmoothFollowCoordinator _camFollowCoord;
        private Camera _mainCam;

        public void Init()
        {
            _stageIdx = -1;
            _data = null;
            _scaledTimeStart = 0;
            _unscaledTimeStart = 0;
            _clearTimeWithPause = 0;
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
            _clearedBossList.Clear();
            _mainCam = null;
            _camFollowCoord = null;
        }

        public void CollectCollection(int collectionIdx)
        {
            if(Managers.Instance.DataManager.CollectionDataDict.ContainsKey(collectionIdx) && _collection.Contains(collectionIdx) == false)
            {
                _collection.Add(collectionIdx);
                Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.EFFECT_0, Managers.Instance.DataManager.CollectionDataDict[collectionIdx].CollectingSound, false, Managers.Instance.GameManager.SFXVolume);
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

                double totalScaledTime = Time.timeAsDouble - _scaledTimeStart;
                double totalUnscaledTime = Time.unscaledTimeAsDouble - _unscaledTimeStart;
                _clearTimeWithPause = totalUnscaledTime;

                if (save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause < 0 || totalScaledTime < save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause)
                {
                    save.StageSaveDatas[_stageIdx].ClearTimeWithOutPause = totalScaledTime;
                    save.StageSaveDatas[_stageIdx].ClearTimeWithPause = totalUnscaledTime;
                }
            }
        }

        public void SaveCheckpoint(Vector3 checkpointPos, string bgmKey)
        {
            _checkPointData = new CheckPointSaveData()
            {
                Pos = checkpointPos,
                BGMKey=bgmKey,
                CamPos = Camera.main.transform.position,
                CameraFollowState = _camFollowCoord.GetFollowState(),
                CameraFollowDeadZoneHeight = _camFollowCoord.GetDeadZoneHeight(),
                CameraFollowDeadZoneWidth = _camFollowCoord.GetDeadZoneWidth()
            };
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
            var player = GameObject.FindAnyObjectByType<PlayerCoordinator>();

            Managers.Instance.UIManager.ShowPopupUI<LifeCountPopup>("LifeCountPopup").SetData(_life);

            player.Respawn();

            player.transform.position = _checkPointData.Pos;
            _camFollowCoord.transform.position = _checkPointData.CamPos;
            _camFollowCoord.SetFollowState(_checkPointData.CameraFollowState);
            _camFollowCoord.SetDeadZoneHeight(_checkPointData.CameraFollowDeadZoneHeight);
            _camFollowCoord.SetDeadZoneWidth(_checkPointData.CameraFollowDeadZoneWidth);
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.BGM_0, _checkPointData.BGMKey, true, Managers.Instance.GameManager.BGMVolume);

            foreach(var chunk in _spawnedChunks.Values)
            {
                chunk.chunk.InitChunk();
            }
            GameObject.FindAnyObjectByType<PlayerHUD>().InitUIDatas();
        }

        private void OnFail()
        {
            //컷씬 넣어줘야됨
            Managers.Instance.UIManager.ShowPopupUI<StageFailedPopup>("StageFailedPopup");
        }

        public void OnClear()
        {
            TrySyncToPlayerData(true);
            Managers.Instance.UIManager.ShowPopupUI<StageClearedPopup>("StageClearedPopup").SetupData(_stageIdx,_clearTimeWithPause);
        }


        public void ClearBoss(string bossPrefabName)
        {
            _clearedBossList.Add(bossPrefabName);
        }

        public bool IsBossCleared(string bossPrefabName)
        {
            return _clearedBossList.Contains(bossPrefabName);
        }


        

        public void ReturnToLobby()
        {
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
            _scaledTimeStart = Time.timeAsDouble;
            _unscaledTimeStart = Time.unscaledTimeAsDouble;
            _life = life;
            _mainCam = Camera.main;
            _camFollowCoord = _mainCam.GetComponent<SmoothFollowCoordinator>();
            Managers.Instance.GlobalSoundManager.Play(Defines.SoundChannel.BGM_0, _data.InitialBGM, true, Managers.Instance.GameManager.BGMVolume);
            _checkPointData = new CheckPointSaveData()
            {
                Pos = Vector3.zero,
                BGMKey = _data.InitialBGM,
                CamPos = _mainCam.transform.position,
                CameraFollowState = _camFollowCoord.GetFollowState(),
                CameraFollowDeadZoneHeight = _camFollowCoord.GetDeadZoneHeight(),
                CameraFollowDeadZoneWidth = _camFollowCoord.GetDeadZoneWidth()
            };
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