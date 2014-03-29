using System.Collections.Generic;

namespace BMAPI
{
    public class BeatmapInfo
    {
        //Info
        public int? Format = null;
        public string Filename;

        //General
        public string AudioFilename;
        public int? AudioLeadIn = null;
        public int? PreviewTime = null;
        public int? Countdown = null;
        public string SampleSet;
        public double? StackLeniency = null;
        public GameMode? Mode = null;
        public int? LetterboxInBreaks = null;
        public int? SpecialStyle = null;
        public int? CountdownOffset = null;
        public OverlayOptions? OverlayPosition = null;
        public string SkinPreference;
        public int? WidescreenStoryboard = null;
        public int? UseSkinSprites = null;
        public int? StoryFireInFront = null;
        public int? EpilepsyWarning = null;
        public int? CustomSamples = null;
        public List<int> EditorBookmarks = new List<int>();
        public double? EditorDistanceSpacing = null;
        public string AudioHash;
        public bool? AlwaysShowPlayfield = null;

        //Editor (Other Editor tag stuff (v12))
        public int? GridSize = null;
        public List<int> Bookmarks = new List<int>();
        public int? BeatDivisor = null;
        public double? DistanceSpacing = null;
        public int? CurrentTime = null;
        public double? TimelineZoom = null;

        //Metadata
        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicode;
        public string Creator;
        public string Version;
        public string Source;
        public List<string> Tags = new List<string>();
        public int? BeatmapID = null;
        public int? BeatmapSetID = null;

        //Difficulty
        public int HPDrainRate = 3;
        public int CircleSize = 3;
        public int OverallDifficulty = 3;
        public int ApproachRate = 4;
        public double? SliderMultiplier = null;
        public double? SliderTickRate = null;

        //Events
        public List<BaseEvent> Events = new List<BaseEvent>();

        //Timingpoints
        public List<TimingPointInfo> TimingPoints = new List<TimingPointInfo>();

        //Colours
        public List<ComboInfo> ComboColours = new List<ComboInfo>();
        public ColourInfo SliderBorder = new ColourInfo();

        //Hitobjects
        public List<BaseCircle> HitObjects = new List<BaseCircle>();
    }
}
