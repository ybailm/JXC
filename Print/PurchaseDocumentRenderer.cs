using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Project.Extentions;

namespace Project.Print
{
    class PurchaseDocumentRenderer : IDocumentRenderer
    {
        Style NoBorderedCellLeft;
        Style NoBorderedCellCenter;
        Style NoBorderedCellRight;
        Style BorderedCellLeft;
        Style BorderedCellRight;
        PurchaseData purchaseData;
        FlowDocument document;
        public void Render(FlowDocument doc, object data)
        {
            document = doc;

            NoBorderedCellLeft = doc.Resources["NoBorderedCellLeft"] as Style;
            NoBorderedCellCenter = doc.Resources["NoBorderedCellCenter"] as Style;
            NoBorderedCellRight = doc.Resources["NoBorderedCellRight"] as Style;
            BorderedCellLeft = doc.Resources["BorderedCellLeft"] as Style;
            BorderedCellRight = doc.Resources["BorderedCellRight"] as Style;
            purchaseData = (PurchaseData)data;
            RenderPurchaseDetail();
        }        
       
        public void RenderPurchaseDetail()
        {
            TableRowGroup group = document.FindName("PurchaseDetails") as TableRowGroup;

            double SumPurchasePrice = 0;
            foreach (PurchaseDetailItem item in purchaseData.PurchaseItemCollection)
            {
                SumPurchasePrice += item.SumCostPrice;
                TableRow row = new TableRow();

                TableCell cell = new TableCell(new Paragraph(new Run(item.ID.ToString())));
                cell.Style = NoBorderedCellCenter;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(item.Product.PDM)));
                cell.Style = NoBorderedCellLeft;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(item.Product.ProductName)));
                cell.Style = NoBorderedCellLeft;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(item.Product.Category)));
                cell.Style = NoBorderedCellLeft;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(item.Product.Unit)));
                cell.Style = NoBorderedCellCenter;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(string.Format("{0:N0}", item.Number))));
                cell.Style = NoBorderedCellCenter;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.CostPrice))));
                cell.Style = NoBorderedCellRight;
                row.Cells.Add(cell);

                cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.SumCostPrice))));
                cell.Style = NoBorderedCellRight;
                row.Cells.Add(cell);
                group.Rows.Add(row);
            }
            var sumRow = CreateSumPurchaseRow(SumPurchasePrice);
            group.Rows.Add(sumRow);
        }
        
        public TableRow CreateSumPurchaseRow(double sumPurchasePrice)
        {
            TableRow sumRow = new TableRow();

            TableCell sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("配件合计：")));
            sumCell.Style = BorderedCellRight;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run(sumPurchasePrice.ToCaptal())));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellRight;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", sumPurchasePrice))));
            sumCell.Style = BorderedCellRight;
            sumRow.Cells.Add(sumCell);

            return sumRow;
        }        
    }
    public class PurchaseData
    {
        public Purchase Purchase { get; set; }
        public ObservableCollection<PurchaseDetailItem> PurchaseItemCollection;
    }
}