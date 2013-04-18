<%@ Page Language="C#" AutoEventWireup="true" CodeFile="devicemodelselection.aspx.cs" Inherits="Workarea_devicemodelselection" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
       <script type="text/javascript">
        function SetDeviceModel(name)
        {
          parent.OnDeviceClicked(name);
        }  
        
         Ektron.ready(function ()
          {
                $ektron("div.DVresult a").cluetip({ positionBy: "bottomTop", cursor: 'pointer', waitImage:true, leftOffset: "25px", topOffset: "20px", cluezIndex: 3500 });
                $ektron("tr.pageRow td").attr("style", "width: 35px");
                $ektron("tr.pageRow td span").addClass("pageingSelected");
                $ektron("tr.pageRow td a").each(function(){
                    $ektron(this).addClass("pageingMarkUp");
                });
                
                // Checks to see if browser is FF, display the grid as table.
                if(navigator.appName.indexOf("Internet Explorer") == -1)
                {
                    $ektron("#DeviceModelView").attr("style", "display:table !important");
                }
          });
        
    </script>
    <style type="text/css">
        .pageRow {  background-color:#D5E7F4; height:30px; }
        .pageingMarkUp { font-family:Lucida Console; font-weight:bold; }
        .pageingSelected { font-family:Lucida Console; font-weight:bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox  ID="txtSearch" Width="49%"  runat="server" />
        <asp:Button  ID="btnSearch" CssClass="greenHover button buttonSearch" Text="Search"  runat="server" OnClick="btnSearch_Click" />
        <asp:Button  ID="btnClear" CssClass="redHover button buttonClear" Text="Clear"   runat="server" OnClick="btnClear_Click" />
    </div>
    <div class="ektronTopSpace"></div>
    <div class="ektronPageContainer ektronPageGrid" style="overflow:hidden;">
              <asp:GridView ID="DeviceModelView"
                            Width="100%"
                            AllowPaging="true" 
                            PageSize="5" 
                            AllowSorting="true"  
                            runat="server" 
                            cssclass="ektronGrid"
                            AutoGenerateColumns="false"
                            EmptyDataText="Your search did not match any model."  
                            OnPageIndexChanging="DeviceModelView_PageIndexChanging"
                            OnRowDataBound="DeviceModelView_RowDataBound" >
                        <HeaderStyle CssClass="title-header" />
                        <Columns>
                           <asp:TemplateField HeaderText="Model" >
                                <ItemTemplate >
                                  <div class="DVresult">
                                      <a href="#" onclick="SetDeviceModel('<%#DataBinder.Eval(Container.DataItem,"modelName").ToString().Replace("'","")%>');" rel="<%=_ContentApi.ApplicationPath %>controls/DeviceConfiguration/deviceTree.ashx?detail=<%#DataBinder.Eval(Container.DataItem,"userAgent")%>">
                                        <span class="title" ><%#DataBinder.Eval(Container.DataItem,"modelName")%></span>
                                     </a>
                                  </div>
                                   
                                </ItemTemplate>
                           </asp:TemplateField>
                        </Columns>
                        
                        <pagersettings mode="Numeric" position="Bottom" pagebuttoncount="10"/>
                        <pagerstyle CssClass="pageRow" verticalalign="Bottom" horizontalalign="Center"/>
              </asp:GridView>
    </div>
    </form>
</body>
</html>
