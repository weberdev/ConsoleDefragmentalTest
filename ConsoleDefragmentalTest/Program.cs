using System;
using System.Collections.Generic;
using System.Linq;
using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace CombatTest
{
    
    class Program
    {
        static void Main(string[] args)
        {

            string yourName;
            string yourResolution;
            Random whimsOfFate = new Random();
            Console.WriteLine("Please enter your name.");
            yourName = Console.ReadLine();
            Console.WriteLine($"Howdy, {yourName}, are you a dice thrower or a card hustler? D for dice, C for cards.");
            yourResolution = Console.ReadLine().ToLower();
            while(yourResolution!= "c" && yourResolution != "d")
            {
                Console.WriteLine("Please choose dice or cards. C or D.");
                yourResolution = Console.ReadLine().ToLower();
            }
            Gamestate currentState = new(yourName, 15, 4, 5, 4, 3, 4);
            if (yourResolution == "c")
            {
                currentState.ResolutionFunction = CardMechanics.StatDraw;
            }
            else 
            {
                currentState.ResolutionFunction = DiceMechanics.StatRoll;
            }
            currentState.Deck.Shuffle();
            Console.WriteLine(currentState.statString);
            Monster currentMonster = new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3, DiceMechanics.StatRoll);
            CombatMechanics.FightLoop(currentState, currentMonster, whimsOfFate);
        }
    }
}

