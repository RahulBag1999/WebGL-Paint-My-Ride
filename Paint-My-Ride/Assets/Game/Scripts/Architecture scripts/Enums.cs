public enum GameStates
{
    NONE,
    SPLASH,
    LOADING,
    LOGIN,
    HOME,
    GAMEPLAY,
    RESULT,
    QUIT,
    PAUSE
}

public enum PropType
{
    NONE,
    SIGNPOST,
    LAMPPOST,
    TRAFFICLIGHT
}

public enum Tag
{
    NONE,
    GROUND
}

public enum ColorCode
{
    NONE,
    RED,
    GREEN,
    BLUE,
    YELLOW,
    PURPLE
}

public enum GridCellType
{
    NONE,
    EMPTY,
    REDCELL,
    GREENCELL,
    BLUECELL,
    YELLOWCELL,
    REDCAT,
    GREENCAT,
    BLUECAT,
    YELLOWCAT,
    PURPLECELL,
    PURPLECAT,
    ORANGECELL,
    ORANGECAT
}

public enum AnimationState
{
    Idle,
    Walk
}

public enum ViewDirection
{
    Side,
    TopDown
}

public enum GameLoseType
{
    NONE,
    TIMEUP,
    WRONGPLAY
}

public enum GameEndType
{
    NONE,
    WIN,
    LOSE
}