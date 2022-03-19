// ReSharper disable all

using UnityEngine;

namespace AIEngineTest
{
    /**
     * Golden rules:
     * 1. Every action must be usable for more than one goal.
     * 2. Every goal-target must have more than one action that's able to finish it.
     * 3. Every plan must be at least 2 actions long.
     */
    internal static class AbilitiesAndVariables
    {
        private struct Base
        {
            // equip - set current equipment to owned equipment
            // unequip - set current equipment to none
            // patrol - current equipment must be none, earns safety (not really)
            // hit react
        }

        private struct Fighter
        {
            // attack - earns safety (not really), earns rampage (not really), and is expensive when the health is low, is cheaper after shield bashing
            // charge - relocate to target, must be far away, earns rampage (not really), has cooldown
            // shield wall - earns safety (not really) and is cheap when the health is low, has cooldown
            // spinarooni - earns safety (not really), earns rampage (not really), earns rampage (not really), is cheap when surrounded, has cooldown
            // shield bash - earns rampage (not really), earns enemy weakened, has cooldown

            // block react
            // block and counterattack react
        }

        private struct Rogue
        {
            // attack - earns guile, earns bloodlust, costs stamina
            // flurry - costs bloodlust, earns guile, costs stamina, has cooldown
            // backlash - costs bloodlust, earns guile, costs stamina, has cooldown
            // throwing knife - costs guile, earns bloodlust, costs stamina, has cooldown
            // kick - costs guile, earns bloodlust, costs stamina, has cooldown
            // catch breath - earns stamina, costs anonymity, costs 

            // dodge react
            // dodge and counterattack react
        }

        private struct Wizard
        {
            // attack - earns havoc, earns support, costs stamina
            // fireball - costs havoc, earns support, costs spellpoints, has cooldown
            // mass heal - costs support, earns havoc, costs spellpoints, has cooldown
            // hailstorm - costs support, earns havoc, costs spellpoints, has cooldown
            // summon demon - costs havoc, earns support, costs spellpoints, has cooldown
            // catch breath - earns stamina, costs havoc, costs support

            // phase react
        }
    }
}