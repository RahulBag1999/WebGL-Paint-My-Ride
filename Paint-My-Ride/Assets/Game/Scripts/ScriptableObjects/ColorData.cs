using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ColorData))]
public class ColorData : EssentialConfigScriptableObject
{
    public List<ColorDatum> colorDatumList = new List<ColorDatum>();

    public ColorDatum GetColorDatum(ColorCode colorCode)
    {
        return colorDatumList.Find(x => x.colorCode == colorCode);
    }

    public override void Init()
    {
        
    }

    [Serializable]
    public class ColorDatum
    {
        public ColorCode colorCode;
        public Color color;
        public Sprite coloredTile;
    }
}