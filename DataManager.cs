using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;

namespace NCrawler.Demo
{
    class DataManager
    {
        DirectoryInfo outputDir = new DirectoryInfo(@"e:\Tests\EMagProducts\");
        const string XLSFileName = "EmagProducts.xlsx";
        private List<sProduct> m_lProducts = new List<sProduct>();

        ExcelPackage m_XLSPackage = new ExcelPackage();
        ExcelWorksheet m_WorkSheet;
        bool m_bFileOk;

        private static DataManager m_Instance;

        private DataManager()
        {
            m_bFileOk = openXLSFile();
        }

        public static DataManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new DataManager();
                }
                return m_Instance;
            }
        }

        private bool openXLSFile()
        {
            bool retVal = false;

            try
            {
                FileInfo newFile = new FileInfo(outputDir.FullName + XLSFileName);
                if (!newFile.Exists)
                {
                    m_XLSPackage = new ExcelPackage(newFile);
                    m_XLSPackage.Workbook.Worksheets.Add("Products");
                    m_WorkSheet = m_XLSPackage.Workbook.Worksheets["Products"];
                    addXLSSchema();
                    m_XLSPackage.Save();
                }
                else
                {
                    m_XLSPackage = new ExcelPackage(newFile);
                    m_WorkSheet = m_XLSPackage.Workbook.Worksheets["Products"];
                }
                retVal = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return retVal;
        }

        private void addXLSSchema()
        {
            m_WorkSheet.Cells["A1"].Value = "Name";
            m_WorkSheet.Cells["B1"].Value = "ID";
            m_WorkSheet.Cells["C1"].Value = "URL";
            m_WorkSheet.Cells["D1"].Value = "Price";
        }

        private void addDummyProduct()
        {
            m_WorkSheet.Cells["A2"].Value = "CD Manele";
            m_WorkSheet.Cells["B2"].Value = "IDX123";
            m_WorkSheet.Cells["C2"].Value = "www.indulap.ro/test2";
            m_WorkSheet.Cells["D2"].Value = "55";
        }

        public eProductStat pushProduct(sProduct newProduct)
        {
            eProductStat retVal = eProductStat.PRODUCT_NOACTION;

            ExcelAddress productAddress = getProductById(newProduct.Id);

            int priceOffset = 2;//id pos 2 price pos 4 then offset 2

            for (priceOffset = 2; priceOffset < 15; priceOffset++)
            {
                if (m_WorkSheet.Cells[productAddress.Address].Offset(0, priceOffset).GetValue<string>() == null)
                {
                    break;
                }
                else 
                {
                    Console.WriteLine("Price {0} for {1}", productAddress.Address, m_WorkSheet.Cells[productAddress.Address].Offset(0, priceOffset).GetValue<string>());
                }
            }

            priceOffset -= 1;


            if (null != productAddress)
            {
            //    if (newProduct.Price != productRow.['price'])
            //    {
            //        retVal = eProductStat.PRODUCT_UPDATE_OK;
            //        if(true != updateProductPrice(newProduct))
            //        {
            //            retVal = eProductStat.PRODUCT_UPDATE_ERR;
            //        }
            //    }
            }
            else 
            {
            //    retVal = eProductStat.PRODUCT_INSERT_OK;
            //    if(true != insertNewProduct(newProduct))
            //    {
            //        retVal = eProductStat.PRODUCT_INSERT_ERR;
            //    }
            }

            return retVal;
        }

        private ExcelAddress getProductById(string productId)
        {
            ExcelAddress retVal = new ExcelAddress("A1");
            try
            {
                var pQuery = (from cell in m_WorkSheet.Cells["b:b"]
                              where cell.Value is string &&
                              (string)cell.GetValue<string>() == productId
                              select cell);
                foreach (var cell in pQuery)
                {
                    retVal = cell; 
                    Console.WriteLine("Cell {0} has value {1} Name is {2}", cell.Address, cell.Value, cell.Offset(0, -1).GetValue<string>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return retVal;
        }
    }
}
