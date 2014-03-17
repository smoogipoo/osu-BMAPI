using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Globalization;
using smgiFuncs;


namespace BMAPI
{
    public class Beatmap : BeatmapInfo
    {
        private readonly BeatmapInfo Info = new BeatmapInfo();
        private readonly Dictionary<string, string> BM_Sections = new Dictionary<string, string>();
        private readonly List<string> WriteBuffer = new List<string>();
        private readonly Dictionary<string, int> SectionLength = new Dictionary<string, int>();

        public Beatmap(string beatmapFile = "")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            //Variable init
            BM_Sections.Add("AudioFilename,AudioLeadIn,PreviewTime,Countdown,SampleSet,StackLeniency,Mode,LetterboxInBreaks,SpecialStyle,CountdownOffset,OverlayPosition,SkinPreference,WidescreenStoryboard,UseSkinSprites,StoryFireInFront,EpilepsyWarning,CustomSamples,EditorDistanceSpacing,AudioHash,AlwaysShowPlayfield", "General");
            BM_Sections.Add("GridSize,BeatDivisor,DistanceSpacing,CurrentTime,TimelineZoom", "Editor");
            BM_Sections.Add("Title,TitleUnicode,Artist,ArtistUnicode,Creator,Version,Source,BeatmapID,BeatmapSetID", "Metadata");
            BM_Sections.Add("HPDrainRate,CircleSize,OverallDifficulty,ApproachRate,SliderMultiplier,SliderTickRate", "Difficulty");

            if (beatmapFile != "")
            {
                if (File.Exists(beatmapFile))
                {
                    Parse(beatmapFile);
                }
            }
        }

        private void Parse(string bm)
        {
            Info.Filename = bm;
            using (StreamReader sR = new StreamReader(bm))
            {
                string currentSection = "";

                while (sR.Peek() != -1)
                {
                    sString line = sR.ReadLine();

                    //Check for blank lines
                    if ((line.ToString() == "") || (line.ToString().Length < 2))
                        continue;

                    //Check for section tag
                    if (line.SubString(0, 1) == "[")
                    {
                        currentSection = line.ToString();
                        continue;
                    }

                    //Check for commented-out line
                    if (line.SubString(0, 2) == "//")
                        continue;

                    //Check for version string
                    if (line.ToString().Length > 17)
                    {
                        if (line.SubString(0, 17) == "osu file format v")
                        {
                            Info.Format = Convert.ToInt32(line.SubString(17).Replace(Environment.NewLine, "").Replace(" ", ""));
                        }
                    }

                    //Do work for [General], [Metadata], [Difficulty] and [Editor] sections
                    if ((currentSection == "[General]") || (currentSection == "[Metadata]") || (currentSection == "[Difficulty]") || (currentSection == "[Editor]"))
                    {
                        string cProperty = line.SubString(0, line.nthDexOf(":", 0));
                        string cValue;

                        //Check for blank value
                        if (line.ToString().Length == line.nthDexOf(":", 0) + 1)
                            cValue = "";
                        else
                        {
                            //Check if there is a space between : and the data
                            cValue = line.SubString(line.nthDexOf(":", 0) + 1, line.nthDexOf(":", 0) + 2) == " " ? line.SubString(line.nthDexOf(":", 0) + 2) : line.SubString(line.nthDexOf(":", 0) + 1);
                        }

                        //Import properties into Info
                        switch (cProperty)
                        {
                            case "EditorBookmarks":
                            {
                                string[] marks = cValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string m in marks.Where(m => m != ""))
                                {
                                    Info.EditorBookmarks.Add(Convert.ToInt32(m));
                                }
                            }
                                break;
                            case "Bookmarks":
                            {
                                string[] marks = cValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string m in marks.Where(m => m != ""))
                                {
                                    Info.Bookmarks.Add(Convert.ToInt32(m));
                                }
                            }
                                break;
                            case "Tags":
                            {
                                string[] tags = cValue.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string t in tags)
                                    Info.Tags.Add(t);
                            }
                                break;
                            case "Mode":
                                switch (cValue)
                                {
                                    case "0":
                                        Info.Mode = GameMode.osu;
                                        break;
                                    case "1":
                                        Info.Mode = GameMode.Taiko;
                                        break;
                                    case "2":
                                        Info.Mode = GameMode.CatchtheBeat;
                                        break;
                                    case "3":
                                        Info.Mode = GameMode.Mania;
                                        break;
                                }
                                break;
                            case "OverlayPosition":
                                switch (cValue)
                                {
                                    case "Above":
                                        Info.OverlayPosition = OverlayOptions.Above;
                                        break;
                                    case "Below":
                                        Info.OverlayPosition = OverlayOptions.Below;
                                        break;
                                }
                                break;
                            case "AlwaysShowPlayfield":
                                Info.AlwaysShowPlayfield = Convert.ToBoolean(Convert.ToInt32(cValue));
                                break;
                            default:
                            {
                                FieldInfo fi = Info.GetType().GetField(cProperty);
                                if ((fi.FieldType == typeof(double?)) || (fi.FieldType == typeof(double)))
                                    fi.SetValue(Info, Convert.ToDouble(cValue));
                                else if ((fi.FieldType == typeof(int?)) || (fi.FieldType == typeof(int)))
                                    fi.SetValue(Info, Convert.ToInt32(cValue));
                                else if (fi.FieldType == typeof(string))
                                    fi.SetValue(Info, cValue);
                                break;
                            }
                        }
                        continue;
                    }

