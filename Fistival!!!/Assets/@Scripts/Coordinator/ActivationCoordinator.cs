using Assets._Scripts.Coordinator;
using UnityEngine;

namespace Coordinator
{
    //스테이지 청크가 초기화될 때 자동으로 켜둘지/꺼둘지 그런거 해주는 클래스
    public class ActivationCoordinator : MonoBehaviour, IBasicInitializer
    {
        [SerializeField]
        private bool _defaultValue;
        public void Init()
        {
            gameObject.SetActive(_defaultValue);
        }
    }
}