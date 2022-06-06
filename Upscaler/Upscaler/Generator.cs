using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneration
{
    internal struct Step
    {
        internal int Tick { get; private set; }
        internal int ID { get; private set; }
        internal int PlayerID { get; private set; }
        internal string Path { get; private set; }
        internal List<int> PreviousImageOffsets { get; private set; }
        public Step(int tick, int id,int playerID, string path, List<int> previousImageOffsets)
        {
            Tick = tick;
            ID = id;
            PlayerID = playerID;
            Path = path;
            PreviousImageOffsets = previousImageOffsets;
        }
    }
    internal class Generator
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private int _widthSmall;
        private int _heightSmall;
        private int _widthBig;
        private int _heightBig;
        private string _destinationPath;
        private List<Step> _steps;
        private string _previousPath = null;
        internal Generator(string path)
        {
            string[] lines = File.ReadAllLines(path);
            string[] widthSmall = lines[0].Split(';');
            string[] heightSmall = lines[1].Split(';');
            string[] widthBig = lines[2].Split(';');
            string[] heightBig = lines[3].Split(';');
            string[] destinationPath = lines[4].Split(';');

            if (widthSmall[0] == "widthSmall")
                _widthSmall = int.Parse(widthSmall[1]);

            if (heightSmall[0] == "heightSmall")
                _heightSmall = int.Parse(heightSmall[1]);

            if (widthBig[0] == "widthBig")
                _widthBig = int.Parse(widthBig[1]);

            if (heightBig[0] == "heightBig")
                _heightBig = int.Parse(heightBig[1]);

            if (destinationPath[0] == "destinationPath")
                _destinationPath = BuildPath(destinationPath[1]);

            if (Directory.Exists(_destinationPath))
                throw new Exception("Desitination already exists.");

            string currentReplayPath = null;
            this._steps = new List<Step>();

            int id = 0;
            for (int i = 5; i < lines.Length; i++)
            {
                string line = lines[i];
                if (!string.IsNullOrEmpty(lines[i]))
                {
                    string[] temp = line.Split(';');
                    if (temp[0] == "replayPath")
                    {
                        currentReplayPath = BuildPath(temp[1]);
                        if (!File.Exists(currentReplayPath))
                            throw new Exception("File: "+ currentReplayPath+" does not exicst.");
                    }
                    else if (temp[0] == "step")
                    {
                        int tick = int.Parse(temp[1]);
                        int playerID = int.Parse(temp[2]);
                        List<int> offset = new List<int>();
                        for (int n = 3; n < temp.Length; n++)
                        {
                            offset.Add(int.Parse(temp[n]));
                        }
                        if (string.IsNullOrEmpty(currentReplayPath))
                            throw new Exception("No replay path is set.");

                        _steps.Add(new Step(tick,id, playerID, currentReplayPath,offset));
                        id++;
                    }

                }
            }
        }
        private string BuildPath(string path)
        { 
            string current = Directory.GetCurrentDirectory();
            if (path[0] == '.')
                return Path.Combine(current,path);
            return path;
        }

        internal void Start()
        {
            IntPtr window = FindWindow(null, "Counter-Strike: Global Offensive - Direct3D 9");

            //TODO initial settings

            Controller.SetResolution(_widthBig, _heightBig, window);
            foreach (Step step in _steps)
                DoStep(step, true, window);

            Controller.SetResolution(_widthSmall,_heightSmall, window);
            foreach (Step step in _steps)
                DoStep(step,false, window);
        }
        private void DoStep(Step step,bool bigNotSmall, IntPtr window)
        {
            if (_previousPath != step.Path)
            {
                Controller.SetReplayFile(step.Path, window);
                Controller.Pause(window);
                _previousPath = step.Path;
            }
            List<Bitmap> frames = new List<Bitmap>();
            Controller.GotToTick(step.Tick, window);
            Controller.GoToPlayer(step.PlayerID,window);
            frames.Add(Controller.TakeScreenshot(window));
            foreach (int offset in step.PreviousImageOffsets)
            {
                Controller.GotToTick(step.Tick+ offset, window);
                frames.Add(Controller.TakeScreenshot(window));
            }
            Save(step,frames,bigNotSmall);
        }
        private void Save(Step step, List<Bitmap> frames, bool bigNotSmall)
        {
            string path = Path.Combine(_destinationPath, step.ID.ToString());
            Directory.CreateDirectory(path);
            path = Path.Combine(path,(bigNotSmall ? "big" : "small") + "tick_" + step.Tick + ".bmp");

            frames[0].Save(path);
            for (int i = 0;i<step.PreviousImageOffsets.Count;i++)
            {
                path = Path.Combine(_destinationPath, step.ID.ToString(), (bigNotSmall ? "big" : "small") + "tick_" + (step.Tick+step.PreviousImageOffsets[i]) + ".bmp");
                frames[i+1].Save(path);
            }
        }
    }
}
