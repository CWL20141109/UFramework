/*
 * @Author: fasthro
 * @Date: 2020-07-01 19:51:50
 * @Description: Application Config
 */
using System.Collections.Generic;
using UFramework.Config;
using UnityEngine;

namespace UFramework
{
    /// <summary>
    /// App类型
    /// </summary>
    public enum AppEnvironmentType
    {
        /// <summary>
        /// 测试环境
        /// </summary>
        Debug,

        /// <summary>
        /// 正式环境
        /// </summary>
        Release,
    }

    public class AppConfig : IConfigObject
    {
        public string name { get { return "AppConfig"; } }

        #region version
        /// <summary>
        /// 开发版本
        /// </summary>
        public bool isDevelopmentVersion = true;

        /// <summary>
        /// App 环境类型
        /// </summary>
        public AppEnvironmentType appEnvironmentType = AppEnvironmentType.Debug;

        /// <summary>
        /// 日志
        /// </summary>
        public bool isLogEnable = true;

        #endregion

        #region i18n language

        /// <summary>
        /// 是否使用系统语言
        /// </summary>
        public bool useSystemLanguage;

        /// <summary>
        /// 默认语言
        /// </summary>
        public SystemLanguage defaultLanguage = SystemLanguage.ChineseSimplified;

        /// <summary>
        /// 支持的语言列表
        /// </summary>
        public List<SystemLanguage> supportedLanguages = new List<SystemLanguage>();

        #endregion

        public void Save()
        {
            UConfig.Write<AppConfig>(this);
        }
    }
}