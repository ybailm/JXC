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
    class SaleDocumentRenderer : IDocumentRenderer
    {
        Style NoBorderedCellLeft;
        Style NoBorderedCellCenter;
        Style NoBorderedCellRight;
        Style BorderedCellLeft;
        Style BorderedCellCenter;
        Style BorderedCellRight;
        SaleData saleData;
        FlowDocument document;
        public void Render(FlowDocument doc, object data)
        {
            document = doc;

            NoBorderedCellLeft = doc.Resources["NoBorderedCellLeft"] as Style;
            NoBorderedCellCenter = doc.Resources["NoBorderedCellCenter"] as Style;
            NoBorderedCellRight = doc.Resources["NoBorderedCellRight"] as Style;

            BorderedCellLeft = doc.Resources["BorderedCellLeft"] as Style;
            BorderedCellCenter = doc.Resources["BorderedCellCenter"] as Style;
            BorderedCellRight = doc.Resources["BorderedCellRight"] as Style;

            saleData = (SaleData)data;
            RenderSaleDetail();
            RenderServiceDetail();
        }

        public TableRow CreateSaleHeaderRow()
        {
            TableRow row = new TableRow();
            row.FontWeight = FontWeights.Bold;

            TableCell cell = new TableCell(new Paragraph(new Run("序号")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("厂码")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("配件名称")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("类别")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("单位")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("数量")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("单价")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("金额")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            return row;
        }
        public TableRow CreateSumSaleRow()
        {
            TableRow sumRow = new TableRow();

            Style cellStyle;
            if (saleData.ServiceItemCollection.Count > 0)
            {
                cellStyle = NoBorderedCellCenter;
            }
            else
            {
                cellStyle = BorderedCellCenter;
            }
            TableCell sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = cellStyle ;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("配件合计：")));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run(saleData.Sale.SalePrice.ToCaptal())));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", saleData.Sale.SalePrice))));
            sumCell.Style = cellStyle;
            sumRow.Cells.Add(sumCell);

            return sumRow;
        }
        public void RenderSaleDetail()
        {
            TableRowGroup group = document.FindName("SaleDetails") as TableRowGroup;
            if (saleData.SaleItemCollection.Count > 0)
            {
                var headerRow = CreateSaleHeaderRow();
                group.Rows.Add(headerRow);
                foreach (SaleDetailItem item in saleData.SaleItemCollection)
                {
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

                    cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.UnitPrice))));
                    cell.Style = NoBorderedCellRight;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.SumSalePrice))));
                    cell.Style = NoBorderedCellRight;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);
                }
                var sumRow = CreateSumSaleRow();
                group.Rows.Add(sumRow);
            }
        }
        public TableRow CreateServiceHeaderRow()
        {
            TableRow row = new TableRow();
            row.FontWeight = FontWeights.Bold;
            TableCell cell = new TableCell(new Paragraph(new Run("序号")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("维修项目")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("单位")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("数量")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("单价")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run("金额")));
            cell.Style = BorderedCellCenter;
            row.Cells.Add(cell);

            return row;
        }
        public TableRow CreateSumServiceRow()
        {
            TableRow sumRow = new TableRow();

            TableCell sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = NoBorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("工时费合计：")));
            sumCell.Style = NoBorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = NoBorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = NoBorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = NoBorderedCellCenter;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", saleData.Sale.ServicePrice))));
            sumCell.Style = NoBorderedCellRight;
            sumRow.Cells.Add(sumCell);

            return sumRow;
        }        
        public TableRow CreateSumRow()
        {
            TableRow sumRow = new TableRow();

            TableCell sumCell = new TableCell(new Paragraph(new Run("")));
            sumCell.Style = BorderedCellLeft;
            sumRow.Cells.Add(sumCell);

            sumCell = new TableCell(new Paragraph(new Run("费用合计："+saleData.Sale.SumPrice.ToCaptal())));
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

            sumCell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", saleData.Sale.SumPrice))));
            sumCell.Style = BorderedCellRight;
            sumRow.Cells.Add(sumCell);

            return sumRow;
        }

        public void RenderServiceDetail()
        {
            if (saleData.ServiceItemCollection.Count > 0)
            {
                TableRowGroup group = document.FindName("ServiceDetails") as TableRowGroup;
                var headerRow = CreateServiceHeaderRow();
                group.Rows.Add(headerRow);

                foreach (ServiceDetailItem item in saleData.ServiceItemCollection)
                {
                    TableRow row = new TableRow();

                    TableCell cell = new TableCell(new Paragraph(new Run(item.ID.ToString())));
                    cell.Style = NoBorderedCellCenter;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run(item.Service.ServiceName)));
                    cell.Style = NoBorderedCellLeft;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run("次")));
                    cell.Style = NoBorderedCellCenter;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run(string.Format("{0:N0}", item.Number))));
                    cell.Style = NoBorderedCellCenter;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.UnitPrice))));
                    cell.Style = NoBorderedCellRight;
                    row.Cells.Add(cell);

                    cell = new TableCell(new Paragraph(new Run(string.Format("{0:N2}", item.SumServicePrice))));
                    cell.Style = NoBorderedCellRight;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);
                }
                var sumServiceRow = CreateSumServiceRow();
                group.Rows.Add(sumServiceRow);

                if (saleData.SaleItemCollection.Count > 0)
                {
                    var sumRow = CreateSumRow();
                    group.Rows.Add(sumRow);
                }
            }
        }
    }
    public class SaleData
    {
        public Sale Sale { get; set; }
        public ObservableCollection<SaleDetailItem> SaleItemCollection;
        public ObservableCollection<ServiceDetailItem> ServiceItemCollection;
    }
}