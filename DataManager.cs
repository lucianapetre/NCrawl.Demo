using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;

namespace NCrawler.Demo
{
    struct sData
    {
        string Name;
        string Id;
        decimal Price;
        sData(string name, string id, decimal price)
        {
            Name = name;
            Id = id;
            Price = price;
        }
    };

    class DataManager
    {
        const string XLSFilePath = "emagProducts.xls";
        private List<sData> m_lProducts = new List<sData>();

        ExcelPackage m_XLSPackage = new ExcelPackage();

        public bool readXLSFile()
        {
            bool retVal = false;

            try
            {
                m_XLSPackage.Load(File.OpenRead(XLSFilePath));
            }
            catch (Exception e)
            {
                Console.WriteLine("File {0} not found", XLSFilePath);
            }
            return retVal;
        }
    }
}
