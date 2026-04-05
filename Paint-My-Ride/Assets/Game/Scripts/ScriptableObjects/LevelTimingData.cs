using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelTimingData : EssentialConfigScriptableObject
{
    [SerializeField] private List<LevelTimeRange> levelTimeRanges = new List<LevelTimeRange>();
    public int GetTimeForLevel(int level)
    {
        foreach (var range in levelTimeRanges)
        {
            if (level >= range.minLevel && level <= range.maxLevel)
            {
                return range.timeInSeconds;
            }
        }

        Debug.LogWarning($"No timing defined for level {level}");
        return 60; // fallback
    }

    public override void Init()
    {
        
    }

    [Serializable]
    public class LevelTimeRange
    {
        public int minLevel;
        public int maxLevel;
        public int timeInSeconds;
    }
}