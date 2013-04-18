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
using System.Xml;
using Ektron.Cms.Common;
using Ektron.Cms.Controls;
using Ektron.Cms.ToolBar;

namespace CMSUtils
{
    namespace CMSMenuUtils
    {

        public class NStateMenu : Control
        {

            public bool IsInitialized = false;
            private string UninitializedMessage = "NStateMenu not initialized.  Set values for menu_id, targettype, and targetid and call the Fill() method.  (Altering any of those values after .Fill() will return NStateMenu to an uninitialized state.)";
            private string ExceptionSource;
            const string CMS_CONTENTBLOCKID_VARNAME = "id";

            private CMSIDTypes.menu_id menu_id_val;
            public CMSIDTypes.menu_id menu_id
            {
                get
                {
                    return menu_id_val;
                }
                set
                {
                    menu_id_val = value;
                    IsInitialized = false;
                }
            }

            private EkEnumeration.CMSMenuItemType targettype_val;
            public EkEnumeration.CMSMenuItemType targettype
            {
                get
                {
                    return targettype_val;
                }
                set
                {
                    targettype_val = value;
                    IsInitialized = false;
                }
            }

            private CMSIDTypes.menuitem_id targetid_val;
            public CMSIDTypes.menuitem_id targetid
            {
                get
                {
                    return targetid_val;
                }
                set
                {
                    targetid_val = value;
                    IsInitialized = false;
                }
            }

            private Ektron.Cms.Controls.Menu MenuObj_val;
            public Ektron.Cms.Controls.Menu MenuObj
            {
                get
                {
                    if (IsInitialized == true)
                    {
                        return MenuObj_val;
                    }
                    else
                    {
                        UnintializedException ex = new UnintializedException(UninitializedMessage);
                        ex.Source = ExceptionSource;
                        throw (ex);
                    }
                }
            }
            private XmlNode TargetCrumb_val;
            public XmlNode TargetCrumb
            {
                get
                {
                    if (IsInitialized == true)
                    {
                        return TargetCrumb_val;
                    }
                    else
                    {
                        UnintializedException ex = new UnintializedException(UninitializedMessage);
                        ex.Source = ExceptionSource;
                        throw (ex);
                    }
                }
            }
            internal XmlNodeList TierCrumb_val;
            private XMLNodeListIndexer TierCrumb_idx;
            public XMLNodeListIndexer TierCrumb
            {
                get
                {
                    if (IsInitialized == true)
                    {
                        return TierCrumb_idx;
                    }
                    else
                    {
                        UnintializedException ex = new UnintializedException(UninitializedMessage);
                        ex.Source = ExceptionSource;
                        throw (ex);
                    }
                }
            }
            internal XmlNodeList[] TierList_val;

            // public readonly TierListIndexer TierList = new TierListIndexer(this);

            public string menuIDVarName
            {
                get
                {
                    return ID + "_mid";
                }
            }
            public string targetIDVarName
            {
                get
                {
                    return ID + "_mtid";
                }
            }
            public string targetTypeVarName
            {
                get
                {
                    return ID + "_mtt";
                }
            }

            public NStateMenu(CMSIDTypes.menu_id menu_id, ref EkEnumeration.CMSMenuItemType targettype, ref CMSIDTypes.menuitem_id targetid)
            {
                ExceptionSource = this.GetType().ToString();

                Ektron.Cms.Controls.Menu motemp = new Ektron.Cms.Controls.Menu();
                motemp.CacheInterval = 60;
                motemp.DefaultMenuID = menu_id.val;
                motemp.Fill();
                CommonConstructor(motemp, targettype, targetid);
            }

            public NStateMenu(ref Ektron.Cms.Controls.Menu menuObject, ref EkEnumeration.CMSMenuItemType targettype, ref CMSIDTypes.menuitem_id targetid)
            {
                ExceptionSource = this.GetType().ToString();

                CommonConstructor(menuObject, targettype, targetid);
            }

            public NStateMenu(CMSIDTypes.menu_id menu_id) //If no parameters are passed, try to get them from URL vars
            {
                ExceptionSource = this.GetType().ToString();


                menu_id_val = menu_id;
                this.AcquireVars();

                //if (!((this.menu_id_val == null) || (this.targetid_val == null) || (this.targettype_val == null)))
                //{
                try
                {
                    Fill();
                }
                catch (NStateMenu.TargetNotInMenuException)
                {
                    //Eat the exception and remain uninitialized if
                    // target doesn't exist.
                }
                //}

            }

