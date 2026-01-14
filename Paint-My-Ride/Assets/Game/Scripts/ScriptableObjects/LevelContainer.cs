using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LevelContainer))]
public class LevelContainer : ScriptableObject
{
    public List<LevelConfig> levelConfigs = new List<LevelConfig>();
}
