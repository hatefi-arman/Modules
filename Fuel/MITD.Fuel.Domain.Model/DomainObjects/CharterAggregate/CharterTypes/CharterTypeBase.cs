﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate.CharterTypes
{
    public class CharterTypeBase<T> 
    {
         internal virtual void Add(T charter)
        {
        }

         internal virtual void Update(T charter)
        {
        }


    }
}
