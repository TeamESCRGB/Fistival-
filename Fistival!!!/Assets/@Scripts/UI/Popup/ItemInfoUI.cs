namespace UI
{
    public class ItemInfoUI : UIBase
    {
        enum Texts
        {
            ItemName,
            ItemDescription
        }
        public override bool Init()
        {
            if(base.Init() == false)
            {
                return false;
            }

            BindText(typeof(Texts));

            return true;
        }

        public ItemInfoUI SetName(string name)
        {
            GetText((int)Texts.ItemName).text = name;
            return this;
        }

        public ItemInfoUI SetDescription(string desc)
        {
            GetText((int)Texts.ItemDescription).text = desc;
            return this;
        }
    }
}