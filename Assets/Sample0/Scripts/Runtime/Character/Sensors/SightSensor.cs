using UnityEngine.AI;

namespace AIEngineTest
{
    public class SightSensor : HiraBotRadialSensor
    {
        public SightSensor()
        {
            stimulusMask = (int) HiraBotsDefaultStimulusMask.Sight;
        }
    }
}