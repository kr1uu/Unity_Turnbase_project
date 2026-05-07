using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "RPG/Character Sprite Database")]
public class CharacterSpriteDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public int characterId;
        public Sprite sprite;
        public Sprite splashArt;
        //public Sprite idleSprite;
        //public Sprite attackSprite;
        //public Sprite hurtSprite;
        public RuntimeAnimatorController animator;
    }

    public List<Entry> entries;

    public Sprite GetSpriteById(int id)
    {
        foreach (var e in entries)
        {
            Debug.Log($"DB check: entryId={e.characterId}, sprite={(e.sprite != null)}");

            if (e.characterId == id)
            {
                Debug.Log("FOUND sprite for ID = " + id);
                return e.sprite;
            }
        }

        Debug.LogError("cant find sprite cho ID = " + id);
        return null;
    }
    public RuntimeAnimatorController GetAnimatorById(int id)
    {
        foreach (var e in entries)
        {
            if (e.characterId == id)
                return e.animator;
        }

        Debug.LogError("cant find ANIMATOR cho ID = " + id);
        return null;
    }
    public Sprite GetSplashArt(int id)
    {
        foreach (var e in entries)
        {
            if (e.characterId == id)
                return e.splashArt;
        }

        Debug.LogError("Missing splash art ID = " + id);
        return null;
    }
    //public Entry GetEntry(int id)
    //{
    //    foreach (var e in entries)
    //    {
    //        if (e.characterId == id)
    //            return e;
    //    }

    //    Debug.LogError("cant find entry for ID = " + id);
    //    return null;
    //}

}
