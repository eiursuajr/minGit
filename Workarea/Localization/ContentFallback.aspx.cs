using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Ektron;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Workarea;
using Ektron.Cms.Localization;
using Ektron.Cms.Framework.Localization;
using Ektron.Cms.API;

public partial class Localization_Content_Fallback : workareabase
{
    Ektron.Cms.BusinessObjects.Localization.ILocaleContent contentLocaleManager = ObjectFactory.GetLocaleContent();
    Ektron.Cms.BusinessObjects.Localization.ILocaleManager localeManager = null;
    long folderId = 0;

#region Page Functions


    protected override void OnLoad(EventArgs e)
    {
        localeManager = ObjectFactory.GetLocaleManager(contentLocaleManager.RequestInformation);

        RegisterCSS();
        RegisterJS();
        if (Request.QueryString["id"] != null)
        {
            long.TryParse(Request.QueryString["id"], out m_iID);
        }
        if (Request.QueryString["folder_id"] != null)
        {
            long.TryParse(Request.QueryString["folder_id"], out folderId);
        }
        try
        {
            if (!Utilities.ValidateUserLogin())
                return;

            switch (m_sPageAction)
            {
                default :
                    Reorder1.Initialize(contentLocaleManager.RequestInformation);
                    if (Page.IsPostBack)
                        Process_Reorder();
                    else 
                        Display_Reorder();
                    break;

            }

            Util_SetLabels();

        }
        catch (Exception ex)
        {
            Utilities.ShowError(ex.Message);
        }

    }
#endregion

#region Process

    protected void Process_Reorder()
    {
        if (chkGlobalFallback.Checked)
        {
            contentLocaleManager.DisableContentFallbackLocales(m_iID);
        }
        else
        {
            List<int> idList = new List<int>();
            string[] itemList = Request.Form["LinkOrder"].ToString().Split(',');

            for (int i = 0; i < itemList.Length; i++)
            {
                string[] itemArray = itemList[i].Split('|');
                int localeId = 0;
                if (int.TryParse(itemArray[0], out localeId))
                {
                    idList.Add(localeId);
                }
            }

            contentLocaleManager.SetContentFallbackLocales(m_iID, idList);
        }
        Response.Redirect("../content.aspx?action=View&folder_id=" + folderId.ToString() + "&id=" + m_iID.ToString(), false);
    }
#endregion

#region Display
    protected void Display_Reorder()
    {
        List<int> contentFallbackLocales = contentLocaleManager.GetContentFallbackLocales(m_iID);
        List<LocaleData> systemEnabledLocales = localeManager.GetEnabledLocales();

        List<LocaleData> contentLocales = new List<LocaleData>();
        List<LocaleData> nonUsedLocales = new List<LocaleData>();

        nonUsedLocales.AddRange(systemEnabledLocales);

        if (contentFallbackLocales.Count > 0)
        {
            for (int i = 0; i < contentFallbackLocales.Count; i++)
            {
                if (contentFallbackLocales[i] == 0)
                {
                    contentLocales.Add(new LocaleData(0, 0, "-", "-", true, "", "", "", "", "", "", "", 0, LanguageState.Active));
                }
                else
                {
                    for (int j = 0; j < systemEnabledLocales.Count; j++)
                    {
                        if (contentFallbackLocales[i] == systemEnabledLocales[j].Id)
                        {
                            contentLocales.Add(systemEnabledLocales[j]);
                            for (int k = (nonUsedLocales.Count - 1); k >= 0; k--)
                            {
                                if (nonUsedLocales[k].Id == systemEnabledLocales[j].Id)
                                {
                                    nonUsedLocales.RemoveAt(k);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            chkGlobalFallback.Checked = true;
            Reorder1.SelectEnabled = false;
            lstLocales.Enabled = false;
            //Attributes.Add("disabled", "disabled");
        }

        for (int i = 0; i < contentLocales.Count; i++)
        {
            Reorder1.AddItem(contentLocales[i].NativeName + " [" + contentLocales[i].EnglishName + "]", contentLocales[i].Id, 0);
        }

        for (int i = 0; i < nonUsedLocales.Count; i++)
        {
            nonUsedLocales[i].FlagFile = nonUsedLocales[i].NativeName + " [" + nonUsedLocales[i].EnglishName + "]";
        }

        lstLocales.DataSource = nonUsedLocales;
        lstLocales.DataTextField = "FlagFile";
        lstLocales.DataValueField = "Id";
        lstLocales.DataBind();

    }
#endregion

#region Private Helpers
    protected void Util_SetLabels()
    {
        switch (m_sPageAction)
        {
            default:
                workareamenu actionMenu = new workareamenu("action", this.GetMessage("lbl action"), this.AppImgPath + "../UI/Icons/check.png");
                actionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/save.png", this.GetMessage("btn save"), "document.forms[0].submit();");
                actionMenu.AddBreak();
                actionMenu.AddItem(m_refContentApi.AppPath + "images/ui/icons/back.png", this.GetMessage("generic cancel"), "window.location='../content.aspx?action=View&folder_id=" + folderId.ToString() + "&id=" + m_iID.ToString() + "';");
                this.AddMenu(actionMenu);

                SetTitleBarToString("Content fallback locales");
                AddHelpButton("ContentFallBackLocales");
                break;

        }

        lstLocales.Attributes.Add("style", "margin: 0pt; padding: 0pt; width: 100%;");
    }
    private void RegisterCSS()
    {

        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronModalCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.LessThanEqualToIE7);
        Ektron.Cms.API.Css.RegisterCss(this, this.m_refContentApi.ApplicationPath + "/wamenu/css/com.ektron.ui.menu.css", "EktronMenuCss");
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);

    }

    private void RegisterJS()
    {

        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronJS);
        Ektron.Cms.API.JS.RegisterJS(this, Ektron.Cms.API.JS.ManagedScript.EktronWorkareaJS);
        Ektron.Cms.API.JS.RegisterJS(this, this.m_refContentApi.ApplicationPath + "/wamenu/includes/com.ektron.ui.menu.js", "EktronMenuJs");

    }

#endregion

}
