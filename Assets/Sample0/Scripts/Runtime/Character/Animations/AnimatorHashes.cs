using UnityEngine;

namespace AIEngineTest
{
    public static class AnimatorHashes
    {
        public static readonly int s_Speed = Animator.StringToHash("Speed");
        public static readonly int s_Bowing = Animator.StringToHash("Bowing");
        public static readonly int s_WeaponType = Animator.StringToHash("WeaponType");
        public static readonly int s_PlayMontage = Animator.StringToHash("PlayMontage");
        public static readonly int s_MontageType = Animator.StringToHash("MontageType");
        public static readonly int s_InterruptMontage = Animator.StringToHash("InterruptMontage");
        public static readonly int s_ActionNum = Animator.StringToHash("ActionNum");
    }
}