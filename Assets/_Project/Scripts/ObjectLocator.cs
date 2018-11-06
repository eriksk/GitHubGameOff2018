
using UnityEngine;

public static class ObjectLocator
{
    private static GameStats _stats;
    private static Skater _player;

    public static void Clear()
    {
        _stats = null;
    }

    public static GameStats Stats
    {
        get 
        {
            if(_stats != null) return _stats;

            var reference = GameObject.Find("[GameStats]");
            if(reference == null) return null;
            return _stats = reference.GetComponent<GameStats>();
        }
    }

    public static Skater Player
    {
        get 
        {
            if(_player != null) return _player;

            var reference = GameObject.Find("Player");
            if(reference == null) return null;
            return _player = reference.GetComponent<Skater>();
        }
    }
}