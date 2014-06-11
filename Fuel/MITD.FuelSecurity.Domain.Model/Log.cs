using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class Log
    {
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public DateTime LogDate { get; private set; }
        public string ClassName { get; private set; }
        public string MethodName { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public LogLevel LogLevel { get; private set; }

        public Log(int id, string userName, DateTime logDate, string className,
            string methodName, string title, string message, LogLevel logLevel)
        {
             
        }
    }
}