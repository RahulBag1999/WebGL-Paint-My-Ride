public static class Utility 
{
    public static ColorCode GetColorCodeByGridCellType(GridCellType cellType)
    {
        switch (cellType) 
        {
            case GridCellType.REDCELL:
            case GridCellType.REDCAR:
                return ColorCode.RED;

            case GridCellType.GREENCELL:
            case GridCellType.GREENCAR:
                return ColorCode.GREEN;

            case GridCellType.BLUECELL:
            case GridCellType.BLUECAR:
                return ColorCode.BLUE;

            case GridCellType.YELLOWCELL: 
            case GridCellType.YELLOWCAR:
                return ColorCode.YELLOW;

            case GridCellType.PURPLECELL:
            case GridCellType.PURPLECAR:
                return ColorCode.PURPLE;
        }
        return ColorCode.NONE;
    }
}
