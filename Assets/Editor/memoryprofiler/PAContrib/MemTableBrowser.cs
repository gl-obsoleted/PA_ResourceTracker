using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MemTableBrowser
{
    TableView _typeTable;
    TableView _objectTable;

    public MemTableBrowser(EditorWindow hostWindow)
    {
        // create the table with a specified object type
        _typeTable = new TableView(hostWindow, typeof(FooItem));

        // setup the description for content
        _typeTable.AddColumn("Name", "Name", 0.5f, TextAnchor.MiddleLeft);
        _typeTable.AddColumn("Count_A", "Count_A", 0.1f);
        _typeTable.AddColumn("Time_A", "Time_A", 0.15f, TextAnchor.MiddleCenter, "0.000");
        _typeTable.AddColumn("Count_B", "Count_B", 0.1f);
        _typeTable.AddColumn("Time_B", "Time_B", 0.15f, TextAnchor.MiddleCenter, "0.0");

        // add test data
        List<object> entries = new List<object>();
        for (int i = 0; i < 100; i++)
            entries.Add(FooItem.MakeRandom());
        _typeTable.RefreshData(entries);

        // register the event-handling function
        _typeTable.OnSelected += TableView_Selected;
    }

    public GUIStyle background = "AnimationCurveEditorBackground";

    public void Draw(Rect r)
    {
        GUILayout.BeginArea(r, background);
        if (_typeTable != null)
            _typeTable.Draw(new Rect(10, 10, r.width - 20, r.height - 20));
        GUILayout.EndArea();
    }

    void TableView_Selected(object selected, int col)
    {
        Debug.Log("test");
    }

    void OnDestroy()
    {
        if (_typeTable != null)
            _typeTable.Dispose();

        _typeTable = null;
    }
}
