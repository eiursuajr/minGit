using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using Ektron.Cms;
using Ektron.Cms.Modules;
using Microsoft.VisualBasic;

	public partial class chart : System.Web.UI.Page
	{
		protected EkModule objMod;
		protected ContentAPI m_refContentApi = new ContentAPI();
		private void Page_Load(System.Object sender, System.EventArgs e)
		{
			//Put user code to initialize the page here
			int i;
			
			
			int lMapWidth = 600;
			int lMapHeight = 500;
			string strRptDisplay = "0";
			System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
			Graphics objGraphics;
			int xPos = 200;
			int yPos = 300;
			int xInterval = 20;
			decimal dbPercent = 0;
			StringFormat SF = new StringFormat();
			SF.FormatFlags = StringFormatFlags.DirectionVertical;
			string[] arrValues = null;
			string[] arrFieldValues = null;
			string[] aTotalCount = null;
			string[] strFolderNames;
			string[] arrValueNames = new string[2];
			string strFromPage = "";
			string[] strFieldOptionNames = null;
			string[] strFieldNames = null;
			int arrValuesLength = 0;
			bool bShowPercent = false;
			PointF symbolLeg;
			PointF descLeg;
			int xDimension = 200;
			int yDimension = 200;
			long FormId;
			ArrayList arrItem;
			Collection cForm;
			ArrayList arrResult = new ArrayList();
			Hashtable hshQuestions = new Hashtable();
			int llResponses = 1;
			
			
			
			if (Request.QueryString["showLabels"] == null)
			{
				
				if (!(Request.QueryString["grpdisplay"] == null))
				{
					strRptDisplay = Request.QueryString["grpdisplay"];
                    if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Combined)))
					{
						bShowPercent = true;
					}
				}
				
				if (!(Request.QueryString["form_page"] == null))
				{
					strFromPage = Request.QueryString["form_page"];
					FormId = System.Convert.ToInt64(Request.QueryString["FormId"]);
					llResponses = System.Convert.ToInt32(Request.QueryString["responses"]);
					if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Pie)))
					{
						//the total will be re-calculated for the pie chart below
						llResponses = 0;
					}
					else if (llResponses < 1) //it causes mathematical error if llResponses is <= 0
					{
						llResponses = 1;
					}
					objMod = m_refContentApi.EkModuleRef;
					
					//originally from the formresponse.aspx.vb
					cForm = objMod.GetFormById(FormId); //collection
					arrResult = m_refContentApi.GetFormDataHistogramById(FormId); //array
					hshQuestions = m_refContentApi.GetFormFieldQuestionsById(FormId); //hashtable
					
					//chart.Visible = True
					
					// Now we have the data get the values
					//For Each item In FormStats
					//strNames = "18-21,22-25,26-30,31-40,41-50,51-60,61-over:10k-20k,21k-30k,31k-40k:High School,Some College,Degree(Associates),Master,Doctoral,Professional"
					//strStale = "10,30,25,10,5,5,15:10,50,40:10,10,10,10,10,10"
					//strFieldNames = "Age range:Annual Income:Education level"
					
					//EktComma is used to retain the commas in the fields and field option names
					int idx;
					int j;
					int iOptionHit = 0;
					
					//questions array
					if (hshQuestions.Count > 0 && arrResult.Count > 0)
					{
						strFieldNames = new string[arrResult.Count - 1 + 1];
						for (idx = 0; idx <= arrResult.Count - 1; idx++)
						{
							arrItem = (ArrayList)arrResult[idx];
							if (arrItem.Count > 0)
							{
								//sFieldNames = sFieldNames & ":" & hshQuestions(cItem.Item(0).ToString().Replace(",", "EktComma"))
								strFieldNames[idx] = Ektron.Cms.Common.EkFunctions.HtmlDecode(hshQuestions[arrItem[0].ToString()].ToString());
							}
						}
					}
					
					string sSubmit;
					int iSubmit;
					int iMaxSubmit = 0;
					arrItem = null;
					//For Each cItem In cResult
					if (arrResult.Count > 0)
					{
						strFieldOptionNames = new string[arrResult.Count - 1 + 1];
						arrFieldValues = new string[arrResult.Count - 1 + 1];
						aTotalCount = new string[arrResult.Count - 1 + 1];
						for (idx = 0; idx <= arrResult.Count - 1; idx++)
						{
							arrItem = (ArrayList)arrResult[idx];
							if (arrItem.Count > 1)
							{
								for (j = 1; j <= arrItem.Count - 1; j++)
								{
									//option text list
									strFieldOptionNames[idx] = (string) (strFieldOptionNames[idx] + arrItem[j].ToString().Substring(0, System.Convert.ToInt32(arrItem[j].ToString().LastIndexOf(",") - 5)) + "{sep}"); //Count = 5 chars
									sSubmit = (string) (arrItem[j].ToString().Substring(System.Convert.ToInt32(arrItem[j].ToString().LastIndexOf(",") + 1)));
									iSubmit = int.Parse(sSubmit.ToString().Substring(System.Convert.ToInt32(sSubmit.ToString().IndexOf("/") + 1)));
									iOptionHit = int.Parse(sSubmit.ToString().Substring(0, sSubmit.ToString().IndexOf("/")));
									if (iSubmit > iMaxSubmit)
									{
										iMaxSubmit = iSubmit;
									}
									//iOptionHit = arrItem.Item(j).ToString().Substring(arrItem.Item(j).ToString().LastIndexOf(",") + 1)
                                    if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Pie)))
									{
										//option count list
										aTotalCount[idx] = Convert.ToString(Convert.ToInt32(aTotalCount[idx]) + iOptionHit);
										arrFieldValues[idx] = (string) (arrFieldValues[idx] + iOptionHit + ",");
									}
									else
									{
										//option count (in percent) list
										arrFieldValues[idx] = (string) (arrFieldValues[idx] + System.Convert.ToInt32((iOptionHit * 100) / llResponses) + ",");
									}
									iOptionHit = 0;
								}
								//option text list
								strFieldOptionNames[idx] = (string) (strFieldOptionNames[idx].Substring(0, System.Convert.ToInt32(strFieldOptionNames[idx].Length - 5))); // {sep} = 5 chars
								//option count (in percent) list
								arrFieldValues[idx] = (string) (arrFieldValues[idx].Substring(0, System.Convert.ToInt32(arrFieldValues[idx].Length - 1)));
							}
						}
						//sFieldOptionNames = Server.UrlEncode(sFieldOptionNames.Substring(0, sFieldOptionNames.Length - 1))
						//sFieldOptionValues = sFieldOptionValues.Substring(0, sFieldOptionValues.Length - 1)
						arrValuesLength = arrItem.Count + arrResult.Count;
					}
				}
				
				if (strFromPage == "")
				{
					arrValueNames[0] = "Updated Content";
					arrValueNames[1] = "Stale Content";
					arrValues = Request.QueryString["stale"].Split(",".ToCharArray());
					arrValuesLength = arrValues.Length;
				}

                if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.DataTable)))
				{
					strFolderNames = Request.QueryString["names"].Split(",".ToCharArray());
				}
				else if (strFromPage == "form_page")
				{
					//strFieldOptionNames = Server.UrlDecode(Request.QueryString("fieldOptionNames")).Split(":")
					//strFieldNames = Server.UrlDecode(Request.QueryString("fieldNames")).Split(":")
					//arrFieldValues = Request.QueryString("FormValues").Split(":") 'Values for multiple fields separated by :
					//arrValuesLength = Request.QueryString("FormValues").Split(",").Length + strFieldNames.Length
				}

                if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.DataTable)))
				{
					lMapWidth = System.Convert.ToInt32(220 + (arrValuesLength * 40) + (arrValuesLength - 1) * 20);
					yPos = 400;
					lMapHeight = yPos + 20;
				}

                if ((strRptDisplay == "0") || (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Combined))))
				{
					lMapHeight = System.Convert.ToInt32(220 + (arrValuesLength * 40) + (arrValuesLength - 1) * 20);
				}
				
				int NumOfLegends = 0;
				
				if ((strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Pie)) && (strFromPage == "form_page")))
				{
					lMapWidth = System.Convert.ToInt32(750 + (40 * arrValuesLength) + 700);
					//lMapHeight = (225 * strFieldNames.Length)  'give 500 as padding
					for (i = 0; i <= arrFieldValues.GetUpperBound(0); i++)
					{
						NumOfLegends = NumOfLegends + arrFieldValues[i].Split(",".ToCharArray()).GetLength(0);
						if ((20 * NumOfLegends) > 400)
						{
							lMapHeight = lMapHeight + ((20 * NumOfLegends) + 30);
						}
						else
						{
							lMapHeight = lMapHeight + 450;
						}
					}
					//If (300 * strFieldNames.Length) > ((20 * NumOfLegends) + (40 * strFieldNames.Length)) Then
					//    lMapHeight = 300 * strFieldNames.Length
					//Else
					//    lMapHeight = (20 * NumOfLegends) + (40 * strFieldNames.Length)
					//End If
					yPos = lMapHeight;
				}
				System.Drawing.Bitmap objBitMap = new System.Drawing.Bitmap(lMapWidth, lMapHeight);
				
				objGraphics = Graphics.FromImage(objBitMap);
				
				objGraphics.Clear(Color.White);
				
				if (strFromPage == "")
				{
					objGraphics.DrawString("Stale Content Report", drawFont, Brushes.Black, new PointF(5, 5));
					
					symbolLeg = new PointF(lMapWidth - 190, 20);
					
					descLeg = new PointF(lMapWidth - 165, 16);
					
					for (i = 0; i <= arrValueNames.Length - 1; i++)
					{
						
						objGraphics.FillRectangle(new SolidBrush(GetColor(i % 2)), symbolLeg.X, symbolLeg.Y, 10, 10);
						objGraphics.DrawRectangle(Pens.Black, symbolLeg.X, symbolLeg.Y, 10, 10);
						
						objGraphics.DrawString((string) (arrValueNames[i].ToString()), drawFont, Brushes.Black, descLeg);
						
						symbolLeg.Y += 15;
						
						descLeg.Y += 15;
						
					}
				}
				//Loop through the values to create the Bar Chart.

                if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.DataTable)))
				{
					int j;
					xPos = 50;
					for (i = 0; i <= arrValuesLength - 1; i++)
					{
						// Vertical display
						objGraphics.DrawLine(Pens.Black, xPos, 50, xPos, yPos); // Vertical axis
						objGraphics.DrawString("Percent Stale Content ->", drawFont, Brushes.Black, new PointF(xPos, 20), SF);
						objGraphics.DrawLine(Pens.Black, xPos, yPos, xPos + lMapWidth, yPos); // Horizontal axis
						
						objGraphics.DrawString("Content Folders ->", drawFont, Brushes.Black, new PointF(xPos + lMapWidth - 180, yPos));
						
						for (j = 0; j <= 10; j++)
						{
							objGraphics.DrawLine(Pens.Black, xPos - 2, yPos - (30 * j), xPos + 2, yPos - (30 * j));
							objGraphics.DrawString( Convert.ToString((10 * j)), drawFont, Brushes.Black, new PointF(xPos - 25, yPos - (30 * j) - 5));
							//objGraphics.DrawString("Test", drawFont, Brushes.Black, New PointF(xPos - 50, yPos - (30 * j) - 5))
						}
						
						//objGraphics.DrawString("yPos = " & yPos & " x =" & xPos + xInterval, drawFont, Brushes.Black, New PointF(xPos + xInterval, yPos - dbPercent))
						
						if (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))) > 0)
						{
							dbPercent = (decimal) (System.Math.Round(System.Convert.ToDouble(((System.Convert.ToDouble(arrValues[i].Substring(0, System.Convert.ToInt32(arrValues[i].IndexOf(":"))))) / (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))))) * 100), 3));
						}
						else
						{
							dbPercent = 0;
						}
						
						objGraphics.FillRectangle(new SolidBrush(GetColor(0)), xPos + xInterval, Convert.ToSingle(Convert.ToDecimal(yPos) - (dbPercent * 3)), 20, System.Convert.ToInt32(dbPercent * 3));
                        objGraphics.DrawRectangle(Pens.Black, xPos + xInterval, Convert.ToSingle(Convert.ToDecimal(yPos) - (dbPercent * 3)), 20, System.Convert.ToInt32(dbPercent * 3));
						
						if (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))) > 0)
						{
							dbPercent = (decimal) (System.Math.Round(System.Convert.ToDouble((((System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1)))) - System.Convert.ToDouble(arrValues[i].Substring(0, System.Convert.ToInt32(arrValues[i].IndexOf(":"))))) / (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))))) * 100), 3));
						}
						
						xInterval = xInterval + 20;
                        objGraphics.FillRectangle(new SolidBrush(GetColor(1)), xPos + xInterval, Convert.ToSingle(Convert.ToDecimal(yPos) - (dbPercent * 3)), 20, System.Convert.ToInt32(dbPercent * 3));
                        objGraphics.DrawRectangle(Pens.Black, xPos + xInterval,Convert.ToSingle(yPos), 20, System.Convert.ToInt32(dbPercent * 3));
						//objGraphics.DrawString(strFolderNames(i).Substring(strFolderNames(i).LastIndexOf("\\") + 2), drawFont, Brushes.Black, New PointF(xPos + xInterval, yPos), SF)
						xInterval = xInterval + 40;
					}
				}
                else if ((strRptDisplay == "0") || ((strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Combined))) && (strFromPage == "form_page")))
				{
					// Horizontal display
					if (strFromPage == "form_page")
					{
						int j;
						bool bShowAxis = false;
						//Dim dScale As Double
						
						
						if (!(Request.QueryString["showAxis"] == null))
						{
							bShowAxis = System.Convert.ToBoolean(Request.QueryString["showAxis"]);
						}
						
						//If Not (Request.QueryString("scale") Is Nothing) Then
						//    dScale = CDbl(Request.QueryString("scale"))
						//End If
						
						lMapHeight = yPos + (arrValuesLength * 40) + (arrValuesLength * 20);
						//yPos = lMapHeight - 300
						yPos = 20;
						xPos = 0;
						//objGraphics.DrawString("Responses - " & Request.QueryString("responses"), drawFont, Brushes.Black, New PointF(xPos + 100, yPos - 20))
						
						for (j = 0; j <= strFieldOptionNames.Length - 1; j++)
						{
                            
							strFolderNames = System.Text.RegularExpressions.Regex.Split((string) (strFieldOptionNames[j].ToString()), "{sep}");
							arrValues = arrFieldValues[j].Split(",".ToCharArray());
							
							//objGraphics.DrawString(strFieldNames(j).Replace("EktComma", ","), drawFont, Brushes.Black, New PointF(xPos + 50, yPos + 10)) ' The Bar title below the x axis
							objGraphics.DrawString(strFieldNames[j], drawFont, Brushes.Black, new PointF(xPos + 50, yPos + 10)); // The Bar title below the x axis
							yPos = yPos + 15;
                            if (bShowAxis)
							{
								objGraphics.DrawLine(Pens.Black, xPos, yPos + (arrValues.Length) * 40, xPos, yPos); // Vertical axis
								objGraphics.DrawLine(Pens.Black, xPos, yPos + (arrValues.Length) * 40, xPos + lMapWidth, yPos + (arrValues.Length) * 40); // Horizontal axis
								
								for (i = 0; i <= 10; i++)
								{
									objGraphics.DrawLine(Pens.Black, xPos + (30 * i), System.Convert.ToInt32((yPos + (arrValues.Length) * 40) - 2), xPos + (30 * i), System.Convert.ToInt32((yPos + (arrValues.Length) * 40) + 2));
									objGraphics.DrawString( Convert.ToString((10 * i)), drawFont, Brushes.Black, new PointF(xPos + (30 * i), yPos + (arrValues.Length) * 40));
								}
							}
							yPos = yPos + xInterval;
							//xInterval = xInterval + (30 * arrValues.Length)
							for (i = 0; i <= arrValues.Length - 1; i++)
							{
								//strFolderNames(i) = strFolderNames(i).ToString().Replace("EktComma", ",")
								strFolderNames[i] = (string) (strFolderNames[i].ToString());
								dbPercent = (decimal) (System.Math.Round(double.Parse(arrValues[i]), 3)); //* dScale (?)
								objGraphics.FillRectangle(new SolidBrush(GetColor(i % 24)), xPos, yPos, System.Convert.ToInt32(dbPercent), 10);
								objGraphics.DrawRectangle(Pens.Black, xPos, yPos, System.Convert.ToInt32(dbPercent), 10);
								if (objGraphics.MeasureString(strFolderNames[i], drawFont).Width > xDimension)
								{
									xDimension = System.Convert.ToInt32(objGraphics.MeasureString(strFolderNames[i], drawFont).Width);
								}
								if (bShowPercent)
								{
									objGraphics.DrawString((string) (Ektron.Cms.Common.EkFunctions.HtmlDecode(strFolderNames[i]) + " (" + dbPercent.ToString() + "%)"), drawFont, Brushes.Black, new PointF(xPos, (yPos + 10)));
								}
								else
								{
									objGraphics.DrawString(Ektron.Cms.Common.EkFunctions.HtmlDecode(strFolderNames[i]), drawFont, Brushes.Black, new PointF(xPos, yPos + 10));
								}
								//xInterval = xInterval - 30
								yPos = yPos + 30;
							}
							//yPos = yPos + xInterval + 30
							yPos = yPos + 10;
							//xInterval = 20
						}
						yDimension = yPos;
					}
					else
					{
						lMapHeight = yPos + (arrValuesLength * 40) + (arrValuesLength * 20);
						yPos = lMapHeight - 200;
						
						objGraphics.DrawLine(Pens.Black, xPos, 50, xPos, yPos); // Vertical axis
						objGraphics.DrawLine(Pens.Black, xPos, yPos, xPos + lMapWidth, yPos); // Horizontal axis
						objGraphics.DrawString("Content Folders ->", drawFont, Brushes.Black, new PointF(xPos - 20, 10), SF);
						objGraphics.DrawString("Percent Stale Content ->", drawFont, Brushes.Black, new PointF(lMapWidth - 180, yPos + 15));
						
						for (i = 0; i <= 10; i++)
						{
							objGraphics.DrawLine(Pens.Black, xPos + (30 * i), yPos - 2, xPos + (30 * i), yPos + 2);
							objGraphics.DrawString( Convert.ToString((10 * i)), drawFont, Brushes.Black, new PointF(xPos + (30 * i), yPos));
						}
						
						for (i = 0; i <= arrValuesLength - 1; i++)
						{
							if (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))) > 0)
							{
								dbPercent = (decimal) (System.Math.Round(System.Convert.ToDouble(((System.Convert.ToDouble(arrValues[i].Substring(0, System.Convert.ToInt32(arrValues[i].IndexOf(":"))))) / (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))))) * 100), 3));
							}
							else
							{
								dbPercent = 0;
							}
							
							objGraphics.FillRectangle(new SolidBrush(GetColor(0)), xPos, yPos - xInterval, System.Convert.ToInt32(dbPercent * 3), 20);
							objGraphics.DrawRectangle(Pens.Black, xPos, yPos - xInterval, System.Convert.ToInt32(dbPercent * 3), 20);
							
							if (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))) > 0)
							{
								dbPercent = (decimal) (System.Math.Round(System.Convert.ToDouble((((System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1)))) - System.Convert.ToDouble(arrValues[i].Substring(0, System.Convert.ToInt32(arrValues[i].IndexOf(":"))))) / (System.Convert.ToDouble(arrValues[i].Substring(System.Convert.ToInt32(arrValues[i].IndexOf(":") + 1))))) * 100), 3));
							}
							
							xInterval = xInterval + 20;
							objGraphics.FillRectangle(new SolidBrush(GetColor(1)), xPos, yPos - xInterval, System.Convert.ToInt32(dbPercent * 3), 20);
							objGraphics.DrawRectangle(Pens.Black, xPos, yPos - xInterval, System.Convert.ToInt32(dbPercent * 3), 20);
							
							//objGraphics.DrawString(strFolderNames(i), drawFont, Brushes.Black, New PointF(xPos - 100, yPos - xInterval))
							xInterval = xInterval + 40;
						}
					}
					
					
				}
				else if (strRptDisplay == Convert.ToString(Convert.ToInt32(Ektron.Cms.Common.EkEnumeration.CMSFormReportType.Pie)))
				{
					float sglCurrentAngle = 0;
					float sglTotalAngle = 0;
					int llTotal = 0;
					int j;
					int idx;
					int xMax = 0;
					int yMax = 0;
					decimal dPercent = (decimal) 0.0;
					decimal dTotalCheck = (decimal) 0.0;
					int QuestionBoxHeight = 15;
					yPos = 0;
					xPos = 0;
					//objGraphics.DrawString("Responses - " & Request.QueryString("responses"), drawFont, Brushes.Black, New PointF(xPos + 100, yPos + 5))
					if (strFieldOptionNames.Length > 0)
					{
						strFolderNames = new string[strFieldOptionNames.Length - 1 + 1];
						for (j = 0; j <= strFieldOptionNames.Length - 1; j++)
						{
							strFolderNames = System.Text.RegularExpressions.Regex.Split((string) (strFieldOptionNames[j].ToString()), "{sep}");
							arrValues = arrFieldValues[j].Split(",".ToCharArray());
							llTotal = 0;
							for (i = 0; i <= arrValues.Length - 1; i++)
							{
								llTotal = Convert.ToInt32(llTotal) + Convert.ToInt32(arrValues[i]);
							}
							if (Convert.ToInt32(aTotalCount[j]) > 0)
							{
								QuestionBoxHeight = System.Convert.ToInt32(((System.Convert.ToInt32(strFieldNames[j].Length / 50)) + 1) * 20);
								objGraphics.DrawString((string) (strFieldNames[j].Replace("EktComma", ",")), drawFont, Brushes.Black, new RectangleF(xPos, yPos, 320, QuestionBoxHeight));
							}
							//Dim test As Integer = yPos
							for (i = 0; i <= arrValues.Length - 1; i++)
							{
								
								//Current Value / (sum of all the Values) * 360 degree angle
								if ( Convert.ToInt32(aTotalCount[j]) < 1)
								{
									break;
								}
                                sglCurrentAngle = System.Convert.ToSingle(Convert.ToDouble(Convert.ToDouble(arrValues[i]) / Convert.ToDouble(llTotal)) * 360);
								//yPos = yPos + (i * 200)
								//objGraphics.FillPie(New SolidBrush(GetColor(i Mod 16)), xPos, yPos - 230, 200, 200, sglTotalAngle, sglCurrentAngle)
								//objGraphics.DrawString(strFieldNames(j), drawFont, Brushes.Black, New PointF(xPos + 75, yPos - 275))
								//objGraphics.DrawPie(Pens.Black, xPos, yPos - 230, 200, 200, sglTotalAngle, sglCurrentAngle)
								objGraphics.FillPie(new SolidBrush(GetColor(i % 24)), xPos, yPos + QuestionBoxHeight, 200, 200, System.Convert.ToInt32(sglTotalAngle), System.Convert.ToInt32(sglCurrentAngle));
								//objGraphics.DrawString(strFieldNames(j).Replace("EktComma", ","), drawFont, Brushes.Black, New PointF(xPos, yPos))
								objGraphics.DrawPie(Pens.Black, xPos, yPos + QuestionBoxHeight, 200, 200, System.Convert.ToInt32(sglTotalAngle), System.Convert.ToInt32(sglCurrentAngle));
								yMax = yPos + QuestionBoxHeight + 200;
								//objGraphics.DrawLine(Pens.Blue, xPos + 100, yPos + 175, xPos + 200, yPos + 200)
								//objGraphics.DrawString("center = ", drawFont, Brushes.Black, New PointF(xPos + 100, yPos + 175))
								//objGraphics.DrawString("angle = " & sglCurrentAngle.ToString(), drawFont, Brushes.Black, New PointF(xPos, test))
								
								//test = test + 20
								sglTotalAngle += sglCurrentAngle;
							}
							yPos = yPos + QuestionBoxHeight;
							symbolLeg = new PointF(xPos + 210, yPos);
							descLeg = new PointF(xPos + 220, yPos);
							arrValueNames = System.Text.RegularExpressions.Regex.Split(strFieldOptionNames[j], "{sep}");
							//reset check total for the next set of results
							dTotalCheck = (decimal) 0.0;
							for (idx = 0; idx <= arrValueNames.Length - 1; idx++)
							{
								if ( Convert.ToInt32(aTotalCount[j]) < 1)
								{
									//to avoid division overflow
									break;
								}
								objGraphics.FillRectangle(new SolidBrush(GetColor(idx % 24)), symbolLeg.X, symbolLeg.Y, 10, 10);
								objGraphics.DrawRectangle(Pens.Black, symbolLeg.X, symbolLeg.Y, 10, 10);
								if (xMax < objGraphics.MeasureString((string) (arrValueNames[idx].ToString().Replace("EktComma", ",") + " (" + arrValues[idx] + " %)"), drawFont).Width)
								{
									xMax = System.Convert.ToInt32(objGraphics.MeasureString((string) (arrValueNames[idx].ToString().Replace("EktComma", ",") + " (" + arrValues[idx] + " %)"), drawFont).Width + 10);
								}
								dPercent = decimal.Round(Convert.ToDecimal( Convert.ToDouble(arrValues[idx]) /Convert.ToDouble(aTotalCount[j]) * 100), 2);
								//correct the percentage so if the total is over 100
								dTotalCheck = dTotalCheck + dPercent;
								if ( Convert.ToDouble(dTotalCheck) > 100.0 && dPercent > 0)
								{
									dPercent =  Convert.ToDecimal(Convert.ToDouble(dPercent) - (Convert.ToDouble(dTotalCheck) - 100.0));
									//reset check total for the current set of results
									dTotalCheck = (decimal) 100.0;
								}
								objGraphics.DrawString((string) (Ektron.Cms.Common.EkFunctions.HtmlDecode((string) (arrValueNames[idx].ToString().Replace("EktComma", ","))) + " (" + dPercent + " %)"), drawFont, Brushes.Black, descLeg);
								symbolLeg.Y += 20;
								descLeg.Y += 20;
							}
							yPos = System.Convert.ToInt32(descLeg.Y + 10);
							if (yPos < yMax)
							{
								yPos = yMax;
							}
						}
						if (yPos > yMax)
						{
							// more legends than the circle
							yMax = yPos;
						}
						yDimension = yMax; //- 30 'lMapHeight - 30
						xDimension = xMax + 200;
					}
				}
				//Loop through the values to create the Pie Chart.
				if (strFromPage == "form_page")
				{
					//Calculate exact dimensions
					objGraphics.Dispose();
					System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(xDimension + 25, yDimension + 30);
					objGraphics = Graphics.FromImage(bmp);
					objGraphics.DrawImage(objBitMap, 0, 0, lMapWidth, lMapHeight);
					bmp.Save(Response.OutputStream, ImageFormat.Gif);
				}
				else
				{
					objBitMap.Save(Response.OutputStream, ImageFormat.Gif);
				}
			}
			else
			{
				string strFolder = "";
				strFolder = (string) (Request.QueryString["showLabels"].ToString());
				
				System.Drawing.Bitmap objBitMap = new System.Drawing.Bitmap(50, 200);
				
				objGraphics = Graphics.FromImage(objBitMap);
				objGraphics.Clear(Color.White);
				if (strFolder.Length > 20)
				{
					strFolder = (string) (strFolder.Substring(0, 20) + "...");
				}
				
				objGraphics.DrawString(strFolder, drawFont, Brushes.Black, new PointF(0, 15), SF);
				objBitMap.Save(Response.OutputStream, ImageFormat.Gif);
			}
			
		}
		
		private Color GetColor(int itemIndex)
		{
			int ColorO = 0x99CCFF; //light blue
			// These color is selected in the order of opposite location in the Web Designer's Color Card
			// by Visibone.  It hopes to accommodate every espects including B&W printer and color-blindness.
			switch (itemIndex)
			{
				case 0:
					ColorO = 0x6666FF; //blue
					break;
				case 1:
					ColorO = 0xFFFF66; //yellow
					break;
				case 2:
					ColorO = 0x669999; //grey
					break;
				case 3:
					ColorO = 0x996666; //red
					break;
				case 4:
					ColorO = 0xCC99FF; //light purple
					break;
				case 5:
					ColorO = 0xCCFF99; //light grreen
					break;
				case 6:
					ColorO = 0xFF99CC; //light pink
					break;
				case 7:
					ColorO = 0x99FFCC; //light green
					break;
				case 8:
					ColorO = 0xCC66FF; //purple
					break;
				case 9:
					ColorO = 0x99FF66; //green
					break;
				case 10:
					ColorO = 0xFF66CC; //purple
					break;
				case 11:
					ColorO = 0x66FF99; //green
					break;
				case 12:
					ColorO = 0x666699; //purple
					break;
				case 13:
					ColorO = 0x999966; //green
					break;
				case 14:
					ColorO = 0x66FFFF; //blue
					break;
				case 15:
					ColorO = 0xFF6666; //pink
					break;
				case 16:
					ColorO = 0x9999FF; //Blue
					break;
				case 17:
					ColorO = 0x99CCCC; //gray
					break;
				case 18:
					ColorO = 0xFFFF99; //yellow
					break;
				case 19:
					ColorO = 0xCC9999; //pink
					break;
				case 20:
					ColorO = 0x9933FF; //Lavender
					break;
				case 21:
					ColorO = 0x99FF33; //green
					break;
				case 22:
					ColorO = 0x33FF99; //green
					break;
				case 23:
					ColorO = 0xFF3399; //dark pink
					break;
				default:
					ColorO = 0x99CCFF; //light blue
					break;
					
			}
			
			return ColorTranslator.FromOle(ColorO);
			
		}
	}
