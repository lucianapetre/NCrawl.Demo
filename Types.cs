using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCrawler.Demo
{
    enum eProductStat
    {
        PRODUCT_NOACTION,
        PRODUCT_INSERT_ERR,
        PRODUCT_UPDATE_ERR,
        PRODUCT_UPDATE_OK,
        PRODUCT_INSERT_OK
    }

    struct sProduct
    {
        public string Name;
        public string Id;
        public string URL;
        public decimal Price;

        public sProduct(string name, string id, string url, decimal price)
        {
            Name = name;
            Id = id;
            URL = url;
            Price = price;
        }
    };
}
