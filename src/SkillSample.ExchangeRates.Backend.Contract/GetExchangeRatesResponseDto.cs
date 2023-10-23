using System;
using System.Collections.Generic;

namespace SkillSample.ExchangeRates.Backend.Contract
{
    public class GetExchangeRatesResponseDto
    {
        public DateTime EffectiveDate { get; set; }
        public string TableNumber { get; set; }
        public IEnumerable<CurrencyEntry> CurrencyRates { get; set; }

        public class CurrencyEntry
        {
            public string CurrencyCode { get; set; }
            public string CurrencyName { get; set; }
            public decimal Rate { get; set; }
        }
    }
}
