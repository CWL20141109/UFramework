/*
 * @Author: fasthro
 * @Date: 2019-12-17 20:45:12
 * @Description: excel reader options
 */
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using UnityEngine;

namespace UFramework.Table
{
    /// <summary>
    /// 格式
    /// </summary>
    public enum FormatOptions
    {
        CSV,
        JSON,
        LUA,
    }

    /// <summary>
    /// 数据格式
    /// </summary>
    public enum DataFormatOptions
    {
        Array,
        IntDictionary,
        StringDictionary,
        Int2IntDictionary,
    }

    public class ExcelReaderOptions
    {
        public FormatOptions outFormatOptions;            // 导出格式
        public DataFormatOptions dataFormatOptions;       // 数据格式
        public string tableName;                          // 表名称
        public string dataOutDirectory;                   // 数据文件输出目录
        public string tableModelOutDirectory;             // table model 文件输出目录
        public string luaOutDirectory;                    // lua 文件输出目录


        // 数据文件输出路径
        public string dataOutFilePath
        {
            get
            {
                if (outFormatOptions == FormatOptions.CSV) return IOPath.PathCombine(dataOutDirectory, tableName + ".csv");
                else return IOPath.PathCombine(dataOutDirectory, tableName + ".json");
            }
        }

        // table model 文件输出路径
        public string tableModelOutFilePath
        {
            get { return IOPath.PathCombine(tableModelOutDirectory, tableName + "Table.cs"); }
        }

        // lua 文件输出路径
        public string luaOutFilePath
        {
            get { return IOPath.PathCombine(luaOutDirectory, tableName + ".lua"); }
        }
    }
}