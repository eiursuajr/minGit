using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Data;
using System.Web.Caching;
using System.Xml.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.Security;
using System;
using System.Text;
using Microsoft.VisualBasic;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Web.Profile;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Web;
using Ektron.Cms;
using Ektron.Cms.Common;
using Ektron.Cms.API;

	public partial class controls_Reorder_Reorder : System.Web.UI.UserControl
	{
		
		
		
		#region Private Members
		private EkMessageHelper _messageHelper = null;
		private string _appPath = "";
		private string _appImgPath = "";
		private ListItemCollection _itemCollection = new ListItemCollection();
		private bool _selectenabled = true;
		
		#endregion
		
		
		#region Page Functions
		
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			RegisterJS();
			
		}
		
		
		#endregion
		
		
		#region Properties
		
		
		public ListItemCollection ItemList
		{
			get
			{
				return _itemCollection;
			}
			set
			{
				_itemCollection = value;
			}
		}
		
		public EkMessageHelper MessageHelper
		{
			get
			{
				return _messageHelper;
			}
			set
			{
				_messageHelper = value;
			}
		}
		
		public string AppPath
		{
			get
			{
				return _appPath;
			}
			set
			{
				_appPath = value;
			}
		}
		
		public string AppImgPath
		{
			get
			{
				return _appImgPath;
			}
			set
			{
				_appImgPath = value;
			}
		}
		
		public string ReOrderList
		{
			get
			{
				return ConvertToString();
			}
		}
		
		public bool SelectEnabled
		{
			get
			{
				return _selectenabled;
			}
			set
			{
				_selectenabled = value;
			}
		}
		
		#endregion
		
		
		#region Public Functions
		
		
		public string GetMessage(string resourceString)
		{
			
			return MessageHelper.GetMessage(resourceString);
			
		}
		
		public void AddItem(string title, long id, long language)
		{
			
			_itemCollection.Add(new ListItem(title, id.ToString() + "|" + language.ToString()));
			
		}
		
		public void Initialize(EkRequestInformation requestInformation)
		{
			
			_messageHelper = new EkMessageHelper(requestInformation);
			_appPath = requestInformation.ApplicationPath;
			_appImgPath = requestInformation.AppImgPath;
			
		}
		
		
		#endregion
		
		
		#region Private Functions
		
		
		private string ConvertToString()
		{
			
			string commaSeperatedList = "";
			
			for (int i = 0; i <= (_itemCollection.Count - 1); i++)
			{
				
				if (commaSeperatedList != "")
				{
					commaSeperatedList += (string) ("," + _itemCollection[i].Value);
				}
				else
				{
					commaSeperatedList = (string) (_itemCollection[i].Value);
				}
				
			}
			
			return commaSeperatedList;
			
		}
		
		private void RegisterJS()
		{
			
			Ektron.Cms.ContentAPI _ContentAPI;
			_ContentAPI = new ContentAPI();
			Ektron.Cms.API.JS.RegisterJS(this, _ContentAPI.AppPath + "controls/Reorder/js/Reorder.js", "EktronControlsReorderJs");
			
		}
		
		protected string GetDisabled()
		{
			if (_selectenabled)
				return "";
			else
				return "disabled=\"disabled\"";
		}
		
		#endregion
		
		
	}