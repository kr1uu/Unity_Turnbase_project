public static class UtilityPresets
{
    public static BattleAI.UtilityProfile GetProfile(int aiProfileId)
    {
        switch (aiProfileId)
        {
            case 1: return Aggressive;
            case 2: return Debuffer;
            case 3: return Balanced;
            case 4: return Assasin;
            case 5: return Shielder;
            case 6: return Captain;
            case 7: return Oldgaurd;
            case 8: return Nemesis;
            case 9: return Healer;
            default: return Balanced;
        }
    }

    public static BattleAI.UtilityProfile Aggressive = new BattleAI.UtilityProfile
    {
        attackWeight = 1.5f,
        aoeWeight = 1.2f,
        healWeight = 0.2f,
        defendWeight = 0.5f,
        debuffWeight = 0.5f,
        finisherWeight = 1.5f
    };

    public static BattleAI.UtilityProfile Debuffer = new BattleAI.UtilityProfile
    {
        attackWeight = 0.8f,
        aoeWeight = 0.7f,
        healWeight = 0.3f,
        defendWeight = 0.8f,
        debuffWeight = 2.0f,
        finisherWeight = 1.0f
    };

    public static BattleAI.UtilityProfile Balanced = new BattleAI.UtilityProfile
    {
        attackWeight = 1.0f,
        aoeWeight = 1.0f,
        healWeight = 1.0f,
        defendWeight = 1.0f,
        debuffWeight = 1.0f,
        finisherWeight = 1.0f
    };

    public static BattleAI.UtilityProfile Assasin = new BattleAI.UtilityProfile
    {
        attackWeight = 1.4f,
        aoeWeight = 0.3f,
        healWeight = 0.1f,
        defendWeight = 0.4f,
        debuffWeight = 0.4f,
        finisherWeight = 2.2f
    };

    public static BattleAI.UtilityProfile Shielder = new BattleAI.UtilityProfile
    {
        attackWeight = 0.6f,
        aoeWeight = 0.2f,
        healWeight = 0.6f,
        defendWeight = 2.0f,
        debuffWeight = 0.5f,
        finisherWeight = 0.3f
    };

    public static BattleAI.UtilityProfile Captain = new BattleAI.UtilityProfile
    {
        attackWeight = 0.8f,
        aoeWeight = 0.6f,
        healWeight = 0.4f,
        defendWeight = 0.9f,
        debuffWeight = 1.8f,
        finisherWeight = 0.6f
    };

    public static BattleAI.UtilityProfile Oldgaurd = new BattleAI.UtilityProfile
    {
        attackWeight = 1.0f,
        aoeWeight = 0.7f,
        healWeight = 0.5f,
        defendWeight = 1.0f,
        debuffWeight = 0.8f,
        finisherWeight = 0.9f
    };

    public static BattleAI.UtilityProfile Nemesis = new BattleAI.UtilityProfile
    {
        attackWeight = 1.0f,
        debuffWeight = 0.0f,
        defendWeight = 0.0f,
        healWeight = 0.0f,
        aoeWeight = 0.0f,
        finisherWeight = 0.0f,

        hpBias = 0.0f,
        threatBias = 3.0f     
    };
    public static BattleAI.UtilityProfile Healer = new BattleAI.UtilityProfile
    {
        attackWeight = 0.2f,
        aoeWeight = 0.1f,
        healWeight = 2.5f,
        defendWeight = 1.2f,
        debuffWeight = 0.6f,
        finisherWeight = 0.0f,

        hpBias = 2.0f,
        threatBias = 0.3f
    };

}