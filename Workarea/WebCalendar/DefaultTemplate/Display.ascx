<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Display.ascx.cs" Inherits="Workarea_WebCalendar_DefaultTemplate_Display" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Label ToolTip="Title" ID="title" runat="server"></asp:Label>
<telerik:RadToolTip ID="description"
                    runat="server" 
                    Animation="None" 
                    HideEvent="Default" 
                    Position="BottomCenter" 
                    ShowEvent="OnMouseOver" 
                    ManualClose="false" 
                    Sticky="true" 
                    VisibleOnPageLoad="false" />
