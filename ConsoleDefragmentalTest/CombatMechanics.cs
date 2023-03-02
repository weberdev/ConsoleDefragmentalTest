using System;
using System.Collections.Generic;

namespace CombatTest
{
    public static class CombatMechanics
    {
        //EndOfTurnForIndividual:
        //As a matter of style, I really like automating everything I possibly can.
        //Thus, EndOfTurn is a call to EndOfTurnForIndividual for each participant
        //Theoretically this might be beneficial to other people down the road.
        //That other person could even be me.
        //This is called by EndOfTurn
        //Iterates through the input entity's status effects and applies them before removing them from the list.
        //If there are effects that have a counter, they are put on nextEndOfTurnStatusEffects.
        //When EndOfTurnStatusEffects is empty, all elements of NextEndOfTurnStatusEffects are added to EndOfTurnStatusEffects
        //That list is then cleared.
        public static void EndOfTurnForIndividual(Entity currentEntity)
        {
            foreach (StatusEffect activeEffect in currentEntity.EndOfTurnStatusEffects)
            {
                activeEffect.statusName(currentEntity, activeEffect.statusCounter);
                currentEntity.NextEndOfTurnStatusEffects.Remove(activeEffect);
            }
            foreach (StatusEffect inactiveEffect in currentEntity.NextEndOfTurnStatusEffects)
            {
                currentEntity.EndOfTurnStatusEffects.Add(inactiveEffect);
            }
            currentEntity.NextEndOfTurnStatusEffects.Clear();
            currentEntity.EndOfTurnStatusEffects = currentEntity.MergeStatusEffects(currentEntity.EndOfTurnStatusEffects);
        }
        //EndOfTurn:
        //Called at the end of a turn cycle. Calls EndOfTurnForIndividual on the player and foe.
        //Checks for death.
        //As before, if player.currentHP < 0 calls Defeat.
        //THEN, if foe.currentHP < 0 calls Victory.
        public static void EndOfTurn(Gamestate player, Monster foe)
        {
            EndOfTurnForIndividual(player);
            EndOfTurnForIndividual(foe);
            if (player.getCurrentHP() < 0)
            {
                Defeat();
            }
            else if (foe.getCurrentHP() < 0)
            {
                Random rng = new Random();
                Victory(player, foe, rng);
            }
        }
        //ApplyStatusEffect
        //Take an entity and a StatusEffectObject
        //Appends the StatusEffect to the entity's EndOfTurnStatusEffects list
        public static void ApplyStatusEffect(Entity currentEntity, StatusEffect currentEffect)
        {
            currentEntity.EndOfTurnStatusEffects.Add(currentEffect);
        }
        //ReapplyStatusEffect
        //Takes an entity and a statusEffect object.
        //appends the statusEffect to the NextEndOfTurnStatus list.
        public static void ReapplyStatusEffect(Entity currentEntity, StatusEffect currentEffect)
        {
            currentEntity.NextEndOfTurnStatusEffects.Add(currentEffect);
        }
        //Bleed:
        //A simple, easy bleed function.
        //No, seriously.
        //breakpoint is the value above which the bleed will worsen.
        //If the bleedCounter value is equal or less than breakpoint, the bleed will inevitably recede.
        //This is vaguely realistic.
        public static void Bleed(Entity bleedingEntity, int bleedCounter)
        {
            int breakpoint = 3;
            DealDamage(bleedingEntity, bleedCounter);
            int nextBleed = (int)Math.Sqrt((bleedCounter ^ 2) - (breakpoint ^ 2 + 1));
            if (nextBleed > 0)
            {
                StatusEffect BleedObject = new StatusEffect("Bleed", Bleed, nextBleed);
                ReapplyStatusEffect(bleedingEntity, BleedObject);
            }
        }
        //Poison:
        //Not an atypical poison mechanic.
        //Poison deals damage.
        //Ticks down.
        public static void Poison(Entity poisonedEntity, int poisonCounter)
        {
            DealDamage(poisonedEntity, poisonCounter);
            if (poisonCounter > 1)
            {
                StatusEffect PoisonObject = new StatusEffect("Poison", Poison, poisonCounter - 1);
                ReapplyStatusEffect(poisonedEntity, PoisonObject);
            }
        }
        public static void LocalizedProteanism(Entity proteanEntity, int proteanIntegerForTypeChecking)
        {
            int currentStat = proteanEntity.getPower();
            proteanEntity.setPower(proteanEntity.getAgility());
            proteanEntity.setAgility(proteanEntity.getEndurance());
            proteanEntity.setEndurance(proteanEntity.getPrecision());
            proteanEntity.setPrecision(currentStat);
            StatusEffect ProteanObject = new StatusEffect("Localized Proteanism", LocalizedProteanism, proteanIntegerForTypeChecking);
            ReapplyStatusEffect(proteanEntity, ProteanObject);
        }

