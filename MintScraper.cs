// Copyright (c) 2011-2013, Sells Brothers, Inc. All rights reserved. No warranty extended. Use at your own risk. Some assembly required. Void where prohibited. Batties not included. Keep calm and carry on.
using LINQtoCSV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace MintScraper {
  class MintScraper {
    CookieContainer cookies;
    string homePageXml;

    public string User { get; set; }
    public string Password { get; set; }

    void Login() {
      if (cookies != null) { return; }

      cookies = new CookieContainer();
      const string postDataFormat = "username={0}&password={1}&remember=T&task=L&timezone=-8&nextPage=&browser=Explorer&browserVersion=9&os=v";
      const string loginUrl = @"https://wwws.mint.com/loginUserSubmit.xevent";

      string postData = string.Format(postDataFormat, User, Password);
      var request = (HttpWebRequest)WebRequest.Create(loginUrl);
      request.Method = "POST";
      request.ContentType = @"application/x-www-form-urlencoded";
      request.CookieContainer = cookies;
      using (var writer = new StreamWriter(request.GetRequestStream())) {
        writer.Write(postData);
      }

      var response = request.GetResponse();
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream)) {
        homePageXml = reader.ReadToEnd();
        // TODO: remove
        //File.WriteAllText(@"c:\temp\mintHomePage.html", homePageXml);
      }
      response.Close();
    }

    public string GetTransactionCsv() {
      Login();

      const string downloadUrl = @"https://wwws.mint.com/transactionDownload.event?";
      var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
      request.Method = "GET";
      request.CookieContainer = cookies;

      var tx = "";
      var response = request.GetResponse();
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream)) {
        tx = reader.ReadToEnd();
      }
      response.Close();
      return tx;
    }

    public string GetBalanceColumnHeaders() {
      var context = new CsvContext();
      var columns = new StringBuilder();
      var writer = new StringWriter(columns);
      context.Write<AccountBalance>(new AccountBalance[0], writer, new CsvFileDescription() { FirstLineHasColumnNames = true });
      return columns.ToString();
    }

    public string GetBalanceCsv(bool includeColumnHeaders = true) {
      Login();

      // get the user token from the home page HTML
      var token = GetToken();

      // download the JSON using the user token
      var json = GetBalanceJson(token);
      // TODO: remove
      //File.WriteAllText(@"c:\temp\mintAccounts.json", json);

      // parse the JSON into account balance data
      var jobj = JObject.Parse(json);
      var balances = new List<AccountBalance>();
      var accounts = jobj["response"]["90371"]["response"].Where(a => !(bool)a["isClosed"] && (bool)a["isActive"] && (string)a["accountSystemStatus"] != "DEAD");

      foreach (var account in accounts) {
        balances.Add(new AccountBalance() {
          AccountId = (string)account["accountId"],
          Balance = (Decimal)account["value"] * ((string)account["accountType"] == "loan" ? -1 : 1),
          InstitutionName = (string)account["fiName"],
          LastUpdatedText = (string)account["lastUpdatedInString"],
          AccountName = (string)account["accountName"],
          AccountNumberLast4 = (string)account["yodleeAccountNumberLast4"],
          LastUpdated = (new DateTime(1970, 1, 1)).AddMilliseconds((long)account["lastUpdated"]),
        });
      }

      //var mint = XDocument.Parse(xml);
      //var accounts = mint.Descendants(XName.Get("li", ns)).Where(e => GetSafeValue(e.Attribute("class")) == "account refreshing" || GetSafeValue(e.Attribute("class")) == "account");
      //var balances = new List<AccountBalance>();
      //foreach (var account in accounts) {
      //  var lastUpdated = account.Descendants(XName.Get("span", ns)).Single(e => GetSafeValue(e.Attribute("class")) == "last-updated" && !string.IsNullOrWhiteSpace(e.Value)).Value;
      //  var accountId = account.Attribute("id").Value.Substring(8);
      //  var balance = Decimal.Parse(account.Descendants(XName.Get("span", ns)).Single(e => GetSafeValue(e.Attribute("class")) == "balance").Value.Replace("–", "-"), NumberStyles.Currency);
      //  var institutionName = GetSafeValue(account.Descendants(XName.Get("a", ns)).SingleOrDefault(e => GetSafeValue(e.Attribute("class")) == ""));
      //  var accountName = account.Descendants(XName.Get("h6", ns)).First().Value;
      //  if (accountName.StartsWith(lastUpdated)) { accountName = accountName.Substring(lastUpdated.Length); }

      //  balances.Add(new AccountBalance() {
      //    AccountId = accountId,
      //    Balance = balance,
      //    InstitutionName = institutionName,
      //    LastUpdated = lastUpdated,
      //    AccountName = accountName,
      //  });
      //}

      // Render the balance data as CSV
      var context = new CsvContext();
      var csv = new StringBuilder();
      var writer = new StringWriter(csv);
      context.Write<AccountBalance>(balances, writer, new CsvFileDescription() { FirstLineHasColumnNames = false, EnforceCsvColumnAttribute = true });
      return csv.ToString();
    }

    private string GetBalanceJson(string token) {
      string downloadUrl = @"https://wwws.mint.com/bundledServiceController.xevent?token=" + token;
      var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
      request.Method = "POST";
      request.CookieContainer = cookies;
      request.Accept = "*/*";
      request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

      string body = @"input=%5B%7B%22args%22%3A%7B%22placement%22%3A%22HYPOTHESIS_TESTING%22%2C%22platform%22%3A%22Web%22%7D%2C%22service%22%3A%22MintAdviceService%22%2C%22task%22%3A%22getAdviceForUser%22%2C%22id%22%3A%22302299%22%7D%2C%7B%22args%22%3A%7B%22feature%22%3A%22businessbudgets%22%7D%2C%22service%22%3A%22MintNewFeatureEnablementService%22%2C%22task%22%3A%22isEnabled%22%2C%22id%22%3A%22272959%22%7D%2C%7B%22args%22%3A%7B%22feature%22%3A%22mintHB%22%7D%2C%22service%22%3A%22MintNewFeatureEnablementService%22%2C%22task%22%3A%22isEnabled%22%2C%22id%22%3A%22395330%22%7D%2C%7B%22args%22%3A%7B%22placement%22%3A%22OVERVIEW_PAGE%22%2C%22platform%22%3A%22Web%22%7D%2C%22service%22%3A%22MintAdviceService%22%2C%22task%22%3A%22getAdviceForUser%22%2C%22id%22%3A%22996237%22%7D%2C%7B%22args%22%3A%7B%22propertyName%22%3A%22prefModuleRemindersClosed%22%7D%2C%22service%22%3A%22MintUserService%22%2C%22task%22%3A%22getUserProperty%22%2C%22id%22%3A%22537657%22%7D%2C%7B%22args%22%3A%7B%22startDate%22%3A1363128676000%2C%22numOfDays%22%3A100%7D%2C%22service%22%3A%22MintUserEventService%22%2C%22task%22%3A%22getUserEventOccurrencesForTimeline%22%2C%22id%22%3A%22695775%22%7D%2C%7B%22args%22%3A%7B%22startDate%22%3A1365893476000%2C%22numOfDays%22%3A28%7D%2C%22service%22%3A%22MintUserEventService%22%2C%22task%22%3A%22getUserEventOccurrences%22%2C%22id%22%3A%22847875%22%7D%2C%7B%22args%22%3A%7B%22month%22%3A1365893476000%7D%2C%22service%22%3A%22MintBudgetService%22%2C%22task%22%3A%22getBusinessPersonalSpendingBudgets%22%2C%22id%22%3A%22560768%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintTurboTaxProductService%22%2C%22task%22%3A%22getUserTaxSeasonInfo%22%2C%22id%22%3A%22901377%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintTurboTaxProductService%22%2C%22task%22%3A%22getTurboTaxProductSuggestion%22%2C%22id%22%3A%22490618%22%7D%2C%7B%22args%22%3A%7B%22feature%22%3A%22goals%22%7D%2C%22service%22%3A%22MintNewFeatureEnablementService%22%2C%22task%22%3A%22isEnabled%22%2C%22id%22%3A%22358880%22%7D%2C%7B%22args%22%3A%7B%22activeOnly%22%3Atrue%7D%2C%22service%22%3A%22MintUserGoalService%22%2C%22task%22%3A%22getUserGoalsOrderedByProjectedDate%22%2C%22id%22%3A%22248405%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintUserGoalService%22%2C%22task%22%3A%22getUserGoalActions%22%2C%22id%22%3A%22890506%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintInvestmentService%22%2C%22task%22%3A%22getPortfolioMoversAndShakers%22%2C%22id%22%3A%22731340%22%7D%2C%7B%22args%22%3A%7B%22moversSponsored%22%3A%22movers.sponsored%22%2C%22countryId%22%3A1%7D%2C%22service%22%3A%22MintOfferService%22%2C%22task%22%3A%22loadDynamicOffer%22%2C%22id%22%3A%2258331%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintUserService%22%2C%22task%22%3A%22getUserProfile%22%2C%22id%22%3A%22507573%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestCreditCardOffer%22%2C%22id%22%3A%22782951%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestCheckingOffer%22%2C%22id%22%3A%22272477%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestSavingsOffer%22%2C%22id%22%3A%22411362%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestCDOffer%22%2C%22id%22%3A%22709155%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestBrokerageOffer%22%2C%22id%22%3A%22760187%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMatchedOfferService%22%2C%22task%22%3A%22getBestIRARolloverOffer%22%2C%22id%22%3A%22191468%22%7D%2C%7B%22args%22%3A%7B%22numMonths%22%3A6%7D%2C%22service%22%3A%22MintTransactionService%22%2C%22task%22%3A%22getCashFlow%22%2C%22id%22%3A%22694623%22%7D%2C%7B%22args%22%3A%7B%22types%22%3A%5B%22BANK%22%2C%22CREDIT%22%2C%22INVESTMENT%22%2C%22LOAN%22%2C%22MORTGAGE%22%2C%22OTHER_PROPERTY%22%2C%22REAL_ESTATE%22%2C%22VEHICLE%22%2C%22UNCLASSIFIED%22%5D%7D%2C%22service%22%3A%22MintAccountService%22%2C%22task%22%3A%22getAccountsSortedByBalanceDescending%22%2C%22id%22%3A%2290371%22%7D%2C%7B%22args%22%3A%7B%22feature%22%3A%22loan_transaction%22%7D%2C%22service%22%3A%22MintNewFeatureEnablementService%22%2C%22task%22%3A%22isEnabled%22%2C%22id%22%3A%22261425%22%7D%2C%7B%22args%22%3A%7B%22feature%22%3A%22investments%22%7D%2C%22service%22%3A%22MintNewFeatureEnablementService%22%2C%22task%22%3A%22isEnabled%22%2C%22id%22%3A%2286022%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintUserService%22%2C%22task%22%3A%22getUserPreferences%22%2C%22id%22%3A%22394611%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintFILoginService%22%2C%22task%22%3A%22isUserFILoginRefreshing%22%2C%22id%22%3A%22752211%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintTurboTaxProductService%22%2C%22task%22%3A%22getTaxSeasonInfo%22%2C%22id%22%3A%22103823%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintTurboTaxProductService%22%2C%22task%22%3A%22getTurboTaxWelcomeMatInfo%22%2C%22id%22%3A%22872747%22%7D%2C%7B%22args%22%3A%7B%7D%2C%22service%22%3A%22MintMyBusinessProductService%22%2C%22task%22%3A%22showMyBusinessWelcomeMat%22%2C%22id%22%3A%22514207%22%7D%5D";
      byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
      request.ContentLength = postBytes.Length;
      using (var stream = request.GetRequestStream()) {
        stream.Write(postBytes, 0, postBytes.Length);
      }

      using (var response = request.GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream)) {
        return reader.ReadToEnd();
      }
    }

    string GetToken() {
      // Parse the XHTML for user's data
      var ns = "http://www.w3.org/1999/xhtml";
      var xml = homePageXml
        .Replace("&rsquo;", "'")
        .Replace("&nbsp;", " ")
        .Replace("&copy;", "")
        .Replace("&ndash;", "-")
        .Replace("&mdash;", "-")
        .Replace("&mdash;", "-")
        .Replace("&hellip;", "-")
        ;
      var mint = XDocument.Parse(xml);
      var jsUserInput = mint.Descendants(XName.Get("input", ns)).Where(e => GetSafeValue(e.Attribute("id")) == "javascript-user").FirstOrDefault();
      if (jsUserInput == null) { return null; }

      // pull the user's API token out of the hidden form element
      string value = jsUserInput.Attribute(XName.Get("value")).Value;
      string json = System.Uri.UnescapeDataString(value);
      var jobj = JObject.Parse(json);
      return jobj["token"].Value<string>();
    }

    static string GetSafeValue(XElement element) {
      return element == null ? "" : element.Value;
    }

    static string GetSafeValue(XAttribute attribute) {
      return attribute == null ? "" : attribute.Value;
    }

    class AccountBalance {
      public AccountBalance() { Date = DateTime.Now; }

      [CsvColumn(FieldIndex = 0)]
      public DateTime Date { get; set; }

      [CsvColumn(FieldIndex = 1, Name = "Institution Name")]
      public string InstitutionName { get; set; }

      [CsvColumn(FieldIndex = 2, Name = "Mint Account ID")]
      public string AccountId { get; set; }

      [CsvColumn(FieldIndex = 3, Name = "Account Name")]
      public string AccountName { get; set; }

      [CsvColumn(FieldIndex = 4)]
      public Decimal Balance { get; set; }

      [CsvColumn(FieldIndex = 5, Name = "Last Updated Text")]
      public string LastUpdatedText { get; set; }

      [CsvColumn(FieldIndex = 6, Name = "Account Number Last 4")]
      public string AccountNumberLast4 { get; set; }

      [CsvColumn(FieldIndex = 7, Name = "Last Updated")]
      public DateTime LastUpdated { get; set; }

      [CsvColumn(FieldIndex = 8, Name = "Old Mint Account ID")]
      public string OldAccountId { get; set; }
    }

  }
}
