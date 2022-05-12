using CurrencyRates.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface IApiRepository
    {
        string EuBankCurrency();
        string AlfaCurriency();
        string KaspiCurriency();
        string HalykCurriency();
        string CrossCourse();
        List<CurrencyRateModel> GetCurrencyRatesForProcessing();
        string NationalBankCurriency();
        string DosCredoCurriency();
        string DosCredoCurriencyNal();
        string CBUCurriency();
        string FMFMCurriency();
        string AlifBank();
        string NationalBankTJS();
        string NationalBankMDL();
        string VoltonBank();
        string AgroindbankMDL();
        string PayvandTJS();
        string OptimaBank();
    }
}
