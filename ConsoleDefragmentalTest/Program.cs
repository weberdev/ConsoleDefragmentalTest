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
        //only the player will have a deck of cards at this stage
        //it's in entity instead of GameState because it's easier to pass a different function this way
        //more to the point, it also allows for non-player entities to use decks later on
        public Xyaneon.Games.Cards.StandardPlayingCards.StandardPlayingCardDeck Deck = new(false, 0);
        public List<StandardPlayingCard> Discard = new();
        public String DisplayStats()
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
    public class DiceMechanics
    {
        public int AttemptASpell(int attribute)
        {
            Random random = new Random();
            int DIESIZE = 8;
            int SCALAR = (DIESIZE/2)+1;
            int target = attribute * SCALAR;
            int currentTotal = 0;
            string choice = "t";
            int currentScore = 0;
            while (choice == "t")
            {

                if (choice == "t")
                {
                    for (int i = 0; i < currentTotal; i++)
                    {
                        Console.Write("|");
                    }
                    for (int i = 0; i < (target - currentTotal); i++)
                    { Console.Write("."); }
                    Console.WriteLine("X\n");
                    Console.WriteLine($"You cannot exceed {target}.\n Would you like to roll a die?\n t for yes, anything else for no.");
                    choice = Console.ReadLine();
                    int roll = DieRoll(DIESIZE, random);
                    if (roll == DIESIZE) { currentTotal -= SCALAR; currentScore++; target--; }
                    else if (roll >= 6)
                    {
                        currentScore++;
                        target--;
                    }
                    else
                    {
                        currentTotal += roll;
                    }
                    if (currentTotal < 0)
                    {
                        currentTotal = 0;
                    }
                    else if (currentTotal > target)
                    {
                        Console.WriteLine("You got greedy, as foul dabblers in the arcane often do.");
                        return 0;
                    }
                    else if (currentTotal == target)
                    {
                        Console.WriteLine("A masterful success.");
                        return currentTotal + 2;
                    }
                    Console.WriteLine($"Last roll was {roll}.");
                    Console.WriteLine($"Your score is {currentScore}.");


                }
            }
            return currentScore;
        }
        //roll a die of size die
        //fate is the default random object
        public static int DieRoll(int die, Random fate)
        {
            int roll = fate.Next(die + 1);
            return roll;
        }
        //subtraction, but as a method
        public static int OpposedRoll(int attacker, int defender)
        {
            return attacker - defender;
        }
        //generates the nth triangular number for an input integer n
        //please don't pass it negative values
        public static int TriangularNumber(int n)
        {
            int triangularNumber = n * (n + 1) / 2;
            return triangularNumber;
        }
        //rolls a number of dice of size die equal to the total stat offered as input.
        //if the value is the maximum, roll two more
        //basic parameters for testing, can be modified later
        //prints out a string for player clarity and testing
        public static int StatRoll(int totalStat, Random fate, Entity activeEntity, int skew = 0)
        {
            int realDie;
            realDie = activeEntity.statDie + skew;
            int hits = 0;
            for (int i = 0; i < totalStat; i++)
            {
                int currentRoll = DieRoll(realDie, fate);
                Console.Write($" {currentRoll}");
                if (currentRoll == realDie - activeEntity.critMod)
                {
                    hits++;
                    i -= activeEntity.bonusRolls;
                    Console.Write("! Two more rolls.");
                }
                else if (currentRoll >= activeEntity.successValue)
                {
                    hits++;
                    Console.Write("!");
                }
                else
                {
                    Console.Write(".");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"\n A total of {hits} successes.");
            return hits;
        }
        public static int QuietRoll(int totalStat, Random fate, int skew = 0, int die = 8, int bonusRolls = 2, int critMod = 0, int successValue = 6)
        {
            int realDie;
            realDie = die + skew;
            int hits = 0;
            for (int i = 0; i < totalStat; i++)
            {
                int currentRoll = DieRoll(realDie, fate);
                if (currentRoll >= realDie - critMod)
                {
                    hits++;
                    i -= bonusRolls;
                }
                else if (currentRoll >= successValue)
                {
                    hits++;
                }
            }
            Console.WriteLine($"\n A total of {hits} successes. \n");
            Console.ReadKey();
            return hits;
        }
    }
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
                    damage += damageDieRoll;
                            }
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
        public void Attack(Entity attacker, Entity defender, int attackingStat, int defendingStat, Random fate, int skewUsed, Func<int, Random, Entity, int,  int> ResolutionFunction)
        {
            Console.WriteLine("Attacker's Results: ");
            int attackerHits = ResolutionFunction(attackingStat, fate, attacker, skewUsed);
            Console.ReadKey();
            Console.WriteLine("Defender's results: ");
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
            String statBlock = foe.statString;
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
            if (foe.currentHP < 1) { this.Victory(protagonist, foe, fate); }
            else { MonsterAttack(foe, protagonist, fate, 0); }

        }
        private void MonsterAttack(Monster foe, Gamestate protagonist, Random fate, int skew)
        {
            Console.WriteLine($"It is now the {foe.name}'s turn.");
            Attack(foe, protagonist, foe.precision, protagonist.endurance, fate, 0, DiceMechanics.StatRoll);
            if (protagonist.currentHP < 1) { this.Defeat(); }
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
    //Not that much here at this point, just a different version of DiceMechanics for card implementation.
    public static class CardMechanics
    {
        public static string SPCToString(StandardPlayingCard card)
        {
            int cardRank = RankToInt(card.Rank);
            String cardRankString;
            if(cardRank == 13)
            {
                cardRankString = "K";
            }
            else if(cardRank == 12)
            {
                cardRankString = "Q";
            }
            else if (cardRank == 11)
            {
                cardRankString = "J";
            }
            else
            {
                cardRankString = cardRank.ToString();
            }
            String s = cardRankString + SuitToChar(card.Suit);
            return s;
        }

        public static char SuitToChar(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return '♣';
                case Suit.Diamonds:
                    return '♦';
                case Suit.Hearts:
                    return '♥';
                case Suit.Spades:
                    return '♠';
                default:
                    throw new ArgumentException("Invalid suit value");
            }
        }
        public static int RankToInt(Rank rank)
        {
            switch (rank)
            {
                case Rank.None:
                    return 0;
                case Rank.Ace:
                    return 1;
                case Rank.Two:
                    return 2;
                case Rank.Three:
                    return 3;
                case Rank.Four:
                    return 4;
                case Rank.Five:
                    return 5;
                case Rank.Six:
                    return 6;
                case Rank.Seven:
                    return 7;
                case Rank.Eight:
                    return 8;
                case Rank.Nine:
                    return 9;
                case Rank.Ten:
                    return 10;
                case Rank.Jack:
                    return 11;
                case Rank.Queen:
                    return 12;
                case Rank.King:
                    return 13;
                default:
                    throw new ArgumentException("Invalid rank value");
            }
        }
        public static int StatDraw(int totalStat, Random rand, Entity activeEntity, int skew)
        {
            int total = 0;
            for (int i = 0; i < totalStat; i++)
            {
                if (activeEntity.Deck.IsEmpty){
                    foreach(StandardPlayingCard discardedCard in activeEntity.Discard)
                    {
                        activeEntity.Deck.PlaceOnTop(discardedCard);
                        activeEntity.Discard.Remove(discardedCard);
                    }
                    activeEntity.Deck.Shuffle();
                }
                StandardPlayingCard currCard = activeEntity.Deck.Draw();
                //There is temptation to add skew to every i adjustment here, but that seems unreasonably punishing
                //It would be desirable to implement a consequence for skew in this function, however.
                int cardRank = RankToInt(currCard.Rank);
                switch (cardRank+skew)
                {
                    case <9:
                        Console.WriteLine(SPCToString(currCard) + ".");
                        break;
                    case  9:
                        total++;
                        Console.WriteLine(SPCToString(currCard) + "! A success!");
                        break;
                    case 10:
                        total++;
                        i--;
                        Console.WriteLine(SPCToString(currCard) + "! A success and an additional draw!");
                        break;
                    case 11:
                        total++;
                        i -= 2;
                        Console.WriteLine(SPCToString(currCard) + "! A success and two additional draws!");
                        break;
                    case 12:
                        total += 2;
                        i--;
                        Console.WriteLine(SPCToString(currCard) + "! Two successes and an additional draw!");
                        break;
                    case > 12:
                        total += 2;
                        i -= 2;
                        Console.WriteLine(SPCToString(currCard) + "! Two successes and two additional draws!");
                        break;

                }
            }
            Console.WriteLine("");
            Console.Write("A total of " + total.ToString()+ " successes.");
            return total;
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
            statString = DisplayStats();
            int[][] map;
        }
        public void GainStats(Random fate, int threshold)
        {
            Console.WriteLine($"You have won. Each attribute will increase if you achieve *fewer* than {threshold} successes on the roll.");
            int unchangedAttributes = 4;
            Console.WriteLine("Trying power.");
            if (DiceMechanics.QuietRoll(this.power, fate, 0) < threshold)
            {
                this.power++;
                Console.WriteLine("Your power increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying precision");
            
            if (DiceMechanics.QuietRoll(this.precision, fate, 0) < threshold)
            {
                this.precision++;
                Console.WriteLine("Your precision increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying endurance.");
            if (DiceMechanics.QuietRoll(this.endurance, fate, 0) < threshold)
            {
                this.endurance++;
                Console.WriteLine("Your endurance increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying agility.");
            if (DiceMechanics.QuietRoll(this.agility, fate, 0) < threshold)
            {
                this.agility++;
                Console.WriteLine("Your agility increased!\n");
                unchangedAttributes--;
            }
            if (unchangedAttributes > 0)
            {
                Console.WriteLine($"As a consolation prize, your maximum HP increases by {unchangedAttributes}.");
                this.maxHP += unchangedAttributes;
            }
        }
        public void UpdateCharacterSheet()
        {
            this.statString = this.DisplayStats();
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
    
