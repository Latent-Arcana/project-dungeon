[System.Serializable]
public class ScreenOptions
{
    public int screenWidth;
    public int screenHeight;
    public bool fullScreen;

    public ScreenOptions(bool full_screen)
    {
        // screenWidth = width;
        // screenHeight = height;
        fullScreen = full_screen;
    }
}