// ReSharper disable all

using UnityEngine;

namespace AIEngineTest
{
    internal static class AbilitiesAndVariables
    {
        private struct Fighter
        {
            private float Stamina;
            private float BoredomFromCurrentEnemy;
            private float Aggression; // increases w every attack, reduce by 50 every kill

            private void CatchBreath()
            {
                Stamina += 100f;
            }

            private void SingleAttack()
            {
                if (Stamina >= 15)
                {
                    Stamina -= Random.Range(10f, 15f);
                    BoredomFromCurrentEnemy += Random.Range(10f, 20f);
                    Aggression += Random.Range(5f, 10f);
                }
            }

            private void ComboAttack2Hit()
            {
                if (Aggression >= 30 && Stamina >= 25)
                {
                    Stamina -= Random.Range(20f, 25f);
                    BoredomFromCurrentEnemy += Random.Range(15f, 20f);
                    Aggression += Random.Range(10f, 20f);
                }
            }

            private void ComboAttack3Hit()
            {
                if (Aggression >= 50 && Stamina >= 37.5)
                {
                    Stamina -= Random.Range(30f, 37.5f);
                    BoredomFromCurrentEnemy += Random.Range(17.5f, 20f);
                    Aggression += Random.Range(15f, 30f);
                }
            }

            private void ChargeToDifferentEnemy()
            {
                if (BoredomFromCurrentEnemy >= 100f && Stamina >= 45f)
                {
                    Stamina -= Random.Range(35f, 45f);
                    BoredomFromCurrentEnemy = 0f;
                    Aggression += Random.Range(0f, 5f);
                }
            }
        }
    }
}