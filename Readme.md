Welcome to MintScraper!
=======================

MintScraper is a little program I wrote to log into mint.com and download my
current set of account balances and my current set of transactions. The balances
are appended to an existing CSV file and the transactions replace the current
CSV file, since mint.com provides the transactions in their entirety every time.
This means that it's easy to go to mint.com, update budget settings, etc. and
then get the updates when you next run MintScraper.

MintScraper is meant to be used regularly (I've been using it for years on a
daily basis) to be able to track your account balances over time. This is
important because mint.com doesn't let you download your account balances over
time, only your transactions. I use this information for custom data queries in
Excel and in custom apps to be able to do budging and tracking.

Usage
=====

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
MintScraper 1.0.0.0 Copyright (c) Sells Brothers, Inc. 2011-2013
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -t, --tx-file      Required. The path to the transactions file.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -b, --bal-file     Required. The path to the balances file.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -l, --log-file     The path to the log file.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -u, --user-name    Required. The mint.com user name.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -p, --password     Required. The mint.com password.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  --help             Display this help screen.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



For example, I schedule MintScraper in the Windows Task Scheduler to run every
day at 3am with the following command line:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
c:\> MintScrapper.exe -t MintTransactions.csv -b MintBalances.csv -u  MINTUSER -p MINTPW -l mintlog.txt
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



The -l (--log-file) option is optional and will append to an existing log file
if you have one, which is what I do to see if there are any issues between
updates (it will keep track of errors). The output into the log file looks like
the following:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/16/2013 2:05:05 PM: Downloading mint.com transactions to MintTransactions.csv
Downloaded 37 account balances as of 11/16/2013 Downloaded 9970 transactions from 3/13/2009 to 11/16/2013
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



The transactions file is the mint.com file format, which looks like this:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
"Date","Description","Original Description","Amount","Transaction Type","Category","Account Name","Labels","Notes" 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/16/2013","J Jconnect Service","J2 *JCONNECT SERVICE","15.00","debit","Office Supplies","Chase Freedom Visa","","" "
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/01/2013","Advance","Advance","854.40","debit","Shopping","Equity Line","","" "10/24/2013","Transfer from Checking","$ Automatic Payment","1000.00","credit","Credit Card Payment","Equity Line","",""
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
...
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



The balances file is scraped from the HTML on the home page when you first log
into mint.com and has a file format that looks like this:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Date,Institution Name,Mint Account ID,Account Name,Balance,Last Updated Text,Account Number Last 4,Last Updated,
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/16/2013 2:21:18 PM,Chase Bank,REMOVED,Chase Freedom Visa,-9.47,15 minutes,YYYY,11/16/2013 10:06:05 PM,
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/16/2013 2:21:18 PM,American Express Credit Card,REMOVED,Costco TrueEarnings Card,0,15 minutes,XXXX-XXXXXX-XYYYY,11/16/2013 10:05:39 PM,
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
11/16/2013 2:21:18 PM,US Bank,REMOVED,loan,0,15 minutes,YYYY,11/16/2013 10:05:29 PM,
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
...
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



These CSV files load directly into Excel or your can easily write your own apps
against them (I'm a fan of LinqPad for such things).

Version History
===============

1.0.0.0: Released on Nov 11, 2013.

Copyright
=========

I built this program for my own use and am sharing it in case anyone finds value
in it. Any violations of the mint.com end user license agreement are on you.

Copyright (c) 2011-2013, Sells Brothers, Inc. All rights reserved. No warranty
extended. Use at your own risk. Some assembly required. Void where prohibited.
Batties not included. Keep calm and carry on.
