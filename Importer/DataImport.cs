using MrHuo.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MrHuo.Controls.ImportTools
{
    /// <summary>
    /// CSV文件数据导入工具
    /// </summary>
    public class DataImport : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="importArgument"></param>
        public DataImport(ImportArgument importArgument)
        {
            this.ImportArgument = importArgument;
        }

        private const String CreateTempTableSql = "SELECT TOP 0 * FROM {0}";
        private const String InsertSql = "INSERT INTO [{0}] VALUES ({1})";

        /// <summary>
        /// 获取或者设置一个值，该值表示导入参数
        /// </summary>
        public ImportArgument ImportArgument { get; set; }

        /// <summary>
        /// 导入之前执行的事件
        /// </summary>
        public event EventHandler OnBeginImport;
        /// <summary>
        /// 导入一条数据之后执行的事件
        /// </summary>
        public event EventHandler<ImportingEventArgs> OnDataImported;
        /// <summary>
        /// 全部导入完成之后执行的事件
        /// </summary>
        public event EventHandler OnEndImport;

        /// <summary>
        /// 导入方法
        /// </summary>
        /// <returns></returns>
        public RestResult Import()
        {
            RestResult result = new RestResult();
            Check.NullOrEmpty(ImportArgument, "ImportArgument");

            try
            {
                using (SqlConnection conn = new SqlConnection(ImportArgument.DataConnectionString))
                {
                    conn.Open();
                    Check.True(OnBeginImport != null, () => { this.OnBeginImport(this, new EventArgs()); });

                    try
                    {
                        int secCount = 0;
                        int faidCount = 0;
                        int startRow = ImportArgument.FirstRowIsHead ? 1 : 0;

                        using (DataBaseHelper helper = new DataBaseHelper(conn))
                        {
                            IEnumerable<Column> TableColumns = helper.GetColumnsByTableName(ImportArgument.DataTable);
                            Check.True(TableColumns != null, () =>
                            {
                                var datas = File.ReadLines(ImportArgument.FilePath, Encoding.Default).ToArray();
                                string insertSql = string.Empty;
                                for (int i = startRow; i < datas.Length; i++)
                                {
                                    var mapSql = Mapping(TableColumns.Where(p => !p.IsPrimaryKey), datas[i].Split(','));
                                    insertSql = string.Format(InsertSql, ImportArgument.DataTable, mapSql);
                                    Check.True(conn.State != ConnectionState.Open, () => { conn.Open(); });

                                    using (SqlCommand cmd2 = new SqlCommand(insertSql, conn))
                                    {
                                        var ret3 = cmd2.ExecuteNonQuery();
                                        Check.True(ret3 != -1, () => { secCount++; }, () => { faidCount++; });
                                        Check.True(this.OnDataImported != null, () =>
                                        {
                                            this.OnDataImported(this, new ImportingEventArgs()
                                            {
                                                IsImportSuccessed = ret3 != -1,
                                                CurrentImportIndex = startRow == 0 ? i + 1 : i,
                                                DataCount = datas.Length
                                            });
                                        });
                                    }
                                }

                                Check.True(faidCount > 0, () =>
                                {
                                    result.IsSuccess = false;
                                    result.Message = string.Format(RS.get("Importer_Result_HasFaildItem"), secCount, faidCount);
                                }, () =>
                                {
                                    result.IsSuccess = true;
                                    result.Message = string.Format(RS.get("Importer_Result_Success"), secCount);
                                });
                            }, () =>
                            {
                                result.IsSuccess = false;
                                result.Message = RS.get("Importer_Exception_CanNotGetConstruction");
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = ex.Message;
                    }
                    Check.True(OnEndImport != null, () => { this.OnEndImport(this, new EventArgs()); });
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private string Mapping(IEnumerable<Column> sColumns, string[] dColumns)
        {
            StringBuilder sb = new StringBuilder();
            var index = 0;
            foreach (var column in sColumns)
            {
                var sqlType = column.ColumnType.ToLower();
                if (sqlType == "int" || sqlType == "bigint" || sqlType == "bit")
                {
                    sb.Append(dColumns[index++]);
                }
                else
                {
                    sb.Append("'" + dColumns[index++] + "'");
                }
                if (index < sColumns.Count())
                {
                    sb.Append(",");
                }
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 释放系统资源
        /// </summary>
        public void Dispose()
        {
            if (ImportArgument != null)
            {
                GC.ReRegisterForFinalize(ImportArgument);
            }
            GC.Collect();
        }
    }
}