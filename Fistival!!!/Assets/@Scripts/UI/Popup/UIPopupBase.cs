namespace UI.Popup
{
    public abstract class UIPopupBase : UIBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            return true;
        }
    }
}