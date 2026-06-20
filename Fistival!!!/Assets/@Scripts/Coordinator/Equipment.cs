namespace Coordinator
{
    public abstract class Equipment
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