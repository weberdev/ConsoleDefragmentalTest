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
        public Action<Entity> OnPickup;
        List<Relic> RelicList;
        List<Relic> DefaultRelicList()
        {
            List<Relic> DefaultRelicList = new List<Relic>();
            DefaultRelicList.Add(new Relic("Protean Symbiote", InflictProteanism));
            DefaultRelicList.Add(new Relic("Bullfighter's Capote", AddRedCapote));
            DefaultRelicList.Add(new Relic("Stabilizing Field", StabilizingField));
            DefaultRelicList.Add(new Relic("Eye of the Ophidian Cyclops", AddOphidianCyclops));
            return DefaultRelicList;
        }
        public Relic(string name, Action<Entity> onPickup)
        {
            Name = name;
            OnPickup = onPickup;
        }
        //NothingOnPickup:
        public static void NothingOnPickup(Entity target)
        {
            Console.WriteLine("This relic has not affected you. Yet.");
        }
        //NothingOnActivation:
        //Does nothing. This is mainly implemented to ensure that the relic objects work smoothly in general.
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
        public static void ProteanSymbiote(Entity target)
        {
            CombatMechanics.LocalizedProteanism(target, 1);
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
        //StabilizingField
        //Adjusts the player's successValue downward (making it easier) and bonusRolls Downward (fewer generated
        public static void StabilizingField(Entity target)
        {
            target.successValue -= 1;
            target.bonusRolls-= 1;
        }
        //AddOphidianCyclops:
        //If a die roll is a one, explode the die, take one damage.
        public static void AddOphidianCyclops(Entity target)
        {
            DiceMechanics.DieRollIsOneEvent += OphidianCyclops;
        }

        //OphidianCyclops:
        //If a die roll is a one, get an additional roll, take one damage.
        public static int OphidianCyclops(Entity player)
        {
            player.currentHP-= 1;
            return 1;
        }
        
      
    }
}