using System;
using System.IO;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using UnityEditor;

namespace MemoryProfilerWindow
{
    public partial class MemoryProfilerWindow 
    {
        float _extendedButtonHeight = 35f;

        string[] _snapshotFiles = new string[] {};

        public float TopButtonsVerticalSpaces { get { return 25 /*+ _extendedButtonHeight*/; } }
        public bool EnhancedMode { get { return _enhancedMode; } }

        bool _enhancedMode = true;
        bool _autoSaveForComparison = true;
        int _selectedBegin = 0;
        int _selectedEnd = 0;

        bool _isRequestingCompBegin = false;
        string _compBeginFilename;
        string SaveTimestampedSnapshot(PackedMemorySnapshot snapshot)
        {
            try
            {
                string filename = MemorySnapshotUtil.GetFullpath(string.Format("{0}-{1}.memsnap",
                    SysUtil.FormatDateAsFileNameString(DateTime.Now),
                    SysUtil.FormatTimeAsFileNameString(DateTime.Now)));

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (Stream stream = File.Open(filename, FileMode.Create))
                {
                    bf.Serialize(stream, snapshot);
                }
                return filename;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return "";
            }
        }

        void OnSnapshotBeginComparing(PackedMemorySnapshot snapshot)
        {
            _compBeginFilename = SaveTimestampedSnapshot(snapshot);
            _isRequestingCompBegin = false;
        }

        void OnGUI_Entended()
        {
            //GUILayout.BeginHorizontal(GUILayout.Height(_extendedButtonHeight));

            //GUILayout.Space(5);

            //if (GUILayout.Button("Snapshot (Begin)", GUILayout.MaxWidth(120)))
            //{
            //    _isRequestingCompBegin = true;
            //    _compBeginFilename = "";
            //    UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            //}
            //if (GUILayout.Button("Snapshot (End)", GUILayout.MaxWidth(120)))
            //{
            //    UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            //    if (!string.IsNullOrEmpty(_compBeginFilename))
            //    {
            //        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //        using (Stream stream = File.Open(_compBeginFilename, FileMode.Open))
            //        {
            //            MemoryCompareTarget.Instance.SetCompareTarget(bf.Deserialize(stream) as PackedMemorySnapshot);

            //            var newlyAdded = MemoryCompareTarget.Instance.GetNewlyAdded(_unpackedCrawl);

            //            if (_treeMapView != null)
            //                _treeMapView.Setup(this, _unpackedCrawl, newlyAdded);
            //        }
            //    }
            //}

            //GUILayout.Label(_compBeginFilename);

            GUILayout.FlexibleSpace();

            EditorGUIUtility.labelWidth = 40;

            _selectedBegin = EditorGUILayout.Popup(
                string.Format("Begin"),
                _selectedBegin,
                _snapshotFiles, GUILayout.Width(250));

            GUILayout.Space(50);

            _selectedEnd = EditorGUILayout.Popup(
                string.Format("End"),
                _selectedEnd,
                _snapshotFiles, GUILayout.Width(250));

            EditorGUIUtility.labelWidth = 0; // reset to default

            GUILayout.Space(50);

            bool disabling = _selectedBegin == _selectedEnd;

            if (disabling)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Compare", GUILayout.MaxWidth(120)))
            {
                Debug.LogFormat("Compare '{0}' with '{1}'", _snapshotFiles[_selectedBegin], _snapshotFiles[_selectedEnd]);

                var snapshotBegin = MemorySnapshotUtil.Load(_snapshotFiles[_selectedBegin]);
                var snapshotEnd = MemorySnapshotUtil.Load(_snapshotFiles[_selectedEnd]);

                if (snapshotBegin != null && snapshotEnd != null)
                {
                    MemoryCompareTarget.Instance.SetCompareTarget(snapshotBegin);

                    IncomingSnapshot(snapshotEnd);

                    if (_treeMapView != null)
                        _treeMapView.Setup(this, _unpackedCrawl, MemoryCompareTarget.Instance.GetNewlyAdded(_unpackedCrawl));
                }
            }
            if (disabling)
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Open Dir", GUILayout.MaxWidth(120)))
            {
                EditorUtility.RevealInFinder(MemorySnapshotUtil.SnapshotsDir);
            }
        }

        void RefreshSnapshotList()
        {
            _snapshotFiles = MemorySnapshotUtil.GetFiles();
        }
    }
}
