using HtmlAgilityPack;
using Mugs.Items;
using System;
using System.Collections.Generic;

namespace Mugs.Services.Interfaces
{
    public interface IHtmlParser
    {
        HtmlDocument GetHtmlDocumentFromString(string html);
        HtmlDocument GetHtmlDocumentFromUrl(string url);
        Tuple<List<Inmate>, HtmlDocument> FullParseInmatesOnNextPage(string url, HtmlDocument document);
        List<Inmate> PartialParseInmatesOnPage(string url, HtmlDocument document);
        Tuple<List<Inmate>, HtmlDocument> PartialParseInmatesOnNextPage(string url, HtmlDocument document);
        Tuple<List<Inmate>, HtmlDocument> PartialParseInmatesOnNextPage(string url, string uri, HtmlDocument document);
        Inmate IndividualInmateParse(Inmate inmate);
    }
}
