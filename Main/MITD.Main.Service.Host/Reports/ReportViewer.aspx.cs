using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace MITD.Main.Service.Host.Reports
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportServerUrlAddress = System.Configuration.ConfigurationManager.AppSettings["ReportServerUrl"];
                var reportsRootPathAddress = System.Configuration.ConfigurationManager.AppSettings["ReportsRootPath"];
                var reportPath = reportsRootPathAddress + "/" + Request.QueryString["ItemPath"];

                MainReportViewer.ProcessingMode = ProcessingMode.Remote;
                MainReportViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials("ReportViewer", "MnHoPAXw$");
                MainReportViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrlAddress, UriKind.Absolute);
                MainReportViewer.ServerReport.ReportPath = reportPath;
                MainReportViewer.ServerReport.SetParameters(new ReportParameter("User", "User"));
                MainReportViewer.ServerReport.Timeout = 10000;
                MainReportViewer.ServerReport.Refresh();
            }
        }
    }

    public class CustomReportCredentials : IReportServerCredentials
    {
        private string _UserName;
        private string _PassWord;
        private string _DomainName;


        public CustomReportCredentials(string UserName, string PassWord)
        {
            _UserName = UserName;
            _PassWord = PassWord;
        }

        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName;
        }


        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get { return string.IsNullOrWhiteSpace(_DomainName) ? new NetworkCredential(_UserName, _PassWord) : new NetworkCredential(_UserName, _PassWord, _DomainName); }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user,
         out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}