
namespace Coordinator.Items
{
    public class Chupa : ItemBase
    {
        private const int _attackDmgAdd = 1;
        public override void OnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().Damage += _attackDmgAdd;
            base.OnEquip(player);
        }

        public override void OnUnEquip(PlayerCoordinator player)
        {
            player.GetPlayerData().Damage -= _attackDmgAdd;
            base.OnUnEquip(player);
        }
    }
}