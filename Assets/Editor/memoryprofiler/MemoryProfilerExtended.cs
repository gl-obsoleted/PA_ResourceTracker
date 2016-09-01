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


        bool _isRequestingCompBegin = false;
        string _compBeginFilename;
        void OnSnapshotBeginComparing(PackedMemorySnapshot snapshot)
        {
            string filename = string.Format("{0}/{1}-{2}.memsnap",
                Application.persistentDataPath, 
                SysUtil.FormatDateAsFileNameString(DateTime.Now), 
                SysUtil.FormatTimeAsFileNameString(DateTime.Now));

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                bf.Serialize(stream, snapshot);
            }

            _compBeginFilename = filename;
            _isRequestingCompBegin = false;
        }

        void OnGUI_Entended()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(_extendedButtonHeight));

            GUILayout.Space(5);

            if (GUILayout.Button("Snapshot (Begin)", GUILayout.MaxWidth(120)))
            {
                _isRequestingCompBegin = true;
                _compBeginFilename = "";
                UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            }
            if (GUILayout.Button("Snapshot (End)", GUILayout.MaxWidth(120)))
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
                if (!string.IsNullOrEmpty(_compBeginFilename))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    using (Stream stream = File.Open(_compBeginFilename, FileMode.Open))
                    {
                        MemoryCompareTarget.Instance.SetCompareTarget(bf.Deserialize(stream) as PackedMemorySnapshot);

                        var newlyAdded = MemoryCompareTarget.Instance.GetNewlyAdded(_unpackedCrawl);

                        if (_treeMapView != null)
                            _treeMapView.Setup(this, _unpackedCrawl, newlyAdded);
                    }
                }
            }

            GUILayout.Label(_compBeginFilename);
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
    }
}
