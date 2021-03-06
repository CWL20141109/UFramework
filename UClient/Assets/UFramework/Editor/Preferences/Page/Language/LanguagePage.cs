/*
 * @Author: fasthro
 * @Date: 2020-07-01 20:01:09
 * @Description: Language Page
 */
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UFramework.Config;
using UFramework.Language;
using UnityEditor;
using UnityEngine;

namespace UFramework.Editor.Preferences
{
    public class LanguagePage : IPage, IPageBar
    {
        public string menuName { get { return "Language"; } }

        static AppConfig describeObject;

        /// <summary>
        /// 是否使用系统语言
        /// </summary>
        [BoxGroup("General Settings")]
        [LabelText("    Use System Language")]
        public bool useSystemLanguage;

        /// <summary>
        /// 默认语言
        /// </summary>
        [BoxGroup("General Settings")]
        [LabelText("    Default Language")]
        public SystemLanguage defaultLanguage = SystemLanguage.ChineseSimplified;

        /// <summary>
        /// 支持的语言列表
        /// </summary>
        [ListDrawerSettings(Expanded = true)]
        public List<SystemLanguage> supportedLanguages = new List<SystemLanguage>();

        public object GetInstance()
        {
            return this;
        }

        public void OnRenderBefore()
        {
            bool hasNew = false;
            describeObject = UConfig.Read<AppConfig>();
            if (describeObject.supportedLanguages.Count == 0)
            {
                hasNew = true;
                describeObject.supportedLanguages.Add(describeObject.defaultLanguage);
            }

            useSystemLanguage = describeObject.useSystemLanguage;
            defaultLanguage = describeObject.defaultLanguage;
            supportedLanguages = describeObject.supportedLanguages;

            if (hasNew)
            {
                describeObject.Save();
            }
        }

        public void OnPageBarDraw()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Generate")))
            {
                var opt = new ExcelReaderOptions();
                opt.languages = describeObject.supportedLanguages;
                var reader = new ExcelReader(opt);
                reader.Read();

                new Excel2Text(reader);
                new Excel2Index(reader);
                // TODO 实现多语言Lua Index
                // new Excel2LuaIndex(reader);

                AssetDatabase.Refresh();
            }
        }

        public void OnSaveDescribe()
        {
            if (describeObject == null) return;
            describeObject.useSystemLanguage = useSystemLanguage;
            describeObject.defaultLanguage = defaultLanguage;
            describeObject.supportedLanguages = supportedLanguages;
            describeObject.Save();
        }
    }
}