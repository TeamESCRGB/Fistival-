namespace Coordinator
{
    public abstract class ItemBase
    {
        public virtual void OnEquip(PlayerCoordinator player)
        {
            player.UpdateUpdatedDatas();
        }

        public virtual void OnUnEquip(PlayerCoordinator player)
        {
            player.UpdateUpdatedDatas();
        }
    }
}