using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public static class ContentHelper
    {
        
        public static readonly Color VSTGreen   = new Color(0.36f, 0.76f, 0.45f);
        public static readonly Color VSTRed     = new Color(0.74f, 0.06f, 0.17f);
        public static readonly Color VSTBlue    = new Color(0.32f, 0.70f, 0.76f);
        public static readonly Color VSTOrange  = new Color(0.86f, 0.45f, 0.16f);
        public static readonly Color VSTYellow  = new Color(1f, 1f, 0f);

        public static readonly GUIStyle StyleBanner = new GUIStyle
        {
            margin    = new RectOffset(0, 0, 10, 10),
            alignment = TextAnchor.MiddleCenter
        };

        public static readonly GUIStyle StyleTitle = new GUIStyle
        { 
            normal   = { textColor = Color.white },
            fontSize = 50,
            alignment = TextAnchor.MiddleLeft,
            wordWrap = false
        };

        public static readonly GUIStyle WrapStyleTitle = new GUIStyle
        { 
            normal   = { textColor = Color.white },
            fontSize = 50,
            alignment = TextAnchor.UpperLeft,
            wordWrap = true
        };

        public static readonly GUIStyle StyleNormal = new GUIStyle
        {
            normal   = { textColor = Color.white },
            fontSize = 20,
            wordWrap = true
        };

        public static readonly GUIStyle StyleSubTitle = new GUIStyle
        { 
            normal    = { textColor = Color.white },
            fontStyle = FontStyle.Italic,
            fontSize  = 15,
            wordWrap  = true
        };
        
        public static readonly GUIStyle StyleHeader = new GUIStyle
        {
            normal    = {textColor = Color.white},
            fontSize  = 20,
            alignment = TextAnchor.MiddleCenter
        };
        
        public static string GetEllipsisString(string text, GUIStyle style, Rect rect)
        {
            string ellipsisStr = "...";
            float width = style.CalcSize(new GUIContent(text)).x;

            if (width <= rect.width || rect.width < 3)
            {
                return text;
            }
            
            for (int i = text.Length; i >= 0; i--)
            {
                if (style.CalcSize(new GUIContent(text + ellipsisStr)).x <= rect.width)
                {
                    return text + ellipsisStr;
                }

                text = text.Remove(i - 1, 1);
            }

            return text;
        }

        private static readonly string   TexturePath           = "Assets/VoodooPackages/VoodooStore/Textures/";
        public static readonly Texture2D UIBanner              = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Banner.png", typeof(Texture2D));
        public static readonly Texture2D UIplus                = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Plus.png", typeof(Texture2D));
        public static readonly Texture2D UIreturn              = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Return.png", typeof(Texture2D));
        public static readonly Texture2D UIdragDrop            = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "DragAndDrop.png", typeof(Texture2D));
        public static readonly Texture2D UIfolder              = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Folder.png", typeof(Texture2D));
        public static readonly Texture2D UIdownload            = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Download.png", typeof(Texture2D));
        public static readonly Texture2D UIvalidate            = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Validate.png", typeof(Texture2D));
        public static readonly Texture2D UIcross               = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Cross.png", typeof(Texture2D));
        public static readonly Texture2D UIrefresh             = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Refresh.png", typeof(Texture2D));
        public static readonly Texture2D UIDependency          = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Dependency.png", typeof(Texture2D));
        public static readonly Texture2D UIAlphabetical        = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "AlphabeticalOrder.png", typeof(Texture2D));
        public static readonly Texture2D UIAlphabeticalReverse = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "AlphabeticalOrderReverse.png", typeof(Texture2D));
        public static readonly Texture2D UIFold                = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Fold.png", typeof(Texture2D));
        public static readonly Texture2D UIUnfold              = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Unfold.png", typeof(Texture2D));
        public static readonly Texture2D UIQuestionMark        = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "QuestionMark.png", typeof(Texture2D));
        public static readonly Texture2D UIBug                 = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "bug.png", typeof(Texture2D));
        public static readonly Texture2D UISlack               = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "slack.png", typeof(Texture2D));
        public static readonly Texture2D UIMultiSelection      = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "layer.png", typeof(Texture2D));
        public static readonly Texture2D UISave                = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "save.png", typeof(Texture2D));
        public static readonly Texture2D UIFavorite            = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Resources/star.png", typeof(Texture2D));
        public static readonly Texture2D UIFavoriteEmpty       = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "starOutline.png", typeof(Texture2D));
        public static readonly Texture2D UIAddToCart           = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "addToCart.png", typeof(Texture2D));
        public static readonly Texture2D UICart                = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "Resources/cart.png", typeof(Texture2D));
        public static readonly Texture2D UIManual              = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "hammer.png", typeof(Texture2D));
        public static readonly Texture2D UITrash               = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "trash.png", typeof(Texture2D));
        public static readonly Texture2D UIStatusAscending     = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "switch01.png", typeof(Texture2D));
        public static readonly Texture2D UIStatusDescending    = (Texture2D) AssetDatabase.LoadAssetAtPath(TexturePath + "switch02.png", typeof(Texture2D));
    }
}