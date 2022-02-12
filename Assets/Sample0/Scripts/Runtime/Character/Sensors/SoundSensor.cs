using UnityEngine.AI;

namespace AIEngineTest
{
    public class SoundSensor : HiraBotSphericalSensor
    {
        public SoundSensor()
        {
            stimulusMask = (int) HiraBotsDefaultStimulusMask.Sound;
        }
    }
}