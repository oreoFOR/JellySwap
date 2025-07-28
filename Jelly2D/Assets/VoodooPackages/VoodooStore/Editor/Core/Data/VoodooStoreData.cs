using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{
    [System.Serializable]
    public class VoodooStoreData
    {
        public string               signInToken;
    
        public string               masterSha;
        public List<Package>        packages  = new List<Package>();

        public PackagePreset        cart      = PackagePreset.cart;
        public PackagePreset        favorites = PackagePreset.favorites;
    
        public List<PackagePreset>  presets   = new List<PackagePreset>();
    }
}