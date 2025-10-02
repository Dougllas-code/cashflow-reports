using CashFlow.Application.Factory.Generator.PDF.Colors;
using CashFlow.Application.Factory.Generator.PDF.Fonts;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Resources.Report;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.Factory.Generator.PDF
{
    internal class PdfReportGenerator : IReportGenerator
    {
        private const string CURRENCY_SYMBOL = "R$";
        private const int HEIGHT = 25;

        public PdfReportGenerator()
        {
            GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
        }

        public byte[] GenerateAsync(List<Expense> expenses, DateOnly month)
        {
            var document = CreateDocument(month);
            var page = CreatePage(document);

            // header
            CreateHeaderWithImage(page);

            // total spent   
            var totalExpenses = expenses.Sum(e => e.Amount);
            CreateTotalSpentSection(page, month, totalExpenses);

            // expenses table
            foreach (var expense in expenses)
            {
                var table = CreateExpensesTable(page);

                // first row
                var row = table.AddRow();
                row.Height = HEIGHT;

                AddExpenseTitle(row.Cells[0], expense.Title);
                AddHeaderForAmount(row.Cells[3]);

                // second row
                row = table.AddRow();
                row.Height = HEIGHT;

                row.Cells[0].AddParagraph(expense.Date.ToString("D"));
                SetStyleForExpenseInformation(row.Cells[0]);
                row.Cells[0].Format.LeftIndent = 20;

                row.Cells[1].AddParagraph(expense.Date.ToString("t"));
                SetStyleForExpenseInformation(row.Cells[1]);

                row.Cells[2].AddParagraph(expense.PaymentType.PaymentTypeToString());
                SetStyleForExpenseInformation(row.Cells[2]);

                AddAmountForExpense(row.Cells[3], expense.Amount);

                // third row
                if (!string.IsNullOrWhiteSpace(expense.Description))
                {
                    var descriptionRow = table.AddRow();
                    descriptionRow.Height = HEIGHT;

                    AddDescriptionForExpense(descriptionRow.Cells[0], expense.Description);

                    row.Cells[3].MergeDown = 1;
                }

                // space between tables
                AssSpaceBetweenTables(table);
            }

            return RenderDocument(document);
        }

        private static Document CreateDocument(DateOnly month)
        {
            var document = new Document();
            document.Info.Title = $"{ResourceReportGenerationMessages.EXPENSES_FOR} {month:Y}";
            document.Info.Author = "CashFlow Reports";

            var styles = document.Styles["Normal"];
            styles!.Font.Name = FontHelper.RALEWAY_REGULAR;

            return document;
        }

        private static Section CreatePage(Document document)
        {
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();

            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = 40;
            section.PageSetup.RightMargin = 40;
            section.PageSetup.TopMargin = 80;
            section.PageSetup.BottomMargin = 80;

            return section;
        }
        
        private void CreateHeaderWithImage(Section page)
        {
            var table = page.AddTable();
            table.AddColumn();
            table.AddColumn("300");

            var row = table.AddRow();

            var assembly = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(assembly.Location);
            var pathFile = Path.Combine(directoryName!, "Factory", "Generator", "PDF", "Logo", "user.png");

            row.Cells[0].AddImage(pathFile);
            row.Cells[1].AddParagraph("CashFlow Report");
            row.Cells[1].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 };
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        }

        private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
        {
            var paragraph = page.AddParagraph();
            paragraph.Format.SpaceBefore = 40;
            paragraph.Format.SpaceAfter = 40;

            var title = string.Format(ResourceReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y"));

            paragraph.AddFormattedText(title, new Font { Name = FontHelper.RALEWAY_REGULAR, Size = 15 });
            paragraph.AddLineBreak();

            paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalExpenses}", new Font { Name = FontHelper.WORKSANS_BLACK, Size = 50 });
        }

        private static Table CreateExpensesTable(Section page)
        {
            var table = page.AddTable();

            table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;


            return table;
        }

        private void AddExpenseTitle(Cell cell, string title)
        {
            cell.AddParagraph(title);
            cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.RED_LIGHT;
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.MergeRight = 2;
            cell.Format.LeftIndent = 20;
        }

        private void AddHeaderForAmount(Cell cell)
        {
            cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
            cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.WHITE };
            cell.Shading.Color = ColorsHelper.RED_DARK;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void SetStyleForExpenseInformation(Cell cell)
        {
            cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.GREEN_DARK;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void AddAmountForExpense(Cell cell, decimal amount)
        {
            cell.AddParagraph($"-{amount} {CURRENCY_SYMBOL}");
            cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 14, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.WHITE;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void AddDescriptionForExpense(Cell cell, string description)
        {
            cell.AddParagraph(description);
            cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 10, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.GREEN_LIGHT;
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.MergeRight = 2;
            cell.Format.LeftIndent = 20;
        }

        private static void AssSpaceBetweenTables(Table table)
        {
            var row = table.AddRow();
            row.Height = 30;
            row.Borders.Visible = false;
        }

        private byte[] RenderDocument(Document document)
        {
            var renderer = new PdfDocumentRenderer
            {
                Document = document
            };

            renderer.RenderDocument();

            using var file = new MemoryStream();
            renderer.PdfDocument.Save(file);

            return file.ToArray();
        }
    }
}