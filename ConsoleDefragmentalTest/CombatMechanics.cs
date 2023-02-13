using System;
using System.Collections.Generic;

namespace CombatTest
{
    public class CombatMechanics
    {
        //rolls a number of damage dice equal to the nth triangular number 
        //where n is the successes value
        public int DamageRoll(int successes, int die, Func<int, int> func, Random fate)
        {
            int damage = 0;
            for (int i = 1; func(successes) >= i; i++)
            {
                int damageDieRoll = DiceMechanics.DieRoll(die, fate);
                Console.Write(damageDieRoll + " ");
                damage += damageDieRoll;
            }
            Console.WriteLine();
            return damage;
        }
        //damage is dealt to a target entity and is subtracted from their current HP
        public void DealDamage(Entity target, int damage)
        {
            target.currentHP -= damage;
        }
        //attacker and defender make opposed die rolls to determine if an attack hits
        //if attacker has more successes
        //damage is dealt proportionally
        //if not
        //damage is not dealt
        public void Attack(Entity attacker, Entity defender, int attackingStat, int defendingStat, Random fate, int skewUsed, Func<int, Random, Entity, int, int> ResolutionFunction)
        {
            Console.WriteLine("Attacker's Results: ");
            int attackerHits = ResolutionFunction(attackingStat, fate, attacker, skewUsed);
            Console.ReadKey();
            Console.WriteLine("\n Defender's results: ");
            int defenderHits = DiceMechanics.StatRoll(defendingStat, fate, defender);
            Console.ReadKey();
            int netHits = DiceMechanics.OpposedRoll(attackerHits, defenderHits);
            if (netHits > 0)
            {
                Console.WriteLine($"{netHits} more successes!");
                int damageTotal = DamageRoll(netHits, attacker.damageDie, DiceMechanics.TriangularNumber, fate);
                DealDamage(defender, damageTotal);
                Console.WriteLine($"The swing connects for {damageTotal} damage.");
            }
        }
        //this is the primary combat engine
        //the monster appears and is displayed
        //then the player has a choice of actions
        //so far (4/30/2021): the player only has the option of bonking it over the head or bonking it over the head more precisely
        public void FightLoop(Gamestate protagonist, Monster foe, Random fate)
        {
            Console.WriteLine($"The {foe.name} approaches!");
            string statBlock = foe.statString;
            Console.WriteLine(statBlock);
            while (foe.currentHP > 0)
            {
                Console.WriteLine($"{protagonist.name}: HP: {protagonist.currentHP}/{protagonist.maxHP} SKEW: {protagonist.currentSkew}/{protagonist.maxSkew}");
                Console.WriteLine($"{foe.name}: {foe.currentHP}/{foe.maxHP}");
                Console.WriteLine("This is the demo, you may only attack.");
                PlayerAttack(foe, protagonist, fate, CardMechanics.StatDraw);
            }


        }
        private void PlayerAttack(Monster foe, Gamestate protagonist, Random fate, Func<int, Random, Entity, int, int> ResolutionFunction)
        {
            Console.WriteLine("Press h to attack heavily, or p to attack precisely");
            string actionChoice;
            int skewUsed;
            actionChoice = Console.ReadLine();
            while (actionChoice != "h" && actionChoice != "s" && actionChoice != "p")
            {
                Console.WriteLine("Please enter an acceptable value.");
                actionChoice = Console.ReadLine();
            }
            Console.WriteLine("You may use up to two Skew.");
            string skewCatcher = Console.ReadLine();
            while (!int.TryParse(skewCatcher, out skewUsed))
            {
                Console.WriteLine("That was invalid. Enter a valid integer.");
                skewCatcher = Console.ReadLine();
            }
            skewUsed = Convert.ToInt32(skewCatcher);
            if (skewUsed > 2 || skewUsed > protagonist.currentSkew || skewUsed < 0)
            {
                Console.WriteLine("I'll assume you meant zero.");
                skewUsed = 0;
            }
            protagonist.currentSkew -= skewUsed;
            if (skewUsed != 0)
            {
                Console.WriteLine("Would you like to skew up or down? \nPress u to skew up, and anything else to skew down.");
                string skewMod;
                skewMod = Console.ReadLine();
                if (skewMod != "u")
                {
                    skewUsed = -skewUsed;
                }
            }

            if (actionChoice == "h")
            {
                Console.WriteLine("You swing boldly.");
                Attack(protagonist, foe, protagonist.power, foe.endurance, fate, skewUsed, ResolutionFunction);
            }
            else if (actionChoice == "p")
            {
                Console.WriteLine("You swing precisely.");
                Attack(protagonist, foe, protagonist.precision, foe.agility, fate, skewUsed, ResolutionFunction);
            }
            else
            {
                Console.WriteLine("Code shouldn't have been able to get here.");
                Console.WriteLine("You might have been trying to cast a spell");
            }
            if (foe.currentHP < 1) { Victory(protagonist, foe, fate); }
            else { MonsterAttack(foe, protagonist, fate, 0); }

        }
        private void MonsterAttack(Monster foe, Gamestate protagonist, Random fate, int skew)
        {
            Console.WriteLine($"It is now the {foe.name}'s turn.");
            Attack(foe, protagonist, foe.precision, protagonist.endurance, fate, 0, DiceMechanics.StatRoll);
            if (protagonist.currentHP < 1) { Defeat(); }
        }
        public void Victory(Gamestate protagonist, Monster foe, Random fate)
        {
            List<Monster> MonsterTable = new List<Monster>();
            MonsterTable.Add(new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3));
            MonsterTable.Add(new Monster("Walking Chaffbeast", 20, 2, 2, 4, 4, 2));
            MonsterTable.Add(new Monster("Gremlin Assassin", 12, 8, 8, 1, 1, 1));
            MonsterTable.Add(new Monster("Slime Hydra", 15, 6, 6, 6, 6, 3));
            MonsterTable.Add(new Monster("Rotating Fiend", 10, 5, 6, 4, 3, 3));
            MonsterTable.Add(new Monster("Recursed Wanderer", 8, 2, 2, 7, 7, 3));
            Console.WriteLine($"The {foe.name} is slain!");
            protagonist.GainStats(fate, 2);
            Console.ReadKey();
            int listLength = MonsterTable.Count;
            Monster currentMonster = MonsterTable[fate.Next(listLength)];
            currentMonster = currentMonster.CopyMonster(currentMonster);
            CombatMechanics combat = new CombatMechanics();
            protagonist.UpdateCharacterSheet();
            combat.FightLoop(protagonist, currentMonster, fate);
        }
        public void Defeat()
        {
            Console.WriteLine("You're out of HP, and take your final bow.\n Would you like to play again? \n Choose No or No.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}