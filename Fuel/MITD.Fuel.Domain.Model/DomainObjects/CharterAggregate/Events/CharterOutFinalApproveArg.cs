﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;

namespace MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate.Events
{
    public class CharterOutFinalApproveArg
    {
        public CharterOut SendObject { get; set; }
    }
}
