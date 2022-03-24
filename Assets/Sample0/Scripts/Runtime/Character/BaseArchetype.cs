using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class BaseArchetype : MonoBehaviour,
        IHiraBotArchetype<Animator>,
        IHiraBotArchetype<AnimatorHelper>,
        IHiraBotArchetype<CharacterAttributes>,
        IHiraBotArchetype<CharacterMeshWeaponSocketProvider>,
        IHiraBotArchetype<SkinnedMeshRenderer>,
        IHiraBotArchetype<NavMeshAgent>,
        IHiraBotArchetype<ConsolidatedSensor>,
        IHiraBotArchetype<HiraLGOAPRealtimeBot>,
        IHiraBotArchetype<SightStimulus>,
        IHiraBotArchetype<SoundStimulus>
    {
        [SerializeField] public Animator m_Animator = null;
        [SerializeField] public AnimatorHelper m_AnimatorHelper = null;
        [SerializeField] public CharacterAttributes m_CharacterAttributes = null;
        [SerializeField] public CharacterMeshWeaponSocketProvider m_CharacterMeshWeaponSocketProvider = null;
        [SerializeField] public SkinnedMeshRenderer m_SkinnedMeshRenderer = null;
        [SerializeField] public NavMeshAgent m_NavMeshAgent = null;
        [SerializeField] public ConsolidatedSensor m_Sensor = null;
        [SerializeField] public HiraLGOAPRealtimeBot m_Brain = null;
        [SerializeField] public SightStimulus m_SightStimulus = null;
        [SerializeField] public SoundStimulus m_SoundStimulus = null;

        Animator IHiraBotArchetype<Animator>.component => m_Animator;
        AnimatorHelper IHiraBotArchetype<AnimatorHelper>.component => m_AnimatorHelper;
        CharacterAttributes IHiraBotArchetype<CharacterAttributes>.component => m_CharacterAttributes;
        CharacterMeshWeaponSocketProvider IHiraBotArchetype<CharacterMeshWeaponSocketProvider>.component => m_CharacterMeshWeaponSocketProvider;
        SkinnedMeshRenderer IHiraBotArchetype<SkinnedMeshRenderer>.component => m_SkinnedMeshRenderer;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
        ConsolidatedSensor IHiraBotArchetype<ConsolidatedSensor>.component => m_Sensor;
        HiraLGOAPRealtimeBot IHiraBotArchetype<HiraLGOAPRealtimeBot>.component => m_Brain;
        SightStimulus IHiraBotArchetype<SightStimulus>.component => m_SightStimulus;
        SoundStimulus IHiraBotArchetype<SoundStimulus>.component => m_SoundStimulus;
    }
}