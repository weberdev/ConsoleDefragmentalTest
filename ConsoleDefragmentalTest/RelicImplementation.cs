using System;
using System.Collections.Generic;

namespace CombatTest
{
    //Relic:
    //Class that defines items that are carried and have game changing effects on the player.
    //These are held in a list as part of Gamestate.
    public class Relic
    {
        public string Name;
        public Action<Entity> RelicEffect;
        public Action<Entity> OnPickup;
        List<Relic> RelicList;
        List<Relic> DefaultRelicList()
        {
            List<Relic> DefaultRelicList = new List<Relic>();
            DefaultRelicList.Add(new Relic("Protean Symbiote", InflictProteanism, IncreaseThreshold));
            DefaultRelicList.Add(new Relic("Bullfighter's Capote", NothingOnActivation, AddRedCapote));
            return DefaultRelicList;
        }
        public Relic(string name, Action<Entity> relicEffect, Action<Entity> onPickup)
        {
            Name = name;
            RelicEffect = relicEffect;
            OnPickup = onPickup;
        }
        //NothingOnPickup:
        public static void NothingOnPickup(Entity target)
        {
            Console.WriteLine("This relic has not affected you. Yet.");
        }
        //NothingOnActivation
        public static void NothingOnActivation(Entity target)
        {

        }
        //InflictProteanism:
        //First effect of Protean Symbiote.
        //Applies Localized Proteanism to the player.
        public static void InflictProteanism(Entity target)
        {
            CombatMechanics.LocalizedProteanism(target, 1);
        }
        //IncreaseThreshold:
        //Second effect of Protean Symbiote.
        //Increases the player's threshold by one.
        //This allows attributes to grow more easily.
        public static  void IncreaseThreshold(Entity target)
        {
            target.threshold++;
        }
        //AddRedCapote:
        //Sets up an event listener on all opposed rolls of less than one. 
        //The event listener calls Red Capote, a free attack.
        public static void AddRedCapote(Entity target) {
            DiceMechanics.OpposedRollNegativeEvent += RedCapote;
        }
        //RedCapote:
        //I implemented Matador!
        //Free attack under certain circumstatnces.
        public static void RedCapote(Entity dodging, Entity missedAttacker)
        {
            Console.WriteLine("A glorious dodge and reflexive counterattack!");
            Random rng = new Random();
            CombatMechanics.Attack(dodging, missedAttacker, dodging.precision, missedAttacker.agility, rng, 0);
        }
      
    }
}