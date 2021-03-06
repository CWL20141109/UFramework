/*
 * @Author: fasthro
 * @Date: 2020-09-16 18:51:28
 * @Description: build
 */

using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UFramework.Config;
using UnityEditor;
using UnityEngine;

namespace UFramework.Editor.VersionControl
{
    public class BuilderPage : IPage, IPageBar
    {
        public string menuName { get { return "Build"; } }
        static VersionControl_BuildConfig describeObject;

        /// <summary>
        /// 版本
        /// </summary>
        [OnValueChanged("OnValueChanged_appVersion")]
        [DisableIf("_isBuild")]
        [LabelText("App Version (Only Display)")]
        public string appVersion;

        private void OnValueChanged_appVersion()
        {
            describeObject.appVersion = appVersion;
            describeObject.Save();
        }

        [ShowInInspector, HideLabel]
        public BuildPageBuildTable buildTable = new BuildPageBuildTable();

        private bool _isBuild { get { return buildTable.isBuild; } }

        public object GetInstance()
        {
            return this;
        }

        public void OnRenderBefore()
        {
            describeObject = UConfig.Read<VersionControl_BuildConfig>();
            appVersion = describeObject.appVersion;
            if (string.IsNullOrEmpty(appVersion))
                appVersion = "1.0.0";

            ApplySetting();
        }

        public void OnSaveDescribe()
        {
            describeObject.appVersion = appVersion;
            describeObject.Save();
        }

        public void OnPageBarDraw()
        {

        }

        private void ApplySetting()
        {
            PlayerSettings.Android.useAPKExpansionFiles = describeObject.packageType == PACKAGE_TYPE.GOOGLE_PLAY_OBB;
            EditorUserBuildSettings.buildAppBundle = describeObject.packageType == PACKAGE_TYPE.GOOGLE_PLAY_APPBUNDLE;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, describeObject.scripting);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, describeObject.scripting);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, describeObject.scripting);

            // arm64
            var _googlePlaySupportARM64 = describeObject.scripting == ScriptingImplementation.IL2CPP && (describeObject.packageType == PACKAGE_TYPE.GOOGLE_PLAY_APPBUNDLE || describeObject.packageType == PACKAGE_TYPE.GOOGLE_PLAY_OBB);
            if (_googlePlaySupportARM64)
            {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            }
            else
            {
                if (describeObject.supportARM64)
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
                else
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}