using UnityEngine;

namespace UI.Scene
{
    public class UI_TitleScene : UISceneBase
    {
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }
            //bind objects
            Debug.Log($"{name} init completed");
            return true;
        }
    }
}