                    //The following are version-dependent, the version is stored as a numeric value inside Info.Format

                    //Do work for [Events] section
                    if (currentSection == "[Events]")
                    {
                        switch (Info.Format)
                        {
                            case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                                sString eventType = line.SubString(0, line.nthDexOf(",", 0));
                                if (eventType.ToString() == "0")
                                {
                                    BackgroundInfo tempEvent = new BackgroundInfo();
                                    tempEvent.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempEvent.filename = line.CountOf(",") > 2 ? line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)).Replace("\"", "") : tempEvent.filename = line.SubString(line.nthDexOf(",", 1) + 1).Replace("\"", "");
                                    Info.Events.Add(tempEvent);
                                }
                                else if ((eventType.ToString() == "1") || (eventType.ToString().ToLower() == "video"))
                                {
                                    VideoInfo tempEvent = new VideoInfo();
                                    tempEvent.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempEvent.filename = line.CountOf(",") > 2 ? line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)).Replace("\"", "") : tempEvent.filename = line.SubString(line.nthDexOf(",", 1) + 1).Replace("\"", "");
                                    Info.Events.Add(tempEvent);
                                }
                                else if (eventType.ToString() == "2")
                                {
                                    BreakInfo tempEvent = new BreakInfo();
                                    tempEvent.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempEvent.endTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1));
                                    Info.Events.Add(tempEvent);
                                }
                                else if (eventType.ToString() == "3")
                                {
                                    ColourInfo tempEvent = new ColourInfo();
                                    tempEvent.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempEvent.r = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    tempEvent.g = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                    tempEvent.b = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1));
                                    Info.Events.Add(tempEvent);
                                }
                                break;
                        }

                    }

                    //Do work for [TimingPoints] section
                    if (currentSection == "[TimingPoints]")
                    {
                        TimingPointInfo tempTimingPoint = new TimingPointInfo();
                        switch (Info.Format)
                        {
                            case 3:
                                tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1));
                                Info.TimingPoints.Add(tempTimingPoint);
                                break;
                            case 4:
                                string[] splitString = line.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                switch (splitString.Length)
                                {
                                    case 4:
                                        tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                        tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                        tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                        tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                        tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1));
                                        Info.TimingPoints.Add(tempTimingPoint);
                                        break;
                                    case 5:
                                        tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                        tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                        tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                        tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                        tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)));
                                        Info.TimingPoints.Add(tempTimingPoint);
                                        break;
                                }

                                break;
                            case 5:
                                splitString = line.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                switch (splitString.Length)
                                {
                                    case 6:
                                        tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                        tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                        tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                        tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                        tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)));
                                        tempTimingPoint.volumePercentage = Convert.ToInt32(line.SubString(line.nthDexOf(",", 4) + 1));
                                        Info.TimingPoints.Add(tempTimingPoint);
                                        break;
                                    case 7:
                                        tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                        tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                        tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                        tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                        tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)));
                                        tempTimingPoint.volumePercentage = Convert.ToInt32(line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)));
                                        tempTimingPoint.inheritsBPM = !Convert.ToBoolean(Convert.ToInt32(line.SubString(line.nthDexOf(",", 5) + 1)));
                                        Info.TimingPoints.Add(tempTimingPoint);
                                        break;
                                    case 8:
                                        tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                        tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                        tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                        tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                        tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)));
                                        tempTimingPoint.volumePercentage = Convert.ToInt32(line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)));
                                        tempTimingPoint.inheritsBPM = !Convert.ToBoolean(Convert.ToInt32(line.SubString(line.nthDexOf(",", 5) + 1, line.nthDexOf(",", 6))));
                                        switch (line.SubString(line.nthDexOf(",", 6) + 1))
                                        {
                                            case "1":
                                                tempTimingPoint.kiaiTime = true;
                                                break;
                                            case "8":
                                                tempTimingPoint.omitFirstBarLine = true;
                                                break;
                                            case "9":
                                                tempTimingPoint.kiaiTime = true;
                                                tempTimingPoint.omitFirstBarLine = true;
                                                break;
                                        }
                                        Info.TimingPoints.Add(tempTimingPoint);
                                        break;
                                }
                                break;
                            case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                                tempTimingPoint.time = Convert.ToDouble(line.SubString(0, line.nthDexOf(",", 0)));
                                tempTimingPoint.bpmDelay = Convert.ToDouble(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                tempTimingPoint.timeSignature = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                tempTimingPoint.sampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                tempTimingPoint.customSampleSet = Convert.ToInt32(line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)));
                                tempTimingPoint.volumePercentage = Convert.ToInt32(line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)));
                                tempTimingPoint.inheritsBPM = !Convert.ToBoolean(Convert.ToInt32(line.SubString(line.nthDexOf(",", 5) + 1, line.nthDexOf(",", 6))));
                                if (line.SubString(line.nthDexOf(",", 6) + 1) == "1")
                                    tempTimingPoint.kiaiTime = true;
                                else if (line.SubString(line.nthDexOf(",", 6) + 1) == "8")
                                    tempTimingPoint.omitFirstBarLine = true;
                                else if (line.SubString(line.nthDexOf(",", 6) + 1) == "9")
                                {
                                    tempTimingPoint.kiaiTime = true;
                                    tempTimingPoint.omitFirstBarLine = true;
                                }
                                Info.TimingPoints.Add(tempTimingPoint);
                                break;
                        }
                    }

                    //Do work for [Colours] section
                    if (currentSection == "[Colours]")
                    {
                        switch (Info.Format)
                        {
                            case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                                if (line.SubString(0, line.nthDexOf(":", 0)).Replace(" ", "") == "SliderBorder")
                                {
                                    sString value = line.SubString(line.nthDexOf(":", 0) + 1).Replace(" ", "");
                                    Info.SliderBorder = new ColourInfo();
                                    Info.SliderBorder.r = Convert.ToInt32(value.SubString(0, value.nthDexOf(",", 0)));
                                    Info.SliderBorder.g = Convert.ToInt32(value.SubString(value.nthDexOf(",", 0) + 1, value.nthDexOf(",", 1)));
                                    Info.SliderBorder.b = Convert.ToInt32(value.SubString(value.nthDexOf(",", 1) + 1));
                                }
                                else if (line.SubString(0, 5) == "Combo")
                                {
                                    sString value = line.SubString(line.nthDexOf(":", 0) + 1).Replace(" ", "");
                                    ComboInfo tempCombo = new ComboInfo();
                                    tempCombo.comboNumber = Convert.ToInt32(line.SubString(5, 6));
                                    tempCombo.colour.r = Convert.ToInt32(value.SubString(0, value.nthDexOf(",", 0)));
                                    tempCombo.colour.g = Convert.ToInt32(value.SubString(value.nthDexOf(",", 0) + 1, value.nthDexOf(",", 1)));
                                    tempCombo.colour.b = Convert.ToInt32(value.SubString(value.nthDexOf(",", 1) + 1));
                                    Info.ComboColours.Add(tempCombo);
                                }
                                break;
                        }
                    }

                    //Do work for [HitObjects] section
                    if (currentSection == "[HitObjects]")
                    {
                        if (line.SubString(line.ToString().Length - 1) == ",")
                            line = line.SubString(0, line.ToString().Length - 1);
                        string[] splitString = line.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        switch (Info.Format)
                        {
                            case 3: case 4:
                                if (splitString.Length == 5)
                                {
                                    //Circle
                                    CircleInfo tempCircle = new CircleInfo();
                                    tempCircle.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempCircle.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempCircle.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    int tempNewCombo = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                    if (tempNewCombo == 1)
                                    {
                                        tempCircle.newCombo = false;
                                    }
                                    else
                                    {
                                        tempCircle.newCombo = (tempNewCombo + 1) % 2 == 0;
                                    }

                                    switch (line.SubString(line.nthDexOf(",", 3) + 1))
                                    {
                                        case "0":
                                            tempCircle.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempCircle.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempCircle.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempCircle.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempCircle.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempCircle.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempCircle.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempCircle.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    Info.HitObjects.Add(tempCircle);
                                }
                                else if ((splitString[5].Substring(0, 1) == "B") || (splitString[5].Substring(0, 1) == "C") || (splitString[5].Substring(0, 1) == "L") || (splitString[5].Substring(0, 1) == "P"))
                                {
                                    //Slider
                                    SliderInfo tempSlider = new SliderInfo();
                                    tempSlider.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempSlider.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempSlider.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    tempSlider.newCombo = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3))) % 6 == 0;
                                    switch (line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)))
                                    {
                                        case "0":
                                            tempSlider.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempSlider.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempSlider.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempSlider.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempSlider.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempSlider.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempSlider.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempSlider.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    switch (splitString[5].Substring(0, 1))
                                    {
                                        case "B":
                                            tempSlider.type = SliderType.Bezier;
                                            break;
                                        case "C":
                                            tempSlider.type = SliderType.Cumulative;
                                            break;
                                        case "L":
                                            tempSlider.type = SliderType.Linear;
                                            break;
                                        case "P":
                                            tempSlider.type = SliderType.PassThrough;
                                            break;
                                    }
                                    string[] pts = line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)).Split(new[] { "|" }, StringSplitOptions.None);
                                    for (int i = 1; i <= pts.Length - 1; i++)
                                    {
                                        PointInfo p = new PointInfo();
                                        p.x = Convert.ToInt32(pts[i].Substring(0, pts[i].IndexOf(":", StringComparison.Ordinal)));
                                        p.y = Convert.ToInt32(pts[i].Substring(pts[i].IndexOf(":", StringComparison.Ordinal) + 1));
                                        tempSlider.points.Add(p);
                                    }
                                    tempSlider.repeatCount = Convert.ToInt32(line.SubString(line.nthDexOf(",", 5) + 1, line.nthDexOf(",", 6)));
                                    double tempDbl;
                                    if (double.TryParse(line.SubString(line.nthDexOf(",", 6) + 1), out tempDbl))
                                    {
                                        tempSlider.maxPoints = tempDbl;
                                    }
                                    else if (line.CountOf(",") >= 8)
                                    {
                                        if (double.TryParse(line.SubString(line.nthDexOf(",", 6) + 1, line.nthDexOf(",", 7)), out tempDbl))
                                        {
                                            tempSlider.maxPoints = tempDbl;
                                        }
                                    }
                                    Info.HitObjects.Add(tempSlider);
                                }
                                else
                                {
                                    //Spinner
                                    SpinnerInfo tempSpinner = new SpinnerInfo();
                                    tempSpinner.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempSpinner.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempSpinner.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    switch (line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)))
                                    {
                                        case "0":
                                            tempSpinner.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempSpinner.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempSpinner.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempSpinner.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempSpinner.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempSpinner.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempSpinner.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempSpinner.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    tempSpinner.endTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 4) + 1));
                                    Info.HitObjects.Add(tempSpinner);
                                }
                                break;
                            case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12: //Note: Until I found out what the last few bytes at the end of some of these versions represent, I will ignore them
                                int circleCount = 5;
                                if (splitString[splitString.Length - 1].Contains(":"))
                                    circleCount = 6;
                                if (splitString.Length == circleCount)
                                {
                                    //Circle
                                    CircleInfo tempCircle = new CircleInfo();
                                    tempCircle.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempCircle.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempCircle.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    int tempNewCombo = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3)));
                                    if (tempNewCombo == 1)
                                    {
                                        tempCircle.newCombo = false;
                                    }
                                    else
                                    {
                                        tempCircle.newCombo = (tempNewCombo + 1) % 2 == 0;
                                    }

                                    switch (line.SubString(line.nthDexOf(",", 3) + 1))
                                    {
                                        case "0":
                                            tempCircle.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempCircle.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempCircle.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempCircle.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempCircle.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempCircle.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempCircle.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempCircle.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    Info.HitObjects.Add(tempCircle);
                                }
                                else if ((splitString[5].Substring(0, 1) == "B") || (splitString[5].Substring(0, 1) == "C") || (splitString[5].Substring(0, 1) == "L") || (splitString[5].Substring(0, 1) == "P"))
                                {
                                    //Slider
                                    SliderInfo tempSlider = new SliderInfo();
                                    tempSlider.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempSlider.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempSlider.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    tempSlider.newCombo = Convert.ToInt32(line.SubString(line.nthDexOf(",", 2) + 1, line.nthDexOf(",", 3))) % 6 == 0;
                                    switch (line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)))
                                    {
                                        case "0":
                                            tempSlider.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempSlider.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempSlider.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempSlider.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempSlider.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempSlider.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempSlider.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempSlider.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    switch (splitString[5].Substring(0, 1))
                                    {
                                        case "B":
                                            tempSlider.type = SliderType.Bezier;
                                            break;
                                        case "C":
                                            tempSlider.type = SliderType.Cumulative;
                                            break;
                                        case "L":
                                            tempSlider.type = SliderType.Linear;
                                            break;
                                        case "P":
                                            tempSlider.type = SliderType.PassThrough;
                                            break;
                                    }
                                    string[] pts = line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)).Split(new[] { "|" }, StringSplitOptions.None);
                                    for (int i = 1; i <= pts.Length - 1; i++)
                                    {
                                        PointInfo p = new PointInfo();
                                        p.x = Convert.ToInt32(pts[i].Substring(0, pts[i].IndexOf(":", StringComparison.Ordinal)));
                                        p.y = Convert.ToInt32(pts[i].Substring(pts[i].IndexOf(":", StringComparison.Ordinal) + 1));
                                        tempSlider.points.Add(p);
                                    }
                                    tempSlider.repeatCount = Convert.ToInt32(line.SubString(line.nthDexOf(",", 5) + 1, line.nthDexOf(",", 6)));
                                    double tempDbl;
                                    if (double.TryParse(line.SubString(line.nthDexOf(",", 6) + 1), out tempDbl))
                                    {
                                        tempSlider.maxPoints = tempDbl;
                                    }
                                    else if (line.CountOf(",") >= 8)
                                    {
                                        if (double.TryParse(line.SubString(line.nthDexOf(",", 6) + 1, line.nthDexOf(",", 7)), out tempDbl))
                                        {
                                            tempSlider.maxPoints = tempDbl;
                                        }
                                    }
                                    Info.HitObjects.Add(tempSlider);
                                }
                                else
                                {
                                    //Spinner
                                    SpinnerInfo tempSpinner = new SpinnerInfo();
                                    tempSpinner.location.x = Convert.ToInt32(line.SubString(0, line.nthDexOf(",", 0)));
                                    tempSpinner.location.y = Convert.ToInt32(line.SubString(line.nthDexOf(",", 0) + 1, line.nthDexOf(",", 1)));
                                    tempSpinner.startTime = Convert.ToInt32(line.SubString(line.nthDexOf(",", 1) + 1, line.nthDexOf(",", 2)));
                                    switch (line.SubString(line.nthDexOf(",", 3) + 1, line.nthDexOf(",", 4)))
                                    {
                                        case "0":
                                            tempSpinner.effect = EffectType.None;
                                            break;
                                        case "2":
                                            tempSpinner.effect = EffectType.Whistle;
                                            break;
                                        case "4":
                                            tempSpinner.effect = EffectType.Finish;
                                            break;
                                        case "6":
                                            tempSpinner.effect = EffectType.WhistleFinish;
                                            break;
                                        case "8":
                                            tempSpinner.effect = EffectType.Clap;
                                            break;
                                        case "10":
                                            tempSpinner.effect = EffectType.ClapWhistle;
                                            break;
                                        case "12":
                                            tempSpinner.effect = EffectType.ClapFinish;
                                            break;
                                        case "14":
                                            tempSpinner.effect = EffectType.ClapWhistleFinish;
                                            break;
                                    }
                                    tempSpinner.endTime = Convert.ToInt32(splitString.Length == 7 ? line.SubString(line.nthDexOf(",", 4) + 1, line.nthDexOf(",", 5)) : line.SubString(line.nthDexOf(",", 4) + 1));
                                    Info.HitObjects.Add(tempSpinner);
                                }
                                break;
                        }
                    }
                }
            }
            foreach (FieldInfo fi in Info.GetType().GetFields())
            {
                FieldInfo ff = GetType().GetField(fi.Name);
                ff.SetValue(this, fi.GetValue(Info));
            }
        }

        public void Save(string filename)
        {
            CultureInfo lastCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            Save("", "osu file format v" + Info.Format);
            FieldInfo[] newFields = GetType().GetFields();
            FieldInfo[] oldFields = Info.GetType().GetFields();

            foreach (FieldInfo f1 in newFields)
            {
                foreach (FieldInfo f2 in oldFields.Where(f2 => f1.Name == f2.Name))
                {
                    switch (f1.Name)
                    {
                        case "EditorBookmarks":
                            {
                                List<int> temps = (List<int>)f1.GetValue(this);
                                if (temps.Count != 0)
                                {
                                    Save("General", "EditorBookmarks:" + string.Join(",", temps.Select(t => t.ToString(CultureInfo.InvariantCulture)).ToArray()));
                                }
                            }
                            break;
                        case "Bookmarks":
                            {
                                List<int> temps = (List<int>)f1.GetValue(this);
                                if (temps.Count != 0)
                                {
                                    Save("Editor", "Bookmarks:" + string.Join(",", temps.Select(t => t.ToString(CultureInfo.InvariantCulture)).ToArray()));
                                }
                            }
                            break;
                        case "Tags":
                            {
                                List<string> temps = (List<string>)f1.GetValue(this);
                                if (temps.Count != 0)
                                {
                                    Save("Metadata", "Tags:" + string.Join(" ", temps.ToArray()));
                                }
                            }
                            break;
                        case "Events":
                            if ((Info.Format >= 3) && (Info.Format <= 12))
                            {
                                foreach (dynamic o in (IEnumerable<dynamic>)f1.GetValue(this))
                                {
                                    if (o.GetType() == typeof(BackgroundInfo))
                                    {
                                        Save("Events", "0," + o.startTime + ",\"" + o.filename + "\"");
                                    }
                                    else if (o.GetType() == typeof(VideoInfo))
                                    {
                                        Save("Events", "1," + o.startTime + ",\"" + o.filename + "\"");
                                    }
                                    else if (o.GetType() == typeof(BreakInfo))
                                    {
                                        Save("Events", "2," + o.startTime + "," + o.endTime);
                                    }
                                    else if (o.GetType() == typeof(ColourInfo))
                                    {
                                        Save("Events", "3," + o.startTime + "," + o.r + "," + o.g + "," + o.b);
                                    }
                                }
                            }
                            break;
                        case "TimingPoints":
                            switch (Info.Format)
                            {
                                case 3:
                                    foreach (TimingPointInfo o in (IEnumerable<TimingPointInfo>)f1.GetValue(this))
                                        Save("TimingPoints", o.time + "," + o.bpmDelay);
                                    break;
                                case 4:
                                    foreach (TimingPointInfo o in (IEnumerable<TimingPointInfo>)f1.GetValue(this))
                                        Save("TimingPoints", o.time + "," + o.bpmDelay + "," + o.timeSignature + "," + o.sampleSet + "," + o.customSampleSet);
                                    break;
                                case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                                    foreach (TimingPointInfo o in (IEnumerable<TimingPointInfo>)f1.GetValue(this))
                                    {
                                        int options = 0;
                                        if (o.kiaiTime)
                                            options += 1;
                                        if (o.omitFirstBarLine)
                                            options += 8;
                                        Save("TimingPoints", o.time + "," + o.bpmDelay + "," + o.timeSignature + "," + o.sampleSet + "," + o.customSampleSet + "," + o.volumePercentage + "," + Convert.ToInt32(!o.inheritsBPM) + "," + options);
                                    }
                                    break;
                            }
                            break;
                        case "ComboColours":
                            if ((Info.Format >= 5) && (Info.Format <= 12))
                            {
                                foreach (ComboInfo o in (IEnumerable<ComboInfo>)f1.GetValue(this))
                                    Save("Colours", "Combo" + o.comboNumber + ":" + o.colour.r + "," + o.colour.g + "," + o.colour.b);
                            }
                            break;
                        case "SliderBorder":
                            if ((Info.Format >= 5) && (Info.Format <= 12))
                            {
                                ColourInfo o = (ColourInfo)f1.GetValue(this);
                                if (o.startTime != null)
                                    Save("Colours", "SliderBorder: " + o.r + "," + o.g + "," + o.b);
                            }
                            break;
                        case "HitObjects":
                            switch (Info.Format)
                            {
                                case 3: case 4:
                                    foreach (dynamic obj in (IEnumerable<dynamic>)f1.GetValue(this))
                                    {
                                        int combo = 5;
                                        if (obj.newCombo == false)
                                            combo = 1;
                                        if (obj.GetType() == typeof(CircleInfo))
                                        {
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + "," + combo + "," + (int)obj.effect + ",");
                                        }
                                        else if (obj.GetType() == typeof(SliderInfo))
                                        {
                                            string pointString = "";
                                            foreach (PointInfo p in obj.points)
                                                pointString += "|" + p.x + ":" + p.y;
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + "," + (combo + 1) + "," + (int)obj.effect + "," + obj.type.ToString().Substring(0, 1) + pointString + "," + obj.repeatCount + "," + obj.maxPoints + ",");
                                        }
                                        else if (obj.GetType() == typeof(SpinnerInfo))
                                        {
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + ",12," + (int)obj.effect + "," + obj.endTime + ",");
                                        }
                                    }
                                    break;
                                case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
                                    foreach (dynamic obj in (IEnumerable<dynamic>)f1.GetValue(this))
                                    {
                                        int combo = 5;
                                        if (obj.newCombo == false)
                                            combo = 1;
                                        if (obj.GetType() == typeof(CircleInfo))
                                        {
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + "," + combo + "," + (int)obj.effect);
                                        }
                                        else if (obj.GetType() == typeof(SliderInfo))
                                        {
                                            string pointString = "";
                                            foreach (PointInfo p in obj.points)
                                                pointString += "|" + p.x + ":" + p.y;
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + "," + (combo + 1) + "," + (int)obj.effect + "," + obj.type.ToString().Substring(0, 1) + pointString + "," + obj.repeatCount + "," + obj.maxPoints);
                                        }
                                        else if (obj.GetType() == typeof(SpinnerInfo))
                                        {
                                            Save("HitObjects", obj.location.x + "," + obj.location.y + "," + obj.startTime + ",12," + (int)obj.effect + "," + obj.endTime);
                                        }
                                    }
                                    break;
                            }
                            break;
                        default:
                            if (f1.Name != "Format" && f1.Name != "Filename")
                            {
                                if (f1.GetValue(this) != null)
                                {
                                    if (f2.GetValue(Info) != null)
                                    {
                                        if ((f1.GetValue(this).GetType() == typeof(GameMode)) || (f1.GetValue(this).GetType() == typeof(OverlayOptions)))
                                            Save(GetSection(f1.Name), f1.Name + ":" + (int)f1.GetValue(this));
                                        else
                                            Save(GetSection(f1.Name), f1.Name + ":" + f1.GetValue(this));
                                    }
                                    else
                                    {
                                        if ((f2.GetValue(Info).GetType() == typeof(GameMode)) || (f2.GetValue(Info).GetType() == typeof(OverlayOptions)))
                                            Save(GetSection(f2.Name), f2.Name + ":" + (int)f2.GetValue(Info));
                                        else
                                            Save(GetSection(f2.Name), f2.Name + ":" + f2.GetValue(Info));
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            FinishSave(filename);
            Thread.CurrentThread.CurrentCulture = lastCulture;
        }

        private void Save(string section, string contents)
        {
            if (section == "")
                WriteBuffer.Add(contents);
            else if (WriteBuffer.Contains("[" + section + "]") == false)
            {
                WriteBuffer.Add("");
                WriteBuffer.Add("[" + section + "]");
                WriteBuffer.Add(contents);
                SectionLength.Add(section, 1);
            }
            else
            {
                if (WriteBuffer.IndexOf("[" + section + "]") + SectionLength[section] == WriteBuffer.Count)
                {
                    WriteBuffer.Add(contents);
                    SectionLength[section] += 1;
                }
                else
                {
                    WriteBuffer.Insert(WriteBuffer.IndexOf("[" + section + "]") + SectionLength[section] + 1, contents);
                    SectionLength[section] += 1;
                }
            }
        }

        private void FinishSave(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (string l in WriteBuffer)
                {
                    sw.WriteLine(l);
                }
            }
        }

        private string GetSection(string name)
        {
            foreach (string k in BM_Sections.Keys.Where(k => k.Contains(name)))
            {
                return BM_Sections[k];
            }
            return "";
        }
    }
}
