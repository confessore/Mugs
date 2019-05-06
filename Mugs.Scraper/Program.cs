using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mugs.Models;
using Mugs.Services;
using Mugs.Services.Interfaces;
using Mugs.Services.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Mugs.Scraper
{
    class Program
    {
        IConfiguration Configuration { get; }
        IServiceProvider Services { get; }

        Program()
        {
            Configuration = BuildConfiguration();
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();
            DatabaseReady();
            while (true)
            {
                Console.WriteLine($"[{DateTime.Now}]    beginning update");
                var url = "http://www.mugshotsocala.com/";
                IteratePages(url);
                Console.WriteLine($"[{DateTime.Now}]    finished update");
                Thread.Sleep(900000);
            }
        }

        IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMySqlQuerier, MySqlQuerier>();
            services.Configure<MySqlQuerierOptions>(Configuration.GetSection("ConnectionStrings"));
        }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                new Program();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        string Get(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        string Post(string url, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(bytes, 0, bytes.Length);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        void IterateInmates(string url, HtmlDocument document)
        {
            foreach (var node in document.DocumentNode.SelectNodes(@"//ul[@id='mugs']/li/a"))
            {
                var bookingUrl = $"{url}{node.Attributes["href"].Value}";
                if (InmateExists(Convert.ToUInt32(Regex.Match(bookingUrl, @"\d+").Value))) continue;
                var doc2 = GetHtmlDocument(bookingUrl);
                var inmate = new Inmate
                {
                    ImageUrl = GetInmateImageUrl(doc2, url),
                    Name = GetInmateName(doc2),
                    BookingNumber = GetInmateBookingNumber(doc2),
                    DateOfBooking = GetInmateDateOfBooking(doc2),
                    County = GetInmateCounty(doc2),
                    DateOfBirth = GetInmateDateOfBirth(doc2),
                    Age = GetInmateAge(doc2),
                    Gender = GetInmateGender(doc2),
                    Race = GetInmateRace(doc2)
                };
                foreach (var charge in GetInmateCharges(doc2))
                    inmate.Charges.Add(charge);
                InsertInmate(inmate);
                Console.WriteLine($"[{DateTime.Now}]    added inmate    [{inmate.Name}]");
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// iterate through pages
        /// </summary>
        /// <param name="url">must include the 'www' subdomain</param>
        /// <param name="pages">exclusive</param>
        void IteratePages(string url)
        {
            var response = Get(url);
            var doc = GetHtmlDocumentFromString(response);
            IterateInmates(url, doc);
            Thread.Sleep(5000);
            for (int i = 2; i < GetTotalPages(doc) + 1; i++)
            {
                var html = GetHtmlDocumentFromString(response);
                var eventTarget = GetEventTarget();
                var viewState = GetViewState(html);
                var viewStateGenerator = GetViewStateGenerator(html);
                var eventValidation = GetEventValidation(html);
                var content = FormData(eventTarget, viewState, viewStateGenerator, eventValidation, i);
                response = Post(url, content);
                IterateInmates(url, GetHtmlDocumentFromString(response));
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// iterate through pages
        /// </summary>
        /// <param name="url">must include the 'www' subdomain</param>
        /// <param name="pages">exclusive</param>
        void IteratePages(string url, int pages)
        {
            var response = Get(url);
            IterateInmates(url, GetHtmlDocumentFromString(response));
            for (int i = 2; i < pages; i++)
            {
                var html = GetHtmlDocumentFromString(response);
                var eventTarget = GetEventTarget();
                var viewState = GetViewState(html);
                var viewStateGenerator = GetViewStateGenerator(html);
                var eventValidation = GetEventValidation(html);
                var content = FormData(eventTarget, viewState, viewStateGenerator, eventValidation, i);
                response = Post(url, content);
                IterateInmates(url, GetHtmlDocumentFromString(response));
                Thread.Sleep(5000);
            }
        }

        HtmlDocument GetHtmlDocumentFromString(string html)
        {
            var tmp = new HtmlDocument();
            tmp.LoadHtml(html);
            return tmp;
        }

        string FormData(string eventTarget, string viewState, string viewStateGenerator, string eventValidation, int page) =>
            string.Format("__EVENTTARGET={0}&__EVENTARGUMENT={1}&__LASTFOCUS={2}&__VIEWSTATE={3}&__VIEWSTATEGENERATOR={4}&__EVENTVALIDATION={5}&{6}={7}",
                    eventTarget, string.Empty, string.Empty, viewState, viewStateGenerator, eventValidation, eventTarget, page);

        uint GetTotalPages(HtmlDocument document) =>
            Convert.ToUInt32(document.DocumentNode.SelectSingleNode(@"//span[@id='ContentPlaceHolder1_lblTotalPages']").InnerText);


        string GetEventTarget() =>
            HttpUtility.UrlEncode("ctl00$ContentPlaceHolder1$ddlPaging");

        string GetViewState(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__VIEWSTATE']").GetAttributeValue("value", string.Empty));

        string GetViewStateGenerator(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__VIEWSTATEGENERATOR']").GetAttributeValue("value", string.Empty));

        string GetEventValidation(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__EVENTVALIDATION']").GetAttributeValue("value", string.Empty));

        uint GetInmateAge(HtmlDocument document) =>
            Convert.ToUInt32(document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Age:']/following-sibling::text()").InnerText);

        uint GetInmateBookingNumber(HtmlDocument document) =>
            Convert.ToUInt32(document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Booking Number:']/following-sibling::text()").InnerText);

        DateTime GetInmateDateOfBooking(HtmlDocument document) =>
            DateTime.Parse(document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Booking on:']/following-sibling::text()").InnerText);

        List<Charge> GetInmateCharges(HtmlDocument document)
        {
            var tmp = new List<Charge>();
            foreach (var node in document.DocumentNode.SelectNodes(@"//div[@class='mugshotsArrestInfoDetail']/ul/li"))
            {
                tmp.Add(
                    new Charge
                    {
                        ViolationCode = node.SelectSingleNode($"{node.XPath}/b[.='Violation Code:']/following-sibling::text()").InnerText.Trim(),
                        ViolationDescription = node.SelectSingleNode($"{node.XPath}/b[.='Violation Description:']/following-sibling::text()").InnerText.Trim(),
                    });
            }
            return tmp;
        }

        string GetInmateCounty(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='County:']/following-sibling::text()").InnerText.Trim();

        DateTime GetInmateDateOfBirth(HtmlDocument document) =>
            DateTime.Parse(document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Date of Birth:']/following-sibling::text()").InnerText);

        string GetInmateGender(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Gender:']/following-sibling::text()").InnerText.Trim();

        string GetInmateImageUrl(HtmlDocument document, string url) =>
            url + document.DocumentNode.SelectSingleNode("//img[@id='ContentPlaceHolder1_imgMug']").GetAttributeValue("src", string.Empty).Trim();

        string GetInmateRace(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/b[.='Race:']/following-sibling::text()").InnerText.Trim();

        HtmlDocument GetHtmlDocument(string url) =>
            new HtmlWeb().Load(url);

        string GetInmateName(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//div[@class='mugshotsNameDetail']/h2").InnerText.Trim();

        bool DatabaseReady()
        {
            var mySqlQuerier = Services.GetRequiredService<IMySqlQuerier>();
            if (mySqlQuerier.TableExistsAsync("mugs", "ocala").Result) return true;
            else
            {
                var tmp = new Dictionary<string, string>
                {
                    { "BookingNumber", "INT UNSIGNED NOT NULL PRIMARY KEY" },
                    { "Name", "VARCHAR(255)" },
                    { "DateOfBooking", "DATETIME" },
                    { "County", "VARCHAR(255)" },
                    { "DateOfBirth", "DATETIME" },
                    { "Age", "INT UNSIGNED" },
                    { "Gender", "VARCHAR(255)" },
                    { "Race", "VARCHAR(255)" },
                    { "Charges", "JSON" },
                    { "ImageUrl", "VARCHAR(255)" },
                    { "Display", "BIT DEFAULT 1" }
                };
                mySqlQuerier.TryCreateTableAsync("ocala", tmp).GetAwaiter();
                return true;
            }
        }

        bool InmateExists(uint bookingNumber)
        {
            var mySqlQuerier = Services.GetRequiredService<IMySqlQuerier>();
            return mySqlQuerier.InmateExistsAsync(bookingNumber).Result;
        }

        void InsertInmate(Inmate inmate)
        {
            var mySqlQuerier = Services.GetRequiredService<IMySqlQuerier>();
            mySqlQuerier.InsertInmateAsync(inmate).GetAwaiter();
        }
    }
}
