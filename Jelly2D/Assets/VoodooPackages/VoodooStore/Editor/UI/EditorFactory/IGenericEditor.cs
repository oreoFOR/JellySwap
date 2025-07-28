namespace VoodooPackages.Tool.VST
{
    public interface IGenericEditor<T> : IEditorTarget
    {
        void OnGUI(T target);
    }
}