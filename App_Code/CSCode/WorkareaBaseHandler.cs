namespace Ektron.Cms.Workarea.Framework
{
	/// <summary>
	/// Base class for all workarea handler pages - *.ashx pages
	/// Security should be enforced here
	/// </summary>
	public abstract class WorkareaBaseHttpHandler : System.Web.IHttpHandler 
	{
		private Ektron.Cms.Common.EkMessageHelper m_refMsg = null;
		private Ektron.Cms.CommonApi m_api = null;

		public virtual void ProcessRequest(System.Web.HttpContext context)
		{
			AssertInternalReferrer(context.Request);
		}

		public virtual bool IsReusable
		{
			get
			{
				return false;
			}
		}

		protected Ektron.Cms.CommonApi GetCommonApi()
		{
			if (null == m_api)
			{
				m_api = new Ektron.Cms.CommonApi();
			}
			return m_api;
		}

		protected string GetMessage(string key)
		{
			if (null == m_refMsg)
			{
				m_refMsg = GetCommonApi().EkMsgRef;
			}
			return m_refMsg.GetMessage(key);
		}

		protected void AssertInternalReferrer(System.Web.HttpRequest Request)
		{
			if (Request.IsLocal || Request.Url.IsLoopback) return;
			if (null == Request.UrlReferrer || Request.Url.Authority != Request.UrlReferrer.Authority)
			{
				throw new System.Exception("This page may be used only from within this site.");
			}
		}
	}
}