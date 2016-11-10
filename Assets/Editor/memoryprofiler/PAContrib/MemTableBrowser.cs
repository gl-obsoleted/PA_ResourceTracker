using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using MemoryProfilerWindow;
using Assets.Editor.Treemap;

public class MemType
{
    public string TypeName = "Foo";
    public int Count = 0;
    public int Size = 0;
    public string SizeLiterally = "";

    public List<object> Objects = new List<object>();

    public void AddObject(MemObject mo)
    {
        Objects.Add(mo);
        Count = Objects.Count;
        Size += mo.Size;
        SizeLiterally = EditorUtility.FormatBytes(Size);
    }
}

public class MemObject
{
    public string InstanceName;

    public int Size = 0;

    public string Content = "";

    public MemObject(ThingInMemory thing)
    {
        _thing = thing;

        if (_thing != null)
        {
            InstanceName = _thing.caption;
            Size = _thing.size;
            Content = "content";
        }
    }

    public ThingInMemory _thing;
}

public class MemTableBrowser
{
    CrawledMemorySnapshot _unpacked;

    TableView _typeTable;
    TableView _objectTable;
    EditorWindow _hostWindow;

    private Dictionary<string, MemType> _types = new Dictionary<string, MemType>();

    public MemTableBrowser(EditorWindow hostWindow)
    {
        _hostWindow = hostWindow;

        // create the table with a specified object type
        _typeTable = new TableView(hostWindow, typeof(MemType));
        _objectTable = new TableView(hostWindow, typeof(MemObject));

        // setup the description for content
        _typeTable.AddColumn("TypeName", "Type Name", 0.6f, TextAnchor.MiddleLeft);
        _typeTable.AddColumn("Count", "Count", 0.15f);
        _typeTable.AddColumn("Size", "Size", 0.25f, TextAnchor.MiddleCenter, PAEditorConst.BytesFormatter);

        _objectTable.AddColumn("InstanceName", "Instance Name", 0.5f, TextAnchor.MiddleLeft);
        _objectTable.AddColumn("Size", "Size", 0.15f, TextAnchor.MiddleCenter, PAEditorConst.BytesFormatter);
        _objectTable.AddColumn("Content", "Content", 0.35f);

        // sorting
        _typeTable.SetSortParams(2, true);
        _objectTable.SetSortParams(1, true);

        // register the event-handling function
        _typeTable.OnSelected += OnTypeSelected;
        _objectTable.OnSelected += OnObjectSelected;
    }

    public void RefreshData(CrawledMemorySnapshot unpackedCrawl)
    {
        _types.Clear();
        _unpacked = unpackedCrawl;

        List<object> types = new List<object>();
        foreach (ThingInMemory thingInMemory in _unpacked.allObjects)
        {
            string typeName = MemUtil.GetGroupName(thingInMemory);
            if (typeName.Length == 0)
                continue;

            MemType theType;
            if (!_types.ContainsKey(typeName))
            {
                theType = new MemType();
                theType.TypeName = typeName;
                theType.Objects = new List<object>();
                _types.Add(typeName, theType);
                types.Add(theType);
            }
            else
            {
                theType = _types[typeName];
            }

            MemObject item = new MemObject(thingInMemory);
            theType.AddObject(item);
        }

        _typeTable.RefreshData(types);
        _objectTable.RefreshData(null);
    }

    public void Draw(Rect r)
    {
        int border = MemConst.TableBorder;
        float split = MemConst.SplitterRatio;

        GUILayout.BeginArea(r, MemStyles.background);
        if (_typeTable != null)
            _typeTable.Draw(new Rect(border, border, (int)(r.width * split - border * 1.5f), r.height - border * 2));
        if (_objectTable != null)
            _objectTable.Draw(new Rect((int)(r.width * split + border * 0.5f), border, (int)r.width * (1.0f - split) - border * 1.5f, (int)r.height - border * 2));
        GUILayout.EndArea();
    }

    void OnTypeSelected(object selected, int col)
    {
        MemType mt = selected as MemType;
        if (mt == null)
            return;

        _objectTable.RefreshData(mt.Objects);
    }

    void OnObjectSelected(object selected, int col)
    {
        var mpw = _hostWindow as MemoryProfilerWindow.MemoryProfilerWindow;
        if (mpw == null)
            return;

        var memObject = selected as MemObject;
        if (memObject == null)
            return;

        mpw.SelectThingInInspector(memObject._thing);
    }

    public void SelectThing(ThingInMemory thing)
    {
        string typeName = MemUtil.GetGroupName(thing);

        MemType mt;
        if (!_types.TryGetValue(typeName, out mt))
            return;

        if (_typeTable.GetSelected() != mt)
        {
            _typeTable.SetSelected(mt);
            _objectTable.RefreshData(mt.Objects);
        }

        foreach (var item in mt.Objects)
        {
            var mo = item as MemObject;
            if (mo != null && mo._thing == thing)
            {
                if (_objectTable.GetSelected() != mo)
                {
                    _objectTable.SetSelected(mo);
                }
                break;
            }
        }
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