            public bool AcquireVars()
            {
                if (!(this.Page == null))
                {
                    if (this.Page.Request.QueryString[this.menuIDVarName] != "")
                    {
                        menu_id_val = new CMSIDTypes.menu_id(int.Parse(this.Page.Request.QueryString[this.menuIDVarName]));
                    }

                    if (this.Page.Request.QueryString[this.targetIDVarName] != "")
                    {
                        targetid_val = new CMSIDTypes.menuitem_id(int.Parse(this.Page.Request.QueryString[this.targetIDVarName]));
                    }
                    else if (this.Page.Request.QueryString[CMS_CONTENTBLOCKID_VARNAME] != "")
                    {
                        //If no NStateMenu target ID, try to use CMS content block ID
                        // This could throw an exception, if the ID isn't in the menu
                        targetid_val = new CMSIDTypes.menuitem_id(int.Parse(this.Page.Request.QueryString[CMS_CONTENTBLOCKID_VARNAME]));
                    }

                    if (this.Page.Request.QueryString[this.targetTypeVarName] != "")
                    {
                        targettype_val = (EkEnumeration.CMSMenuItemType)Enum.Parse(typeof(EkEnumeration.CMSMenuItemType), this.Page.Request.QueryString[this.targetTypeVarName]);
                    }
                    else
                    {
                        //Default to "content" itemtype
                        targettype_val = Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.content;
                    }

                }

                //if ((this.menu_id_val == null) || (this.targetid_val == null) || (this.targettype_val == null))
                //{
                //    return false;
                //}
                //else
                //{
                return true;
                //}

            }

            public void Fill()
            {
                //if ((this.menu_id_val == null) || (this.targetid_val == null) || (this.targettype_val == null))
                //{
                //    Exception ex = new Exception(this.GetType().ToString() + ": Menu_id or targettype or targetid is null.");
                //    ex.Source = ExceptionSource;
                //    throw (ex);
                //}
                //else
                //{
                Ektron.Cms.Controls.Menu motemp = new Ektron.Cms.Controls.Menu();
                motemp.CacheInterval = 60;
                motemp.DefaultMenuID = this.menu_id_val.val;
                motemp.Fill();
                this.CommonConstructor(motemp, this.targettype_val, this.targetid_val);
                //}
            }

