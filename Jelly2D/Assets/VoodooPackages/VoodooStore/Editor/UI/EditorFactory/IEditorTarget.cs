using System;

namespace VoodooPackages.Tool.VST
{
    public interface IEditorTarget
    {
        System.Type targetType { get; }
    }
}