using UnityEngine;
using System.Collections;

enum eShowType
{
    InTable,
    InTreemap,
}

public class MemConst 
{
    public static float TopBarHeight = 25;
    public static int InspectorWidth = 400;
    public static string[] ShowTypes = new string[] { "Table View", "TreeMap View" };
    public static GUIStyle ToolbarButton = "ToolbarButton";
}
