using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance;

    public PlayerState player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

            return;
        }
    }

    // =================================================
    // EXP REQUIRED
    // =================================================

    public int GetRequiredEXP()
    {
        return player.level * 100;
    }

    // =================================================
    // ADD EXP
    // =================================================

    public void AddEXP(int amount)
    {
        player.currentExp += amount;

        Debug.Log(
            $"Gain EXP: {amount}"
        );

        while (
            player.currentExp >= GetRequiredEXP()
        )
        {
            player.currentExp -= GetRequiredEXP();

            LevelUp();
        }
    }

    // =================================================
    // LEVEL UP
    // =================================================

    void LevelUp()
    {
        player.level++;

        UnlockManager.Instance.CheckUnlocks();

        Debug.Log(
            $"LEVEL UP -> {player.level}"
        );

        player.currentHP += 50;
    }

    // =================================================
    // GOLD
    // =================================================

    public void AddGold(int amount)
    {
        player.gold += amount;

        Debug.Log(
            $"Gain Gold: {amount}"
        );
    }
}