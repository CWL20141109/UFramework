/*
 * @Author: fasthro
 * @Date: 2020-06-30 11:55:50
 * @Description: 
 */
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UFramework.Config;
using UnityEditor;
using UnityEngine;

namespace UFramework.FrameworkWindow
{
    /// <summary>
    /// AssetBundle 路径打包类型
    /// </summary>
    public enum AssetBundleBuildPathType
    {
        /// <summary>
        /// 单文件
        /// </summary>
        File,
        /// <summary>
        /// 目录内单文件
        /// </summary>
        DirectoryFile,
        /// <summary>
        /// 整目录
        /// </summary>
        Directory,
        /// <summary>
        /// 整个子目录
        /// </summary>
        SubDirectory,
        /// <summary>
        /// 标准资源
        /// </summary>
        Standard,
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum AssetBundleBuildAssetType
    {
        File,
        Prefab,
        Texture,
        Materail,
        Animation,
        AnimatorController,
    }

    /// <summary>
    /// AssetBundle PathItem
    /// </summary>
    [System.Serializable]
    public class AssetBundleAssetPathItem
    {
        /// <summary>
        /// 资源路径/资源目录
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup]
        [ValidateInput("ValidateInputPath", "$validateInputPathMsg", InfoMessageType.Error)]
        [FolderPath]
        public string path;

        /// <summary>
        /// 打包类型
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup(120f)]
        public AssetBundleBuildPathType buildType;

        /// <summary>
        /// 资源类型
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup(120f)]
        public AssetBundleBuildAssetType assetType;

        /// <summary>
        /// 忽略模式
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup(100f)]
        public string pattern = "*.*";

        #region 路径验证
        private string validateInputPathMsg;
        private bool ValidateInputPath(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                validateInputPathMsg = "error: path is empty.";
            }
            else if (!IOPath.FileExists(value) && !IOPath.DirectoryExists(value))
            {
                validateInputPathMsg = "error: path not exists.";
            }
            return !string.IsNullOrEmpty(value) && (IOPath.FileExists(value) || IOPath.DirectoryExists(value));
        }

        #endregion
    }

    /// <summary>
    /// AssetBundle 资源信息
    /// </summary>
    [System.Serializable]
    public class AssetBundleAssetItem
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup]
        [ReadOnly]
        public string path;

        /// <summary>
        /// bundle
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup]
        [ReadOnly]
        public string bundle;


        /// <summary>
        /// 资源类型
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup(120f)]
        [ReadOnly]
        public AssetBundleBuildAssetType assetType;

        /// <summary>
        /// 资源大小
        /// </summary>
        [HideInInspector]
        public long size;

        /// <summary>
        /// 资源大小文本
        /// </summary>
        [ShowInInspector]
        [HideLabel]
        [HorizontalGroup(150f)]
        [ReadOnly]
        public string sizeString
        {
            get
            {
                if (size == 0)
                {
                    return "--";
                }
                return EditorUtility.FormatBytes(size);
            }
        }

        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "Byte", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;
            if (bytes > 1024)
                for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    dblSByte = bytes / 1024.0;
            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }

    /// <summary>
    /// 资源路径配置
    /// </summary>
    public class AssetBundleAssetPathItemConfig : IConfigObject
    {
        public string name { get { return "AssetBundleAssetPathItemConfig"; } }
        public List<AssetBundleAssetPathItem> assetPathItems = new List<AssetBundleAssetPathItem>();

        public void Save()
        {
            UConfig.Write<AssetBundleAssetPathItemConfig>(this);
        }
    }

    /// <summary>
    /// 资源配置
    /// </summary>
    public class AssetBundleAssetItemConfig : IConfigObject
    {
        public string name { get { return "AssetBundleAssetItemConfig"; } }
        public List<AssetBundleAssetItem> assets = new List<AssetBundleAssetItem>();

        public void Save()
        {
            UConfig.Write<AssetBundleAssetItemConfig>(this);
        }
    }
}