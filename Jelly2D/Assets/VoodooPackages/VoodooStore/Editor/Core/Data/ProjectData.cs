using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{
    [System.Serializable]
    public class ProjectData
    {
        public List<string> packagesNames   = new List<string>();
        public List<string> packagesSha     = new List<string>();
    }
}