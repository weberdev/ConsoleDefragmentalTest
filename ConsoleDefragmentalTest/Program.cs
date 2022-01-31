using System;

namespace CombatTest
{
    //Entity is the base class for creatures involved in combat
    //weirdly enough, also the gamestate
    public class Entity
    {
        //attributes are self explanatory
        // I expect that no one involved in this project likely has any lack of understanding of what these are
        //that said, the list of people involved in the project can be counted on one (zero indexed) finger
        public String name;
        //maxHP and currentHP show how close a creature is to death
        public int maxHP;
        public int currentHP;
        //powe and precision are the two primary stats that attacking dice pools are based on
        public int power;
        public int precision;
        //endurance and agility are for defense
        public int endurance;
        public int agility;
        //statString holds the basic attributes for player perusal
        public String statString;
        //damageDie is there for testing, it'll be replaced later on
        //for now it serves as the way to resolve injury
        public int damageDie;
        public String displayStats()
        {
            string stats = $"{name} \n {currentHP}/{maxHP}HP\n Power: {power}   Precision: {precision} \n Endurance: {endurance}   Agility {agility}";
            return stats;
        }

    }

    //Monster as a class is easier and cleaner than entity and allows for moderately more clarity for myself when writing code
    public class Monster : Entity
    {   //this is a constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="MAXHP"></param>
        /// <param name="PWR"></param>
        /// <param name="PRC"></param>
        /// <param name="END"></param>
        /// <param name="AGI"></param>
        /// <param name="DIE"></param>
        public Monster(string Name, int MAXHP, int PWR, int PRC, int END, int AGI, int DIE)
        {
            name = Name;
            maxHP = MAXHP;
            power = PWR;
            precision = PRC;
            endurance = END;
            agility = AGI;
            damageDie = DIE;
            currentHP = MAXHP;
            statString = displayStats();
        }
    }
    //all functions for dice chucking are contained here
    public class DiceMechanics
    {
        //roll a die of size die
        //fate is the default random object
        public static int dieRoll(int die, Random fate)
        {
            return fate.Next(die) + 1;
        }
        //subtraction, but as a method
        public static int opposedRoll(int attacker, int defender)
        {
            return attacker - defender;
        }
        //rolls a number of dice of size die equal to the total stat offered as input.
        //if the value is the maximum, roll two more
        //basic parameters for testing, can be modified later
        //prints out a string for player clarity and testing
        public static int statRoll(int totalStat, Random fate, int skew = 0, int die = 8, int bonusRolls = 2, int critMod = 0, int successValue = 6)
        {
            int realDie;
            realDie = die + skew;
            int hits = 0;
            for (int i = 0; i < totalStat; i++)
            {
                int currentRoll = dieRoll(realDie, fate);
                Console.Write($" {currentRoll}");
                if (currentRoll == realDie - critMod)
                {
                    hits++;
                    i = i - bonusRolls;
                    Console.Write("! Two more rolls. ");
                }
                else if (currentRoll >= successValue)
                {
                    hits++;
                }
            }
            Console.WriteLine($"\n A total of {hits} successes.");
            return hits;
        }
    }

