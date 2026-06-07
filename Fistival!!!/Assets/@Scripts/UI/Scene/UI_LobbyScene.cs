using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UI.Scene
{
    public class UI_LobbyScene : UISceneBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            //bind objects
            Debug.Log($"{name} init completed");
            return true;
        }
    }
}
