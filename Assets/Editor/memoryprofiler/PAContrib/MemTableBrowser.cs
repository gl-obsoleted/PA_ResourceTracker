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
        _objectTable = new TableView(hostWindow, typeof(FooItem));

        // setup the description for content
        _typeTable.AddColumn("Name", "Name", 0.5f, TextAnchor.MiddleLeft);
        _typeTable.AddColumn("Count_A", "Count_A", 0.1f);
        _typeTable.AddColumn("Time_A", "Time_A", 0.15f, TextAnchor.MiddleCenter, "0.000");
        _typeTable.AddColumn("Count_B", "Count_B", 0.1f);
        _typeTable.AddColumn("Time_B", "Time_B", 0.15f, TextAnchor.MiddleCenter, "0.0");

        _objectTable.AddColumn("Name", "Name", 0.5f, TextAnchor.MiddleLeft);
        _objectTable.AddColumn("Time_B", "Time_B", 0.15f, TextAnchor.MiddleCenter, "0.0");

        // add test data
        List<object> entries = new List<object>();
        for (int i = 0; i < 100; i++)
            entries.Add(FooItem.MakeRandom());
        _typeTable.RefreshData(entries);
        _objectTable.RefreshData(entries);

        // register the event-handling function
        _typeTable.OnSelected += OnTypeSelected;
        _objectTable.OnSelected += OnObjectSelected;
    }

    public GUIStyle background = "AnimationCurveEditorBackground";

    public void Draw(Rect r)
    {
        GUILayout.BeginArea(r, background);
        if (_typeTable != null)
            _typeTable.Draw(new Rect(10, 10, r.width * 0.5f - 20, r.height - 20));
        if (_objectTable != null)
            _objectTable.Draw(new Rect(r.width * 0.5f + 10, 10, (int)r.width * 0.5f - 20, (int)r.height - 20));
        GUILayout.EndArea();
    }

    void OnTypeSelected(object selected, int col)
    {
        Debug.Log("type selected");
    }

    void OnObjectSelected(object selected, int col)
    {
        Debug.Log("object selected");
    }

    void OnDestroy()
    {
        if (_typeTable != null)
            _typeTable.Dispose();
        if (_objectTable != null)
            _objectTable.Dispose();

        _typeTable = null;
        _objectTable = null;
    }
}
