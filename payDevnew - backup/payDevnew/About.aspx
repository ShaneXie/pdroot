<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="payDevnew.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-header">
      <h1>Tree Branch Project <small>Still Developing...</small></h1>
    </div>
    <p>Idea: Run one query on ALL Database from each SQL server at SAME TIME</p>

    <div class="col-lg-4">
    <div class="input-group">
      <input type="text" class="form-control" id='inName' placeholder="Search Employee">
      <span class="input-group-btn">
        <button type="button" class="btn btn-info" id="searchByName">Search Employee</button>
      </span>
    </div>
    </div>
    <button type="button" class="btn btn-info" id="showall">Show all Employees</button>

    <br />
    <br />
    <div class="col-lg-4">
        <div class="input-group">
          <input type="text" class="form-control" id='txtQuery' placeholder="Enter some Query">
          <span class="input-group-btn">
            <button type="button" class="btn btn-info" id="go">GO!</button>
          </span>
        </div>
    </div>
    <hr />
    <div id="searchResult">
    <!-- quertResult("select SERVERPROPERTY('ServerName') As Server, DB_NAME() As CustomerID, EI.EmployeeNumber, EI.LastName, EI.FirstName from EmployeeIdentification EI");-->
    </div>
    <script>
        $("#showall").click(function () {
            $.ajax({
                url: "/getTable.aspx?name=all", success: function (result) {
                    $("#searchResult").html(result);
                }
            });
        });

        $("#searchByName").click(function () {

            $.ajax({
                url: "/getTable.aspx?name=" + $("#inName").val(), success: function (result) {
                    $("#searchResult").html(result);
                }
            });
        });

        $("#go").click(function () {

            $.ajax({
                url: "/getTable.aspx?query=" + $("#txtQuery").val(), success: function (result) {
                    $("#searchResult").html(result);
                }
            });
        });

    </script>
   
</asp:Content>
