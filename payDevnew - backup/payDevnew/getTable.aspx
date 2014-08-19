<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getTable.aspx.cs" Inherits="payDevnew.getTable" %>


<%
    string theName = Request.QueryString["name"];
    string anyQuery = "";
    string query = "";
    if (Request.QueryString["name"] != null)
    {
        if (theName == "all")
        {
            query = "select SERVERPROPERTY('ServerName') As Server, DB_NAME() As CustomerID, EI.EmployeeNumber, EI.LastName, EI.FirstName from EmployeeIdentification EI";
            queryResult(query);
        }
        else
        {
            query = "select SERVERPROPERTY('ServerName') As Server, DB_NAME() As CustomerID, EI.EmployeeNumber, EI.LastName, EI.FirstName from EmployeeIdentification EI where LastName = '" + theName + "' or FirstName = '" + theName + "'";
            queryResult(query);
        }
    }
    if (Request.QueryString["query"] != null)
    {
        anyQuery = Request.QueryString["query"];
        queryResult(query);
    }
    
    
    
%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
