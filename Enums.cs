
namespace BMAPI
{
    public enum OverlayOptions
    {
        Above = 0,
        Below = 1,
    }
    public enum GameMode
    {
        osu = 0,
        Taiko = 1,
        CatchtheBeat = 2,
        Mania = 3,
    }
    public enum EffectType
    {
        None = 0,
        Whistle = 2,
        Finish = 4,
        WhistleFinish = 6,
        Clap = 8,
        ClapWhistle = 10,
        ClapFinish = 12,
        ClapWhistleFinish = 14,
    }
    public enum SliderType
    {
        Linear = 0,
        Cumulative = 1,
        Bezier = 2,
        PassThrough = 3,
    }
}
