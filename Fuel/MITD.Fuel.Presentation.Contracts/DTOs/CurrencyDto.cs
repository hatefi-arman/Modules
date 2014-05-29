﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Contracts.DTOs
{
   public partial class CurrencyDto
    {
        private long id;
        private string abbreviation;
        private string name;
       private decimal currencyToMainCurrencyRate;


       public long Id
        {
            get { return id; }
            set { this.SetField(p => p.Id, ref id, value); }
        }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        public string Abbreviation
        {
            get { return abbreviation; }
            set { this.SetField(p => p.Abbreviation, ref abbreviation, value); }
        }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Code is required")]
        public string Name
        {
            get { return name; }
            set { this.SetField(p => p.Name, ref name, value); }
        }

        public decimal CurrencyToMainCurrencyRate
        {
            get { return currencyToMainCurrencyRate; }
            set { this.SetField(p => p.CurrencyToMainCurrencyRate, ref currencyToMainCurrencyRate, value); }
        }

    }
}
