<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="MITD.Main.Service.Host.Reports.ReportViewer" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reports Viewer</title>
</head>
<body>
    <form id="MainForm" runat="server">
        <rsweb:ReportViewer ID="MainReportViewer" runat="server" Width="100%" Height="100%"></rsweb:ReportViewer>
        <asp:ScriptManager ID="MainScriptManager" runat="server" ViewStateMode="Enabled" EnableViewState="True"></asp:ScriptManager>
    </form>
</body>
</html>
