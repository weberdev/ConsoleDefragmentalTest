using System;
using System.Collections.Generic;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace ConsoleDefragmentalTest
{
    //all functions for dice chucking are contained here
    public class DiceMechanics
    {
        public int AttemptASpell(int attribute)
        {
            Random random = new Random();
            int DIESIZE = 8;
            int SCALAR = DIESIZE / 2 + 1;
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
                    for (int i = 0; i < target - currentTotal; i++)
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
    public static class CardMechanics
    {
        public static string SPCToString(StandardPlayingCard card)
        {
            int cardRank = RankToInt(card.Rank);
            string cardRankString;
            if (cardRank == 13)
            {
                cardRankString = "K";
            }
            else if (cardRank == 12)
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
            string s = cardRankString + SuitToChar(card.Suit);
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
                if (activeEntity.Deck.IsEmpty)
                {
                    foreach (StandardPlayingCard discardedCard in activeEntity.Discard)
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
                switch (cardRank + skew)
                {
                    case < 9:
                        Console.WriteLine(SPCToString(currCard) + ".");
                        break;
                    case 9:
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
            Console.Write("A total of " + total.ToString() + " successes.");
            return total;
        }
    }

}