﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Presentation;
namespace MITD.Fuel.Presentation.Contracts.DTOs
{
    public partial class PartyDto
    {
        string partyName;

        public string PartyName
        {
            get { return partyName; }
            set { this.SetField(p => p.PartyName, ref partyName, value); }

        }

        Dictionary<int, bool> customActions = new Dictionary<int, bool>();

        public Dictionary<int, bool> CustomActions
        {
            get { return customActions; }
            set { this.SetField(p => p.CustomActions, ref customActions, value); }
        }

    }
}
