<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BaseBallEspnMlb.ascx.cs"
    Inherits="widgets_BaseBallEspnMlb" %>
<div style="padding: 12px;">
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">

            <object id="<%= ClientID %>_mlb" type="application/x-shockwave-flash"
                    data="http://widgets.clearspring.com/<%=TeamID%>"
                    name="flashobj" width="400" height="387">
                <param name="movie" value="http://widgets.clearspring.com/<%=TeamID%>" />
                <param name="wmode" value="transparent" />
                <param name="allowNetworking" value="all" />
                <param name="allowScriptAccess" value="always" />
                <embed src="http://widgets.clearspring.com/<%=TeamID%>"
                       name="flashObj"
                       width="100%"
                       height="387"
                       type="application/x-shockwave-flash"
                       wmode="transparent"
                       pluginspage="http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash">
                </embed>
            </object>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit">
                <asp:DropDownList ID="TeamSelectDropDownList" runat="server">
                    <asp:ListItem Value="o/472a171df0a00ab5/49f43685931caf90/472a171df0a00ab5/b0a858f1/-cpid/ffbd769d2e14f40">Los Angeles Angels</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f436d9805a39cf/472a171df0a00ab5/67cdc132/-cpid/ffbd769673f92c9">Colorado Rockies</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f571db660d018b/472a171df0a00ab5/f6f13a6/-cpid/ffbd7699e3a897e">Detroit Tigers</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f57ccdcdc40032/472a171df0a00ab5/1ac6d488/-cpid/ffbd769818f2d16">Florida Marlins</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f57d642f597d98/472a171df0a00ab5/64494616/-cpid/ffbd7698df8a46e">San Francisco Giants</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f57eb8e91a5e5d/472a171df0a00ab5/d675f75c/-cpid/ffbd7695ae7eb0">Philadelphia Phillies</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f57f3d0e65000c/472a171df0a00ab5/d85c375c/-cpid/ffbd769fa1d6798">Cincinnati Reds</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5cd99f2e47ad9/472a171df0a00ab5/dac1627a/-cpid/ffbd769cc3e66d1">Washington Nationals</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5ce4023bac081/472a171df0a00ab5/3d51fcff/-cpid/ffbd769543a6b56">Seattle Mariners</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5ce9c2acfbf54/472a171df0a00ab5/95a3259e/-cpid/ffbd769ede27b1c">Texas Rangers</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5d874b13f9255/472a171df0a00ab5/ae2928d7/-cpid/ffbd769ae84c08e">St. Louis Cardinals</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5d8bbc0d24cfa/472a171df0a00ab5/23c9056d/-cpid/ffbd769a545f2f9">Chicago Cubs </asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5d8f659d8a440/472a171df0a00ab5/586b15f0/-cpid/ffbd769da23ab7e">Atlanta Braves</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5d940fbb8d608/472a171df0a00ab5/f52714dc/-cpid/ffbd76963ee70fd">Houston Astros</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dbe10e5f3863/472a171df0a00ab5/9e4d2021/-cpid/ffbd769962e3d75">Pittsburgh Pirates </asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dc1eca1c58f3/472a171df0a00ab5/84c96ddf/-cpid/ffbd769e4c2be60">Los Angeles Dodgers</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dc50ad4512a5/472a171df0a00ab5/887417bb/-cpid/ffbd769bacac5c3">New York Yankees</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dc98dcea9032/472a171df0a00ab5/a096795f/-cpid/ffbd7694ad4eae0">Tampa Bay Rays</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dcc8dc6a8ec0/472a171df0a00ab5/b2513e3e/-cpid/ffbd769f98e47c5">Kansas City Royals</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dcf9d06f1983/472a171df0a00ab5/24b76646/-cpid/ffbd769221e04e6">Boston Red Sox</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dd4fb738b142/472a171df0a00ab5/c306af82/-cpid/ffbd7691a2b6e44">Minnesota Twins</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5dd8b37f6af5a/472a171df0a00ab5/fa8dc68b/-cpid/ffbd76919983250">Toronto Blue Jays</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5ddb6e403ee8a/472a171df0a00ab5/b85d7171/-cpid/ffbd76981c10796">Chicago White Sox</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5ddd815fb071b/472a171df0a00ab5/b967d812/-cpid/ffbd769aabc832e">San Diego Padres</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5de19a92bc5b4/472a171df0a00ab5/e9be1647/-cpid/ffbd769baa14b6f">Arizona Diamondbacks</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5de46a3382735/472a171df0a00ab5/b7e2f398/-cpid/ffbd769ffbd769">Cleveland Indians</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5de6dc1beeee6/472a171df0a00ab5/1217bd73/-cpid/ffbd76926131ccf">New York Mets</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5de994dc85dad/472a171df0a00ab5/7b4bd18f/-cpid/ffbd769e1306ecb">Baltimore Orioles</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5decf15b68206/472a171df0a00ab5/ef661a13/-cpid/ffbd769bf98a303">Oakland Athletics</asp:ListItem>
                    <asp:ListItem Value="o/472a171df0a00ab5/49f5def004dc7dc4/472a171df0a00ab5/d93ff387/-cpid/ffbd7698e070738">Milwaukee Brewers</asp:ListItem>

                </asp:DropDownList>
                <br />
                <br />
                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                &nbsp;&nbsp;
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
            </div>
        </asp:View>
    </asp:MultiView>
</div>
