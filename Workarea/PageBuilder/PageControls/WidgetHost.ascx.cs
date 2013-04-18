using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using Ektron.Cms;
//using Ektron.Cms.API;
using Ektron.Cms.Content;

using Ektron.Cms.PageBuilder;
using Ektron.Cms.Common;


public partial class WidgetHostCtrl : Ektron.Cms.PageBuilder.WidgetHost
{
    private string appPath = "";
    private bool editing = false;

    protected Ektron.Cms.Common.EkMessageHelper m_refMsg;
    protected EkRequestInformation requestInformation;
    

    #region Constructor

    public WidgetHostCtrl()
    {
        requestInformation = ObjectFactory.GetRequestInfoProvider().GetRequestInformation();
        m_refMsg = new EkMessageHelper(requestInformation);
        appPath = requestInformation.ApplicationPath;
    }

    #endregion

    #region Event Handlers

    protected void Page_Init(object sender, EventArgs e)
    {        

        imgPBclosebutton.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_close.png";
        imgPBeditbutton.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/edit_off.png";
        imgPBexpandbutton.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_expand.png";
        imgPBhelpbutton.Src = appPath + "/PageBuilder/PageControls/" + (Page as PageBuilder).Theme + "images/icon_help.png";
        // provide translations
        lbEditWidget.ToolTip = m_refMsg.GetMessage("generic edit title");
        imgPBeditbutton.Alt = lbEditWidget.ToolTip;
        lbDeleteWidget.ToolTip = m_refMsg.GetMessage("generic delete title");
        imgPBclosebutton.Alt = lbDeleteWidget.ToolTip;
        lbExpandWidget.Title = imgPBexpandbutton.Alt;
        imgPBexpandbutton.Alt = m_refMsg.GetMessage("lbl expand");
        lbExpandWidget.Attributes.Add("title", imgPBexpandbutton.Alt);
        lbHelpWidget.Title = imgPBhelpbutton.Alt;
        imgPBhelpbutton.Alt = m_refMsg.GetMessage("generic help");
        lbHelpWidget.Attributes.Add("title", imgPBhelpbutton.Alt);

        (this as Ektron.Cms.Widget.IWidgetHost).Load += new Ektron.Cms.Widget.LoadDelegate(WidgetHost_Load);
    }

    protected void buttonSetup()
    {
        lbEditWidget.CssClass = "edit";
        lbDeleteWidget.Visible = false;
        lbEditWidget.Visible = false;
        lbExpandWidget.Visible = false;
        lbHelpWidget.Visible = false;

        if (phWidgetContent.Controls.Count > 0 && IsEditable)
        {
            lbEditWidget.Visible = EditSupported();
            lbDeleteWidget.Visible = true;
            lbHelpWidget.Visible = (HelpFile != "");
            if (lbHelpWidget.Visible)
            {
                string path = "";
                if (HelpFile.Contains(requestInformation.SitePath) || HelpFile.StartsWith("http://"))
                {
                    path = HelpFile;
                }
                else
                {
                    path = requestInformation.SitePath + ((HelpFile.Contains("~")) ? path += ResolveClientUrl(HelpFile) : path += HelpFile);
                }
                lbHelpWidget.HRef = path;
                lbHelpWidget.Attributes.Add("onclick", "window.open('" + path + "','widgetHelp','status=1,height=400,width=400,resizable=1,scrollbars=1'); return false;");
            }
            if (ExpandOptions == Ektron.Cms.Widget.Expandable.ExpandOnExpand)
            {
                Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/java/plugins/modal/ektron.modal.css");
                lbExpandWidget.Visible = true;
            }
            if (ExpandOptions == Ektron.Cms.Widget.Expandable.ExpandOnEdit)
            {
                if (editing)
                {
                    Ektron.Cms.Framework.UI.Css.Register(this, appPath + "/java/plugins/modal/ektron.modal.css");
                    lbEditWidget.CssClass += " OpeninModal";
                }
            }
        }
    }

    public void WidgetHost_Load()
    {
        try
        {
            if (base.InternalWidget == null) LoadWidget();
            if (base.InternalWidget != null)
            {
                phWidgetContent.Controls.Clear();
                base.InternalWidget.ID = this.ID + "_widget";
                phWidgetContent.Controls.Add(base.InternalWidget);
                if (IsEditable)
                {
                    toolbar.Attributes.Add("widget-type-id", PBWidgetInfo.ID.ToString());
                }
            }
		    else
            {
                lblErrorMessage.Text = m_refMsg.GetMessage("lbl widget not found");
            }
        }
        catch (Exception ex)
        {
            // Handle widgets that have been deleted or are missing. 
            if (ex.InnerException is HttpException)
            {
                phWidgetContent.Controls.Add(lblErrorMessage);
                lblErrorMessage.Text = m_refMsg.GetMessage("lbl widget not found");
            }
            else
            {
                throw;
            }
        }

        buttonSetup();
        lblTitle.Text = base.Title;
    }

    protected override void OnPreRender(EventArgs e)
    {
        buttonSetup();
        toolbar.Visible = IsEditable;

        base.OnPreRender(e);
        lblTitle.Text = base.Title;
    }

    protected void lbEdit_Click(object sender, EventArgs e)
    {
		this.OnEdit();
    }

	public override void OnEdit()
	{
        lbEditWidget.Attributes.Add("data-ektron-editMode", "true");
        editing = true;
        base.OnEdit();
	}

    protected void lbDelete_Click(object sender, EventArgs e)
    {
        Control tmp = this;
        PageFactory.GetDropZone(this).UpdatePanel.Update();
        base.Delete();
    }

    protected void widgetPanelLoad(object sender, EventArgs e)
    {
        //WidgetHost_Load();
        //toolbar.Attributes.Remove("Deleted");
    }
    #endregion
}
