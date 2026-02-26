using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Manager.Core
{
    //string -> enum으로 방식 바꾸기
    public class ResourceManager
    {
        private Dictionary<string, ValueTuple<int,UnityEngine.Object>> _resources
            = new Dictionary<string, ValueTuple<int, UnityEngine.Object>>();//리소스 키 / <참조카운트,리소스>
        private Dictionary<string, List<string>> _lableStatus
            = new Dictionary<string, List<string>>();//라벨 / 리스트(리소스 키 <= 자기가 로드한 리소스 키들)

        public T Load<T>(string key) where T : UnityEngine.Object
        {
            if (_resources.TryGetValue(key, out var value))
            {
                return value.Item2 as T;
            }

#if UNITY_EDITOR
            Debug.LogWarning("load failed");
#endif

            return null;
        }

        public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
        {
            GameObject prefab = Load<GameObject>(key);
            if (prefab == null)
            {
                return null;
            }

            if (pooling)
            {
                return Managers.Instance.GameObjectPoolManager.GetFromPool(prefab);
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab, parent);

            go.name = prefab.name;
            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (Managers.Instance.GameObjectPoolManager.ReturnToPool(go))
            {
                return;
            }

            UnityEngine.Object.Destroy(go);
        }

        public void ReleaseIn(string lable)
        {
            List<string> keys;
            if(_lableStatus.TryGetValue(lable,out keys) == false)
            {
                return;
            }

            for(int i = 0;  i < keys.Count; i++)
            {
                var resource = _resources[keys[i]];
                Addressables.Release(resource.Item2);
                resource.Item1--;

                if(resource.Item1<=0)
                {
                    _resources.Remove(keys[i]);
                }
                else
                {
                    _resources[keys[i]] = resource;
                }
            }

            _lableStatus.Remove(lable);
        }

        public void ReleaseAll()
        {
            foreach(var keys in _lableStatus.Values)
            {
                for(int i = 0; i < keys.Count; i++)
                {
                    Addressables.Release(_resources[keys[i]].Item2);
                }
            }

            _resources.Clear();
            _lableStatus.Clear();
        }

        private void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
        {

            var asyncOperation = Addressables.LoadAssetAsync<T>(key);
            asyncOperation.Completed += (op) =>
            {
                if(_resources.ContainsKey(key) == false)
                {
                    _resources[key] = (0,op.Result);
                }

                var resource = _resources[key];
                resource.Item1++;
                _resources[key] = resource;

                callback?.Invoke(op.Result);
            };
        }

        public void LoadAsyncAllIn(string lable, Action<string, int, int> callback)
        {
            if(_lableStatus.ContainsKey(lable))
            {
                callback?.Invoke(lable,1,1);
                return;
            }

            var asyncHandle = Addressables.LoadResourceLocationsAsync(lable, typeof(UnityEngine.Object));
            _lableStatus.Add(lable, new List<string>(16));

            asyncHandle.Completed += (handle) =>
            {
                int loadedCnt = 0;
                int targetCnt = handle.Result.Count;

                foreach (var result in handle.Result)
                {
                    LoadAsync<UnityEngine.Object>(result.PrimaryKey, (obj) =>
                    {
                        
                        loadedCnt++;
                        callback?.Invoke(result.PrimaryKey, loadedCnt, targetCnt);

                        if(loadedCnt == targetCnt)
                        {
                            Addressables.Release(asyncHandle);
                        }

                    });
                }
            };
        }
    }
}