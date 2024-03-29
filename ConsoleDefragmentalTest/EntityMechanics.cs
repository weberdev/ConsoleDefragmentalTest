﻿using System;
using System.Collections.Generic;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace CombatTest
{
    //Will be implemented later
    public enum AttributeType
    {
        Power = 0,
        Precision = 0,
        Endurance = 0,
        Agility = 0
    }
    //Consumable:
    //Class for items that are single use.
    //Stored in an array in Gamestate.
    public class Consumable
    {
        public string Name;
        public Action<Entity> ConsumableEffect;
    }


    //StatusEffect:
    //Class that contains a void function that takes in an entity and an integer, and a counter
    //The class is held on a list that is part of the entity class, and a

    //Entity is the base class for creatures involved in combat
    //weirdly enough, also the gamestate
    public class Entity
    {
        string Name;
        public void SetName(string Name)
        {
            this.Name = Name;
        }
        public string getName()
        {
            return this.Name;
        }
        //maxHP and currentHP show how close a creature is to death
        int maxHP;
        public void setMaxHP(int changeTo)
        {
            maxHP = changeTo;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }

        public int getCurrentHP()
        {
            return currentHP;
        }

        public void changeMaxHP(int changeBY)
        {
            maxHP += changeBY;
            currentHP += changeBY;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }

        public int getMaxHP()
        {
            return this.maxHP;
        }

        int currentHP;
        public void setCurrentHP(int changeTo)
        {
            currentHP = changeTo;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }

        public void changeCurrentHP(int changeBY)
        {
            this.currentHP += changeBY;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }
        //power and precision are the two primary stats that attacking dice pools are based on
        int power;
        public void setPower(int newPower)
        {
            power = newPower;
        }
        public int getPower()
        {
            return this.power;
        }
        public void incPower()
        {
            this.power++;
        }
        int precision;
        public void setPrecision(int newPrecision)
        {
            precision = newPrecision;
        }
        public int getPrecision()
        {
            return this.precision;
        }
        public void incPrecision()
        {
            this.precision++;
        }
        //endurance and agility are for defense
        int endurance;
        public void setEndurance(int newEndurance)
        {
            endurance = newEndurance;
        }
        public int getEndurance()
        {
            return this.endurance;
        }
        public void incEndurance()
        {
            this.endurance++;
        }
        int agility;
        public void setAgility(int newAgility)
        {
            agility = newAgility;
        }
        public int getAgility()
        {
            return this.agility;
        }
        public void incAgility()
        {
            this.agility++;
        }
        public AttributeType Power { get => Power; set => Power = value; }
        public AttributeType Precision { get => Precision; set => Precision = value; }
        public AttributeType Endurance { get => Endurance; set => Endurance = value; }
        public AttributeType Agility { get => Agility; set => Agility = value; }
        //statString holds the basic attributes for player perusal
        public string statString;
        //damageDie is there for testing, it'll be replaced later on
        //for now it serves as the way to resolve injury
        public int damageDie;
        //only the player will have a deck of cards at this stage
        //it's in entity instead of GameState because it's easier to pass a different function this way
        //more to the point, it also allows for non-player entities to use decks later on
        //this is my excuse
        public StandardPlayingCardDeck Deck = new(false, 0);
        public List<StandardPlayingCard> Discard = new();
        public string DisplayStats()
        {
            string stats = $"{Name} \n {currentHP}/{maxHP}HP\n Power: {power}   Precision: {precision} \n Endurance: {endurance}   Agility {agility}";
            return stats;
        }
        public Random rng = new Random();
        public int statDie = 8;
        public int bonusRolls = 2;
        public int critMod = 0;
        public int successValue = 6;
        public Func<int, Random, Entity, int, int> ResolutionFunction = DiceMechanics.StatRoll;
        public List<StatusEffect> EndOfTurnStatusEffects = new List<StatusEffect>();
        public List<StatusEffect> NextEndOfTurnStatusEffects = new List<StatusEffect>();
        public int threshold = 2;
        //MergeStatusEffects:
        //Takes an a list of status effects as input- this list can contain duplicates and can be unsorted.
        //Sorts them.
        //Creates a new empty list to receive values.
        //Iterates through the freshly sorted list.
        //If the name of a status effect is shared between two adjacent values, the counter of the current one to the counter of the following one.
        //If not, the current effect is the last one of its type in the list. Knowing that that is such, it will have been given the counter values of all preceding elments.
        //Therefore, it has the sum total of all status effects, and can be pushed to the merged list.
        public List<StatusEffect> MergeStatusEffects(List<StatusEffect> unmergedList)
        {
            unmergedList.Sort();
            List<StatusEffect> mergedList = new();
            for (int i = 0; i < unmergedList.Count; i++)
            {
                if (unmergedList[i].Name == unmergedList[i + 1].Name)
                {
                    unmergedList[i + 1].statusCounter += unmergedList[i].statusCounter;
                }
                else
                {
                    mergedList.Add(unmergedList[i]);
                }
            }
            return mergedList;
        }

    }

    //Monster as a class is easier and cleaner than entity and allows for moderately more clarity for myself when writing code
    public class Monster : Entity
    {
        //this is a constructor
        public Monster(string Name, int MAXHP, int PWR, int PRC, int END, int AGI, int DIE, Func<int, Random, Entity, int, int> resFunc)
        {
            SetName(Name);
            setMaxHP(MAXHP);
            setPower(PWR);
            setPrecision(PRC);
            setEndurance(END);
            setAgility(AGI);
            damageDie = DIE;
            setCurrentHP(MAXHP);
            statString = DisplayStats();
            ResolutionFunction = resFunc;
        }
        //this takes an existing monster template
        //and pushes it somewhere
        public Monster CopyMonster(Monster baseMonster)
        {
            Monster newMonster = new Monster(baseMonster.getName(), baseMonster.getMaxHP(), baseMonster.getPower(), baseMonster.getPrecision(), baseMonster.getEndurance(), baseMonster.getAgility(), baseMonster.damageDie, baseMonster.ResolutionFunction);
            return newMonster;
        }
    }



    //this class will be expanded when moving from console to unity
    public class Location
    {
        public List<Monster> MonsterTable;
        public List<Relic> RelicTable;

        public Location()
        {
            MonsterTable.Add(new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Walking Chaffbeast", 20, 2, 2, 4, 4, 2, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Gremlin Assassin", 12, 8, 8, 1, 1, 1, CardMechanics.StatDraw));
            MonsterTable.Add(new Monster("Slime Hydra", 15, 6, 6, 6, 6, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Rotating Fiend", 10, 5, 6, 4, 3, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Recursed Wanderer", 8, 2, 2, 7, 7, 3, CardMechanics.StatDraw));
            RelicTable.Add(new Relic("Protean Symbiote", Relic.InflictProteanism));
        }
    }

    //Gamestate tracks the player's attributes and position.
    //The player *is* the Gamestate.
    //This may not be ideal, but given that the game is intended to be single player, it's fine.
    public class Gamestate : Entity
    {
        //skew is spent as a resource to influence random mechanics
        //There is a real chance that it, even though it is the secret sauce of the designer, will prove to be unimportant in gameplay
        //If that is the case, it will be rebalanced.
        public int maxSkew = 8;
        public int currentSkew;
        public Gamestate(string Name, int MAXHP, int PWR, int PRC, int END, int AGI, int DIE)
        {
            SetName(Name);
            setMaxHP(MAXHP);
            setPower(PWR);
            setPrecision(PRC);
            setEndurance(END);
            setAgility(AGI);
            damageDie = DIE;
            setCurrentHP(MAXHP);
            statString = DisplayStats();
            setCurrentHP(MAXHP);
            currentSkew = maxSkew;
            bool isLookingUp;
            bool isLookingRight;
            statString = DisplayStats();
            int[][] map;
            Consumable[] ConsumableInventory;
            Queue<Relic> RelicsHeld;
        }
        public void normalAttributes()
        {
            this.setMaxHP(15);
            this.setCurrentHP(this.getMaxHP());
            this.setPower(5);
            this.setPrecision(5);
            this.setEndurance(5);
            this.setAgility(5);
            this.SetName("Chad Testman");
        }

        public void randomAttributes()
        {
            Random dieRoll = new Random();
            this.setMaxHP(dieRoll.Next(1, 11) + 10);
            this.setCurrentHP(this.getMaxHP());
            this.setPower(dieRoll.Next(1, 4) + 3);
            this.setPrecision(dieRoll.Next(1, 4) + 3);
            this.setEndurance(dieRoll.Next(1, 4) + 3);
            this.setAgility(dieRoll.Next(1, 4) + 3);
            this.SetName("Chad Testman");
        }
        //GainStats:
        //whenever the player would gain stats, this function is called
        //the player rolls for each of their attributes
        //if they get fewer than $threshold successes on that roll, they increment the attribute by one
        //the total number of attributes not increased is added to their currentHP and maxHP
        //IMPLICATIONS:
        //as attributes go up, the likelihood of them increasing goes down
        //this implies that as attributes increase, maxHP is likely to increase instad
        //given how swingy the game's mechanics are (BY DESIGN), this isn't as good as it sounds
        public void GainStats(Random fate, int threshold)
        {
            Console.WriteLine($"You have won. Each attribute will increase if you achieve *fewer* than {threshold} successes on the roll.");
            int unchangedAttributes = 4;
            Console.WriteLine("Trying power.");
            if (DiceMechanics.QuietRoll(getPower(), fate, this, 0) < threshold)
            {
                incPower();
                Console.WriteLine("Your power increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying precision");

            if (DiceMechanics.QuietRoll(getPrecision(), fate, this, 0) < threshold)
            {
                incPrecision();
                Console.WriteLine("Your precision increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying endurance.");
            if (DiceMechanics.QuietRoll(this.getEndurance(), fate, this, 0) < threshold)
            {
                incEndurance();
                Console.WriteLine("Your endurance increased!\n");
                unchangedAttributes--;
            }
            Console.WriteLine("Trying agility.");
            if (DiceMechanics.QuietRoll(getAgility(), fate, this, 0) < threshold)
            {
                incAgility();
                Console.WriteLine("Your agility increased!\n");
                unchangedAttributes--;
            }
            if (unchangedAttributes > 0)
            {
                Console.WriteLine($"As a consolation prize, your maximum HP increases by {unchangedAttributes}.");
                changeMaxHP(unchangedAttributes);
                changeCurrentHP(unchangedAttributes);
            }
        }
        public void UpdateCharacterSheet()
        {

            Console.WriteLine(DisplayStats());
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
}
