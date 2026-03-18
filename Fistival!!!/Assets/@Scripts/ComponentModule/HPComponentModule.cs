namespace ComponentModule
{
    public class HPComponentModule
    {
        private int _hp = 0;
        private int _maxHP = 0;
        //private bool _isDead; <= ComponentModule쪽이 맞을거같긴 한데, 코디네이터로만 여기에 접근하고, 가장 많이 체크하는게 isdead여서, 캐시 효율성 측면에서는 위로 올리는게 나을듯
        //public event Action<int, int, int> OnHPChanged;
        //public event Action OnDead; 이벤트들은 이거의 경우는 Coordinator쪽에서 처리하는게 나을듯
        
        public void Init(int initialHP, int maxHP)
        {
            if(maxHP < 1)
            {
                maxHP = 1;
            }

            if(initialHP < 1)
            {
                initialHP = 1;
            }

            if(initialHP > maxHP)
            {
                initialHP = maxHP;
            }

            _maxHP = maxHP;
            _hp = initialHP;
        }

        public int GetHP()
        {
            return _hp;
        }

        /// <summary>
        /// subtract given hp from now hp, and returns bool whether hp down to zero
        /// </summary>
        /// <param name="hp"></param>
        /// <returns></returns>
        public bool SubHP(int hp)
        {
            _hp -= hp;
            return IsDead();
        }

        public void AddHP(int hp)
        {
            _hp += hp;
            if(_hp > _maxHP)
            {
                _hp = _maxHP;
            }
        }

        public bool IsDead()
        {
            return _hp <= 0;
        }
    }
}