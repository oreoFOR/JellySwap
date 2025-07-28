namespace VoodooPackages.Tool.VST
{
    public interface IEditor
    {
        void OnEnable();

        void OnDisable();

        void Controls();

        void OnGUI();
    }
}