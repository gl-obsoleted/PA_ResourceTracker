using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using MemoryProfilerWindow;

public class MemType
{
    public string TypeName = "Foo";
    public int Count = 0;
    public int Size = 0;

    public static MemType MakeRandom()
    {
        return new MemType()
        {
            TypeName = "Foo " + PAEditorUtil.GetRandomString(),
            Count = (int)(Random.value * 100.0f),
            Size = (int)(Random.value * 100.0f),
        };
    }
}

public class MemObject
{
    public string InstanceName = "Foo";
    public int Size = 0;
    public string Content = "content";

    public static MemObject MakeRandom()
    {
        return new MemObject()
        {
            InstanceName = "Foo " + PAEditorUtil.GetRandomString(),
            Size = (int)(Random.value * 100.0f),
            Content = "content",
        };
    }
}


public class MemTableBrowser
{
    CrawledMemorySnapshot _unpacked;

    TableView _typeTable;
    TableView _objectTable;

    public MemTableBrowser(EditorWindow hostWindow)
    {
        // create the table with a specified object type
        _typeTable = new TableView(hostWindow, typeof(MemType));
        _objectTable = new TableView(hostWindow, typeof(MemObject));

        // setup the description for content
        _typeTable.AddColumn("TypeName", "Type Name", 0.5f, TextAnchor.MiddleLeft);
        _typeTable.AddColumn("Count", "Count", 0.2f);
        _typeTable.AddColumn("Size", "Size", 0.3f);

        _objectTable.AddColumn("InstanceName", "Instance Name", 0.5f, TextAnchor.MiddleLeft);
        _objectTable.AddColumn("Size", "Size", 0.15f);
        _objectTable.AddColumn("Content", "Content", 0.35f);

        // add test data
        {
            List<object> entries = new List<object>();
            for (int i = 0; i < 100; i++)
                entries.Add(MemType.MakeRandom());
            _typeTable.RefreshData(entries);
        }
        {
            List<object> entries = new List<object>();
            for (int i = 0; i < 100; i++)
                entries.Add(MemObject.MakeRandom());
            _objectTable.RefreshData(entries);
        }

        // register the event-handling function
        _typeTable.OnSelected += OnTypeSelected;
        _objectTable.OnSelected += OnObjectSelected;
    }

    public void RefreshData(CrawledMemorySnapshot unpackedCrawl)
    {
        _unpacked = unpackedCrawl;

    }

    public void Draw(Rect r)
    {
        int border = MemConst.TableBorder;
        GUILayout.BeginArea(r, MemStyles.background);
        if (_typeTable != null)
            _typeTable.Draw(new Rect(border, border, (int)(r.width * 0.5f - border * 1.5f), r.height - border * 2));
        if (_objectTable != null)
            _objectTable.Draw(new Rect((int)(r.width * 0.5f + border * 0.5f), border, (int)r.width * 0.5f - border * 1.5f, (int)r.height - border * 2));
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