            private void CommonConstructor(Ektron.Cms.Controls.Menu menuObject, Ektron.Cms.Common.EkEnumeration.CMSMenuItemType targettype, CMSIDTypes.menuitem_id targetid)
            {

                this.menu_id_val = new CMSIDTypes.menu_id(menuObject.DefaultMenuID);
                this.targettype_val = targettype;
                this.targetid_val = targetid;
                MenuObj_val = menuObject;

                // Test for standard menu XML xpaths

                XmlNodeList xnl;

                StringCollection strcolValidationPaths = new StringCollection();
                string[] strarrValidationPaths = new string[] { "/MenuDataResult", "/MenuDataResult/Item", "/MenuDataResult/Item/Item" };
                strcolValidationPaths.AddRange(strarrValidationPaths);

                foreach (string strValidationPath in strcolValidationPaths)
                {
                    xnl = MenuObj_val.XmlDoc.SelectNodes(strValidationPath);
                    if (xnl.Count <= 0)
                    {
                        TargetNotInMenuException ex = new TargetNotInMenuException(this.GetType().ToString() + ": CMS returns invalid menu data.  XPath \"" + strValidationPath + "\" is missing from the XML returned for menu_id " + MenuObj_val.DefaultMenuID.ToString() + ".  This may indicate that no menu with ID " + MenuObj_val.DefaultMenuID.ToString() + " exists.");
                        ex.Source = ExceptionSource;
                        throw (ex);
                    }
                }

                string menuTargetXPath = "/descendant::Item[child::ItemID=\'" + targetid.val.ToString() + "\' and ItemType=\'" + targettype.ToString() + "\']";
                xnl = MenuObj_val.XmlDoc.SelectNodes(menuTargetXPath);
                if (xnl.Count > 0) // Test to ensure that the target item exists in the menu
                {
                    TargetCrumb_val = xnl[0];
                }
                else
                {
                    TargetNotInMenuException ex = new TargetNotInMenuException(this.GetType().ToString() + ": Item ID " + targetid.val.ToString() + ", Type \"" + targettype.ToString() + "\" does not exist in menu_id " + MenuObj_val.DefaultMenuID.ToString());
                    ex.Source = ExceptionSource;
                    throw (ex);
                }

                string menuAncestorsXPath = "(" + menuTargetXPath + "/ancestor::Item[ItemType=\'" + Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu.ToString() + "\'])|(" + menuTargetXPath + ")";
                TierCrumb_val = MenuObj_val.XmlDoc.SelectNodes(menuAncestorsXPath);
                TierCrumb_idx = new XMLNodeListIndexer(TierCrumb_val);

                XmlNode xn;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType xn_ItemType;
                int xn_ItemID;

                string tierXPath;
                int TierListLength;

                if (targettype == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                {
                    //If target is a submenu we'll grab the children
                    TierListLength = TierCrumb_val.Count;
                }
                else
                {
                    TierListLength = TierCrumb_val.Count - 1;
                }

                TierList_val = new XmlNodeList[TierListLength + 1];

                for (int I = 0; I <= (TierListLength); I++)
                {
                    if (I == 0)
                    {

                        xn = TierCrumb_val[0];
                        xn_ItemType = (Ektron.Cms.Common.EkEnumeration.CMSMenuItemType)(Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText));
                        xn_ItemID = int.Parse(xn.SelectSingleNode("ItemID").InnerText);
                        tierXPath = "/descendant::Item[child::ItemID=\'" + xn_ItemID.ToString() + "\' and ItemType=\'" + xn_ItemType.ToString() + "\']";
                    }
                    else if (TierCrumb_val.Count > 0)
                    {
                        xn = TierCrumb_val[I - 1];
                        xn_ItemType = (Ektron.Cms.Common.EkEnumeration.CMSMenuItemType)(Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText));
                        xn_ItemID = int.Parse((string)(xn.SelectSingleNode("ItemID").InnerText));
                        tierXPath = "/descendant::Item[child::ItemID=\'" + xn_ItemID.ToString() + "\' and ItemType=\'" + xn_ItemType.ToString() + "\']/child::Menu/child::Item";
                        break;
                    }
                    else
                    {
                        xn = TierCrumb_val[I];
                        xn_ItemType = (Ektron.Cms.Common.EkEnumeration.CMSMenuItemType)(Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSMenuItemType), xn.SelectSingleNode("ItemType").InnerText));
                        xn_ItemID = int.Parse(xn.SelectSingleNode("ItemID").InnerText);
                        tierXPath = "/descendant::Item[child::ItemID=\'" + xn_ItemID.ToString() + "\' and ItemType=\'" + xn_ItemType.ToString() + "\']/parent::Menu/child::Item";
                    }

                    xnl = MenuObj_val.XmlDoc.SelectNodes(tierXPath);
                    TierList_val.SetValue(xnl, I);
                }

