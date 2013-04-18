<%@ WebHandler Language="C#" Class="controls_reports_GoogleChart" %>

using System;
using System.IO;
using System.Web;
using System.Net;

public class controls_reports_GoogleChart : Ektron.Cms.Workarea.Framework.WorkareaBaseHttpHandler 
{

	// Adapted from http://www.schnieds.com/2008/03/aspnet-google-charts-implementation.html
	
	public override void ProcessRequest(HttpContext context) 
    {
		base.ProcessRequest(context);
		
        // Set up the response settings
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
		context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
		context.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0, 0, 0));
		context.Response.Cache.SetLastModified(DateTime.Now);
		context.Response.BufferOutput = false;
		
        try
        {
			string queryString = context.Request.Url.Query;
			string chartUrl = "http://chart.apis.google.com/chart" + queryString;

			// Create a web request to the URL   
			HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(chartUrl);

            // Get the web response   
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

			context.Response.ContentType = myResponse.ContentType;
			context.Response.StatusCode = myResponse.StatusCode.GetHashCode();
			context.Response.StatusDescription = myResponse.StatusDescription;

            // Make sure the response is valid   
            if (HttpStatusCode.OK == myResponse.StatusCode)
            {
                // Open the response stream   
                using (Stream myResponseStream = myResponse.GetResponseStream())
                {
                    // Create a 4K buffer to chunk the file   
                    byte[] myBuffer = new byte[4096];
                    int iBytesRead;
                    // Read the chunk of the web response into the buffer   
                    while (0 < (iBytesRead = myResponseStream.Read(myBuffer, 0, myBuffer.Length)))
                    {
                        // Write the chunk from the buffer to the response stream   
                        context.Response.OutputStream.Write(myBuffer, 0, iBytesRead);
                    }
                }
            }
        }
        catch (Exception ex)
        {
			context.Response.StatusCode = HttpStatusCode.BadRequest.GetHashCode();
			context.Response.StatusDescription = ex.Message;
        }   
	}
}