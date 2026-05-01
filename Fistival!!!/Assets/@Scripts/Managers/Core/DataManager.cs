using System.Collections.Generic;
using Data.DataLoaders;
using UnityEngine;
using Newtonsoft.Json;
using Data;

namespace Manager.Core
{
    public class DataManager
    {

        //동적 로드/해제도 생각해봤는데, 당장 할 필요는 없어보이고
        //오히로 로드/해제하면서 생기는 쓰레기가 많을거같아서
        //더 좋은 방법 찾기 전까지는 기존 방식으로 간다

        public Dictionary<int,ObjectData> ObjectDataDict { get; private set; }
        public Dictionary<int,CommonModeData> CommonModeDataDict { get;private set; }
        public Dictionary<string,PatternData> PatternDataDict { get; private set; }
        public Dictionary<int,ProjectileData> ProjectileDataDict { get; private set; }

        public void Init()
        {
            ObjectDataDict = LoadJson<ObjectDataLoader, int, ObjectData>("ObjectData").MakeDict();
            CommonModeDataDict = LoadJson<CommonModeDataLoader, int, CommonModeData>("CommonModeData").MakeDict();
            PatternDataDict = LoadJson<PatternDataLoader, string, PatternData>("PatternData").MakeDict();
            ProjectileDataDict = LoadJson<ProjectileDataLoader, int, ProjectileData>("ProjectileData").MakeDict();
        }

        //솔직히, 이게 어떻게 가능한건지 아직 모르겠다
        TLoader LoadJson<TLoader, TKey, TVal>(string jsonPath) where TLoader : ILoader<TKey, TVal>
        {
            TextAsset textAsset = Managers.Instance.ResourceManager.Load<TextAsset>(jsonPath);
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }

    }
}