using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UI.Scene
{
    public class UI_MainScene : UISceneBase
    {
        enum Buttons
        {
            NewGame,
            LoadGame,
            Setting,
            QuitGame
        }
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            BindButton(typeof(Buttons));

            return true;
        }
    }
}