        //DamageRoll:
        //Takes in the number of net successes on a resisted attack roll, the size of the user's damage die, a function from int to int, and a random object
        //Golly that's a lot of parameters
        //The damage die will be rolled a number of times equal to damageCounterFunc(successes)
        //The total of these rolls will be returned to the calling function.
        //I have been informed that a triangular number of damage rolls is too swingy
        //This allows future proofing if the critics prove right.
        public static int DamageRoll(int successes, int die, Func<int, int> damageCounterFunc, Random fate)
        {
            int damage = 0;
            for (int i = 1; damageCounterFunc(successes) >= i; i++)
            {
                int damageDieRoll = DiceMechanics.DieRoll(die, fate);
                Console.Write(damageDieRoll + " ");
                damage += damageDieRoll;
            }
            Console.WriteLine();
            return damage;
        }
        //DealDamage:
        //damage is dealt to a target entity and is subtracted from their current HP
        public static void DealDamage(Entity target, int damage)
        {
            target.changeCurrentHP(-damage);
        }
        //Attack:
        //takes in two entities, an attacker and a defender, as well as their defensive stats
        //AND a random object, the amount of skew the attacker used,
        //and a ResolutionFunction with parameters for the attacker's primary stat, random object, attacker, and skew used
        //The attacker makes a call to the ResolutionFunction
        //Defender makes a call to StatRoll
        //netHits is evaluated with a call to OpposedRoll between the attacker and defender
        //if attacker has more successes
        //damage is dealt proportionally
        //if not
        //damage is not dealt
        public static void Attack(Entity attacker, Entity defender, int attackingStat, int defendingStat, Random fate, int skewUsed)
        {
            Console.WriteLine("Attacker's Results: ");
            int attackerHits = attacker.ResolutionFunction(attackingStat, fate, attacker, skewUsed);
            Console.ReadKey();
            Console.WriteLine("\n Defender's results: ");
            int defenderHits = defender.ResolutionFunction(defendingStat, fate, defender, 0);
            Console.ReadKey();
            int netHits = DiceMechanics.OpposedRoll(attackerHits, defenderHits, attacker, defender);
            if (netHits > 0)
            {
                Console.WriteLine($"{netHits} more successes!");
                int damageTotal = DamageRoll(netHits, attacker.damageDie, DiceMechanics.TriangularNumber, fate);
                DealDamage(defender, damageTotal);
                Console.WriteLine($"The swing connects for {damageTotal} damage.");
            }
        }
        //FightLoop:
        //Takes in the player as a Gamestate, a Monster, and a Random object
        //this is the primary combat engine
        //the monster appears and is displayed
        //then the player has a choice of actions
        //so far (2/13/2023): the player only has the option of bonking it over the head or bonking it over the head more precisely
        //When the foe's HP is 0 or less, the loop ends.
        public static void FightLoop(Gamestate protagonist, Monster foe, Random fate)
        {
            Console.WriteLine($"The {foe.getName()} approaches!");
            string statBlock = foe.DisplayStats();
            Console.WriteLine(statBlock);
            while (foe.getCurrentHP() > 0)
            {
                Console.WriteLine($"{protagonist.getName()}: HP: {protagonist.getCurrentHP()}/{protagonist.getMaxHP()} SKEW: {protagonist.currentSkew}/{protagonist.maxSkew}");
                Console.WriteLine($"{foe.getName()}: {foe.getCurrentHP()}/{foe.getMaxHP()}");
                Console.WriteLine("This is the demo, you may only attack.");
                PlayerAttack(foe, protagonist, fate);
            }


        }
        //PlayerAttack:
        //Takes in a Monster, the player as a Gamestate, a Random Object, and a ResolutionFunction.
        //RESOLUTION FUNCTIONS:
        //ResolutionFunctions are functions that take in an integer for the actor's currently used attribute, a Random object, the active Entity, and the skew used, and return an integer
        //Designer intent is to have them check for a random condition attribute times, increasing the number of checks in the case of an optimal result, and returning number of successful checks
        //KEY ATTACK STAT:
        //The function begins with the player being given to attack heavily or precisely (keying their attack this action to EITHER power or precision)
        //HANDLING SKEW:
        //Once this is determined (and determined correctly), they are prompted to use up to two skew from their stack.
        //If they attempt to use more than that or less than zero, they use zero instead.
        //After this, the skew is deducted from their stack.
        //The player is then given the option to skew the values upwards or downwards.
        //Skewing downwards sets the skewUsed to its additive inverse.
        //AFTER SKEW RESOLUTION:
        //A printed description is given of the attack's type, and Attack is called with the player's appropriate stat, random object, player object, skew used, and resolution function.
        //If the player reduces the monster's currentHP to 0 or less, the player wins and Victory is called, taking in the player, the monster, and the random object.
        //If the player does not, the monster gets a turn. If the player's currentHP remains above zero, the FightLoop continues.
        private static void PlayerAttack(Monster foe, Gamestate protagonist, Random fate)
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
                Attack(protagonist, foe, protagonist.getPower(), foe.getEndurance(), fate, skewUsed);
            }
            else if (actionChoice == "p")
            {
                Console.WriteLine("You swing precisely.");
                Attack(protagonist, foe, protagonist.getPrecision(), foe.getAgility(), fate, skewUsed);
            }
            else
            {
                Console.WriteLine("Code shouldn't have been able to get here.");
                Console.WriteLine("You might have been trying to cast a spell");
            }
            if (foe.getCurrentHP() < 1) { Victory(protagonist, foe, fate); }
            else { MonsterAttack(foe, protagonist, fate, 0); }

        }
        //MonsterAttack:
        //Takes in a Monster, the Gamestate, the random object, and a skew value (generally set to zero).
        //The foe acts, acting with rudimentary AI.
        //GENERALLY BETTER DECISION:
        //Two times out of three it will choise the (generally) best stat comparison.
        //If it can put its best attribute into the defender's corresponding weakest attribute, it will do that.
        //If its attributes match, it will instead attack the defender's weakest attribute.
        //If it has a high attribute that matches the defender's higher attribute, it will calculate the difference between the two corresponding high attributes and the two corresponding low attributes.
        //Then choose the one with the largest difference.
        //If there's an oversight and not all cases are covered, it will default to a general power into endurance attack.
        //GENERALLY WORSE DECISION:
        //The monster attacks with its higher attribute into the defender's lower attribute, defaulting to power-> endurance in case of a tie.
        //CONCLUSION:
        //If the player's HP is reduced to 0 or less, the player loses.
        //Otherwise the fight continues. 
        private static void MonsterAttack(Monster foe, Gamestate protagonist, Random fate, int skew)
        {
            int betterAttackingStat = Math.Max(foe.getPower(), foe.getPrecision());
            int worseAttackingStat = Math.Min(foe.getPower(), foe.getPrecision());
            int betterTargetStat = Math.Min(protagonist.getAgility(), protagonist.getEndurance());
            int worseTargetStat = Math.Min(protagonist.getAgility(), protagonist.getEndurance());
            Console.WriteLine($"It is now the {foe.getName()}'s turn.");
            if (DiceMechanics.DieRoll(3, fate) > 2)
            {
                if (betterAttackingStat == foe.getPower() && betterTargetStat == protagonist.getEndurance())
                {
                    Attack(foe, protagonist, foe.getPower(), protagonist.getEndurance(), fate, 0);
                }
                else if (betterAttackingStat == foe.getPrecision() && betterTargetStat == protagonist.getAgility())
                {
                    Attack(foe, protagonist, foe.getPrecision(), protagonist.getEndurance(), fate, 0);
                }
                else if (betterAttackingStat == worseAttackingStat)
                {
                    Attack(foe, protagonist, betterAttackingStat, worseTargetStat, fate, 0);
                }
                else if (foe.getPower() - protagonist.getEndurance() > foe.getPrecision() - protagonist.getAgility())
                {
                    Attack(foe, protagonist, foe.getPower(), protagonist.getEndurance(), fate, 0);

                }
                else if (foe.getPower() - protagonist.getEndurance() < foe.getPrecision() - protagonist.getAgility())
                {
                    Attack(foe, protagonist, foe.getPrecision(), protagonist.getAgility(), fate, 0);
                }
                else
                {
                    Attack(foe, protagonist, foe.getPower(), protagonist.getEndurance(), fate, 0);
                }
            }
            else
            {
                if (foe.getPower() < foe.getPrecision())
                {
                    Attack(foe, protagonist, foe.getPrecision(), protagonist.getAgility(), fate, 0);
                }
                else
                {
                    Attack(foe, protagonist, foe.getPower(), protagonist.getEndurance(), fate, 0);
                }
            }
            if (protagonist.getCurrentHP() < 1) { Defeat(); }
            EndOfTurn(protagonist, foe);
        }
        //Victory:
        //Takes in the Gamestate and a Monster, as well as (surprise) a random.
        //Populates the MonsterTable, a list of monsters that might be generated.
        //Calls GainStates on the player.
        //Generates a new monster from the MonsterTable.
        //displays the player's new stats.
        //Calls fightloop with the generated monster, starting a fight.
        public static void Victory(Gamestate protagonist, Monster foe, Random fate)
        {
            List<Monster> MonsterTable = new List<Monster>();
            MonsterTable.Add(new Monster("Hardwired Vargoblin", 8, 3, 3, 2, 4, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Walking Chaffbeast", 20, 2, 2, 4, 4, 2, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Gremlin Assassin", 12, 8, 8, 1, 1, 1, CardMechanics.StatDraw));
            MonsterTable.Add(new Monster("Slime Hydra", 15, 6, 6, 6, 6, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Rotating Fiend", 10, 5, 6, 4, 3, 3, DiceMechanics.StatRoll));
            MonsterTable.Add(new Monster("Recursed Wanderer", 8, 2, 2, 7, 7, 3, CardMechanics.StatDraw));
            Console.WriteLine($"The {foe.getName()} is slain!");
            protagonist.GainStats(fate, protagonist.threshold);
            Console.ReadKey();
            int listLength = MonsterTable.Count;
            Monster currentMonster = MonsterTable[fate.Next(listLength)];
            currentMonster = currentMonster.CopyMonster(currentMonster);
            protagonist.UpdateCharacterSheet();
            FightLoop(protagonist, currentMonster, fate);
        }
        //Defeat:
        //Called when the player's HP hits zero.
        //Prints a message, and hangs on input.
        //Then quits.
        public static void Defeat()
        {
            Console.WriteLine("You're out of HP, and take your final bow.\n Would you like to play again? \n Choose No or No.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}