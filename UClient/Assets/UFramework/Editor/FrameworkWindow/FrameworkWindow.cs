﻿/*
 * @Author: fasthro
 * @Date: 2020-06-28 14:01:30
 * @Description: UFramework 编辑器工具
 */
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UFramework.Config;
using UnityEditor;
using UnityEngine;

namespace UFramework.FrameworkWindow
{
    public class FrameworkWindow : OdinMenuEditorWindow
    {
        [MenuItem("UFramework/UFramework")]
        private static void OpenWindow()
        {
            var window = GetWindow<FrameworkWindow>();
            window.titleContent = new GUIContent("UFramework");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        static ConfigPage configPage = new ConfigPage();
        static TablePage tablePage = new TablePage();
        static TablePreferencesPage tablePreferencesPage = new TablePreferencesPage();

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(false);

            tree.Add("Lua", this);

            tree.Add("AssetBundle/Configure", this);
            tree.Add("AssetBundle/Build", this);
            tree.Add("AssetBundle/Setting", this);

            tree.Add("Pool", this);
            tree.Add("I18N", this);
            // config
            tree.Add(configPage.menuName, configPage.GetInstance());
            // table
            tree.Add(tablePage.menuName, tablePage.GetInstance());
            tree.Add(tablePreferencesPage.menuName, tablePreferencesPage.GetInstance());
            // sdk
            tree.Add("SDK", this);
            tree.Add("Version", this);
            tree.Add("Setting", this);

            tree.Selection.SelectionChanged += SelectionChanged;

            return tree;
        }

        void SelectionChanged(SelectionChangedType type)
        {
            if (type == SelectionChangedType.ItemAdded)
            {
                var selection = MenuTree.Selection[0];
                if (configPage.IsPage(selection.Name))
                {
                    configPage.OnRenderBefore();
                }
                else if (tablePage.IsPage(selection.Name))
                {
                    tablePage.OnRenderBefore();
                }
                else if (tablePreferencesPage.IsPage(selection.Name))
                {
                    tablePreferencesPage.OnRenderBefore();
                }
            }
        }
    }
}