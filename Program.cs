// Copyright (c) 2011-2013, Sells Brothers, Inc. All rights reserved. No warranty extended. Use at your own risk. Some assembly required. Void where prohibited. Batties not included. Keep calm and carry on.

using CommandLine;
using CommandLine.Text;
using LINQtoCSV;
using System;
using System.IO;
using System.Linq;

namespace MintScraper {
  public class Transaction {
    public DateTime Date { get; set; }
    public string Description { get; set; }

    [CsvColumn(Name = "Original Description")]
    public string OriginalDescription { get; set; }

    public Decimal Amount { get; set; }

    [CsvColumn(Name = "Transaction Type")]
    public string TransactionType { get; set; }

    public string Category { get; set; }

    [CsvColumn(Name = "Account Name")]
    public string AccountName { get; set; }

    public string Labels { get; set; }
    public string Notes { get; set; }
  }

  class Options {
    [Option('t', "tx-file", Required = true, HelpText = "The path to the transactions file.")]
    public string TransactionsFile { get; set; }

    [Option('b', "bal-file", Required = true, HelpText = "The path to the balances file.")]
    public string BalancesFile { get; set; }

    [Option('l', "log-file", Required = false, HelpText = "The path to the log file.")]
    public string LogFile { get; set; }

    [Option('u', "user-name", Required = true, HelpText = "The mint.com user name.")]
    public string MintUserName { get; set; }

    [Option('p', "password", Required = true, HelpText = "The mint.com password.")]
    public string MintPassword { get; set; }

    [HelpOption]
    public string GetUsage() {
      return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
    }
  }

  class Program {
    static void Main(string[] args) {
      StreamWriter writer = null;

      try {
        var options = new Options();
        if (!CommandLine.Parser.Default.ParseArguments(args, options)) {
          return;
        }

        string txFile = options.TransactionsFile;
        string balanceFile = options.BalancesFile;
        string user = options.MintUserName;
        string password = options.MintPassword;
        string logfile = options.LogFile;

        // if we're handed a file to use as a logfile, log standard and error output
        if (!string.IsNullOrWhiteSpace(logfile)) {
          writer = new StreamWriter(logfile, true);
          Console.SetError(writer);
          Console.SetOut(writer);
          Console.WriteLine();
        }

        Console.WriteLine("{0}: Downloading mint.com transactions to {1}", DateTime.Now, txFile);
        var scraper = new MintScraper() { User = user, Password = password };

        // Get new balances data
        if (!File.Exists(balanceFile)) {
          File.WriteAllText(balanceFile, scraper.GetBalanceColumnHeaders() + "\r\n");
        }
        string balances = scraper.GetBalanceCsv(false);
        File.AppendAllText(balanceFile, balances);

        var balanceLines = 0;
        var reader = new StringReader(balances);
        while (reader.ReadLine() != null) { ++balanceLines; }
        Console.WriteLine("Downloaded {0} account balances as of {1:M/d/yyy}", balanceLines, DateTime.Now);

        // Get all transaction data
        File.WriteAllText(txFile, scraper.GetTransactionCsv());

        var txs = (new CsvContext()).Read<Transaction>(txFile);
        var count = txs.Count();
        var ordered = txs.OrderBy(t => t.Date);
        Console.WriteLine("Downloaded {0} transactions from {1:M/d/yyy} to {2:M/d/yyy}", count, ordered.First().Date, ordered.Last().Date);
      }
      catch (Exception ex) {
        Console.WriteLine("Exception: {0}", ex.Message);
      }
      finally {
        if (writer != null) { writer.Close(); }
      }
    }
  }
}
