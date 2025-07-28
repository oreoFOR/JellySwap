using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{ 
    public interface IPackageComposition : IList<Package>, IDownloadable
    {
        int minVersion { get; set; }
        int maxVersion { get; set; }

        bool includeDependencies { get; set; }
    }
}