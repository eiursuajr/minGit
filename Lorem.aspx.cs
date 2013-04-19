using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Lorem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		LoremIpsum text = new LoremIpsum();
		
		litoutput.Text = WSOL.Messaging.Message(text.GetLoremIpsum(5, true), System.Diagnostics.EventLogEntryType.SuccessAudit);
	}

	public class LoremIpsum : System.Random
	{
		private int MinLength = 5;
		private int MaxLength = 50;
		private int NumberOfSentences = 5;

		#region Constructors

		public LoremIpsum()
		{ }

		#endregion

		#region Public Functions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="NumberOfWords"></param>
		/// <returns></returns>
		private string GetLoremIpsum(int NumberOfWords)
		{
			StringBuilder loremSentence = new StringBuilder();
			loremSentence.Append(ToFirstCharacterUpperCase(Words[Next(Words.Length)]));
			for (int i = 1; i < NumberOfWords; ++i)
			{
				loremSentence.Append(" ").Append(Words[Next(Words.Length)]);
			}
			loremSentence.Append(".");
			return loremSentence.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="NumberOfParagraphs"></param>
		/// <param name="includeHeadings"></param>
		/// <returns></returns>
		public string GetLoremIpsum(int NumberOfParagraphs, bool includeHeadings)
		{
			return GetLoremIpsum(NumberOfParagraphs, includeHeadings, false, MinLength, MaxLength);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="NumberOfParagraphs"></param>
		/// <param name="IncludeHeadings"></param>
		/// <param name="StartWithLoremIpsum"></param>
		/// <returns></returns>
		public string GetLoremIpsum(int NumberOfParagraphs, bool IncludeHeadings, bool StartWithLoremIpsum)
		{
			return GetLoremIpsum(NumberOfParagraphs, IncludeHeadings, StartWithLoremIpsum, MinLength, MaxLength);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="NumberOfParagraphs"></param>
		/// <param name="IncludeHeadings"></param>
		/// <param name="StartWithLoremIpsum"></param>
		/// <param name="MinSentenceLength"></param>
		/// <param name="MaxSentenceLength"></param>
		/// <returns></returns>
		public string GetLoremIpsum(int NumberOfParagraphs, bool IncludeHeadings, bool StartWithLoremIpsum, int MinSentenceLength, int MaxSentenceLength)
		{
			StringBuilder loremIpsum = new StringBuilder();

			if (IncludeHeadings)
				loremIpsum.Append("<h1>" + GetLoremIpsum(Next(5, 5)) + "</h1>");

			loremIpsum.Append("<p>");

			if (StartWithLoremIpsum)
				loremIpsum.Append("Lorem ipsum dolor sit amer. ");

			for (int i = 0; i < NumberOfSentences; ++i)
			{
				loremIpsum.Append(GetLoremIpsum(Next(MinSentenceLength, MaxSentenceLength))).Append(" ");
			}

			loremIpsum.Append("</p>");

			if (IncludeHeadings)
				loremIpsum.Append("<h2>" + GetLoremIpsum(Next(5, 5)) + "</h2>");

			for (int i = 1; i < NumberOfParagraphs; ++i)
			{
				if (IncludeHeadings)
					loremIpsum.Append("<h3>" + GetLoremIpsum(Next(5, 5)) + "</h3>");

				loremIpsum.Append("<p>");

				for (int x = 0; x < NumberOfSentences; ++x)
				{
					loremIpsum.Append(GetLoremIpsum(Next(MinSentenceLength, MaxSentenceLength))).Append(" ");
				}

				loremIpsum.Append("</p>");

			}
			return loremIpsum.ToString();
		}

		#endregion

		#region Private Variables

		private string[] Words = new string[] 
		{ 
			"consetetur","sadipscing","elitr","sed","diam","nonumy","eirmod","tempor","invidunt","ut","labore","et",
			"dolore","magna","aliquyam","erat","sed","diam","voluptua","at","vero","eos","et","accusam","et","justo",
			"duo","dolores","et","ea","rebum","stet","clita","kasd","gubergren","no","sea","takimata","sanctus","est",
			"lorem","ipsum","dolor","sit","amet","lorem","ipsum","dolor","sit","amet","consetetur","sadipscing","elitr",
			"sed","diam","nonumy","eirmod","tempor","invidunt","ut","labore","et","dolore","magna","aliquyam","erat",
			"sed","diam","voluptua","at","vero","eos","et","accusam","et","justo","duo","dolores","et","ea","rebum",
			"stet","clita","kasd","gubergren","no","sea","takimata","sanctus","est","lorem","ipsum","dolor","sit","amet",
			"lorem","ipsum","dolor","sit","amet","consetetur","sadipscing","elitr","sed","diam","nonumy","eirmod",
			"tempor","invidunt","ut","labore","et","dolore","magna","aliquyam","erat","sed","diam","voluptua",
			"at","vero","eos","et","accusam","et","justo","duo","dolores","et","ea","rebum","stet","clita",
			"kasd","gubergren","no","sea","takimata","sanctus","est","lorem","ipsum","dolor","sit","amet","duis",
			"autem","vel","eum","iriure","dolor","in","hendrerit","in","vulputate","velit","esse","molestie",
			"consequat","vel","illum","dolore","eu","feugiat","nulla","facilisis","at","vero","eros","et",
			"accumsan","et","iusto","odio","dignissim","qui","blandit","praesent","luptatum","zzril","delenit",
			"augue","duis","dolore","te","feugait","nulla","facilisi","lorem","ipsum","dolor","sit","amet",
			"consectetuer","adipiscing","elit","sed","diam","nonummy","nibh","euismod","tincidunt","ut","laoreet",
			"dolore","magna","aliquam","erat","volutpat","ut","wisi","enim","ad","minim","veniam","quis",
			"nostrud","exerci","tation","ullamcorper","suscipit","lobortis","nisl","ut","aliquip","ex","ea",
			"commodo","consequat","duis","autem","vel","eum","iriure","dolor","in","hendrerit","in","vulputate",
			"velit","esse","molestie","consequat","vel","illum","dolore","eu","feugiat","nulla","facilisis","at",
			"vero","eros","et","accumsan","et","iusto","odio","dignissim","qui","blandit","praesent","luptatum",
			"zzril","delenit","augue","duis","dolore","te","feugait","nulla","facilisi","nam","liber","tempor",
			"cum","soluta","nobis","eleifend","option","congue","nihil","imperdiet","doming","id","quod","mazim",
			"placerat","facer","possim","assum","lorem","ipsum","dolor","sit","amet","consectetuer","adipiscing",
			"elit","sed","diam","nonummy","nibh","euismod","tincidunt","ut","laoreet","dolore","magna","aliquam",
			"erat","volutpat","ut","wisi","enim","ad","minim","veniam","quis","nostrud","exerci","tation",
			"ullamcorper","suscipit","lobortis","nisl","ut","aliquip","ex","ea","commodo","consequat","duis",
			"autem","vel","eum","iriure","dolor","in","hendrerit","in","vulputate","velit","esse","molestie",
			"consequat","vel","illum","dolore","eu","feugiat","nulla","facilisis","at","vero","eos","et","accusam",
			"et","justo","duo","dolores","et","ea","rebum","stet","clita","kasd","gubergren","no","sea",
			"takimata","sanctus","est","lorem","ipsum","dolor","sit","amet","lorem","ipsum","dolor","sit",
			"amet","consetetur","sadipscing","elitr","sed","diam","nonumy","eirmod","tempor","invidunt","ut",
			"labore","et","dolore","magna","aliquyam","erat","sed","diam","voluptua","at","vero","eos","et",
			"accusam","et","justo","duo","dolores","et","ea","rebum","stet","clita","kasd","gubergren","no",
			"sea","takimata","sanctus","est","lorem","ipsum","dolor","sit","amet","lorem","ipsum","dolor","sit",
			"amet","consetetur","sadipscing","elitr","at","accusam","aliquyam","diam","diam","dolore","dolores",
			"duo","eirmod","eos","erat","et","nonumy","sed","tempor","et","et","invidunt","justo","labore",
			"stet","clita","ea","et","gubergren","kasd","magna","no","rebum","sanctus","sea","sed","takimata",
			"ut","vero","voluptua","est","lorem","ipsum","dolor","sit","amet","lorem","ipsum","dolor","sit",
			"amet","consetetur","sadipscing","elitr","sed","diam","nonumy","eirmod","tempor","invidunt","ut",
			"labore","et","dolore","magna","aliquyam","erat","consetetur","sadipscing","elitr","sed","diam",
			"nonumy","eirmod","tempor","invidunt","ut","labore","et","dolore","magna","aliquyam","erat","sed",
			"diam","voluptua","at","vero","eos","et","accusam","et","justo","duo","dolores","et","ea",
			"rebum","stet","clita","kasd","gubergren","no","sea","takimata","sanctus","est","lorem","ipsum" };
	}
	#endregion

	#region Helper Methods

	/// <summary>
	/// 
	/// </summary>
	/// <param name="Input"></param>
	/// <returns></returns>
	private static string ToFirstCharacterUpperCase(string Input)
	{
		if (string.IsNullOrEmpty(Input))
			return null;

		char[] InputChars = Input.ToCharArray();
		for (int i = 0; i < InputChars.Length; ++i)
		{
			if (InputChars[i] != ' ' && InputChars[i] != '\t')
			{
				InputChars[i] = char.ToUpper(InputChars[i]);
				break;
			}
		}

		return new string(InputChars);
	}

	#endregion

}