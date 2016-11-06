using Common;
using Entities.Enums;
using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Web.Areas.Management.Filters;
using Web.Areas.Management.Helpers;
using Web.Helpers;

namespace Web.Areas.Management.Controllers
{
    [RouteArea("Management", AreaPrefix = "quan-ly")]
    [RoutePrefix("import")]
    public class ImportController : BaseController
    {
        [Route("import-du-lieu", Name = "ImportIndex")]
        [ValidationPermission(Action = ActionEnum.Read, Module = ModuleEnum.Import)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// import
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("import-nha-modal", Name = "ImportNhaModal")]
        public async Task<ActionResult> ImportNha()
        {

            return PartialView("ImportNhaModal");
        }

        [HttpPost]
        [Route("import-nha")]
        public string ImportNha(HttpPostedFileBase uploadFile)
        {
            StringBuilder strValidations = new StringBuilder(string.Empty);
            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    string filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/files"), Path.GetFileName(uploadFile.FileName));
                    uploadFile.SaveAs(filePath);
                    DataSet ds = new DataSet();

                    //A 32-bit provider which enables the use of

                    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=Excel 12.0;";

                    using (OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ConnectionString))
                    {
                        conn.Open();
                        using (DataTable dtExcelSchema = conn.GetSchema("Tables"))
                        {
                            string sheetName = "Data$";
                            string query = "SELECT * FROM [" + sheetName + "]";
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                            adapter.Fill(ds, "Items");
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        //Now we can insert this data to database...  
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return "";
        }

        /// <summary>
        /// import
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("import-khach-modal", Name = "ImportKhachModal")]
        public async Task<ActionResult> ImportKhach()
        {

            return PartialView("ImporKhachModal");
        }


    }
}