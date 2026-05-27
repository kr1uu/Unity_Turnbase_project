using System;

public static class InstantEffectFactory
{
    public static InstantEffect Create(
        string effectName,
        int value
    )
    {
        InstantEffectType type =
            Enum.Parse<InstantEffectType>(
                effectName
            );

        return new InstantEffect(
            type,
            value
        );
    }
}