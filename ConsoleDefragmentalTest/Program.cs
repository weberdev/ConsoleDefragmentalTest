using System;
using System.Collections.Generic;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace CombatTest
{
    //Entity is the base class for creatures involved in combat
    //weirdly enough, also the gamestate
    public class Entity
    {
        //attributes are self explanatory
        // I expect that no one involved in this project likely has any lack of understanding of what these are
        //that said, the list of people involved in the project can be counted on one (zero indexed) finger
        public string name;
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
        public string statString;
        //damageDie is there for testing, it'll be replaced later on
        //for now it serves as the way to resolve injury
        public int damageDie;
        //only the player will have a deck of cards at this stage
        //it's in entity instead of GameState because it's easier to pass a different function this way
        //more to the point, it also allows for non-player entities to use decks later on
        public StandardPlayingCardDeck Deck = new(false, 0);
        public List<StandardPlayingCard> Discard = new();
        public string DisplayStats()
        {
            string stats = $"{name} \n {currentHP}/{maxHP}HP\n Power: {power}   Precision: {precision} \n Endurance: {endurance}   Agility {agility}";
            return stats;
        }
        public int statDie = 8;
        public int bonusRolls = 2;
        public int critMod = 0;
        public int successValue = 6;

    }

    //Monster as a class is easier and cleaner than entity and allows for moderately more clarity for myself when writing code
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
            statString = DisplayStats();
        }
        public Monster CopyMonster(Monster baseMonster)
        {
            Monster thisMonster = new("Proteus", 0, 0, 0, 0, 0, 0)
            {
                name = baseMonster.name,
                maxHP = baseMonster.maxHP,
                power = baseMonster.power,
                precision = baseMonster.precision,
                agility = baseMonster.agility,
                endurance = baseMonster.endurance,
                damageDie = baseMonster.damageDie,
                currentHP = maxHP,
                statString = DisplayStats()
            };
            return thisMonster;
        }
    }
    //all functions for dice chucking are contained here
    
    public class Location
    {
        public List<Monster> MonsterTable;
        public Location()
        {
            MonsterTable.Add(new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3));
            MonsterTable.Add(new Monster("Walking Chaffbeast", 20, 2, 2, 4, 4, 2));
            MonsterTable.Add(new Monster("Gremlin Assassin", 12, 8, 8, 1, 1, 1));
            MonsterTable.Add(new Monster("Slime Hydra", 15, 6, 6, 6, 6, 3));
            MonsterTable.Add(new Monster("Rotating Fiend", 10, 5, 6, 4, 3, 3));
            MonsterTable.Add(new Monster("Recursed Wanderer", 8, 2, 2, 7, 7, 3));
        }
    }

    //Not that much here at this point, just a different version of DiceMechanics for card implementation.
    
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
            statString = DisplayStats();
            int[][] map;
        }
        public void GainStats(Random fate, int threshold)
        {
            Console.WriteLine($"You have won. Each attribute will increase if you achieve *fewer* than {threshold} successes on the roll.");
            int unchangedAttributes = 4;
            Console.WriteLine("Trying power.");
            if (DiceMechanics.QuietRoll(power, fate, 0) < threshold)
            {
                power++;
                Console.WriteLine("Your power increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying precision");

            if (DiceMechanics.QuietRoll(precision, fate, 0) < threshold)
            {
                precision++;
                Console.WriteLine("Your precision increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying endurance.");
            if (DiceMechanics.QuietRoll(endurance, fate, 0) < threshold)
            {
                endurance++;
                Console.WriteLine("Your endurance increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying agility.");
            if (DiceMechanics.QuietRoll(agility, fate, 0) < threshold)
            {
                agility++;
                Console.WriteLine("Your agility increased!\n");
                unchangedAttributes--;
            }
            if (unchangedAttributes > 0)
            {
                Console.WriteLine($"As a consolation prize, your maximum HP increases by {unchangedAttributes}.");
                maxHP += unchangedAttributes;
            }
        }
        public void UpdateCharacterSheet()
        {
            statString = DisplayStats();
            Console.WriteLine(statString);
        }
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
    class Program
    {
        static void Main(string[] args)
        {

            string yourName;
            Random whimsOfFate = new Random();
            Console.WriteLine("Please enter your name.");
            yourName = Console.ReadLine();
            Gamestate currentState = new(yourName, 15, 4, 5, 4, 3, 4);
            currentState.Deck.Shuffle();
            Console.WriteLine(currentState.statString);
            Monster currentMonster = new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3);
            CombatMechanics combat = new CombatMechanics();
            combat.FightLoop(currentState, currentMonster, whimsOfFate);
        }
    }
}

