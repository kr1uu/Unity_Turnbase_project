using UnityEngine;

public class TeamSlotsUI : MonoBehaviour
{
    public PartySlotUI[] slots;
    public CharacterSpriteDatabase spriteDB;

    void OnEnable()
    {
        PartyManager.Instance.OnPartyChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        PartyManager.Instance.OnPartyChanged -= Refresh;
    }

    public void Refresh()
    {
        var party = PartyManager.Instance.PartyStats;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < party.Count)
            {
                slots[i].SetCharacter(party[i], spriteDB);
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }
}
