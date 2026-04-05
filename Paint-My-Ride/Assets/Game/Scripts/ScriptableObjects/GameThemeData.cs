using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu]
public class GameThemeData : EssentialConfigScriptableObject
{
    public List<GameTheme> gameThemeList = new List<GameTheme>();

    public GameTheme GetGameTheme(int id)
    {
        return gameThemeList[id];
    }

    public override void Init()
    {
        
    }

    [Serializable]
    public class GameTheme
    {
        public Sprite header;
        public Sprite coinHolder;
        public Sprite targetGridBg;
        public Sprite pause;
        public Sprite undo;
        public Sprite gridBg;
        public Sprite gameBg;
        public Sprite gameBgOverlay;

        [Space]

        public TMP_FontAsset fontAsset;
    }
}