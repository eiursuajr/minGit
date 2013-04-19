using System;
using System.Collections.Generic;
using System.Web;
using Ektron.Cms;
using System.Diagnostics;

namespace WSOL
{
	/// <summary>
	/// Summary description for Messaging
	/// </summary>
	public class Messaging
	{
		public Messaging()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Function to output an HTML string, with specific formatting for each message type.
		/// </summary>
		/// <param name="msgText">Output message string</param>
		/// <param name="msgType">Message type</param>
		/// <returns></returns>
		public static string Message(string msgText, EventLogEntryType msgType)
		{
			Ektron.Cms.API.Common EkCommonAPI = new Ektron.Cms.API.Common();
			string sitepath = EkCommonAPI.SitePath;
			double version = Convert.ToDouble(EkCommonAPI.Version);
			string borderTextColor = String.Empty;
			string backgroundColor = String.Empty;
			string iconURL = String.Empty;
			string iconPosition = String.Empty;

			switch (msgType)
			{
				case EventLogEntryType.Error:
				case EventLogEntryType.FailureAudit:
					borderTextColor = "cd0a0a";
					backgroundColor = "ffece6";
					if (version >= 8.5)
					{
						iconPosition = "background-position: 0 -35px;";
					}
					else
					{
						iconURL = sitepath + "Workarea/images/UI/Icons/exclamation.png";
					}
					break;
				case EventLogEntryType.SuccessAudit:
					borderTextColor = "006600";
					backgroundColor = "ccffcc";
					if (version >= 8.5)
					{
						iconPosition = "background-position: 0 0px;";
					}
					else
					{
						iconURL = sitepath + "Workarea/images/UI/Icons/check.png";
					}
					break;
				case EventLogEntryType.Warning:
					borderTextColor = "CF8300";
					backgroundColor = "FFF1AF";
					if (version >= 8.5)
					{
						iconPosition = "background-position: 0 -17px;";
					}
					else
					{
						iconURL = sitepath + "Workarea/images/UI/Icons/error.png";
					}
					break;
				case EventLogEntryType.Information:
				default:
					borderTextColor = "006699";
					backgroundColor = "eff9ff";
					if (version >= 8.5)
					{
						iconPosition = "background-position: 0 -72px;";
					}
					else
					{
						iconURL = sitepath + "Workarea/images/UI/Icons/information.png";
					}
					break;
			}

			if (version >= 8.5)
			{
				iconURL = sitepath + "Workarea/FrameworkUI/images/sprites/interactionCues.png";
			}

			return "<div id=\"ux" + msgType + "Msg\" style=\"padding: 10px; margin: 10px 5px 10px 5px; border: 1px solid #" + borderTextColor + "; color: #" + borderTextColor + "; background-color: #" + backgroundColor + ";\">" +
					"<span style=\"float: left; margin-right: 0.3em; width: 16px; height: 16px; background-image: url(" + iconURL + "); " + iconPosition + "\"></span>" +
					msgText + "</div>";
		}
	}
}