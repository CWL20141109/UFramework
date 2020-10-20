/*
 * @Author: fasthro
 * @Date: 2020-09-16 18:51:28
 * @Description: version
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UFramework.Config;
using UFramework.VersionControl;
using UnityEditor;
using UnityEngine;

namespace UFramework.Editor.VersionControl
{
    public class VersionPage : IPage, IPageBar
    {
        public string menuName { get { return "Version"; } }

        static VersionControl_VersionConfig describeObject;

        /// <summary>
        /// 当前版本
        /// </summary>
        [ReadOnly]
        [BoxGroup("General Settings")]
        [LabelText("    Current Version")]
        public int version;

        /// <summary>
        /// 最低支持版本
        /// </summary>
        [BoxGroup("General Settings")]
        [LabelText("    Minimum Supported Version")]
        [OnValueChanged("OnMinVersionChange")]
        public int minVersion;

        [ShowInInspector, HideLabel]
        [TabGroup("Current Version Patch")]
        [TableList(AlwaysExpanded = true)]
        public List<VPatch> patchs = new List<VPatch>();

        [ShowInInspector, HideLabel, ReadOnly]
        [TabGroup("Support Versions")]
        [DictionaryDrawerSettings(KeyLabel = "Version Code", ValueLabel = "Patch", DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<int, VersionInfo> supportDictionary = new Dictionary<int, VersionInfo>();

        /// <summary>
        /// 版本信息列表
        /// </summary>
        [ShowInInspector, HideLabel, ReadOnly]
        [TabGroup("History Versions")]
        [DictionaryDrawerSettings(KeyLabel = "Version Code", ValueLabel = "Patch", DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<int, VersionInfo> historyDictionary = new Dictionary<int, VersionInfo>();

        public object GetInstance()
        {
            return this;
        }

        public void OnRenderBefore()
        {
            describeObject = UConfig.Read<VersionControl_VersionConfig>();

            version = describeObject.version;
            minVersion = describeObject.minVersion;
            patchs = describeObject.patches;

            supportDictionary.Clear();
            foreach (var item in describeObject.supports)
                supportDictionary.Add(item.version, item);

            historyDictionary.Clear();
            foreach (var item in describeObject.historys)
                historyDictionary.Add(item.version, item);
        }

        public void OnSaveDescribe()
        {
            describeObject.version = version;
            describeObject.minVersion = minVersion;
            describeObject.patches = patchs;

            describeObject.supports.Clear();
            foreach (var item in supportDictionary)
                describeObject.supports.Add(item.Value);

            describeObject.historys.Clear();
            foreach (var item in historyDictionary)
                describeObject.historys.Add(item.Value);

            describeObject.Save();
        }

        public void OnPageBarDraw()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("New Version")))
            {
                if (EditorUtility.DisplayDialog("New Version", "Are you sure to create a new version?", "Confirm", "No"))
                {
                    CreateNewVersion();
                }
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove Current Version")))
            {
                if (EditorUtility.DisplayDialog("Remove Version", "Are you sure to remove the current version?", "Confirm", "No"))
                {
                    RemoveCurrentVersion();
                }
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Remove All Version")))
            {
                if (EditorUtility.DisplayDialog("Remove All Version", "Are you sure to remove the all version?", "Confirm", "No"))
                {
                    RemoveAllVersion();
                }
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Build Version")))
            {
                BuildVersion();
            }
        }

        private void CreateNewVersion()
        {
            var info = new VersionInfo();
            info.version = version;
            info.patchs.Clear();
            info.patchs.AddRange(patchs);
            supportDictionary.Add(version, info);

            version++;
            patchs.Clear();
        }

        private void RemoveCurrentVersion()
        {
            var target = version - 1;
            if (target < 0) return;

            VersionInfo info = null;
            if (supportDictionary.ContainsKey(target))
            {
                info = supportDictionary[target];
                supportDictionary.Remove(target);
            }
            else if (historyDictionary.ContainsKey(target))
            {
                info = historyDictionary[target];
                historyDictionary.Remove(target);
            }

            if (info != null)
            {
                version = info.version;
                patchs.Clear();
                patchs.AddRange(info.patchs);
            }

            if (minVersion > version)
            {
                minVersion = version;
            }
        }

        private void RemoveAllVersion()
        {
            version = 0;
            minVersion = 0;
            patchs.Clear();

            supportDictionary.Clear();
            historyDictionary.Clear();
        }

        private void OnMinVersionChange()
        {
            if (minVersion > version) minVersion = version;
            if (minVersion < 0) minVersion = 0;

            List<int> _removes = new List<int>();
            foreach (KeyValuePair<int, VersionInfo> item in supportDictionary)
            {
                if (item.Key < minVersion)
                    _removes.Add(item.Key);
                historyDictionary.Remove(item.Key);
            }
            for (int i = _removes.Count - 1; i >= 0; i--)
            {
                var key = _removes[i];
                historyDictionary.Add(key, supportDictionary[key]);
                supportDictionary.Remove(_removes[i]);
            }

            for (int i = minVersion; i < version; i++)
            {
                if (!supportDictionary.ContainsKey(i))
                {
                    if (historyDictionary.ContainsKey(i))
                    {
                        supportDictionary.Add(i, historyDictionary[i]);
                        historyDictionary.Remove(i);
                    }
                }
            }
        }

        public static void BuildVersion()
        {
            if (describeObject == null)
                describeObject = UConfig.Read<VersionControl_VersionConfig>();

            var ver = new Version();
            ver.version = describeObject.version;
            ver.minVersion = describeObject.minVersion;
            ver.timestamp = TimeUtils.UTCTimeStamps();
            ver.files.AddRange(describeObject.bundleFiles);
            ver.versions.Clear();

            var vInfo = new VersionInfo();
            vInfo.version = describeObject.version;
            vInfo.baseResCount = describeObject.baseResCount;
            vInfo.patchs.Clear();
            vInfo.patchs.AddRange(describeObject.patches);
            ver.versions.Add(vInfo.version, vInfo);

            foreach (var item in describeObject.supports)
            {
                ver.versions.Add(item.version, item);
            }

            Version.VersionWrite(IOPath.PathCombine(App.UTempDirectory, App.VersionFileName), ver);
        }
    }
}