using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
#if DISABLE_LOBBY_SCENE
#endif
    internal class alkjsdhflkajshfdkjsa : MonoBehaviour
    {
        public  int mask;
        [ContextMenu("asdf")]
        public  void GetLayerNamesFromMask()
        {
            List<string> layerNames = new List<string>();

            // 0번부터 31번 레이어까지 순회
            for (int i = 0; i < 32; i++)
            {
                // (1 << i)를 통해 해당 레이어 비트가 마스크에 포함되어 있는지 확인
                if ((mask & (1 << i)) != 0)
                {
                    string name = LayerMask.LayerToName(i);

                    // 이름이 비어있지 않은 경우에만 추가 (설정되지 않은 레이어 제외)
                    if (!string.IsNullOrEmpty(name))
                    {
                        Debug.Log(name);
                    }
                }
            }
        }
    }
}
