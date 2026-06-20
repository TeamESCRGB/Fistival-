using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator
{
    public class EquipmentFactory
    {
        private Dictionary<int,Equipment> _equipments = new Dictionary<int,Equipment>();

        public EquipmentFactory()
        {
            _equipments[-9999] = new Assets._Scripts.TestScripts.TestItem();
            _equipments[0] = new Equipments.Makta500();
            _equipments[1] = new Equipments.Human();
            _equipments[2] = new Equipments.Chupa();
            _equipments[3] = new Equipments.Malpollo();
            _equipments[4] = new Equipments.Aissyang();
            _equipments[5] = new Equipments.Agalitol();
            _equipments[6] = new Equipments.DrYangban();
            _equipments[7] = new Equipments.NitrogenCider();
            _equipments[8] = new Equipments.Twenty();
            _equipments[9] = new Equipments.MaxSoyMilk();
            _equipments[10] = new Equipments.LikeTheLast();
            _equipments[11] = new Equipments.RedBox();
        }

        public Equipment GetEquipment(int idx)
        {
            if(_equipments.TryGetValue(idx,out var data))
            {
                return data;
            }
            return null;
        }
    }
}
