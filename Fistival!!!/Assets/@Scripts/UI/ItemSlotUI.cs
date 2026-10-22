using Data;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemSlotUI : MonoBehaviour
    {
        protected Image _itemImage;
        //protected EquipmentData _data;

        private void Awake()
        {
            _itemImage = transform.parent.GetComponent<Image>();
        }


        
    }
}
