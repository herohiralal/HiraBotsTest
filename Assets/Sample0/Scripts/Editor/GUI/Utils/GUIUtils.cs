using UnityEngine;

namespace AIEngineTest.Editor
{
    public static class GUIUtils
    {
        private static readonly GUIContent s_Content = new GUIContent();

        public static GUIContent TempContent(string text = null, string tooltip = null, Texture image = null)
        {
            s_Content.text = text;
            s_Content.tooltip = tooltip;
            s_Content.image = image;
            return s_Content;
        }
    }
}