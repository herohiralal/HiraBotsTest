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
            // attack
            // charge
            // shield wall
            // spinarooni
            // shield bash

            // block react
            // block and counterattack react
        }

        private struct Rogue
        {
            // attack
            // flurry
            // backlash
            // throwing knife
            // feign death

            // dodge react
            // dodge and counterattack react
        }

        private struct Wizard
        {
            // staff hit
            // zap
            // fireball
            // teleport away
            // hailstorm

            // phase react
        }
    }
}