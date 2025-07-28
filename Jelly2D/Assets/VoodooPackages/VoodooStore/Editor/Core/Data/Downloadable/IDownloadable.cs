using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{ 
    public interface IDownloadable 
    {
        string          Name            { get; }
        int             VersionStatus   { get; }
        List<Package>   Content         { get; }

        void FillDependencies(ref PackageComposition preset);
        void FillUses(ref PackageComposition preset);
    }

    public static class VersionState 
    {
        public const int NotPresent = 0;
        public const int OutDated   = 1;
        public const int UpToDate   = 2;
        public const int Manually   = 4;
        public const int Invalid    = 8;
    }
}