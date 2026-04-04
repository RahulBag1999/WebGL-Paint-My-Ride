public static class Utility 
{
    public static ColorCode GetColorCodeByGridCellType(GridCellType cellType)
    {
        switch (cellType) 
        {
            case GridCellType.REDCELL:
            case GridCellType.REDCAT:
                return ColorCode.RED;

            case GridCellType.GREENCELL:
            case GridCellType.GREENCAT:
                return ColorCode.GREEN;

            case GridCellType.BLUECELL:
            case GridCellType.BLUECAT:
                return ColorCode.BLUE;

            case GridCellType.YELLOWCELL: 
            case GridCellType.YELLOWCAT:
                return ColorCode.YELLOW;

            case GridCellType.PURPLECELL:
            case GridCellType.PURPLECAT:
                return ColorCode.PURPLE;
        }
        return ColorCode.NONE;
    }
}
