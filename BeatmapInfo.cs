using System.Collections.Generic;

namespace BMAPI
{
    public class BeatmapInfo
    {
        //Info
        public int? Format = null;
        public string Filename;
        public string BeatmapHash;

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
        public double HPDrainRate = 5;
        private double p_CircleSize = 5;
        public double CircleSize
        {
            get
            {
                return p_CircleSize;
            }
            set
            {
                p_CircleSize = value;
                foreach (BaseCircle hO in HitObjects)
                {
                    hO.Radius = 40 - 4 * (value - 2);
                }
            }
        }
        public double OverallDifficulty = 5;
        public double ApproachRate = 5;
        public double SliderMultiplier = 1.4;
        public double SliderTickRate = 1;

        //Events
        public List<BaseEvent> Events = new List<BaseEvent>();

        //Timingpoints
        public List<TimingPointInfo> TimingPoints = new List<TimingPointInfo>();

        //Colours
        public List<ComboInfo> ComboColours = new List<ComboInfo>();
        public ColourInfo SliderBorder = new ColourInfo { R = 255, G = 255, B = 255 };

        //Hitobjects
        public List<BaseCircle> HitObjects = new List<BaseCircle>();
    }
}