    public class CombatMechanics
    {
        //rolls a number of damage dice equal to the nth triangular number 
        //where n is the successes value
        public int damageRoll(int successes, int die, Random fate)
        {
            int damage = 0;
            for (int i = 1; successes >= i; i++)
            {
                for (int j = 1; i >= j; j++)
                {
                    int damageDieRoll = DiceMechanics.dieRoll(die, fate);
                    damage += damageDieRoll;
                }
            }
            return damage;
        }
        //damage is dealt to a target entity and is subtracted from their current HP
        public void dealDamage(Entity target, int damage)
        {
            target.currentHP = target.currentHP - damage;
        }
        //attacker and defender make opposed die rolls to determine if an attack hits
        //if attacker has more successes
        //damage is dealt proportionally
        //if not
        //damage is not dealt
        public void attack(Entity attacker, Entity defender, int attackingStat, int defendingStat, Random fate, int skewUsed)
        {
            Console.Write("Attacker's Results: ");
            int attackerHits = DiceMechanics.statRoll(attackingStat, fate, skewUsed);
            Console.ReadKey();
            Console.Write("Defender's results: ");
            int defenderHits = DiceMechanics.statRoll(defendingStat, fate, 0);
            Console.ReadKey();
            int netHits = DiceMechanics.opposedRoll(attackerHits, defenderHits);
            if (netHits > 0)
            {
                Console.WriteLine($"{netHits} more successes!");
                int damageTotal = damageRoll(netHits, attacker.damageDie, fate);
                dealDamage(defender, damageTotal);
                Console.WriteLine($"The swing connects for {damageTotal} damage.");
            }
        }
        //this is the primary combat engine
        //the monster appears and is displayed
        //then the player has a choice of actions
        //so far (4/30/2021): the player only has the option of bonking it over the head or bonking it over the head more precisely
        public void fightLoop(Gamestate protagonist, Monster foe, Random fate)
        {
            Console.WriteLine($"The {foe.name} approaches!");
            String statBlock = foe.statString;
            Console.WriteLine(statBlock);
            while (foe.currentHP > 0)
            {
                string actionChoice;
                int skewUsed;
                Console.WriteLine($"{protagonist.name}: HP: {protagonist.currentHP}/{protagonist.maxHP} SKEW: {protagonist.currentSkew}/{protagonist.maxSkew}");
                Console.WriteLine($"{foe.name}: {foe.currentHP}/{foe.maxHP}");
                Console.WriteLine("Press h to attack heavily, or p to attack precisely");
                actionChoice = Console.ReadLine();
                Console.WriteLine("You may use up to two Skew.");
                skewUsed = Convert.ToInt32(Console.ReadLine());
                protagonist.currentSkew -= skewUsed;
                Console.WriteLine("Would you like to skew up or down? \nPress u to skew up, and anything else to skew down.");
                string skewMod;
                skewMod = Console.ReadLine();
                if (skewMod != "u")
                {
                    skewUsed = -skewUsed;
                }
                if (actionChoice == "h")
                {
                    Console.WriteLine("You swing boldly.");
                    attack(protagonist, foe, protagonist.power, foe.endurance, fate, skewUsed);
                }
                else if (actionChoice == "p")
                {
                    Console.Write("You swing precisely.");
                    attack(protagonist, foe, protagonist.precision, foe.agility, fate, skewUsed);
                }
                else
                {
                    Console.WriteLine("YOU DID IT WRONG AND CRASHED THE PROGRAM. \n WHAT THE FUCK, DUDE.");
                    Console.ReadKey();
                    break;
                }
                if (foe.currentHP < 1) { break; }
                Console.WriteLine($"It is now the {foe.name}'s turn.");
                attack(foe, protagonist, foe.precision, protagonist.endurance, fate, 0);
            }
            Console.WriteLine($"The {foe.name} is slain!");
            Console.ReadKey();
        }
    }
    //Gamestate tracks the player's attributes and position.
    public class Gamestate : Entity
    {
        public int maxSkew = 8;
        public int currentSkew;
        public Gamestate(string Name, int MAXHP, int PWR, int PRC, int END, int AGI, int DIE)
        {
            name = Name;
            maxHP = MAXHP;
            power = PWR;
            precision = PRC;
            endurance = END;
            agility = AGI;
            damageDie = DIE;
            currentHP = MAXHP;
            currentSkew = maxSkew;
            bool isLookingUp;
            bool isLookingRight;
            statString = displayStats();
            int[][] map;
        }
        //generates an integer array
        //0 is a wall, 1 is a floor
        /*int[,] mapMaker(int length, int depth)
        {
            Random mapSeed = new Random();
            int[,] holder = new int[length, depth];
            for(int i = 0; i<length; i++)
            {
                for (int j = 0; j<depth; j++)
                {
                    holder[i, j] = 0;
                }
            }
            int currentRow;
            int currentColumn;
            currentRow = mapSeed.Next(length) + 1;
            currentColumn = mapSeed.Next(depth) + 1;
            Tuple[] directions = new Tuple[4];


            return holder;
        }
        */
    }
    class Program
    {
        static void Main(string[] args)
        {

            string yourName;
            Random whimsOfFate = new Random();
            Console.WriteLine("Please enter your name.");
            yourName = Console.ReadLine();
            Gamestate currentState = new Gamestate(yourName, 15, 4, 5, 4, 3, 4);
            Console.WriteLine(currentState.statString);
            Monster currentMonster = new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3);
            CombatMechanics combat = new CombatMechanics();
            combat.fightLoop(currentState, currentMonster, whimsOfFate);
        }
    }
}