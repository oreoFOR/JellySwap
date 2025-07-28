using UnityEngine.Networking;

namespace VoodooPackages.Tool.VST
{
    public interface IWebRequestHandler
    {
        void OnSuccess(UnityWebRequest webRequest);

        void OnError(UnityWebRequest webRequest);
    }
}