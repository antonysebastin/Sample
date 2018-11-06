using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forex.Models
{
    public class ExchangeRate
    {
        public DateTime ExchangeDate { get; set; }

        public string Currency { get; set; }

        public decimal ExchangeValue { get; set; }
    }
}