using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LevelContainer))]
public class LevelContainer : EssentialConfigScriptableObject
{
    [SerializeField] private List<LevelConfig> levelConfigs = new List<LevelConfig>();

    public LevelConfig GetLevelConfig(int index)
    {
        if(index >= 0 && index < levelConfigs.Count)
        {
            return levelConfigs[index]; 
        }
        return null;
    }

    public override void Init()
    {
        
    }
}