                IsInitialized = true;

            }

            //public string GetCMSMenuItemLink(ref XmlNode navItem)
            //{
            //    string ItemLinkStr;
            //    bool startsWithJSURI = false;
            //    Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;

            //    ItemType = UtilObj.GetCMSMenuItemType(navItem);

            //    //BE CAREFUL ALTERING THIS
            //    //This case statement uses GoTo statements - variables may have values you don't expect.
            //    if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
            //    {
            //        ItemLinkStr = (string)(navItem.SelectSingleNode("Menu/Link").InnerText);
            //        startsWithJSURI = ItemLinkStr.StartsWith("javascript");
            //        if (startsWithJSURI)
            //        {
            //            goto JSCase;
            //        }
            //        ItemLinkStr = AppendVars(ItemLinkStr, ref navItem);
            //    }
            //    else if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.JavaScript)
            //    {
            //        ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText);
            //    JSCase:
            //        ItemLinkStr = (string)((startsWithJSURI ? "" : "javascript: void ") + ItemLinkStr.Replace("\"", "\'"));
            //    }
            //    else
            //    {
            //        ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText);
            //        startsWithJSURI = ItemLinkStr.StartsWith("javascript");
            //        if (startsWithJSURI)
            //        {
            //            goto JSCase;
            //        }
            //        ItemLinkStr = AppendVars(ItemLinkStr, ref navItem);
            //    }

            //    return (ItemLinkStr);
            //}

            public string GetCMSMenuItemLinkOnClick(ref XmlNode navItem)
            {

                string ItemLinkStr;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = UtilObj.GetCMSMenuItemType(navItem);
                try
                {
                    if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.JavaScript)
                    {
                        ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText.Replace("\"", "\'"));
                    }
                    else
                    {
                        if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                        {
                            ItemLinkStr = (string)(navItem.SelectSingleNode("Menu/Link").InnerText);
                        }
                        else
                        {
                            ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText);
                        }
                        ItemLinkStr = "window.location=\'" + AppendVars(ItemLinkStr, ref navItem) + "\';";
                    }
                }
                catch (Exception)
                {
                    ItemLinkStr = "";
                }
                return (ItemLinkStr);

            }

            private string AppendVars(string ItemLinkStr, ref XmlNode navItem)
            {
                StringBuilder sb = new StringBuilder(ItemLinkStr);
                if (ItemLinkStr != string.Empty)
                {
                    if (ItemLinkStr.IndexOf('?') > 0)
                    {
                        sb.Append('&');
                    }
                    else
                    {
                        sb.Append('?');
                    }
                    sb.Append(targetIDVarName + "=" + UtilObj.GetCMSMenuItemID(navItem) + "&" + targetTypeVarName + "=" + UtilObj.GetCMSMenuItemType(navItem) + "&" + menuIDVarName + "=" + MenuObj.DefaultMenuID.ToString());
                }
                return (sb.ToString());
            }

            public class UnintializedException : System.Exception
            {

                public UnintializedException()
                {
                }
                public UnintializedException(string Message)
                    : base(Message)
                {
                }
            }

            public class TargetNotInMenuException : System.Exception
            {

                public TargetNotInMenuException()
                {
                }
                public TargetNotInMenuException(string Message)
                    : base(Message)
                {
                }
            }

            public class TierListIndexer
            {


                NStateMenu parentTS;

                public int Length
                {
                    get
                    {
                        if (parentTS.IsInitialized == true)
                        {
                            return parentTS.TierList_val.Length;
                        }
                        else
                        {
                            throw (new UnintializedException(parentTS.UninitializedMessage));
                        }
                    }
                }

                public XmlNodeList this[int I]
                {

                    get
                    {

                        if (parentTS.IsInitialized == true)
                        {
                            if ((I >= parentTS.TierList_val.Length) || (I < 0))
                            {
                                throw (new System.IndexOutOfRangeException("TierList: " + I + " not a valid tier number."));
                            }
                            else
                            {
                                return parentTS.TierList_val[I];
                            }
                        }
                        else
                        {
                            throw (new UnintializedException(parentTS.UninitializedMessage));
                        }

                    }

                }

                public TierListIndexer(NStateMenu parentTS)
                {
                    this.parentTS = parentTS;
                }

            }

            public class XMLNodeListIndexer
            {

                //The only reason that I created this class is because
                // System.Xml.XmlNodeList throws an extremely unhelpful
                // NullReferenceException if you try to access a member
                // that isn't there.  This class throws a more helpful
                // IndexOutOfRangeException.

                XmlNodeList innerXNL;

                public int Count
                {
                    get
                    {
                        return innerXNL.Count;
                    }
                }

                public XmlNode this[int I]
                {

                    get
                    {
                        if ((I >= innerXNL.Count) || (I < 0))
                        {
                            throw (new System.IndexOutOfRangeException("XMLNodeListIndexer: " + I + " is out of range." + (I < 0 ? "(Cannot be less than zero.)" : "(Count = " + innerXNL.Count + ")")));
                        }
                        else
                        {
                            return innerXNL.Item(I);
                        }

                    }

                }

                public XMLNodeListIndexer(XmlNodeList innerXNL)
                {
                    this.innerXNL = innerXNL;
                }

            }

        }


        public class UtilObj
        {


            static public XmlNodeList GetCMSSubMenuChildNodes(ref XmlNode navItem)
            {

                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType = GetCMSMenuItemType(navItem);

                if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                {
                    return (navItem.SelectNodes("Menu/Item"));
                }
                else
                {
                    return (navItem.SelectNodes("nonexistent_path_to_create_an_empty_XmlNodeList"));
                }

            }

            static public string GetCMSMenuItemTitle(XmlNode navItem)
            {

                string ItemTitleStr = (string)(navItem.SelectSingleNode("ItemTitle").InnerText);
                return (ItemTitleStr);

            }

            static public EkEnumeration.CMSMenuItemType GetCMSMenuItemType(XmlNode navItem)
            {

                string ItemTypeStr = (string)(navItem.SelectSingleNode("ItemType").InnerText);
                ItemTypeStr = ItemTypeStr.Replace("Javascript", "JavaScript");
                return (EkEnumeration.CMSMenuItemType)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSMenuItemType), ItemTypeStr);

            }

            static public string GetCMSMenuItemLink(ref XmlNode navItem)
            {

                string ItemLinkStr;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = GetCMSMenuItemType(navItem);
                if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                {
                    ItemLinkStr = (string)(navItem.SelectSingleNode("Menu/Link").InnerText);
                }
                else
                {
                    ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText);
                }
                return (ItemLinkStr);

            }

            static public string GetCMSMenuItemImage(ref XmlNode navItem)
            {

                string ItemImageStr;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = GetCMSMenuItemType(navItem);
                if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                {
                    ItemImageStr = (string)(navItem.SelectSingleNode("Menu/Image").InnerText);
                }
                else
                {
                    ItemImageStr = (string)(navItem.SelectSingleNode("ItemImage").InnerText);
                }
                return (ItemImageStr);

            }

            static public bool GetCMSMenuItemImageOverride(ref XmlNode navItem)
            {
                bool ItemImageOverrideBool;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = GetCMSMenuItemType(navItem);
                if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                {
                    ItemImageOverrideBool = bool.Parse((string)(navItem.SelectSingleNode("Menu/ImageOverride").InnerText));
                }
                else
                {
                    ItemImageOverrideBool = bool.Parse((string)(navItem.SelectSingleNode("ItemImageOverride").InnerText));
                }
                return (ItemImageOverrideBool);

            }

            static public EkEnumeration.CMSMenuItemTarget GetCMSMenuItemTarget(ref XmlNode navItem)
            {

                string ItemTargetStr;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget ItemTarget;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = GetCMSMenuItemType(navItem);
                if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu) // As of this writing submenu nodes don't have targets
                {
                    ItemTargetStr = Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Self.ToString();
                }
                else if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.JavaScript)
                {
                    ItemTargetStr = Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Self.ToString();
                }
                else
                {
                    ItemTargetStr = (string)(navItem.SelectSingleNode("ItemTarget").InnerText);
                }
                ItemTarget = (EkEnumeration.CMSMenuItemTarget)Enum.Parse(typeof(Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget), ItemTargetStr);
                return (ItemTarget);

            }

            static public string CMSMenuItemTargetToString(EkEnumeration.CMSMenuItemTarget ItemTarget)
            {

                string ItemTargetStr = "_self";
                if (ItemTarget == Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Self)
                {
                    ItemTargetStr = "_self";
                }
                else if (ItemTarget == Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Popup)
                {
                    ItemTargetStr = "_blank";
                }
                else if (ItemTarget == Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Top)
                {
                    ItemTargetStr = "_top";
                }
                else if (ItemTarget == Ektron.Cms.Common.EkEnumeration.CMSMenuItemTarget.Parent)
                {
                    ItemTargetStr = "_parent";
                }
                return (ItemTargetStr);

            }

            static public string GetCMSMenuItemLinkOnClick(ref XmlNode navItem)
            {

                string ItemLinkStr;
                Ektron.Cms.Common.EkEnumeration.CMSMenuItemType ItemType;
                ItemType = GetCMSMenuItemType(navItem);
                try
                {
                    if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.JavaScript)
                    {
                        ItemLinkStr = (string)(navItem.SelectSingleNode("ItemLink").InnerText.Replace("\"", "\'"));
                    }
                    else if (ItemType == Ektron.Cms.Common.EkEnumeration.CMSMenuItemType.Submenu)
                    {
                        ItemLinkStr = "window.location=\'" + navItem.SelectSingleNode("Menu/Link").InnerText + "\';";
                    }
                    else
                    {
                        ItemLinkStr = "window.location=\'" + navItem.SelectSingleNode("ItemLink").InnerText + "\';";
                    }
                }
                catch (Exception)
                {
                    ItemLinkStr = "";
                }
                return (ItemLinkStr);

            }

            static public int GetCMSMenuItemID(XmlNode navItem)
            {

                int ItemIDInt = int.Parse((string)(navItem.SelectSingleNode("ItemID").InnerText));
                return (ItemIDInt);

            }

        }
    }

    namespace CMSIDTypes
    {
        public abstract class CMS_ID
        {

            private long _val;

            public long val
            {
                get
                {
                    return _val;
                }
                set
                {
                    _val = value;
                }
            }

            public CMS_ID(long valinit)
            {
                this.val = valinit;
            }
        }
        public class menu_id : CMS_ID
        {

            public static menu_id Null = new menu_id(0);
            public menu_id(long valinit)
                : base(valinit)
            {
            }
        }
        public class menuitem_id : CMS_ID
        {

            public static menuitem_id Null = new menuitem_id(0);
            public menuitem_id(int valinit)
                : base(valinit)
            {
            }
        }
    }
}