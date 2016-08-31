using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Editor.Treemap;
using Treemap;
using UnityEditor;
using UnityEngine;
using System;
using System.Net;
using NUnit.Framework.Constraints;
using UnityEditor.MemoryProfiler;
using Object = UnityEngine.Object;
using System.IO;

namespace MemoryProfilerWindow
{
    public partial class MemoryProfilerWindow 
    {
        float _extendedButtonHeight = 35f;

        public float TopButtonsVerticalSpaces { get { return 25 + _extendedButtonHeight; } }

        string _searchString = "";

        void OnGUI_Entended()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(_extendedButtonHeight));

            GUILayout.Space(5);

            if (GUILayout.Button("Take Snapshot (Begin)", GUILayout.MaxWidth(200)))
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            }
            if (GUILayout.Button("Take Snapshot (End)", GUILayout.MaxWidth(200)))
            {
                string fileName = EditorUtility.OpenFilePanel("Compare With", null, "memsnap");
                if (!string.IsNullOrEmpty(fileName))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    using (Stream stream = File.Open(fileName, FileMode.Open))
                    {
                        MemoryCompareTarget.Instance.SetCompareTarget(bf.Deserialize(stream) as PackedMemorySnapshot);

                        var newlyAdded = MemoryCompareTarget.Instance.GetNewlyAdded(_unpackedCrawl);

                        if (_treeMapView != null)
                            _treeMapView.Setup(this, _unpackedCrawl, newlyAdded);
                    }
                }
            }
            GUILayout.FlexibleSpace();

            _searchString = GUILayout.TextField(_searchString, 100, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.MinWidth(300));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                _searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
        }
    }
}
