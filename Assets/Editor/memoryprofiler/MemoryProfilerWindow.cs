using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.MemoryProfiler;

namespace MemoryProfilerWindow
{
    using Item = Assets.Editor.Treemap.Item;
    using Group = Assets.Editor.Treemap.Group;

    public partial class MemoryProfilerWindow : EditorWindow
    {
        [NonSerialized]
        UnityEditor.MemoryProfiler.PackedMemorySnapshot _snapshot;

        [SerializeField]
        PackedCrawlerData _packedCrawled;

        [NonSerialized]
        CrawledMemorySnapshot _unpackedCrawl;

        Vector2 _scrollPosition;

        [NonSerialized]
        private bool _registered = false;
        public Inspector _inspector;
        TreeMapView _treeMapView;

        [MenuItem("Window/Memory/MemoryProfiler")]
        static void Create()
        {
            EditorWindow.GetWindow<MemoryProfilerWindow>();
        }

        [MenuItem("Window/Memory/MemoryProfilerInspect")]
        static void Inspect()
        {
        }

        public void OnDisable()
        {
            //    UnityEditor.MemoryProfiler.MemorySnapshot.OnSnapshotReceived -= IncomingSnapshot;
            if (_treeMapView != null)
                _treeMapView.CleanupMeshes ();
        }

        public void Initialize()
        {
            if (_treeMapView == null)
                _treeMapView = new TreeMapView ();
            
            if (!_registered)
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.OnSnapshotReceived += IncomingSnapshot;
                _registered = true;
            }

            if (_unpackedCrawl == null && _packedCrawled != null && _packedCrawled.valid)
                Unpack();


        }

        void OnGUI()
        {
            Initialize();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Take Snapshot"))
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            }
            if (GUILayout.Button("Save Snapshot..."))
            {
                if (_snapshot != null)
                {
                    string fileName = EditorUtility.SaveFilePanel("Save Snapshot", null, "MemorySnapshot", "memsnap");
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        using (Stream stream = File.Open(fileName, FileMode.Create))
                        {
                            bf.Serialize(stream, _snapshot);
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No snapshot to save.  Try taking a snapshot first.");
                }
            }
            if (GUILayout.Button("Load Snapshot..."))
            {
                string fileName = EditorUtility.OpenFilePanel("Load Snapshot", null, "memsnap");
                if (!string.IsNullOrEmpty(fileName))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    using (Stream stream = File.Open(fileName, FileMode.Open))
                    {
                        IncomingSnapshot(bf.Deserialize(stream) as PackedMemorySnapshot);
                    }
                }
            }
            GUILayout.EndHorizontal();

            OnGUI_Entended();

            if (_treeMapView != null)
                _treeMapView.Draw();
            if (_inspector != null)
                _inspector.Draw();

            //RenderDebugList();
        }

        public string[] FindThingsByName(string name)
        {
            string lower = name.ToLower();
            List<string> ret = new List<string>();
            foreach (var thing in _unpackedCrawl.allObjects)
            {
                var nat = thing as NativeUnityEngineObject;
                if (nat != null && nat.name.ToLower().Contains(lower))
                    ret.Add(nat.name);
            }
            return ret.ToArray();
        }

        public ThingInMemory FindThingInMemoryByExactName(string name)
        {
            foreach (var thing in _unpackedCrawl.allObjects)
            {
                var nat = thing as NativeUnityEngineObject;
                if (nat != null && nat.name == name)
                    return thing;
            }

            return null;
        }

        public void SelectThing(ThingInMemory thing)
        {
            _inspector.SelectThing(thing);
            _treeMapView.SelectThing(thing);
        }

        public void SelectGroup(Group group)
        {
            _treeMapView.SelectGroup(group);
        }

        private void RenderDebugList()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (var thing in _unpackedCrawl.allObjects)
            {
                var mo = thing as ManagedObject;
                if (mo != null)
                    GUILayout.Label("MO: " + mo.typeDescription.name);

                var gch = thing as GCHandle;
                if (gch != null)
                    GUILayout.Label("GCH: " + gch.caption);

                var sf = thing as StaticFields;
                if (sf != null)
                    GUILayout.Label("SF: " + sf.typeDescription.name);
            }

            GUILayout.EndScrollView();
        }

        void Unpack()
        {
            _unpackedCrawl = CrawlDataUnpacker.Unpack(_packedCrawled);
            _inspector = new Inspector(this, _unpackedCrawl, _snapshot);

            if(_treeMapView != null)
                _treeMapView.Setup(this, _unpackedCrawl);
        }

        void IncomingSnapshot(PackedMemorySnapshot snapshot)
        {
            if (_isRequestingCompBegin)
            {
                OnSnapshotBeginComparing(snapshot);
            }

            _snapshot = snapshot;

            _packedCrawled = new Crawler().Crawl(_snapshot);
            Unpack();
        }
    }
}
