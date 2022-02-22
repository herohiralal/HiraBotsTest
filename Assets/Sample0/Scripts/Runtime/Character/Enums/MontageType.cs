namespace AIEngineTest
{
    [System.Serializable, UnityEngine.AI.ExposedToHiraBots("D8D1DA80-E3F8-4899-9DBD-ADB97DBC52A8")]
    public enum MontageType
    {
        None = 0,
        MeleeAttackRight = 1,
        MeleeAttackLeft = 2,
        Unsheathe = 3,
        Sheathe = 4,
        Bow = 5,
        Block = 6,
        Dodge = 7,
        Cast = 8,
        DualAttack = 9,
    }
}