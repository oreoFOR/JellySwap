namespace VoodooPackages.Tool.VST
{
	[System.Serializable]
	public class DependencyPackage
	{
		public string packageName;
		public bool isSelected;

		public DependencyPackage()
		{
			packageName = "";
			isSelected = false;
		}

		public DependencyPackage(DependencyPackage source)
		{
			packageName = source.packageName;
			isSelected = source.isSelected;
		}
	}
}