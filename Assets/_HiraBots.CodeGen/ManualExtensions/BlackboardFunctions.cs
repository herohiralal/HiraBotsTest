using System.Runtime.CompilerServices;
using UnityEngine.AI;

namespace AIEngineTest
{
    public static class BlackboardFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateHiraBotsDecorator("be71e712bb664a279449d0c57d775c5d")]
        public static bool CooldownDecorator(bool invert, ref float globalTime, ref float lastExecutionTime, float cooldownTimer)
        {
            return invert != ((globalTime - lastExecutionTime) >= cooldownTimer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CooldownDecoratorUpdateDescription(bool invert, BlackboardTemplate.KeySelector globalTime, BlackboardTemplate.KeySelector lastExecutionTime, float cooldownTimer, out string staticDescription)
        {
            if (!globalTime.selectedKey.isValid || !lastExecutionTime.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"If it {(invert ? "has not" : "has")} been {cooldownTimer} second(s) or more since {lastExecutionTime.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateHiraBotsEffector("9a59a009ac7647a887c58ca253d753b5")]
        // ReSharper disable once RedundantAssignment
        public static void UpdateLastExecutionTimeEffector(ref float globalTime, ref float lastExecutionTime)
        {
            lastExecutionTime = globalTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateLastExecutionTimeEffectorUpdateDescription(BlackboardTemplate.KeySelector globalTime, BlackboardTemplate.KeySelector lastExecutionTime, out string staticDescription)
        {
            if (!globalTime.selectedKey.isValid || !lastExecutionTime.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"Set {lastExecutionTime.selectedKey.name} to {globalTime.selectedKey.name}.";
        }
    }
}