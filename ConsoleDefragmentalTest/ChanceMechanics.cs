using System;
using System.Collections.Generic;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace CombatTest
{
    //all functions for dice chucking are contained here
    public class DiceMechanics
    {
        //Event Handlers here:
        public delegate void OpposedRollEventHandler(Entity attacker, Entity Defender);
        public static event OpposedRollEventHandler OpposedRollNegativeEvent;
        public delegate void PlayerHitHandler();
        public static event PlayerHitHandler PlayerHitEvent;
        public delegate void dieRollIsOneEventHandler();
        public static event dieRollIsOneEventHandler DieRollIsOneEvent;
        public delegate void PlayerIsAttackedHandler();
        public static event PlayerIsAttackedHandler PlayerIsAttackedEvent;
        //AttemptASpell:
        //Interactive
        //Inspiration: Spellcasting in Vermintide, push your luck dice games of all flavors
        //The function has the same signature as all other task resolution functions
        //The player is repeatedly given the option to roll a die and wishes to exceed the successValue on said roll.
        //Should the player not, the casting attempt ends and their currentScore is returned
        //If the player elects to roll a die, the result is evaluated.
        //If they roll the maximum value on the die, a success is marked and the meter is reduced, and the target number is decremented.
        //If instead they get above the successvalue of the die, a success is marked and the meter is increased by the die roll. The target number is decremented
        //If they do not exceed the successValue of the die, the meter is increased by the die roll
        //Should the value of the meter exceed that of the target number, the spell fails and the player receives zero successes and the casting attempt ends
        //Should the value of the meter be exactly met, the spell is a marvelous success and the player receives two extra successes and the casting attempt ends

        //IMPLICATIONS:
        //The spell will inevitably approach a marvelous success or a complete failure as rolls are made.
        //The target can decrease, but will never increase. 
        public static int AttemptASpell(int totalStat, Random fate, Entity ActiveEntity, int skew = 0)
        {
            int DIESIZE = (ActiveEntity.statDie)+skew;
            int SCALAR = (ActiveEntity.statDie)/2;
            int target = (totalStat * SCALAR)-skew;
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
                    int roll = DieRoll(DIESIZE, fate);
                    if (roll == DIESIZE) { currentTotal -= SCALAR; currentScore++; target--; }
                    else if (roll >= ActiveEntity.successValue)
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
                        //currently just returns zero
                        //This can be changed easily if further spell failure consequences are wished for
                        Console.WriteLine("You got greedy, as foul dabblers in the arcane often do.");
                        return SpellFailure(ActiveEntity);
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
        //SpellFailure:
        //This returns zero in the case of a spell failure.
        //You might think that this is a waste of effort
        //This is mainly included to future proof if I want to include consequences for dabbling in dangerous magickz
        public static int SpellFailure(Entity ActiveEntity)
        {
            //additional code goes here if further consequences for a miscast are desired later
            return 0;
        }
        //DieRoll
        //roll a die of size die, returning a value from 1 to dieSize
        //fate is the default random object
        public static int DieRoll(int dieSize, Random fate)
        {
            int roll = fate.Next(1, dieSize+1);
            return roll;
        }
        //OpposedRoll:
        //subtraction, but as a method
        //Used specifically for code clarity reasons.
        public static int OpposedRoll(int attacker, int defender, Entity AttackingEntity, Entity DefendingEntity)
        {
            int result = attacker - defender;
            if (result < 0)
            {
                if (OpposedRollNegativeEvent != null)
                {
                    OpposedRollNegativeEvent(AttackingEntity, DefendingEntity);
                }
            }
            return result;
        }
        //TriangularNumber:
        //generates the nth triangular number for an input integer n
        //please don't pass it negative values
        public static int TriangularNumber(int n)
        {
            if (n < 0)
            {
                n = -n;
            }
            int triangularNumber = n * (n + 1) / 2;
            return triangularNumber;
        }
        //rolls a number of dice of size die equal to the total stat offered as input.
        //if the value is the maximum, roll two more and mark a success
        //else if the value meets or exceeds the success value, mark a success
        //basic parameters for testing, can be modified later
        //prints out a string for player clarity and testing
        //Skew increases or decreases the size of the die
        //IMPLICATIONS:
        //The number of returned successes can get very big, but generally does not.
        //This function can theoretically run infinitely.
        //It is exceedingly unlikely that it will, and users who experience this should report to an astrologer to have their fates examined.
        public static int StatRoll(int totalStat, Random fate, Entity activeEntity, int skew = 0)
        {
            int realDie;
            realDie = activeEntity.statDie + skew;
            int successes = 0;
            for (int i = 0; i < totalStat; i++)
            {
                int currentRoll = DieRoll(realDie, fate);
                Console.Write($" {currentRoll}");
                if (currentRoll == realDie - activeEntity.critMod)
                {
                    successes++;
                    i -= activeEntity.bonusRolls;
                    Console.Write("! Two more rolls.");
                }
                else if (currentRoll >= activeEntity.successValue)
                {
                    successes++;
                    Console.Write("!");
                }
                else
                {
                    Console.Write(".");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"\n A total of {successes} successes. \n");
            return successes;
        }
        //QuietRoll:
        //statRoll without as many print statements
        //All the comments are the same.
        public static int QuietRoll(int totalStat, Random fate, Entity activeEntity, int skew = 0)
        {
            int realDie;
            realDie = activeEntity.statDie + skew;
            int successes = 0;
            for (int i = 0; i < totalStat; i++)
            {
                int currentRoll = DieRoll(realDie, fate);
                if (currentRoll >= realDie - activeEntity.critMod)
                {
                    successes++;
                    i -= activeEntity.bonusRolls;
                }
                else if (currentRoll >= activeEntity.successValue)
                {
                    successes++;
                }
            }
            Console.WriteLine($"\n A total of {successes} successes. \n");
            Console.ReadKey();
            return successes;
        }
    }
    public static class CardMechanics
    {
        //Helper function, takes StandardPlayingCard
        //converts that card to a string of type ^(10|[1-9]|K|J|Q)[♣♦♥♠]$
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
        //SuitToChar:
        //Takes a suit and returns a symbol matching the suit
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
        //RankToInt:
        //Takes a card rank, and returns an integer.
        //Aces are low, jokers are zero, face cards are 11-13, as one might expect.
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
        //StatDraw:
        //Takes in a totalStat, a random object, an entity, and a skew value
        //draws cards totalStat-skew times, marking a success for each cardRank+skew value above 9.
        //Jacks draw an additional card, Queens draw two, Kings draw two and generate two successes.
        //One will note that this has exactly the same signature as StatRoll, and this is for the purposes of interoperability and lambda inputs.
        //IMPLICATIONS:
        //With a large enough attribute this is broken.
        //You can draw your deck twice and get a full heal as well as an unreasonable number of successes.
        //This will no longer run infinitely.
        //As it no longer runs infinitely, the unreasonably impressive result is written off as "working as intended", particularly as it is uncommon.
        //This may be fixed later on.
        public static int StatDraw(int totalStat, Random rand, Entity activeEntity, int skew= 0)
        {
            int defaultSuccess = 9 - skew;
            int successes = 0;
            bool deckHasBeenDrawn = false;
            for (int i = 0-skew; i < totalStat; i++)
            {
                if (activeEntity.Deck.IsEmpty)
                {

                    foreach (StandardPlayingCard discardedCard in activeEntity.Discard)
                    {
                        Console.WriteLine("Reshuffling. All missing HP restored.");
                        activeEntity.Deck.PlaceOnTop(discardedCard);
                        activeEntity.Discard.Remove(discardedCard);
                        if (!deckHasBeenDrawn)
                        {
                            activeEntity.currentHP = activeEntity.maxHP;
                            deckHasBeenDrawn = true;
                        }
                        else
                        {
                            Console.WriteLine("That's quite enough of that.");
                            return successes;
                        }
                    }
                    activeEntity.Deck.Shuffle();
                }
                StandardPlayingCard currCard = activeEntity.Deck.Draw();
                activeEntity.Discard.Add(currCard);
                //There is temptation to add skew to every i adjustment here, but that seems unreasonably punishing
                //It would be desirable to implement a consequence for skew in this function, however.
                int cardRank = RankToInt(currCard.Rank);
                switch (cardRank + skew)
                {
                    case < 9:
                        Console.WriteLine(SPCToString(currCard) + ".");
                        break;
                    case < 11:
                        successes++;
                        Console.WriteLine(SPCToString(currCard) + "! A success!");
                        break;
                    case 11:
                        successes++;
                        i -= 1;
                        Console.WriteLine(SPCToString(currCard) + "! A success and an additional draw!");
                        break;
                    case 12:
                        successes ++;
                        i-=2;
                        Console.WriteLine(SPCToString(currCard) + "! A success and two additional draws!");
                        break;
                    case > 12:
                        successes += 2;
                        i -= 2;
                        Console.WriteLine(SPCToString(currCard) + "! Two successes and two additional draws!");
                        break;

                }
            }
            Console.WriteLine("");
            Console.Write("A total of " + successes.ToString() + " successes.");
            return successes;
        }
    }

}