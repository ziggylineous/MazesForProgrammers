using UnityEngine;
using UnityEditor;
using System.IO;

public static class Texture2DEditorExtensions
{
    public static void Save(this Texture2D self, string filename)
    {
        byte[] pngData = self.EncodeToPNG();

        if (pngData == null)
        {
            Debug.Log("no image data");
        }
        else
        {
            File.WriteAllBytes(string.Format("Assets/Textures/{0}.png", filename), pngData);
        }

        AssetDatabase.Refresh();
    }
}