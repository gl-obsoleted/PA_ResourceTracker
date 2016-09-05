using System;
using System.IO;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using UnityEditor;

namespace MemoryProfilerWindow
{
    public partial class MemoryProfilerWindow 
    {
        public float TopButtonsVerticalSpaces { get { return 25 /*+ _extendedButtonHeight*/; } }
        public bool EnhancedMode { get { return _enhancedMode; } }

        bool _enhancedMode = true;
        bool _autoSaveForComparison = true;
        int _selectedBegin = 0;
        int _selectedEnd = 0;
        string[] _snapshotFiles = new string[] { };

        void OnGUI_Entended()
        {
            GUILayout.FlexibleSpace();

            {
                GUILayout.Space(50);
                EditorGUIUtility.labelWidth = 40;
                _selectedBegin = EditorGUILayout.Popup(string.Format("Begin"), _selectedBegin, _snapshotFiles, GUILayout.Width(250));
                
                GUILayout.Space(50);

                _selectedEnd = EditorGUILayout.Popup(string.Format("End"), _selectedEnd, _snapshotFiles, GUILayout.Width(250));
                EditorGUIUtility.labelWidth = 0; // reset to default
                GUILayout.Space(50);
            }

            if (_selectedBegin == _selectedEnd)
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
            if (_selectedBegin == _selectedEnd)
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
