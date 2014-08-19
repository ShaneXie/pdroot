<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="payDevnew._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <script src="Scripts/ngCtrls.js"></script>

    <div ng-app="paymapApp">
    <div ng-controller="paymapCtrl">
        <div class="jumbotron">
            <h1>Pay Map</h1>
            <input ng-model="inKey" type="text" class="form-control col-md-12" id="keyInput" placeholder="Enter Key Word To Search">

            <div class="well well-sm">
                <ul class="list-inline">
                    <li><input ng-model="newTitle" type="text" class="form-control" id="newTitle" placeholder="Enter Title"></li>
                    <li><input ng-model="newURL" type="text" class="form-control" id="newURL" placeholder="Enter relative URL"></li>
                    <li><input ng-model="newPath" type="text" class="form-control" id="newPath" placeholder="Enter Path"></li>
                    <li><input ng-model="newKeys" type="text" class="form-control" id="newKeys" placeholder="Enter Key Words"></li>
                    <li class="pull-right"><button ng-click="save()" type="button" class="btn btn-primary">Add New</button></li>
                </ul>
            </div>
            <span class="label label-success">{{msg}}</span>
        </div>


        <div class="row" id="searchResult">
            <div class="col-md-3 well well-sm pmPanel" ng-repeat="node in paymap | filter:inKey">
                <h4><a target="_blank" href="{{baseUrl+node.url}}">{{node.title}}</a></h4>
                <p>FilePath :<br> <span class="label label-primary">{{node.path}}</span></p>
            </div>
        </div>
    </div>
</div>

</asp:Content>
