﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Presentation.Contracts.DTOs.Security
{
    public partial class PartyDTOwithActions : PartyDto
    {
        public List<int> ActionCodes { get; set; }

    }
}
