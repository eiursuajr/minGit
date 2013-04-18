using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Ektron.Cms.API;
using Ektron.Cms;
using System.IO;
using System.Collections.Generic;
using Ektron.Newtonsoft.Json;
using System.ComponentModel;
using Ektron.Cms.Content.Targeting.Rules;
using Ektron.Cms.Framework.UI;
using Ektron.Cms.Common;

    [DefaultBindingProperty()]
    public partial class RuleEditor : System.Web.UI.UserControl
    {
        public event EventHandler<SaveEventArgs> Save;
        protected EkMessageHelper m_refMsg;
        
        private List<Rule> _rules = new List<Rule>();
        public List<Rule> Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        private Dictionary<string, RuleTemplate> _ruleTemplates = new Dictionary<string, RuleTemplate>();
        public Dictionary<string, RuleTemplate> RuleTemplates
        {
            get { return _ruleTemplates; }
            set { _ruleTemplates = value; }
        }

        public void AddRuleTemplate(RuleTemplate ruleTemplate)
        {
            _ruleTemplates.Add(ruleTemplate.ID, ruleTemplate);
        }

        private void RegisterEditor(Dictionary<string, RuleTemplate> ruleTemplates, List<Rule> rules)
        {
            if (ruleTemplates.Count == 0) return;

            ruleTemplateJSON.Value = JsonConvert.SerializeObject(ruleTemplates);
            rulesJSON.Value = JsonConvert.SerializeObject(rules);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Page.ClientScript.GetPostBackEventReference(this, string.Empty);
           
            CommonApi commonApi = new CommonApi();
            m_refMsg = commonApi.EkMsgRef;
            string sitepath = commonApi.SitePath;

            Package ruleEditorUI = new Package()
            {
                Components = new List<Ektron.Cms.Framework.UI.Component>
                {
                    // JS
                    Packages.Ektron.StringObject,
                    Packages.Ektron.JSON,
                    JavaScript.Create(Path.Combine(this.TemplateSourceDirectory, @"js\RuleEditor.js").Replace(@"\", "/")),
                    JavaScript.Create(Path.Combine(this.TemplateSourceDirectory, @"js\ektron.autogrow.js").Replace(@"\", "/")),
                    JavaScript.Create(Path.Combine(this.TemplateSourceDirectory, @"js\ektron.jeditable.js").Replace(@"\", "/")),
                    JavaScript.Create(Path.Combine(this.TemplateSourceDirectory, @"js\ektron.dropdown.js").Replace(@"\", "/")),

                    // Css
                    Packages.jQuery.jQueryUI.ThemeRoller,
                    Ektron.Cms.Framework.UI.Css.Create(Path.Combine(this.TemplateSourceDirectory, @"css\RuleEditor.css").Replace(@"\", "/"))
                }
            };
            ruleEditorUI.Register(this);

            JavaScript.RegisterJavaScriptBlock(this, string.Format("Ektron.RuleEditor.init('{0}', '{1}');", UniqueID, container.ClientID), false);
        }

        public virtual void SaveRules()
        {
            Dictionary<Int32, Rule> rulesToSave =
                JsonConvert.DeserializeObject<Dictionary<Int32, Rule>>(rulesByIdJSON.Value);
            List<Rule> rules = new List<Rule>(rulesToSave.Values);
            SaveRules(rules);
        }

        protected void SaveRules(List<Rule> rules)
        {
            if (Save != null)
            {
                Save(this, new SaveEventArgs(rules));
            }
        }

        public override void DataBind()
        {
            base.DataBind();

            RegisterEditor(_ruleTemplates, _rules);
        }
    }
