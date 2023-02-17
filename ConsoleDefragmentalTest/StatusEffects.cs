using CombatTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatTest
{
    public class StatusEffect
    {
        public string Name;
        public Action<Entity, int> statusName;
        public int statusCounter;
        public StatusEffect(string objectName, Action<Entity, int> statusName, int statusCounter)
        {
            this.Name = objectName;
            this.statusName = statusName;
            this.statusCounter = statusCounter;
        }
        public int CompareTo(StatusEffect compareEffect)
        {
            // A null value means that this object is greater.
            if (compareEffect == null)
                return 1;

            else
                return this.Name.CompareTo(compareEffect.Name);
        }
    }
}
