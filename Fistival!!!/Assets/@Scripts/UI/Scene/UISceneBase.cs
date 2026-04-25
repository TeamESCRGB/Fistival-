using Manager;
using UnityEngine;

namespace UI.Scene
{
    public abstract class UISceneBase : UIBase
    {
        protected readonly static Vector2 _referenceSolution = new Vector2(1920,1080);
        public override bool Init()
        {
            if(base.Init())
            {
                return false;
            }

            Managers.Instance.UIManager.SetCanvas(gameObject,_referenceSolution);
            return true;
        }
    }
}