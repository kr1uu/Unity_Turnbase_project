public enum BattleResultType
{
    None,
    Win,
    Lose
}

public static class BattleResult
{
    public static BattleResultType lastResult = BattleResultType.None;
}