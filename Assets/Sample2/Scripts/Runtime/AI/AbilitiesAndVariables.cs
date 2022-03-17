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
            // patrol
            // hit react
        }

        private struct Fighter
        {
            // attack - earns valor, earns rampage, costs stamina
            // charge - costs rampage, earns valor, cost stamina, has cooldown
            // shield wall - costs valor, earns rampage, costs stamina, has cooldown
            // spinarooni - earns valor, costs rampage, costs stamina, has cooldown
            // shield bash - earns rampage, costs valor, costs stamina, has cooldown
            // catch breath - earns stamina, costs rampage, costs valor

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