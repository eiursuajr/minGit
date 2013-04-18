using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.Device;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Workarea;
using Microsoft.Security.Application;
using Microsoft.VisualBasic;

public partial class template_config : workareabase
{
    protected string m_strStyleSheetJS = "";
    protected Ektron.Cms.SiteAPI m_siteApi = new SiteAPI();
    protected Ektron.Cms.Content.EkContent m_refContent;
    protected Ektron.Cms.PageBuilder.TemplateModel m_templateModel = new Ektron.Cms.PageBuilder.TemplateModel();
    protected CmsDeviceConfigurationCriteria criteria = new CmsDeviceConfigurationCriteria();
    protected List<CmsDeviceConfigurationData> dList = new List<CmsDeviceConfigurationData>();
    protected CmsDeviceConfiguration cDevice;
    protected long chkID;

    protected override void Page_Load(object sender, System.EventArgs e)
    {
        base.Page_Load(sender, e);

        cDevice = new CmsDeviceConfiguration(m_refContentApi.RequestInformationRef);

        Packages.EktronCoreJS.Register(this);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaCss);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronWorkareaIeCss, Ektron.Cms.API.Css.BrowserTarget.AllIE);
        Ektron.Cms.API.Css.RegisterCss(this, Ektron.Cms.API.Css.ManagedStyleSheet.EktronFixedPositionToolbarCss);
        Ektron.Cms.API.Css.RegisterCss(this, "csslib/ektron.widgets.selector.css", "EktronWidgetsSelectorCss");

        // add resource text values as needed
        selectTemplate.Text = m_refMsg.GetMessage("lbl pagebuilder select template");
        lblTemplateFile.Text = m_refMsg.GetMessage("lbl pagebuilder template file");
        lblSelectWidgets.Text = m_refMsg.GetMessage("lbl pagebuilder select widgets");
        ltrTemplateMessage.Text = m_refMsg.GetMessage("js template msg");
        Page.Title = (string)(m_refMsg.GetMessage("generic select template"));
        widgetTitle.Text = m_refMsg.GetMessage("lbl pagebuilder widgets title");
        btnSelectNone.Text = m_refMsg.GetMessage("lbl pagebuilder select none");
        btnSelectNone.ToolTip = btnSelectNone.Text;
        btnSelectAll.Text = m_refMsg.GetMessage("lbl pagebuilder select all");
        btnSelectAll.ToolTip = btnSelectAll.Text;
        cbPageBuilderTemplate.Text = m_refMsg.GetMessage("lbl pagebuilder wireframe");
        m_refContent = m_refContentApi.EkContentRef;
        closeDialogLink.ToolTip = m_refMsg.GetMessage("close title");
        try
        {
            if (!Utilities.ValidateUserLogin())
            {
                return;
            }
            if (m_refContentApi.UserId == 0 || m_refContentApi.RequestInformationRef.IsMembershipUser > 0)
            {
                Response.Redirect("login.aspx?fromLnkPg=1", false);
                return;
            }
            m_refMsg = m_refContentApi.EkMsgRef;
            if (!m_refContentApi.IsAdmin())
            {
                if (!m_refContent.IsARoleMember((long)Ektron.Cms.Common.EkEnumeration.CmsRoleIds.TemplateConfigurations, m_refContent.RequestInformation.UserId, false))
                {
                    Utilities.ShowError((string)(m_refMsg.GetMessage("com: user does not have permission")));
                    return;
                }
            }
            string view = "list";
            if (!String.IsNullOrEmpty(Request.QueryString["view"]))
            {
                view = (string)(Request.QueryString["view"].ToLower());
            }

            if (view != "list" && view != "update" && view != "delete" && view != "add")
            {
                view = "list";
            }

            if (view == "list")
            {
                ViewTemplateList();
            }
            else if (view == "update")
            {
                UpdateTemplate();
            }
            else if (view == "add")
            {
                AddTemplate();
            }
            else if (view == "delete")
            {
                DeleteTemplate();
            }
        }
        catch (Exception ex)
        {
            EkException.ThrowException(ex);
        }
    }

    private void AddTemplate()
    {
        SetTitleBarToMessage("lbl add new template");

        criteria.AddFilter(Ektron.Cms.Device.CmsDeviceConfigurationProperty.Id, CriteriaFilterOperator.GreaterThanOrEqualTo, "0");
        dList = cDevice.GetList(criteria);

        if (Request.Form["addTemplate"] == null)
        {

            sitePathValue.Text = "<td class=\"nowrap\">" + m_refContentApi.SitePath + "</td><td><input type=\"text\" id=\"addTemplate\" maxlength=\"180\" onkeypress=\"return disableEnterKey(event)\" class=\"ektronInputTextMedium\" name=\"addTemplate\" value=\"\"/></td>";
            ltrTemplateDetails.Text = "<tr><td>" + m_refMsg.GetMessage("lbl template name") + ":</td><td></td><td><input id=\"templateName\" title=\"" + m_refMsg.GetMessage("lbl template name") + "\" maxlength=\"50\" name=\"templateName\" type=\"text\" value=\"\"/></td></tr>" +
                "<tr><td>" + m_refMsg.GetMessage("lbl template description") + ":</td><td></td><td><input id=\"templateDescription\" title=\"" + m_refMsg.GetMessage("lbl template description") + "\" maxlength=\"200\" name=\"templateDescription\" type=\"text\" value=\"\"/></td></tr>";

            pnlPageBuilder.Visible = true;

            Ektron.Cms.Widget.WidgetTypeController.SyncWidgetsDirectory(m_refContentApi.RequestInformationRef.WidgetsPath);

            Ektron.Cms.Widget.WidgetTypeModel model = new Ektron.Cms.Widget.WidgetTypeModel();
            Ektron.Cms.Widget.WidgetTypeData[] widgetTypes = model.FindAll();
            repWidgetTypes.DataSource = widgetTypes;
            repWidgetTypes.DataBind();

            string valDeviceInputs = "";
            if ((m_refContentApi.RequestInformationRef.IsDeviceDetectionEnabled))
            {
                StringBuilder sb = new StringBuilder();
                if ((dList.Count > 0))
                {
                    Int32 i = 0;
                    sb.Append("<tr>").Append(Environment.NewLine);
                    sb.Append("     <td >").Append(Environment.NewLine);
                    sb.Append("         <br/><label class=\"deviceheader\" for=\"lblDeviceConfiguration\">").Append("Device Configurations").Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("</tr>").Append(Environment.NewLine);

                    foreach (CmsDeviceConfigurationData dItem in dList)
                    {
                        if (dItem.Name != "Generic")
                        {
                            sb.Append("<tr>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"devicelabel\" >").Append(Environment.NewLine);
                            sb.Append("         <input type=\"checkbox\" class=\"pageBuilderCheckbox clearfix\" name=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" id=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" />").Append(Environment.NewLine);
                            sb.Append("         <label for=\"addTemplateDevice_").Append(dItem.Id).Append("\">").Append(dItem.Name).Append("</label>").Append(Environment.NewLine);
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"nowrap\">").Append(Environment.NewLine);
                            sb.Append(m_refContentApi.SitePath).Append(Environment.NewLine);
                            sb.Append("     </td> ").Append(Environment.NewLine);
                            sb.Append("     <td> ").Append(Environment.NewLine);
                            sb.Append("          <input  type=\"text\" id=\"updateDeviceTemplate_").Append(dItem.Id).Append("\"  maxlength=\"180\" class=\"ektronInputTextMedium\" name=\"updateDeviceTemplate_").Append(dItem.Id).Append("\" value=\"\" />");
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"value\">").Append(Environment.NewLine);
                            sb.Append("         <input type=\"button\" id=\"btnDeviceTemplate_").Append(dItem.Id).Append("\" value=\"...\" class=\"ektronModal browseButton\" onclick=\"SetBtnClicked(").Append(dItem.Id).Append(");OnBrowseButtonClicked()\" />").Append(Environment.NewLine);
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("</tr>").Append(Environment.NewLine);
                            if (i > 0)
                            {
                                valDeviceInputs += "," + dItem.Id;
                            }
                            else
                            {
                                valDeviceInputs += dItem.Id;
                            }
                            i = i + 1;
                        }
                    }
                    ltrDeviceConfigurations.Text = sb.ToString();
                }
            }
            AddButton(m_refContentApi.AppPath + "images/UI/Icons/cancel.png", "javascript:CancelAddTemplate();", m_refMsg.GetMessage("btn cancel"), m_refMsg.GetMessage("btn cancel"), "", StyleHelper.CancelButtonCssClass, true);
            AddButton(m_refContentApi.AppPath + "images/UI/Icons/save.png", "javascript:ConfirmNotEmpty('addTemplate','" + valDeviceInputs + "');", m_refMsg.GetMessage("lbl Add New Template to System"), m_refMsg.GetMessage("lbl Add New Template to System"), "", StyleHelper.SaveButtonCssClass, true);
            AddHelpButton("template_add");

        }
        else
        {
            long template_id = 0;
            string strThumbnail = "";
            string strLocation = "";
            if (Request.Form["addtemplate"].IndexOf("?") != -1)
            {
                strLocation = (string)(Request.Form["addTemplate"].Substring(0, System.Convert.ToInt32(Request.Form["addtemplate"].IndexOf("?"))).ToString());
            }
            else
            {
                strLocation = (string)(Request.Form["addTemplate"].ToString());
            }
            strThumbnail = (string)(m_templateModel.GenerateThumbnail(strLocation));

            if (cbPageBuilderTemplate.Checked)
            {
                Ektron.Cms.PageBuilder.WireframeModel model = new Ektron.Cms.PageBuilder.WireframeModel();
                Ektron.Cms.PageBuilder.WireframeData wireframe = model.Create(Request.Form["addTemplate"].ToString(), System.IO.Path.GetFileName(strThumbnail), AntiXss.HtmlEncode(Request.Form["templateDescription"].ToString()), AntiXss.HtmlEncode(Request.Form["templateName"].ToString()));
                if (wireframe.ID == 0)
                {
                    Utilities.ShowError((string)(m_refMsg.GetMessage("msg template aleady exists")));
                    return;
                }
                foreach (string Key in Request.Form.AllKeys)
                {
                    if (Key.StartsWith("widget"))
                    {
                        try
                        {
                            model.AddWidgetTypeAssociation(wireframe.ID, long.Parse(Key.Substring(6)));
                        }
                        catch (Exception ex)
                        {
                            EkException.ThrowException(ex);
                        }
                    }
                }

                template_id = wireframe.Template.Id;
            }
            else
            {
                Collection newtemplatedata = new Collection();
                newtemplatedata.Add(Request.Form["addTemplate"].ToString(), "TemplateFileName", null, null);
                newtemplatedata.Add(AntiXss.HtmlEncode(Request.Form["templateName"].ToString()), "TemplateName", null, null);
                newtemplatedata.Add(AntiXss.HtmlEncode(Request.Form["templateDescription"].ToString()), "Description", null, null);
                template_id = m_refContentApi.EkContentRef.AddTemplatev2_0(newtemplatedata);
                if (template_id == 0)
                {
                    Utilities.ShowError((string)(m_refMsg.GetMessage("msg template aleady exists")));
                    return;
                }
            }
            if (m_refContentApi.RequestInformationRef.IsDeviceDetectionEnabled)
            {
                SaveDeviceTemplates(dList, template_id, "add");
            }
            ClientScript.RegisterClientScriptBlock(this.GetType(), "windowCloseScript", "AddTemplateEntry(" + template_id + ", \'" + (cbPageBuilderTemplate.Checked ? Request.Form["addTemplate"].ToString().Replace("\'", "\\\'") + " (Wireframe Template)" : Request.Form["addTemplate"].ToString().Replace("\'", "\\\'")) + "\');", true);
            //ClientScript.RegisterClientScriptBlock(this.GetType(), "windowCloseScript", "AddTemplateEntry(" + template_id + ", \'" + Request.Form["addTemplate"].ToString().Replace("\'", "\\\'") + "\');", true);
        }
    }

    private void ViewTemplateList()
    {
        SetTitleBarToMessage("lbl active templates");
        AddButton((string)(m_refContentApi.AppPath + "images/UI/Icons/add.png"), "javascript:OpenAddDialog()", (string)(m_refMsg.GetMessage("lbl Add New Template to System")), (string)(m_refMsg.GetMessage("lbl Add New Template to System")), "", StyleHelper.AddButtonCssClass, true);
        AddHelpButton("template_viewlist");
        TemplateData[] template_data;
        template_data = m_refContentApi.GetAllTemplates("TemplateFileName");
        StringBuilder str = new StringBuilder();
        int i = 0;
        string linkBeginTag;

        str.Append("<div class=\'ektronPageContainer\'>");
        str.Append("<table class=\"ektronGrid\" width=\"100%\">");
        str.Append("<tbody>");
        str.Append("<tr class=\"title-header\">");
        str.Append("<th>" + m_refMsg.GetMessage("generic id") + "</th>");
        str.Append("<th>" + m_refMsg.GetMessage("lbl template name") + "</th>");
        str.Append("<th>" + m_refMsg.GetMessage("template label") + "</th>");
        str.Append("<th>" + m_refMsg.GetMessage("lbl template description") + "</th>");
        str.Append("<th></th>");
        str.Append("</tr>");

        for (i = 0; i <= template_data.Length - 1; i++)
        {
            linkBeginTag = "<a href=\"template_config.aspx?view=update&id=" + template_data[i].Id + "\">";
            str.Append("<tr>");
            str.Append("<td>" + template_data[i].Id + "</td>");
            str.Append("<td>").Append(!string.IsNullOrEmpty(template_data[i].TemplateName) ? template_data[i].TemplateName : System.IO.Path.GetFileNameWithoutExtension(template_data[i].FileName)).Append("</td>");
            str.Append("<td>");
            str.Append(linkBeginTag);
            if (template_data[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.Wireframes)
            {
                str.Append("<img alt=\"Content Wireframe Template\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentWireframeTemplate.png\"/></a>&nbsp;" + linkBeginTag + template_data[i].FileName);
                str.Append(" (" + m_refMsg.GetMessage("lbl pagebuilder wireframe template") + ")");
            }
            else if (template_data[i].SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
            {
                str.Append("<img alt=\"Content Master Layout Template\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentWireframeTemplate.png\"/></a>&nbsp;" + linkBeginTag + template_data[i].FileName);
                str.Append(" (" + m_refMsg.GetMessage("lbl Master Layout") + ")");
            }
            else
            {
                str.Append("<img alt=\"Content Template\" src=\"" + m_refContentApi.AppPath + "images/ui/icons/contentTemplate.png\"/></a>&nbsp;" + linkBeginTag + template_data[i].FileName);
            }
            str.Append("</a>");
            str.Append("</td>");
            str.Append("<td>" + template_data[i].Description + "</td>");
            str.Append("<td><a href=\"template_config.aspx?view=delete&id=" + template_data[i].Id + "\">" + m_refMsg.GetMessage("btn Delete") + "</a></td>");
            str.Append("</tr>");
        }
        str.Append("</tbody></table>");
        str.Append("</div>");
        MainBody.Text = str.ToString();
    }

    private void UpdateTemplate()
    {
        long template_id = System.Convert.ToInt64(Request.QueryString["id"]);
        SetTitleBarToMessage("lbl update template");
        criteria.AddFilter(Ektron.Cms.Device.CmsDeviceConfigurationProperty.Id, CriteriaFilterOperator.GreaterThanOrEqualTo, "0");
        dList = cDevice.GetList(criteria);
        
        if (Request.Form["updateTemplate"] == null)
        {
            TemplateData templateData = m_refContentApi.EkContentRef.GetTemplateInfoByID(template_id);
            string templateName = templateData.FileName;
            sitePathValue.Text = "<td class=\"nowrap\">" + m_refContentApi.SitePath + "</td><td class=\"fullWidth\"><input type=\"text\" id=\"updateTemplate\"  maxlength=\"180\" class=\"fullWidth\" name=\"updateTemplate\" value=\"" + templateName + "\"/></td>";
            ltrTemplateDetails.Text = "<tr><td>" + m_refMsg.GetMessage("lbl template name") + ":</td><td></td><td><input id=\"updatetemplateName\" title=\"" + m_refMsg.GetMessage("lbl template name") + "\" maxlength=\"50\" name=\"updatetemplateName\" type=\"text\" value=\"" + templateData.TemplateName + "\"/></td></tr>" +
                "<tr><td>" + m_refMsg.GetMessage("lbl template description") + ":</td><td></td><td><input id=\"updatetemplateDescription\" title=\"" + m_refMsg.GetMessage("lbl template description") + "\" maxlength=\"200\" name=\"updatetemplateDescription\" type=\"text\" value=\"" + templateData.Description + "\"/></td></tr>";
            browsebuttontd.Visible = true;
            cbPageBuilderTemplate.Enabled = true;

            pnlPageBuilder.Visible = true;
            Ektron.Cms.Widget.WidgetTypeModel model = new Ektron.Cms.Widget.WidgetTypeModel();
            Ektron.Cms.Widget.WidgetTypeData[] widgetTypes = model.FindAll();
            repWidgetTypes.DataSource = widgetTypes;
            repWidgetTypes.DataBind();

            Ektron.Cms.PageBuilder.WireframeModel wireframeModel = new Ektron.Cms.PageBuilder.WireframeModel();
            Ektron.Cms.PageBuilder.WireframeData wireframe = wireframeModel.FindByTemplateID(template_id);

            if (wireframe != null)
            {
                Ektron.Cms.Widget.WidgetTypeData[] selectedWidgets = wireframeModel.GetAssociatedWidgetTypes(wireframe.ID);
                //lblThumbnailFileName.Text = wireframe.ThumbnailFile
                List<string> widgetIds = new List<string>();
                foreach (Ektron.Cms.Widget.WidgetTypeData widget in selectedWidgets)
                {
                    widgetIds.Add(widget.ID.ToString());
                }

                ClientScript.RegisterClientScriptBlock(this.GetType(), "pageBuilderSelectedIds", "Ektron.ready(function(){SelectWidgets([" + string.Join(", ", widgetIds.ToArray()) + "]);});", true);

                cbPageBuilderTemplate.Checked = true;
                if (wireframe.Template.SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
                {
                    browsebuttontd.Visible = false;
                    sitePathValue.Text = "<td class=\"nowrap\">" + templateName + "</td><td class=\"fullWidth\"><input type=\"text\" id=\"updateTemplate\" class=\"masterlayout\" style=\"display:none\" name=\"updateTemplate\" value=\"" + templateName + "\"/></td>";
                    cbPageBuilderTemplate.Enabled = false;
                }
            }

            //Code to show device types         
            string valDeviceInputs = "";
            if (m_refContentApi.RequestInformationRef.IsDeviceDetectionEnabled)
            {
                List<DeviceTemplateData> templateDevices = m_refContentApi.EkContentRef.GetTemplateDevicesByID(template_id);
                StringBuilder sb = new StringBuilder();
                if ((dList.Count > 0))
                {
                    Int32 i = 0;
                    sb.Append("<tr>").Append(Environment.NewLine);
                    sb.Append("     <td >").Append(Environment.NewLine);
                    sb.Append("         <br/><label class=\"deviceheader\" for=\"lblDeviceConfiguration\">").Append(m_refMsg.GetMessage("device configuration")).Append("</label>").Append(Environment.NewLine);
                    sb.Append("     </td>").Append(Environment.NewLine);
                    sb.Append("</tr>").Append(Environment.NewLine);
                    foreach (CmsDeviceConfigurationData dItem in dList)
                    {
                        if (dItem.Name != "Generic")
                        {
                            chkID = dItem.Id;
                            DeviceTemplateData deviceMatch = templateDevices.Find(GetDeviceItem);
                            sb.Append("<tr>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"devicelabel\">").Append(Environment.NewLine);
                            if ((deviceMatch != null) && deviceMatch.IsEnabled == true)
                            {
                                sb.Append("     <input type=\"checkbox\" class=\"pageBuilderCheckbox clearfix\" checked=\"checked\" name=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" id=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" />").Append(Environment.NewLine);
                            }
                            else
                            {
                                sb.Append("     <input type=\"checkbox\" class=\"pageBuilderCheckbox clearfix\" name=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" id=\"cbDeviceTemplate_").Append(dItem.Id).Append("\" />").Append(Environment.NewLine);
                            }
                            sb.Append("         <label for=\"addTemplateDevice_").Append(dItem.Id).Append("\">").Append(dItem.Name).Append("</label>").Append(Environment.NewLine);
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"nowrap\">").Append(Environment.NewLine);
                            sb.Append(m_refContentApi.SitePath).Append(Environment.NewLine);
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("     <td>").Append(Environment.NewLine);
                            if ((deviceMatch != null))
                            {
                                sb.Append("          <input type=\"text\" id=\"updateDeviceTemplate_").Append(dItem.Id).Append("\"  maxlength=\"180\" class=\"masterlayout\" name=\"updateDeviceTemplate_").Append(dItem.Id).Append("\" value=\"").Append(deviceMatch.FileName).Append("\" />");
                            }
                            else
                            {
                                sb.Append("          <input type=\"text\" id=\"updateDeviceTemplate_").Append(dItem.Id).Append("\"  maxlength=\"180\" class=\"masterlayout\" name=\"updateDeviceTemplate_").Append(dItem.Id).Append("\" value=\"\" />");
                            }
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("     <td class=\"value\">").Append(Environment.NewLine);
                            sb.Append("         <input type=\"button\" id=\"btnDeviceTemplate_").Append(dItem.Id).Append("\" value=\"...\" class=\"ektronModal browseButton\" onclick=\"SetBtnClicked(").Append(dItem.Id).Append(");OnBrowseButtonClicked()\" />").Append(Environment.NewLine);
                            sb.Append("     </td>").Append(Environment.NewLine);
                            sb.Append("<tr>").Append(Environment.NewLine);
                            if (i > 0)
                            {
                                valDeviceInputs += "," + dItem.Id;
                            }
                            else
                            {
                                valDeviceInputs += dItem.Id;
                            }
                            i = i + 1;
                        }
                    }
                    ltrDeviceConfigurations.Text = sb.ToString();
                }
            }
            AddBackButton("template_config.aspx?view=list");
            AddButton(m_refContentApi.AppPath + "images/UI/Icons/save.png", "javascript:ConfirmNotEmpty('updateTemplate','" + valDeviceInputs + "');", m_refMsg.GetMessage("lbl update template"), m_refMsg.GetMessage("lbl update template"), "", StyleHelper.SaveButtonCssClass, true);
            AddHelpButton("template_update");
        }
        else
        {
            Ektron.Cms.PageBuilder.WireframeModel wireframeModel = new Ektron.Cms.PageBuilder.WireframeModel();
            Ektron.Cms.PageBuilder.WireframeData wireframe = wireframeModel.FindByTemplateID(template_id);

            if (wireframe != null)
            {
                if (cbPageBuilderTemplate.Checked == true)
                {
                    wireframe.Path = Request.Form["updateTemplate"];
                    wireframe.Template.FileName = wireframe.Path;
                    wireframe.Template.Description = AntiXss.HtmlEncode(Request.Form["updatetemplateDescription"]);
                    wireframe.Template.TemplateName = AntiXss.HtmlEncode(Request.Form["updatetemplateName"]);

                    if (wireframe.Template.SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
                    {
                        wireframe.Template.Thumbnail = m_templateModel.GenerateThumbnail(wireframe.Template.MasterLayoutID);
                    }
                    else
                    {
                        string tempLocation = string.Empty;
                        if (wireframe.Path.IndexOf("?") != -1)
                        {
                            tempLocation = wireframe.Path.Substring(0, wireframe.Path.IndexOf("?")).ToString();
                        }
                        else
                        {
                            tempLocation = wireframe.Path;
                        }
                        wireframe.Template.Thumbnail = m_templateModel.GenerateThumbnail(tempLocation);
                    }

                    wireframeModel.Update(wireframe);
                    wireframeModel.RemoveAllWidgetTypeAssociations(wireframe.ID);

                    foreach (string Key in Request.Form.AllKeys)
                    {
                        if (Key.StartsWith("widget"))
                        {
                            try
                            {
                                wireframeModel.AddWidgetTypeAssociation(wireframe.ID, long.Parse(Key.Substring(6)));
                            }
                            catch (Exception ex)
                            {
                                EkException.ThrowException(ex);
                            }
                        }
                    }
                }
                else
                {
                    wireframeModel.Remove(wireframe.ID);
                }
            }
            else if (cbPageBuilderTemplate.Checked)
            {
                string strThumbnail = (string)(m_templateModel.GenerateThumbnail(Request.Form["updateTemplate"].ToString()));
                wireframe = wireframeModel.Create(Request.Form["updateTemplate"], template_id, System.IO.Path.GetFileName(strThumbnail), AntiXss.HtmlEncode(Request.Form["updatetemplateDescription"].ToString()), AntiXss.HtmlEncode(Request.Form["updatetemplateName"].ToString()));
                foreach (string Key in Request.Form.AllKeys)
                {
                    if (Key.StartsWith("widget"))
                    {
                        try
                        {
                            wireframeModel.AddWidgetTypeAssociation(wireframe.ID, long.Parse(Key.Substring(6)));
                        }
                        catch (Exception ex)
                        {
                            EkException.ThrowException(ex);
                        }
                    }
                }
            }
            else
            {
                Collection newtemplatedata = new Collection();
                TemplateData updateTemplateData = new TemplateData();
                updateTemplateData.Id = template_id;
                updateTemplateData.FileName = Request.Form["updateTemplate"].ToString();
                updateTemplateData.Description = AntiXss.HtmlEncode(Request.Form["updatetemplateDescription"].ToString());
                updateTemplateData.TemplateName = AntiXss.HtmlEncode(Request.Form["updatetemplateName"].ToString());
                m_refContentApi.EkContentRef.UpdateTemplatev2_0(updateTemplateData);
            }
            if (m_refContentApi.RequestInformationRef.IsDeviceDetectionEnabled)
            {
                SaveDeviceTemplates(dList, template_id, "update");
            }

            Response.Redirect("template_config.aspx?view=list", false);
        }
    }
    private bool GetDeviceItem(DeviceTemplateData devItem)
    {
        if (devItem.DeviceId == chkID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DeleteTemplate()
    {
        long template_id = System.Convert.ToInt64(Request.QueryString["id"]);
        Ektron.Cms.PageBuilder.WireframeModel wireframeModel = new Ektron.Cms.PageBuilder.WireframeModel();
        Ektron.Cms.PageBuilder.WireframeData wireframe = wireframeModel.FindByTemplateID(template_id);

        SetTitleBarToMessage("lbl delete template");
        AddBackButton("template_config.aspx?view=list");
        AddHelpButton("template_delete");
        long[] folders = m_refContentApi.EkContentRef.GetTemplateDefaultFolderUsage(template_id);
        Collection contentBlockInfo = m_refContentApi.EkContentRef.GetTemplateContentBlockUsage(template_id);
        StringBuilder str = new StringBuilder();
        str.Append("<div class=\"ektronPageContainer\">");
        if (folders.Length > 0)
        {
            str.Append(m_refMsg.GetMessage("lbl folders with") + " " + "\'<i>" + m_refContentApi.EkContentRef.GetTemplateNameByID(template_id) + "</i>\'" + " " + m_refMsg.GetMessage("lbl as their default template") + ":");
            str.Append("<div class=\"ektronTopSpace\"></div>");
            str.Append("<table width=\"100%\" class=\"ektronGrid ektronBorder\">");
            str.Append("<tbody>");
            str.Append("<tr class=\"title-header\">");
            str.Append("<th>Folder Path</th>");
            str.Append("</tr>");
            int i = 0;
            for (i = 0; i <= folders.Length - 1; i++)
            {
                str.Append("<tr>");
                str.Append("<td>" + m_refContentApi.GetFolderById(folders[i]).NameWithPath + "</td>");
                str.Append("</tr>");
            }
            str.Append("</tbody>");
            str.Append("</table>");
            str.Append("<div class=\"ektronTopSpace\"></div>");
            str.Append(m_refMsg.GetMessage("alert msg set folders") + " " + "\'<i>" + m_refContentApi.EkContentRef.GetTemplateNameByID(template_id) + "</i>\'.");

        }
        else
        {
            if (contentBlockInfo.Count == 0 || !(Request.Form["deleteTemplate"] == null))
            {
                bool messageadded = false;
                if (wireframe != null)
                {
                    if (wireframe.Template.SubType == Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
                    {
                        //delete the layout as well
                        Ektron.Cms.PageBuilder.TemplateModel templmodel = new Ektron.Cms.PageBuilder.TemplateModel();
                        string status = string.Empty;
                        Ektron.Cms.SiteAPI siteApi = new Ektron.Cms.SiteAPI();
                        Ektron.Cms.LanguageData[] langData = siteApi.GetAllActiveLanguages();
                        int j = 0;
                        for (j = 0; j <= langData.Length - 1; j++)
                        {
                            long contType = this.ContentAPIRef.EkContentRef.GetContentType(wireframe.Template.MasterLayoutID, langData[j].Id);
                            if (contType != 0)
                            {
                                int tempLang = System.Convert.ToInt32(ContentAPIRef.ContentLanguage);
                                ContentAPIRef.ContentLanguage = langData[j].Id;
                                status = (string)(this.ContentAPIRef.EkContentRef.GetContentStatev2_0(wireframe.Template.MasterLayoutID)["ContentStatus"]);
                                if (status == "O" || status == "S")
                                {
                                    Collection permissions = ContentAPIRef.EkContentRef.CanIv2_0(wireframe.Template.MasterLayoutID, "content");
                                    if (permissions.Contains("CanIApprove") && System.Convert.ToBoolean(permissions["CanIApprove"]))
                                    {
                                        ContentAPIRef.EkContentRef.TakeOwnership(wireframe.Template.MasterLayoutID);
                                    }
                                }
                                if (status == "S")
                                {
                                    ContentAPIRef.EkContentRef.CheckContentOutv2_0(wireframe.Template.MasterLayoutID);
                                }
                                status = (string)(this.ContentAPIRef.EkContentRef.GetContentStatev2_0(wireframe.Template.MasterLayoutID)["ContentStatus"]);
                                if (status == "O")
                                {
                                    ContentAPIRef.EkContentRef.CheckIn(wireframe.Template.MasterLayoutID, "");
                                }
                                Ektron.Cms.Content.Behaviors.DeleteContentBehavior deletebehavior = new Ektron.Cms.Content.Behaviors.DeleteContentBehavior(ContentAPIRef.RequestInformationRef);
                                deletebehavior.Delete(wireframe.Template.MasterLayoutID, null);
                                ContentAPIRef.ContentLanguage = tempLang;
                                //if content exists and status is now M then show tell user that layout has been marked for delete
                                if (!messageadded)
                                {
                                    Collection cont = ContentAPIRef.EkContentRef.GetContentByIDv2_0(wireframe.Template.MasterLayoutID);
                                    if (cont.Contains("ContentStatus"))
                                    {
                                        if ((string)cont["ContentStatus"] == "M")
                                        {
                                            str.Append("Template <i>" + wireframe.Template.FileName + "</i> is a Master Layout and has been marked for delete. When an approver approves this delete it will be removed from the templates list.");
                                            messageadded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        wireframeModel.Remove(wireframe.ID);
                        Response.Redirect("template_config.aspx", false);
                    }
                }
                try
                {
                    m_refContentApi.EkContentRef.PurgeTemplate(template_id);
                }
                catch (Exception ex)
                {
                    string _error = ex.Message;
                }
                if (!messageadded)
                {
                    Response.Redirect("template_config.aspx", false);
                }
            }
            else
            {
                LocalizationAPI objLocalizationApi = new LocalizationAPI();
                str.Append(m_refMsg.GetMessage("lbl content with") + " " + "\'<i>" + m_refContentApi.EkContentRef.GetTemplateNameByID(template_id) + "</i>\'" + " " + m_refMsg.GetMessage("lbl as a template") + ":");
                str.Append("<div class=\"ektronTopSpace\"></div>");
                str.Append("<table width=\"100%\" class=\"ektronGrid ektronBorder\">");
                str.Append("<tbody>");
                str.Append("<tr class=\"title-header\">");
                str.Append("<th width=\"70%\" align=\"center\">Title</th>");
                str.Append("<th width=\"15%\" align=\"center\">ID</th>");
                str.Append("<th width=\"15%\" align=\"center\">Language</th>");
                str.Append("</tr>");
                int i = 0;

                foreach (Collection col in contentBlockInfo)
                {
                    str.Append("<tr>");
                    m_refContentApi.ContentLanguage = Convert.ToInt32(col["language_id"]);
                    ContentData content_data = m_refContentApi.EkContentRef.GetContentById(Convert.ToInt64(col["content_id"]), Ektron.Cms.Content.EkContent.ContentResultType.Published);
                    str.Append("<td>" + content_data.Title + "</td>");
                    str.Append("<td align=\"center\">" + content_data.Id + "</td>");
                    str.Append("<td align=\"center\"><img src=\"" + objLocalizationApi.GetFlagUrlByLanguageID(content_data.LanguageId) + "\" /></td>");
                    str.Append("</tr>");
                    i++;
                }
                str.Append("</tbody>");
                str.Append("</table>");

                if (wireframe == null || wireframe.Template.SubType != Ektron.Cms.Common.EkEnumeration.TemplateSubType.MasterLayout)
                {
                    str.Append("<div class=\"ektronTopSpace\"></div>");
                    str.Append(m_refMsg.GetMessage("alert msg del template"));
                    str.Append(" <input type=\"submit\" id=\"deleteTemplate\" name=\"deleteTemplate\" value=\"Delete\" />&nbsp;&nbsp;");
                    str.Append("<input type=\"button\" id=\"cancelDelete\" name=\"cancelDelete\" value=\"Cancel\" onclick=\"window.location.replace(\'template_config.aspx\')\" />");
                }
                else
                {
                    str.Append("<div class=\"ektronTopSpace\"></div>");
                    str.Append(m_refMsg.GetMessage("alert msg set layout") + " " + "\'<i>" + wireframe.Template.FileName + "</i>\'.");
                }
            }
        }
        str.Append("</div>");
        MainBody.Text = str.ToString();
    }

    private void SaveDeviceTemplates(List<CmsDeviceConfigurationData> dList, long templateID, string action)
    {
        string enabledDevices = "";
        bool disabled = false;

        if ((dList.Count > 0))
        {
            DeviceTemplateData lDevItem = default(DeviceTemplateData);
            List<DeviceTemplateData> lDevItems = new List<DeviceTemplateData>();
            foreach (CmsDeviceConfigurationData dItem in dList)
            {
                if (!String.IsNullOrEmpty(Request.Form["cbDeviceTemplate_" + dItem.Id]))
                {
                    lDevItem = new DeviceTemplateData();
                    lDevItem.DeviceId = dItem.Id;
                    lDevItem.TemplateId = templateID;
                    lDevItem.FileName = Request.Form["updateDeviceTemplate_" + dItem.Id];
                    lDevItems.Add(lDevItem);
                    enabledDevices += lDevItem.DeviceId.ToString() + ",";
                }
                else if (!String.IsNullOrEmpty(Request.Form["updateDeviceTemplate_" + dItem.Id]))
                {
                    lDevItem = new DeviceTemplateData();
                    lDevItem.DeviceId = dItem.Id;
                    lDevItem.TemplateId = templateID;
                    lDevItem.FileName = Request.Form["updateDeviceTemplate_" + dItem.Id];
                    lDevItems.Add(lDevItem);
                    disabled = true;
                }
            }
            if (enabledDevices != "")
            {
                enabledDevices.Remove(enabledDevices.Length - 1, 1);
            }
            else
            {
                enabledDevices = "-1";
            }
            if (lDevItems.Count > 0)
            {
                if (action == "add")
                {
                    m_refContentApi.EkContentRef.AddDeviceTemplate(lDevItems);

                    if (disabled)
                    {
                        m_refContentApi.EkContentRef.UpdateEnabledDeviceList(enabledDevices, templateID);
                    }
                }
                else
                {
                    m_refContentApi.EkContentRef.UpdateDeviceTemplate(lDevItems);

                    if (disabled)
                    {
                        m_refContentApi.EkContentRef.UpdateEnabledDeviceList(enabledDevices, templateID);
                    }
                }
            }
            else
            {
                m_refContentApi.EkContentRef.UpdateEnabledDeviceList("", templateID);
            }
        }
    }

}

