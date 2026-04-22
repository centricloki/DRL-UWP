using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace DRLMobile.Uwp.Helpers
{
    public class ReceiptPrintHelper
    {
        protected double ApplicationContentMarginLeft = 0.095;

        protected double ApplicationContentMarginTop = 0.03;

        protected PrintDocument printDocument;

        protected IPrintDocumentSource printDocumentSource;

        internal List<UIElement> printPreviewPages;

        protected event EventHandler PreviewPagesCreated;

        protected FrameworkElement pageToPrint;

        protected Page printPage;


        protected Canvas PrintCanvas
        {
            get
            {
                return printPage.FindName("PrintCanvas") as Canvas;
            }
        }

        public ReceiptPrintHelper(Page printReceiptPage)
        {
            printPage = printReceiptPage;
            printPreviewPages = new List<UIElement>();
        }
        public string fromWhere = string.Empty;
        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public virtual void RegisterForPrinting(string fromWhere)
        {
            this.fromWhere = fromWhere;
            printDocument = new PrintDocument();

            printDocumentSource = printDocument.DocumentSource;

            // -+ code commented as required in unregister as per teh code sampel.
            //printDocument.Paginate -= CreatePrintPreviewPages;
            //printDocument.GetPreviewPage -= GetPrintPreviewPage;
            //printDocument.AddPages -= AddPrintPages;

            printDocument.Paginate += CreatePrintPreviewPages;
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;

            PrintManager printMan = PrintManager.GetForCurrentView();

            printMan.PrintTaskRequested -= PrintTaskRequested;
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public virtual void UnregisterForPrinting()
        {
            if (printDocument == null)
            {
                return;
            }

            printDocument.Paginate -= CreatePrintPreviewPages;
            printDocument.GetPreviewPage -= GetPrintPreviewPage;
            printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;

            if (PrintCanvas != null)
            {
                PrintCanvas.Children.Clear();
            }
        }

        public async Task<bool> ShowPrintUIAsync()
        {
            try
            {
                return await PrintManager.ShowPrintUIAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ShowPrintUIAsync", ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// This method will generate print content for the page. This will create the first page from which content will flow.
        /// </summary>
        /// <param name="receiptPageToPrint">The page to print</param>
        public virtual void PreparePrintContent(Page receiptPageToPrint)
        {
            pageToPrint = null;

            if (pageToPrint == null)
            {
                pageToPrint = receiptPageToPrint;

                StackPanel header = (StackPanel)pageToPrint.FindName("Header");
                if (header != null)
                {
                    header.Visibility = Visibility.Visible;
                }
            }

            PrintCanvas.Children.Add(pageToPrint);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();
        }

        // This is the event handler for PrintManager.PrintTaskRequested.
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        { /// commented dby amol start
            PrintTask printTask = null;

            printTask = e.Request.CreatePrintTask("Honey App - Preview Print Document", sourceRequested =>
            {
                printTask.Options.MediaSize = PrintMediaSize.NorthAmericaLegal;
                printTask.Options.Orientation = PrintOrientation.Portrait;
                printTask.Options.MediaType = PrintMediaType.Label;
                printTask.Options.PageRangeOptions.AllowAllPages = true;
                printTask.Options.PrintQuality = PrintQuality.Text;


                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args1) =>
                {

                    if (args1.Completion == PrintTaskCompletion.Failed)
                    {
                        await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await new MessageDialog("Your order is placed successfully.").ShowAsync();
                        });
                    }

                    if (args1.Completion == PrintTaskCompletion.Abandoned)
                    {
                        await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {

                            await new MessageDialog("Your order is placed successfully.").ShowAsync();
                        });
                    }

                    if (args1.Completion == PrintTaskCompletion.Canceled)
                    {
                        await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await new MessageDialog("Your order is placed successfully.").ShowAsync();

                        });
                    }

                };

                sourceRequested.SetSource(printDocumentSource);

            });

            //   printTask.IsPreviewEnabled = false;
        }



        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                printDocument.AddPage(printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            printDoc.AddPagesComplete();
        }

        // AK Code commented for disabling print preview
        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;

            printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            lock (printPreviewPages)
            {
                printPreviewPages.Clear();

                PrintCanvas.Children.Clear();

                RichTextBlockOverflow lastRTBOOnPage;

                PrintTaskOptions printingOptions = e.PrintTaskOptions;

                PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

                lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

                while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Visibility.Visible)
                {
                    lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
                }

                if (PreviewPagesCreated != null)
                {
                    PreviewPagesCreated.Invoke(printPreviewPages, EventArgs.Empty);
                }

                PrintDocument printDoc = (PrintDocument)sender;

                printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
                // added by ak


            }
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        protected virtual RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        {
            FrameworkElement page;

            RichTextBlockOverflow textLink;

            if (lastRTBOAdded == null)
            {
                page = pageToPrint;

                StackPanel footer = (StackPanel)page.FindName("Footer");
                if (footer != null)
                {
                    footer.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Flow content (text) from previous pages
                page = new DRLMobile.Uwp.View.ContinuationPage(lastRTBOAdded);

                // page = pageToPrint;
            }

            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            Grid printableArea = (Grid)page.FindName("PrintableArea");

            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width,
                printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);

            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height,
                printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            printableArea.Width = pageToPrint.Width - marginWidth;
            printableArea.Height = pageToPrint.Height - marginHeight;

            PrintCanvas.Children.Add(page);

            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();

            textLink = (RichTextBlockOverflow)page.FindName("ContinuationPageLinkedContainer");
            if (textLink != null)
            {
                if (!textLink.HasOverflowContent && textLink.Visibility == Visibility.Visible)
                {
                    StackPanel footer = (StackPanel)page.FindName("Footer");
                    if (footer != null)
                    {
                        footer.Visibility = Visibility.Visible;
                    }
                    PrintCanvas.UpdateLayout();
                }
            }

            printPreviewPages.Add(page);

            return textLink;
        }
    }


}