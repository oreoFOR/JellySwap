using System;
using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{
    [Serializable]
    /// <summary>
    /// Container struct for the List
    /// </summary>
    public class ListContainer
    {
        public List<string> dataList;

        public override string ToString()
        {
            if (dataList == null || dataList.Count == 0)
                return "[]";
            string res = "[";

            foreach (var item in dataList)
            {
                res += item + ", ";
            }
            res = res.Remove(res.Length - 2);
            res += "]";
            return res;
        }

    }
}