using HtmlAgilityPack;
using Mugs.Items;
using Mugs.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Mugs.Services
{
    public class HtmlParser : IHtmlParser
    {
        #region UrlMethods

        string GetUrl(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        string PostUrl(string url, string content)
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

        #endregion

        #region HtmlDocumentMethods

        public HtmlDocument GetHtmlDocumentFromString(string html)
        {
            var tmp = new HtmlDocument();
            tmp.LoadHtml(html);
            return tmp;
        }

        public HtmlDocument GetHtmlDocumentFromUrl(string url)
        {
            var tmp = new HtmlDocument();
            tmp.LoadHtml(GetUrl(url));
            return tmp;
        }

        #endregion

        #region FullParse

        public List<Inmate> FullParseInmatesOnPage(string url, HtmlDocument document)
        {
            var tmp = new List<Inmate>();
            foreach (var node in document.DocumentNode.SelectNodes(@"//ul[@id='mugs']/li/a"))
            {
                var bookingUrl = $"{url}{node.Attributes["href"].Value}";
                document = GetHtmlDocumentFromUrl(bookingUrl);
                var inmate = new Inmate
                {
                    ImageUrl = GetInmateDetailImageUrl(url, document),
                    Name = GetInmateDetailName(document),
                    BookingNumber = GetInmateDetailBookingNumber(document),
                    DateOfBooking = GetInmateDetailDateOfBooking(document),
                    County = GetInmateDetailCounty(document),
                    DateOfBirth = GetInmateDetailDateOfBirth(document),
                    Age = GetInmateDetailAge(document),
                    Gender = GetInmateDetailGender(document),
                    Race = GetInmateDetailRace(document)
                };
                foreach (var charge in GetInmateDetailCharges(document))
                    inmate.Charges.Add(charge);
                tmp.Add(inmate);
            }
            return tmp;
        }

        public Tuple<List<Inmate>, HtmlDocument> FullParseInmatesOnNextPage(string url, HtmlDocument document)
        {
            if (GetCurrentPage(document) != GetTotalPages(document))
            {
                var form = FormData(GetEventTarget(), GetViewState(document), GetViewStateGenerator(document),
                    GetEventValidation(document), GetNextPage(document));
                document = GetHtmlDocumentFromString(PostUrl(url, form));
                return new Tuple<List<Inmate>, HtmlDocument>(FullParseInmatesOnPage(url, document), document);
            }
            return new Tuple<List<Inmate>, HtmlDocument>(new List<Inmate>(), document);
        }

        #endregion

        #region PartialParse

        public List<Inmate> PartialParseInmatesOnPage(string url, HtmlDocument document)
        {
            var tmp = new List<Inmate>();
            foreach (var node in document.DocumentNode.SelectNodes(@"//ul[@id='mugs']/li/a"))
            {
                var inmate = new Inmate
                {
                    BookingUrl = GetInmateBookingUrl(url, node),
                    Name = GetInmateName(document, node),
                    DateOfBooking = GetInmateDateOfBooking(document, node),
                    ImageUrl = GetInmateImageUrl(url, document, node),
                };
                tmp.Add(inmate);
            }
            return tmp;
        }

        public Tuple<List<Inmate>, HtmlDocument> PartialParseInmatesOnNextPage(string url, HtmlDocument document)
        {
            if (GetCurrentPage(document) != GetTotalPages(document))
            {
                var form = FormData(GetEventTarget(), GetViewState(document), GetViewStateGenerator(document),
                    GetEventValidation(document), GetNextPage(document));
                document = GetHtmlDocumentFromString(PostUrl(url, form));
                return new Tuple<List<Inmate>, HtmlDocument>(PartialParseInmatesOnPage(url, document), document);
            }
            return new Tuple<List<Inmate>, HtmlDocument>(new List<Inmate>(), document);
        }

        #endregion

        #region IndividualParse

        public Inmate IndividualInmateParse(Inmate inmate)
        {
            var document = GetHtmlDocumentFromUrl(inmate.BookingUrl);
            inmate = new Inmate
            {
                ImageUrl = inmate.ImageUrl,
                Name = GetInmateDetailName(document),
                BookingNumber = GetInmateDetailBookingNumber(document),
                DateOfBooking = GetInmateDetailDateOfBooking(document),
                County = GetInmateDetailCounty(document),
                DateOfBirth = GetInmateDetailDateOfBirth(document),
                Age = GetInmateDetailAge(document),
                Gender = GetInmateDetailGender(document),
                Race = GetInmateDetailRace(document)
            };
            foreach (var charge in GetInmateDetailCharges(document))
                inmate.Charges.Add(charge);
            return inmate;
        }

        #endregion

        #region FormData

        string GetEventTarget() =>
            HttpUtility.UrlEncode("ctl00$ContentPlaceHolder1$ddlPaging");

        string GetViewState(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__VIEWSTATE']").GetAttributeValue("value", string.Empty));

        string GetViewStateGenerator(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__VIEWSTATEGENERATOR']").GetAttributeValue("value", string.Empty));

        string GetEventValidation(HtmlDocument document) =>
            HttpUtility.UrlEncode(document.DocumentNode.SelectSingleNode(@"//input[@id='__EVENTVALIDATION']").GetAttributeValue("value", string.Empty));

        uint GetCurrentPage(HtmlDocument document) =>
            Convert.ToUInt32(document.DocumentNode.SelectSingleNode(@"//option[@selected='selected']").GetAttributeValue("value", string.Empty));

        uint GetNextPage(HtmlDocument document) =>
            GetCurrentPage(document) + 1;

        uint GetTotalPages(HtmlDocument document) =>
            Convert.ToUInt32(document.DocumentNode.SelectSingleNode(@"//span[@id='ContentPlaceHolder1_lblTotalPages']").InnerText);

        string FormData(string eventTarget, string viewState, string viewStateGenerator, string eventValidation, uint page) =>
            string.Format("__EVENTTARGET={0}&__EVENTARGUMENT={1}&__LASTFOCUS={2}&__VIEWSTATE={3}&__VIEWSTATEGENERATOR={4}&__EVENTVALIDATION={5}&{6}={7}",
                    eventTarget, string.Empty, string.Empty, viewState, viewStateGenerator, eventValidation, eventTarget, page);

        #endregion

        #region InmateDetailData

        string GetInmateDetailAge(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Age:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailBookingNumber(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Booking Number:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailDateOfBooking(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Booking on:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        List<Charge> GetInmateDetailCharges(HtmlDocument document)
        {
            var tmp = new List<Charge>();
            var nodes = document.DocumentNode.SelectNodes(@"//div[@class='mugshotsArrestInfoDetail']/ul/li");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var code = node.SelectSingleNode($@"{node.XPath}/b[.='Violation Code:']/following-sibling::text()");
                    var description = node.SelectSingleNode($@"{node.XPath}/b[.='Violation Description:']/following-sibling::text()");
                    var bond = node.SelectSingleNode($@"{node.XPath}/b[.='Bond Amount:']/following-sibling::text()");
                    tmp.Add(
                        new Charge
                        {
                            ViolationCode = code != null ? code.InnerText.Trim() : string.Empty,
                            ViolationDescription = description != null ? description.InnerText.Trim() : string.Empty,
                            BondAmount = bond != null ? bond.InnerText.Trim() : string.Empty
                        });
                }
            }
            return tmp;
        }

        string GetInmateDetailCounty(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='County:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailDateOfBirth(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Date of Birth:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailGender(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Gender:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailImageUrl(string url, HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//img[@id='ContentPlaceHolder1_imgMug']");
            return tmp != null ? url + tmp.GetAttributeValue("src", string.Empty).Trim() : string.Empty;
        }

        string GetInmateDetailRace(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/b[.='Race:']/following-sibling::text()");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        string GetInmateDetailName(HtmlDocument document)
        {
            var tmp = document.DocumentNode.SelectSingleNode(
                @"//div[@class='mugshotsNameDetail']/h2");
            return tmp != null ? tmp.InnerText.Trim() : string.Empty;
        }

        #endregion

        #region InmateData

        string GetInmateName(HtmlDocument document, HtmlNode node) =>
            HttpUtility.HtmlDecode(document.DocumentNode.SelectSingleNode($@"{node.XPath}/div[@class='MugshotsName']").InnerText.Trim());

        string GetInmateDateOfBooking(HtmlDocument document, HtmlNode node) =>
            document.DocumentNode.SelectSingleNode($@"{node.XPath}/div[@class='MugshotsName']/br/following-sibling::text()").InnerText.Trim();

        string GetInmateBookingUrl(string url, HtmlNode node) =>
            url + node.Attributes["href"].Value;
        string GetInmateImageUrl(string url, HtmlDocument document, HtmlNode node) =>
            url + document.DocumentNode.SelectSingleNode($@"{node.XPath}/div[@class='mugImage']/img").GetAttributeValue("src", string.Empty).Trim();

        #endregion
    }
}
