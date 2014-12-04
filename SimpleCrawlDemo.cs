// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="SimpleCrawlDemo.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SimpleCrawlDemo type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Globalization;
using System.Text;
using HtmlAgilityPack;
using System.Threading;

using NCrawler.Extensions;
using NCrawler.HtmlProcessor;
using NCrawler.LanguageDetection.Google;
using NCrawler.Interfaces;
using NCrawler.Demo.Extensions;
using NCrawler.MP3Processor;

namespace NCrawler.Demo
{
	public class SimpleCrawlDemo
	{
		#region Class Methods

		public static void Run()
		{
			NCrawlerModule.Setup();
			Console.Out.WriteLine("Simple crawl demo");

			// Setup crawler to crawl http://ncrawler.codeplex.com
			// with 1 thread adhering to robot rules, and maximum depth
			// of 2 with 4 pipeline steps:
			//	* Step 1 - The Html Processor, parses and extracts links, text and more from html
			//  * Step 2 - Processes PDF files, extracting text
			//  * Step 3 - Try to determine language based on page, based on text extraction, using google language detection
			//  * Step 4 - Dump the information to the console, this is a custom step, see the DumperStep class
			//using (Crawler c = new Crawler(new Uri("http://ncrawler.codeplex.com"),
           //using (Crawler c = new Crawler(new Uri("http://www.indulap.ro"),
            using (Crawler c = new Crawler(new Uri("http://www.emag.ro"),
            //using (Crawler c = new Crawler(new Uri("http://www.emag.ro/pachet-maturizarea-lui-az-gabrielson-al-saptelea-fiu-jay-amory-linda-mcnabb-2000412538250/pd/DZ66QBBBM/"),
				new HtmlDocumentProcessor(), // Process html
				new iTextSharpPdfProcessor.iTextSharpPdfProcessor(), // Add PDF text extraction
				new GoogleLanguageDetection(), // Add language detection
				new Mp3FileProcessor(), // Add language detection
				new DumperStep())
				{
					// Custom step to visualize crawl
					//MaximumThreadCount = 2,
                    MaximumThreadCount = 1,
					MaximumCrawlDepth = 10,
					ExcludeFilter = Program.ExtensionsToSkip,
				})
			{
				// Begin crawl
				c.Crawl();
			}
		}

		#endregion
	}

	#region Nested type: DumperStep

	/// <summary>
	/// Custom pipeline step, to dump url to console
	/// </summary>
	internal class DumperStep : IPipelineStep
	{
		#region IPipelineStep Members

		/// <summary>
		/// </summary>
		/// <param name="crawler">
		/// The crawler.
		/// </param>
		/// <param name="propertyBag">
		/// The property bag.
		/// </param>
		public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            Thread.Sleep(2000);
            CultureInfo contentCulture = (CultureInfo)propertyBag["LanguageCulture"].Value;
            string cultureDisplayValue = "N/A";
            if (!contentCulture.IsNull())
            {
                cultureDisplayValue = contentCulture.DisplayName;
            }

            lock (this)
            {
                Console.Out.WriteLine(ConsoleColor.Gray, "Url: {0}", propertyBag.Step.Uri);
            }

            HtmlDocument htmlDoc = new HtmlDocument
            {
                OptionAddDebuggingAttributes = false,
                OptionAutoCloseOnEnd = true,
                OptionFixNestedTags = true,
                OptionReadEncoding = true
            };

            using (Stream reader = propertyBag.GetResponse())
            {
                Encoding documentEncoding = htmlDoc.DetectEncoding(reader);
                reader.Seek(0, SeekOrigin.Begin);
                if (!documentEncoding.IsNull())
                {
                    htmlDoc.Load(reader, documentEncoding, true);
                }
                else
                {
                    htmlDoc.Load(reader, true);
                }
            }

            //read price tag
            float price = 0;
            string strTitle = "";
            HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

            if (bodyNode != null)
            {
                //reead product name
                HtmlNode divsTitleTag = bodyNode.SelectSingleNode("//div[@id='offer-title']");
                if (null != divsTitleTag)
                {
                    //AgilityPack newer version can extract direclty the collection's node by name
                    //HtmlNode hTitle = divsTitleTag.ChildNodes["h1"];
                    HtmlNodeCollection hTitleNodes = divsTitleTag.ChildNodes;
                    bool bIsTitle = false;
                    foreach (HtmlNode node in hTitleNodes)
                    {
                        if ("h1" == node.Name)
                        {
                            strTitle = node.InnerHtml;
                            Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tTitle:{0}", node.InnerHtml);
                            bIsTitle = true;
                            break;
                        }
                    }
                    if (!bIsTitle)
                    {
                        Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tInconsistent tag for name");
                    }
                }

                //read price tag
                HtmlNode divsPriceTag = bodyNode.SelectSingleNode("//div[@class='prices']");
                if (null != divsPriceTag)
                {
                    //HtmlNode spanPrice = divsPriceTag.ChildNodes["span"];
                    HtmlNodeCollection hPriceNodes = divsPriceTag.ChildNodes;
                    bool bIsPrice = false;
                    foreach (HtmlNode node in hPriceNodes)
                    {
                        if ("span" == node.Name)
                        {
                            string strPrice = node.GetAttributeValue("content", "");
                            price = float.Parse(strPrice, CultureInfo.InvariantCulture.NumberFormat);
                            Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tPrice:{0}", price.ToString());
                            bIsPrice = true;
                            break;
                        }
                    }

                    if (!bIsPrice)
                    {
                        Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tInconsistent tag for price");
                    }
                }

                //read price tag
                string strId = "";
                HtmlNode divsIdTag = bodyNode.SelectSingleNode("//div[@class='product-model']");
                if (null != divsIdTag)
                {
                    HtmlNodeCollection hIdNodes = divsIdTag.ChildNodes;
                    bool bIsId = false;
                    foreach (HtmlNode node in hIdNodes)
                    {
                        if ("span" == node.Name)
                        {
                            strId = node.InnerHtml;
                            Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tID:{0}", strId);
                            bIsId = true;
                            break;
                        }
                    }

                    if (!bIsId)
                    {
                        Console.Out.WriteLine(ConsoleColor.DarkGreen, "\tInconsistent tag for ID");
                    }
                }
            }
            Console.Out.WriteLine();
        }

		#endregion
	}

	#endregion
}