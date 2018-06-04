using Projact.Service;
using Project.Extentions;
using Project.Model;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Project.Service
{
    public partial class SqlCommon : ICommon
    {
        readonly IHelper<SqlParameter, TransactionModel<SqlParameter>> helper = new SqlHelper();
        private ObservableCollection<Product> ProductCollection;

        public ObservableCollection<Supplier> GetAllSupplier()
        {
            var result = new ObservableCollection<Supplier>();
            foreach (DataRow dr in helper.GetTable("Supplier").Rows)
            {
                result.Add(Supplier.FromDataRow(dr));
            }
            return result;
        }

        public ObservableCollection<Manufacturer> GetAllManufacturer()
        {
            var result = new ObservableCollection<Manufacturer>();
            foreach (DataRow dr in helper.GetTable("Manufacturer").Rows)
            {
                result.Add(Manufacturer.FromDataRow(dr));
            }
            return result;
        }

        public ObservableCollection<Customer> GetAllCustomer()
        {
            var result = new ObservableCollection<Customer>();
            foreach (DataRow dr in helper.GetTable("Customer").Rows)
            {
                result.Add(Customer.FromDataRow(dr));
            }
            return result;
        }

        public ObservableCollection<Model.Service> GetAllService()
        {
            var result = new ObservableCollection<Model.Service>();
            foreach (DataRow dr in helper.GetTable("Service").Rows)
            {
                result.Add(Model.Service.FromDataRow(dr));
            }
            return result;
        }

        public ObservableCollection<Attendant> GetAllAttendant()
        {
            var result = new ObservableCollection<Attendant>();
            foreach (DataRow dr in helper.GetTable("Attendant").Rows)
            {
                result.Add(Attendant.FromDataRow(dr));
            }

            return result;
        }

        public ObservableCollection<Product> GetAllProduct()
        {
            string purchaseDetailStr = "SELECT ProductID, sum(Number), sum(Number * CostPrice) FROM PurchaseDetail GROUP BY ProductID";
            string saleDetailStr = "SELECT ProductID, sum(Number) FROM SaleDetail GROUP BY ProductID";
            DataTable productTable = helper.GetTable("Product");
            DataTable purchaseDetailTable = helper.GetTableBySql(purchaseDetailStr);
            DataTable saleDetailTable = helper.GetTableBySql(saleDetailStr);

            ProductCollection = new ObservableCollection<Product>();

            foreach (DataRow dr in productTable?.Rows)
            {
                var product = Product.FromDataRow(dr);

                foreach (DataRow purchaseDR in purchaseDetailTable.Select($"ProductID = '{product.ProductID}'"))
                {
                    product.Number = int.Parse(purchaseDR[1].ToString());
                    product.CostPrice = double.Parse(purchaseDR[2].ToString()) / product.Number;
                }

                foreach (DataRow saleDR in saleDetailTable.Select($"ProductID = '{product.ProductID}'"))
                {
                    product.Number -= int.Parse(saleDR[1].ToString());
                }

                ProductCollection.Add(product);
            }

            return ProductCollection.OrderByDescending(p => p.Number != 0);
        }

        public ObservableCollection<Charge> GetAllCharge()
        {
            var result = new ObservableCollection<Charge>();
            foreach (DataRow dr in helper.GetTable("Charge").Rows)
            {
                result.Add(Charge.FromDataRow(dr));
            }
            return result.OrderByDescending(p => p.ChargeID);
        }

        public ObservableCollection<Purchase> GetAllPurchaseList()
        {
            var supplierList = GetAllSupplier();
            var attendantList = GetAllAttendant();
            var purchaseDetailItemList = GetAllPurchaseDetailItem();
            var result = new ObservableCollection<Purchase>();
            foreach (DataRow dr in helper.GetTable("Purchase").Rows)
            {
                var purchaseID = dr[0].ToString();
                result.Add(new Purchase()
                {
                    PurchaseID = purchaseID,
                    Supplier = supplierList.First(p => p.SupplierID == int.Parse(dr[1].ToString())),
                    Attendant = attendantList.First(p => p.AttendantID == int.Parse(dr[2].ToString())),
                    PurchaseDate = DateTime.Parse(dr[3].ToString()),
                    Note = dr[4].ToString(),
                    Approved = dr[5].ToString().ConvertToBoolean(),
                    SumPrice = purchaseDetailItemList.Where(p => p.PurchaseID == purchaseID).Sum(p => p.SumCostPrice),
                });
            }
            return result;
        }

        public ObservableCollection<Purchase> GetPurchaseListByPurchaseDate(DateTime dateFrom, DateTime dateTo)
        {
            var supplierList = GetAllSupplier();
            var attendantList = GetAllAttendant();
            var purchaseDetailItemList = GetAllPurchaseDetailItem();
            var result = new ObservableCollection<Purchase>();
            foreach (DataRow dr in helper.GetTable("Purchase", $"PurchaseDate Between {dateFrom} And {dateTo}").Rows)
            {
                var purchaseID = dr[0].ToString();
                result.Add(new Purchase()
                {
                    PurchaseID = purchaseID,
                    Supplier = supplierList.First(p => p.SupplierID == int.Parse(dr[1].ToString())),
                    Attendant = attendantList.First(p => p.AttendantID == int.Parse(dr[2].ToString())),
                    PurchaseDate = DateTime.Parse(dr[3].ToString()),
                    Note = dr[4].ToString(),
                    Approved = dr[5].ToString().ConvertToBoolean(),
                    SumPrice = purchaseDetailItemList.Where(p => p.PurchaseID == purchaseID).Sum(p => p.SumCostPrice),
                });
            }
            return result;
        }

        public ObservableCollection<PurchaseDetailItem> GetAllPurchaseDetailItem()
        {
            var productList = GetAllProduct();
            var result = new ObservableCollection<PurchaseDetailItem>();
            foreach (DataRow dr in helper.GetTable("PurchaseDetail").Rows)
            {
                result.Add(new PurchaseDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    PurchaseID = dr[1].ToString(),
                    Product = productList.First(p => p.ProductID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                });
            }
            return result;
        }

        public ObservableCollection<SaleDetailItem> GetSaleDetailItemBySaleID(string saleID)
        {
            var productList = GetAllProduct();
            var result = new ObservableCollection<SaleDetailItem>();
            foreach (DataRow dr in helper.GetTable("SaleDetail", $"SaleID = '{saleID}'").Rows)
            {
                result.Add(new SaleDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    Product = productList.First(p => p.ProductID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                    Note = dr[6].ToString(),
                });
            }
            return result;
        }

        public ObservableCollection<SaleDetailItem> GetAllSaleDetailItem()
        {
            var productList = GetAllProduct();
            var result = new ObservableCollection<SaleDetailItem>();
            foreach (DataRow dr in helper.GetTable("SaleDetail").Rows)
            {
                result.Add(new SaleDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    SaleID = dr[1].ToString(),
                    Product = productList.First(p => p.ProductID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                    Note = dr[6].ToString(),
                });
            }
            return result;
        }

        public ObservableCollection<ServiceDetailItem> GetServiceDetailItemBySaleID(string saleID)
        {
            var serviceList = GetAllService();
            var result = new ObservableCollection<ServiceDetailItem>();
            foreach (DataRow dr in helper.GetTable("ServiceDetail", $"SaleID = '{saleID}'").Rows)
            {
                result.Add(new ServiceDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    Service = serviceList.First(p => p.ServiceID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    UnitPrice = double.Parse(dr[4].ToString()),
                });
            }
            return result;
        }

        public ObservableCollection<ServiceDetailItem> GetAllServiceDetailItem()
        {
            var serviceList = GetAllService();

            var result = new ObservableCollection<ServiceDetailItem>();
            foreach (DataRow dr in helper.GetTable("ServiceDetail").Rows)
            {
                result.Add(new ServiceDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    SaleID = dr[1].ToString(),
                    Service = serviceList.First(p => p.ServiceID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    UnitPrice = double.Parse(dr[4].ToString()),
                });
            }
            return result;
        }

        public ObservableCollection<PurchaseDetailItem> GetPurchaseDetailItemByPurchaseID(string PurchaseID)
        {
            var productList = GetAllProduct();
            var result = new ObservableCollection<PurchaseDetailItem>();
            foreach (DataRow dr in helper.GetTable("PurchaseDetail", $"PurchaseID = '{PurchaseID}'").Rows)
            {
                result.Add(new PurchaseDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    Product = productList.First(p => p.ProductID == int.Parse(dr[2].ToString())),
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                });
            }
            return result;
        }

        public ObservableCollection<Sale> GetAllSaleList()
        {
            DataTable dtSale = helper.GetTable("Sale");
            var attendantList = GetAllAttendant();
            var customerList = GetAllCustomer();
            var manufacturerList = GetAllManufacturer();
            var serviceDetailList = GetAllServiceDetailItem();
            var saleDetailList = GetAllSaleDetailItem();
            var result = new ObservableCollection<Sale>();
            foreach (DataRow dr in dtSale.Rows)
            {
                var saleID = dr[0].ToString();
                var servicePrice = serviceDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumServicePrice);
                var salePrice = saleDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumSalePrice);
                var sumCostPrice = saleDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumCostPrice);

                var sumPrice = servicePrice + salePrice;
                var productProfit = salePrice - sumCostPrice;
                var sumProfit = productProfit + servicePrice;
                result.Add(new Sale()
                {
                    SaleID = saleID,
                    Customer = customerList.First(p => p.CustomerID == int.Parse(dr[1].ToString())),
                    Attendant = attendantList.First(p => p.AttendantID == int.Parse(dr[2].ToString())),
                    SaleDate = DateTime.Parse(dr[3].ToString()),
                    Manufacturer = manufacturerList.First(p => p.ManufacturerID == int.Parse(dr[4].ToString())),
                    MachineID = dr[5].ToString(),
                    EngineID = dr[6].ToString(),
                    Note = dr[7].ToString(),
                    Approved = dr[8].ToString().ConvertToBoolean(),
                    ServicePrice = servicePrice,
                    SalePrice = salePrice,
                    SumPrice = sumPrice,
                    ProductProfit = productProfit,
                    SumProfit = sumProfit,
                });
            }
            return result;
        }

        public ObservableCollection<Sale> GetSaleListBySaleDate(DateTime dateFrom, DateTime dateTo)
        {
            DataTable dtSale = helper.GetTable("Sale", $"SaleDate Between {dateFrom} And {dateTo}");
            var attendantList = GetAllAttendant();
            var customerList = GetAllCustomer();
            var manufacturerList = GetAllManufacturer();
            var serviceDetailList = GetAllServiceDetailItem();
            var saleDetailList = GetAllSaleDetailItem();
            var result = new ObservableCollection<Sale>();
            foreach (DataRow dr in dtSale.Rows)
            {
                var saleID = dr[0].ToString();
                var servicePrice = serviceDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumServicePrice);
                var salePrice = saleDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumSalePrice);
                var sumCostPrice = saleDetailList.Where(p => p.SaleID == saleID).Sum(p => p.SumCostPrice);

                var sumPrice = servicePrice + salePrice;
                var productProfit = salePrice - sumCostPrice;
                var sumProfit = productProfit + servicePrice;
                result.Add(new Sale()
                {
                    SaleID = saleID,
                    Customer = customerList.First(p => p.CustomerID == int.Parse(dr[1].ToString())),
                    Attendant = attendantList.First(p => p.AttendantID == int.Parse(dr[2].ToString())),
                    SaleDate = DateTime.Parse(dr[3].ToString()),
                    Manufacturer = manufacturerList.First(p => p.ManufacturerID == int.Parse(dr[4].ToString())),
                    MachineID = dr[5].ToString(),
                    EngineID = dr[6].ToString(),
                    Note = dr[7].ToString(),
                    Approved = dr[8].ToString().ConvertToBoolean(),
                    ServicePrice = servicePrice,
                    SalePrice = salePrice,
                    SumPrice = sumPrice,
                    ProductProfit = productProfit,
                    SumProfit = sumProfit,
                });
            }
            return result;
        }

        public ObservableCollection<PurchaseDetailItem> GetAllPurchaseDetailItemByProduct(Product product)
        {
            var result = new ObservableCollection<PurchaseDetailItem>();
            foreach (DataRow dr in helper.GetTable("PurchaseDetail", $"ProductID = {product.ProductID}").Rows)
            {
                result.Add(new PurchaseDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    PurchaseID = dr[1].ToString(),
                    Product = product,
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                });
            }
            return result;
        }

        public ObservableCollection<SaleDetailItem> GetAllSaleDetailItemByProduct(Product product)
        {
            var result = new ObservableCollection<SaleDetailItem>();
            foreach (DataRow dr in helper.GetTable("SaleDetail", $"ProductID = {product.ProductID}").Rows)
            {
                result.Add(new SaleDetailItem()
                {
                    ID = int.Parse(dr[0].ToString()),
                    SaleID = dr[1].ToString(),
                    Product = product,
                    Number = int.Parse(dr[3].ToString()),
                    CostPrice = double.Parse(dr[4].ToString()),
                    UnitPrice = double.Parse(dr[5].ToString()),
                    Note = dr[6].ToString(),
                });
            }
            return result;
        }

        public Sale GetSaleByID(string saleID)
        {
            DataTable dtSale = helper.GetTable("Sale", $"SaleID = '{saleID}'");
            if (dtSale?.Rows.Count > 0)
            {
                DataRow dr = dtSale.Rows[0];
                var result = new Sale()
                {
                    SaleID = saleID,
                    Customer = GetCustomerByID(int.Parse(dr[1].ToString())),
                    Attendant = GetAttendantByID(int.Parse(dr[2].ToString())),
                    SaleDate = DateTime.Parse(dr[3].ToString()),
                    Manufacturer = GetManufacturerByID(int.Parse(dr[4].ToString())),
                    MachineID = dr[5].ToString(),
                    EngineID = dr[6].ToString(),
                    Note = dr[7].ToString(),
                    Approved = dr[8].ToString().ConvertToBoolean(),
                };
                return result;
            }
            return null;
        }

        public Purchase GetPurchaseByID(string purchaseID)
        {
            DataTable dt = helper.GetTable("Purchase", $"PurchaseID = '{purchaseID}'");
            if (dt?.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var result = new Purchase()
                {
                    PurchaseID = purchaseID,
                    Supplier = GetSupplierByID(int.Parse(dr[1].ToString())),
                    Attendant = GetAttendantByID(int.Parse(dr[2].ToString())),
                    PurchaseDate = DateTime.Parse(dr[3].ToString()),
                    Note = dr[4].ToString(),
                    Approved = dr[5].ToString().ConvertToBoolean(),

                };
                return result;
            }
            return null;
        }

        public Attendant GetAttendantByID(int attendantID)
        {
            var table = helper.GetTable("Attendant", "AttendantID = " + attendantID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Attendant.FromDataRow(table.Rows[0]);
        }

        public Product GetProductByID(int productID)
        {
            var table = helper.GetTable("Product", "ProductID = " + productID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Product.FromDataRow(table.Rows[0]);
        }

        public Product GetProductByPDM(string pdm)
        {
            var table = helper.GetTable("Product", $"PDM = '{pdm}'");
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Product.FromDataRow(table.Rows[0]);

        }

        public Supplier GetSupplierByID(int supplierID)
        {
            var table = helper.GetTable("Supplier", "SupplierID = " + supplierID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Supplier.FromDataRow(table.Rows[0]);
        }

        public Supplier GetSupplierByName(string supplierName)
        {
            var table = helper.GetTable("Supplier", $"SupplierName = '{supplierName}'");
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Supplier.FromDataRow(table.Rows[0]);
        }

        public Manufacturer GetManufacturerByID(int manufacturerID)
        {
            var table = helper.GetTable("Manufacturer", "ManufacturerID = " + manufacturerID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Manufacturer.FromDataRow(table.Rows[0]);
        }

        public Model.Service GetServiceByID(int serviceID)
        {
            var table = helper.GetTable("Service", "ServiceID = " + serviceID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Model.Service.FromDataRow(table.Rows[0]);
        }

        public Model.Service GetServiceByName(string serviceName)
        {
            var table = helper.GetTable("Service", $"ServiceName = '{serviceName}'");
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Model.Service.FromDataRow(table.Rows[0]);
        }

        public Customer GetCustomerByID(int customerID)
        {
            var table = helper.GetTable("Customer", "CustomerID = " + customerID);
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Customer.FromDataRow(table.Rows[0]);
        }

        public Customer GetCustomerByName(string customerName)
        {
            var table = helper.GetTable("Customer", $"CustomerName = '{customerName}'");
            if (table.Rows.Count < 1)
            {
                return null;
            }
            return Customer.FromDataRow(table.Rows[0]);
        }



        public string GetNewPurchaseID()
        {
            int i = 1;
            DataTable tempTable = helper.GetColumnMaxValue("Purchase", "PurchaseID");
            if (tempTable?.Rows.Count > 0)
            {
                var max = tempTable.Rows[0][0].ToString();
                i = int.Parse(max.Substring(8)) + 1;
            }
            return string.Format("QJMD-JH-{0:D7}", i);
        }

        public string GetNewSaleID()
        {
            int i = 1;
            DataTable tempTable = helper.GetColumnMaxValue("Sale", "SaleID");
            if (tempTable?.Rows.Count > 0)
            {
                var max = tempTable.Rows[0][0].ToString();
                i = int.Parse(max.Substring(8)) + 1;
            }
            return string.Format("QJMD-XS-{0:D7}", i);
        }


        public int GetNewID(string tableName, string columnName = null)
        {
            columnName = columnName ?? tableName + "ID";

            var tempTable = helper.GetColumnMaxValue(tableName, columnName);

            if (tempTable?.Rows.Count > 0)
            {
                var max = int.Parse(tempTable.Rows[0][0].ToString());
                return max + 1;
            }
            else
            {
                return 1;
            }
        }
    }

    #region 入库
    public partial class SqlCommon
    {
        public bool InsertPurchase(Purchase purchase,
            ObservableCollection<PurchaseDetailItem> purchaseItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();

            foreach (var item in purchaseItemCollection)
            {
                trans.Add(InsertPurchaseDetail(purchase.PurchaseID, item));
            }
            trans.Add(InsertPurchase(purchase));

            return helper.ExecuteTransaction(trans);
        }

        public bool DeletePurchase(Purchase purchase)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            if (purchase != null)
            {
                trans.Add(DeletePurchaseDetail(purchase.PurchaseID));//删除已经存在的入库明细表

                trans.Add(DeletePurchase(purchase.PurchaseID));//删除已经存在的入库记录
            }
            return helper.ExecuteTransaction(trans);
        }

        public bool UpdatePurchase(Purchase newPurchase,
            ObservableCollection<PurchaseDetailItem> newPurchaseItemCollection,
            Purchase oldPurchase,
            ObservableCollection<PurchaseDetailItem> oldPurchaseItemCollection)
        {
            var trans = new List<TransactionModel<SqlParameter>>();

            var updatePurchaseDetail = UpdatePurchaseDetailTrans(newPurchase.PurchaseID, newPurchaseItemCollection, oldPurchaseItemCollection);
            foreach (var item in updatePurchaseDetail)
            {
                trans.Add(item);
            }

            if (newPurchase != oldPurchase)
            {
                var updatePurchase = UpDatePurchaseTran(newPurchase, oldPurchase);
                trans.Add(updatePurchase);
            }
            return helper.ExecuteTransaction(trans);

        }


        private List<TransactionModel<SqlParameter>> UpdatePurchaseDetailTrans(
            string purchaseID,
            ObservableCollection<PurchaseDetailItem> newPurchaseItemCollection,
            ObservableCollection<PurchaseDetailItem> oldPurchaseItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            List<PurchaseDetailItem> insertList = new List<PurchaseDetailItem>(newPurchaseItemCollection);
            List<PurchaseDetailItem> deleteList = new List<PurchaseDetailItem>(oldPurchaseItemCollection);
            List<PurchaseDetailItem> updateList = new List<PurchaseDetailItem>();
            foreach (var newItem in newPurchaseItemCollection)
            {
                foreach (var oldItem in oldPurchaseItemCollection)
                {
                    if (newItem.Product.ProductID == oldItem.Product.ProductID)
                    {
                        if (newItem != oldItem)
                        {
                            updateList.Add(newItem);
                        }
                        insertList.Remove(newItem);
                        deleteList.Remove(oldItem);
                    }

                }
            }
            foreach (var oldItem in deleteList)
            {
                trans.Add(DeletePurchaseDetail(purchaseID, oldItem.Product.ProductID));
            }
            foreach (var updateItem in updateList)
            {
                trans.Add(UpdatePurchaseDetailTran(purchaseID, updateItem));
            }
            foreach (var newItem in insertList)
            {
                trans.Add(InsertPurchaseDetail(purchaseID, newItem));
            }
            return trans;
        }
        #region 删除入库单相关数据        
        private TransactionModel<SqlParameter> DeletePurchase(string purchaseID)
        {
            string sql = "Delete From Purchase Where PurchaseID = @purchaseID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
            };
            pars[0].Value = purchaseID;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeletePurchaseDetail(string purchaseID, int productID)
        {
            string sql = "Delete From PurchaseDetail Where PurchaseID = @purchaseID And ProductID = @productID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
            };
            pars[0].Value = purchaseID;
            pars[1].Value = productID;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeletePurchaseDetail(string purchaseID)
        {
            string sql = "Delete From PurchaseDetail Where PurchaseID = @purchaseID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
            };
            pars[0].Value = purchaseID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        #endregion

        #region 插入新入库单相关数据        
        private TransactionModel<SqlParameter> InsertPurchase(Purchase purchase)
        {
            string sql = "Insert Into Purchase (PurchaseID, SupplierID, AttendantID, PurchaseDate, [Note]) Values (@purchaseID, @supplierID, @attendantID, @purchaseDate, @note)";
            SqlParameter[] pars = 
            {
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
                new SqlParameter("@supplierID",DbType.UInt32),
                new SqlParameter("@attendantID",DbType.UInt32),
                new SqlParameter("@purchaseDate",DbType.Date),
                new SqlParameter("@note",SqlDbType.VarChar,50),
            };
            pars[0].Value = purchase.PurchaseID;
            pars[1].Value = purchase.Supplier.SupplierID;
            pars[2].Value = purchase.Attendant.AttendantID;
            pars[3].Value = purchase.PurchaseDate;
            pars[4].Value = purchase.Note;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> InsertPurchaseDetail(string purchaseID, PurchaseDetailItem item)
        {
            string sql = "Insert Into PurchaseDetail (PurchaseID, ProductID, [Number], CostPrice, UnitPrice, [Note]) Values (@purchaseID, @productID, @number, @costPrice, @unitPrice, @note)";
            SqlParameter[] pars = 
            {
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@costPrice",DbType.Double),
                new SqlParameter("@unitPrice",DbType.Double),
                new SqlParameter("@note",SqlDbType.VarChar,50),
            };
            pars[0].Value = purchaseID;
            pars[1].Value = item.Product.ProductID;
            pars[2].Value = item.Number;
            pars[3].Value = item.CostPrice;
            pars[4].Value = item.UnitPrice;
            pars[5].Value = item.Note;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        #endregion

        #region 更新销售相关数据
        private TransactionModel<SqlParameter> UpDatePurchaseTran(Purchase newPurchase, Purchase oldPurchase)
        {
            string sql = "Update Purchase Set SupplierID = @supplierID, AttendantID = @attendantID, PurchaseDate = @purchaseDate, [Note] = @note Where PurchaseID = @purchaseID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@supplierID",DbType.UInt32),
                new SqlParameter("@attendantID",DbType.UInt32),
                new SqlParameter("@purchaseDate",DbType.Date),
                new SqlParameter("@note",SqlDbType.VarChar,50),
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
            };

            pars[0].Value = newPurchase.Supplier.SupplierID;
            pars[1].Value = newPurchase.Attendant.AttendantID;
            pars[2].Value = newPurchase.PurchaseDate;
            pars[3].Value = newPurchase.Note;
            pars[4].Value = oldPurchase.PurchaseID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> UpdatePurchaseDetailTran(string purchaseID, PurchaseDetailItem item)
        {
            string sql = "Update PurchaseDetail Set [Number] = @number, CostPrice = @costPrice, UnitPrice = @unitPrice, [Note] = @note Where PurchaseID = @purchaseID And ProductID = @productID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@costPrice",DbType.Double),
                new SqlParameter("@unitPrice",DbType.Double),
                new SqlParameter("@note",SqlDbType.VarChar,50),
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
            };

            pars[0].Value = item.Number;
            pars[1].Value = item.CostPrice;
            pars[2].Value = item.UnitPrice;
            pars[3].Value = item.Note;
            pars[4].Value = purchaseID;
            pars[5].Value = item.Product.ProductID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }

        #endregion

        public bool ApprovePurchaseItem(Purchase purchase, ObservableCollection<PurchaseDetailItem> purchaseItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            foreach (var item in purchaseItemCollection)
            {
                trans.Add(UpdateProduct(item, Mode.PurchaseIn));
            }

            string sql = "Update Purchase Set Approved = @approved Where PurchaseID = @purchaseID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@approved",DbType.Boolean),
                new SqlParameter("@purchaseID",SqlDbType.VarChar,50),
            };
            pars[0].Value = 1;
            pars[1].Value = purchase.PurchaseID;

            trans.Add(new TransactionModel<SqlParameter>(sql, pars));
            return helper.ExecuteTransaction(trans);
        }
    }
    #endregion

    #region 销售
    public partial class SqlCommon
    {
        public bool InsertSale(Sale sale,
            ObservableCollection<SaleDetailItem> saleItemCollection,
            ObservableCollection<ServiceDetailItem> serviceItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();

            foreach (var item in saleItemCollection)
            {
                trans.Add(InsertSaleDetail(sale.SaleID, item));
            }
            foreach (var item in serviceItemCollection)
            {
                trans.Add(InsertServiceDetail(sale.SaleID, item));
            }
            trans.Add(InsertSale(sale));

            return helper.ExecuteTransaction(trans);
        }

        public bool DeleteSale(Sale sale)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            if (sale != null)
            {
                trans.Add(DeleteSaleDetail(sale.SaleID));//删除已经存在的销售明细表

                trans.Add(DeleteServiceDetail(sale.SaleID)); //删除维修明细

                trans.Add(DeleteSale(sale.SaleID));//删除已经存在的销售记录
            }
            return helper.ExecuteTransaction(trans);
        }

        public bool UpdateSale(Sale newSale,
            ObservableCollection<SaleDetailItem> newSaleItemCollection,
            ObservableCollection<ServiceDetailItem> newServiceItemCollection,
            Sale oldSale,
            ObservableCollection<SaleDetailItem> oldSaleItemCollection,
            ObservableCollection<ServiceDetailItem> oldServiceItemCollection)
        {
            var trans = new List<TransactionModel<SqlParameter>>();

            var updateSaleDetail = UpdateSaleDetailTrans(newSale.SaleID, newSaleItemCollection, oldSaleItemCollection);
            foreach (var item in updateSaleDetail)
            {
                trans.Add(item);
            }

            var updateServiceDetail = UpdateServiceDetailTrans(newSale.SaleID, newServiceItemCollection, oldServiceItemCollection);
            foreach (var item in updateServiceDetail)
            {
                trans.Add(item);
            }
            if (newSale != oldSale)
            {
                var updateSale = UpDateSaleTran(newSale, oldSale);
                trans.Add(updateSale);
            }
            return helper.ExecuteTransaction(trans);

        }


        private List<TransactionModel<SqlParameter>> UpdateSaleDetailTrans(
            string saleID,
            ObservableCollection<SaleDetailItem> newSaleItemCollection,
            ObservableCollection<SaleDetailItem> oldSaleItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            List<SaleDetailItem> insertList = new List<SaleDetailItem>(newSaleItemCollection);
            List<SaleDetailItem> deleteList = new List<SaleDetailItem>(oldSaleItemCollection);
            List<SaleDetailItem> updateList = new List<SaleDetailItem>();
            foreach (var newItem in newSaleItemCollection)
            {
                foreach (var oldItem in oldSaleItemCollection)
                {
                    if (newItem.Product.ProductID == oldItem.Product.ProductID)
                    {
                        if (newItem != oldItem)
                        {
                            updateList.Add(newItem);
                        }
                        insertList.Remove(newItem);
                        deleteList.Remove(oldItem);
                    }

                }
            }
            foreach (var oldItem in deleteList)
            {
                trans.Add(DeleteSaleDetail(saleID, oldItem.Product.ProductID));
            }
            foreach (var updateItem in updateList)
            {
                trans.Add(UpdateSaleDetailTran(saleID, updateItem));
            }
            foreach (var newItem in insertList)
            {
                trans.Add(InsertSaleDetail(saleID, newItem));
            }
            return trans;
        }

        private List<TransactionModel<SqlParameter>> UpdateServiceDetailTrans(
            string saleID,
            ObservableCollection<ServiceDetailItem> newServiceItemCollection,
            ObservableCollection<ServiceDetailItem> oldServiceItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            List<ServiceDetailItem> insertList = new List<ServiceDetailItem>(newServiceItemCollection);
            List<ServiceDetailItem> deleteList = new List<ServiceDetailItem>(oldServiceItemCollection);

            foreach (var newItem in newServiceItemCollection)
            {
                foreach (var oldItem in oldServiceItemCollection)
                {
                    if (newItem.Service.ServiceID == oldItem.Service.ServiceID)
                    {
                        if (newItem != oldItem)
                        {
                            trans.Add(UpdateServiceDetailTran(saleID, newItem));
                        }
                        insertList.Remove(newItem);
                        deleteList.Remove(oldItem);
                    }
                }
            }
            foreach (var item in insertList)
            {
                trans.Add(InsertServiceDetail(saleID, item));
            }
            foreach (var item in deleteList)
            {
                trans.Add(DeleteServiceDetail(saleID, item.Service.ServiceID));
            }
            return trans;
        }

        #region 删除销售单相关数据        
        private TransactionModel<SqlParameter> DeleteSale(string saleID)
        {
            string sql = "Delete From Sale Where SaleID = @saleID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
            };
            pars[0].Value = saleID;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeleteSaleDetail(string saleID, int productID)
        {
            string sql = "Delete From SaleDetail Where SaleID = @saleID And ProductID = @productID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
            };
            pars[0].Value = saleID;
            pars[1].Value = productID;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeleteSaleDetail(string saleID)
        {
            string sql = "Delete From SaleDetail Where SaleID = @saleID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
            };
            pars[0].Value = saleID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeleteServiceDetail(string saleID)
        {
            string sql = "Delete From ServiceDetail Where SaleID = @saleID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
            };
            pars[0].Value = saleID;
            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> DeleteServiceDetail(string saleID, int serviceID)
        {
            string sql = "Delete From ServiceDetail Where SaleID = @saleID And ServiceID = @serviceID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@serviceID",DbType.UInt32),
            };
            pars[0].Value = saleID;
            pars[1].Value = serviceID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        #endregion

        #region 插入新销售单相关数据        
        private TransactionModel<SqlParameter> InsertSale(Sale sale)
        {
            string sql = "Insert Into Sale (SaleID, CustomerID, AttendantID, SaleDate, ManufacturerID, MachineID, EngineID, [Note]) Values (@saleID, @customerID, @attendantID, @saleDate, @manufacturerID, @machineID, @engineID, @note)";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@customerID",DbType.UInt32),
                new SqlParameter("@attendantID",DbType.UInt32),
                new SqlParameter("@saleDate",DbType.Date),
                new SqlParameter("@manufacturerID",DbType.UInt32),
                new SqlParameter("@machineID",SqlDbType.VarChar,50),
                new SqlParameter("@engineID",SqlDbType.VarChar,50),
                new SqlParameter("@note",SqlDbType.VarChar,50),
            };
            pars[0].Value = sale.SaleID;
            pars[1].Value = sale.Customer.CustomerID;
            pars[2].Value = sale.Attendant.AttendantID;
            pars[3].Value = sale.SaleDate;
            pars[4].Value = sale.Manufacturer.ManufacturerID;
            pars[5].Value = sale.MachineID;
            pars[6].Value = sale.EngineID;
            pars[7].Value = sale.Note;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> InsertSaleDetail(string saleID, SaleDetailItem item)
        {
            string sql = "Insert Into SaleDetail (SaleID, ProductID, [Number], CostPrice, UnitPrice, [Note]) Values (@saleID, @productID, @number, @costPrice, @unitPrice, @note)";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@costPrice",DbType.Double),
                new SqlParameter("@unitPrice",DbType.Double),
                new SqlParameter("@note",SqlDbType.VarChar,50),
            };
            pars[0].Value = saleID;
            pars[1].Value = item.Product.ProductID;
            pars[2].Value = item.Number;
            pars[3].Value = item.CostPrice;
            pars[4].Value = item.UnitPrice;
            pars[5].Value = item.Note;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> InsertServiceDetail(string saleID, ServiceDetailItem item)
        {
            string sql = "Insert Into ServiceDetail (SaleID, ServiceID, [Number], UnitPrice) Values (@saleID, @serviceID, @number, @unitPrice)";
            SqlParameter[] pars = 
            {
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@serviceID",DbType.UInt32),
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@unitPrice",DbType.Double),
            };
            pars[0].Value = saleID;
            pars[1].Value = item.Service.ServiceID;
            pars[2].Value = item.Number;
            pars[3].Value = item.UnitPrice;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        #endregion

        #region 更新销售相关数据
        private TransactionModel<SqlParameter> UpDateSaleTran(Sale newSale, Sale oldSale)
        {
            string sql = "Update Sale Set CustomerID = @customerID, AttendantID = @attendantID, SaleDate = @saleDate, ManufacturerID = @manufacturerID, MachineID = @machineID, EngineID = @engineId, [Note] = @note Where SaleID = @saleID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@customerID",DbType.UInt32),
                new SqlParameter("@attendantID",DbType.UInt32),
                new SqlParameter("@saleDate",DbType.Date),
                new SqlParameter("@manufacturerID",DbType.UInt32),
                new SqlParameter("@machineID",SqlDbType.VarChar,50),
                new SqlParameter("@engineID",SqlDbType.VarChar,50),
                new SqlParameter("@note",SqlDbType.VarChar,50),
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
            };

            pars[0].Value = newSale.Customer.CustomerID;
            pars[1].Value = newSale.Attendant.AttendantID;
            pars[2].Value = newSale.SaleDate;
            pars[3].Value = newSale.Manufacturer.ManufacturerID;
            pars[4].Value = newSale.MachineID;
            pars[5].Value = newSale.EngineID;
            pars[6].Value = newSale.Note;
            pars[7].Value = oldSale.SaleID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> UpdateSaleDetailTran(string saleID, SaleDetailItem item)
        {
            string sql = "Update SaleDetail Set [Number] = @number, CostPrice = @costPrice, UnitPrice = @unitPrice, [Note] = @note Where SaleID = @saleID And ProductID = @productID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@costPrice",DbType.Double),
                new SqlParameter("@unitPrice",DbType.Double),
                new SqlParameter("@note",SqlDbType.VarChar,50),
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@productID",DbType.UInt32),
            };

            pars[0].Value = item.Number;
            pars[1].Value = item.CostPrice;
            pars[2].Value = item.UnitPrice;
            pars[3].Value = item.Note;
            pars[4].Value = saleID;
            pars[5].Value = item.Product.ProductID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        private TransactionModel<SqlParameter> UpdateServiceDetailTran(string saleID, ServiceDetailItem item)
        {
            string sql = "Update ServiceDetail Set [Number] = @number, UnitPrice = @unitPrice Where SaleID = @saleID And ServiceID = @serviceID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@number",DbType.UInt32),
                new SqlParameter("@unitPrice",DbType.Double),
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
                new SqlParameter("@serviceID",DbType.UInt32),
            };

            pars[0].Value = item.Number;
            pars[1].Value = item.UnitPrice;
            pars[2].Value = saleID;
            pars[3].Value = item.Service.ServiceID;

            return new TransactionModel<SqlParameter>(sql, pars);
        }
        #endregion

        public bool ApproveSaleItem(Sale sale, ObservableCollection<SaleDetailItem> saleItemCollection)
        {
            List<TransactionModel<SqlParameter>> trans = new List<TransactionModel<SqlParameter>>();
            foreach (var item in saleItemCollection)
            {
                trans.Add(UpdateProduct(item, Mode.SaleOut));
            }

            string sql = "Update Sale Set Approved = @approved Where SaleID = @saleID";
            SqlParameter[] pars = 
            {
                new SqlParameter("@approved",DbType.Boolean),
                new SqlParameter("@saleID",SqlDbType.VarChar,50),
            };
            pars[0].Value = 1;
            pars[1].Value = sale.SaleID;

            trans.Add(new TransactionModel<SqlParameter>(sql, pars));
            return helper.ExecuteTransaction(trans);
        }
    }
    #endregion

    public partial class SqlCommon
    {
        #region 货品
        public bool SaveProduct(Product product)
        {
            string sql = "";
            SqlParameter[] pars = null;
            if (product.ProductID == -1)
            {
                if (GetProductByPDM(product.PDM) == null)
                {
                    product.ProductID = GetNewID("Product");

                    sql = "Insert Into Product (ProductID, PDM, ProductName, PY, Category, Unit, [Number], UnitPrice, CostPrice) Values (@productID, @pdm, @productName, @py, @category, @unit, @number, @unitPrice, @costPrice)";
                    pars = new SqlParameter[]
                    {
                        new SqlParameter("@productID",DbType.UInt32),
                        new SqlParameter("@pdm",SqlDbType.VarChar,50),
                        new SqlParameter("@productName",SqlDbType.VarChar,50),
                        new SqlParameter("@py",SqlDbType.VarChar,50),
                        new SqlParameter("@category",SqlDbType.VarChar,50),
                        new SqlParameter("@unit",SqlDbType.VarChar,50),
                        new SqlParameter("@number",DbType.UInt32),
                        new SqlParameter("@unitPrice",DbType.Double),
                        new SqlParameter("@costPrice",DbType.Double),
                    };
                    pars[0].Value = product.ProductID;
                    pars[1].Value = product.PDM;
                    pars[2].Value = product.ProductName;
                    pars[3].Value = product.PY;
                    pars[4].Value = product.Category;
                    pars[5].Value = product.Unit;
                    pars[6].Value = product.Number;
                    pars[7].Value = product.UnitPrice;
                    pars[8].Value = product.CostPrice;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sql = "Update Product Set PDM = @pdm, ProductName = @productName, PY = @py, Category = @category, Unit = @unit, [Number] = @number, UnitPrice = @unitPrice, CostPrice = @costPrice where ProductID = @productID";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@pdm",SqlDbType.VarChar,50),
                    new SqlParameter("@productName",SqlDbType.VarChar,50),
                    new SqlParameter("@py",SqlDbType.VarChar,50),
                    new SqlParameter("@category",SqlDbType.VarChar,50),
                    new SqlParameter("@unit",SqlDbType.VarChar,50),
                    new SqlParameter("@number",DbType.UInt32),
                    new SqlParameter("@unitPrice",DbType.Double),
                    new SqlParameter("@costPrice",DbType.Double),
                    new SqlParameter("@productID",DbType.UInt32),
                };
                pars[0].Value = product.PDM;
                pars[1].Value = product.ProductName;
                pars[2].Value = product.PY;
                pars[3].Value = product.Category;
                pars[4].Value = product.Unit;
                pars[5].Value = product.Number;
                pars[6].Value = product.UnitPrice;
                pars[7].Value = product.CostPrice;
                pars[8].Value = product.ProductID;
            }
            var result = helper.ExecuteSqlReturnInt(sql, pars);

            return result > 0;
        }

        public TransactionModel<SqlParameter> UpdateProduct(DetailItem item, Mode mode)
        {
            Product old = GetProductByID(item.Product.ProductID);
            int newNumber = 0;
            double newCostPrice = 0;
            double newUnitPrice = 0;
            switch (mode)
            {
                case Mode.PurchaseIn://进货入库
                    newNumber = old.Number + item.Number;
                    if (old.Number <= 0 || newNumber == 0)
                    {
                        newCostPrice = item.CostPrice;
                    }
                    else
                    {
                        newCostPrice = (old.Number * old.CostPrice + item.Number * item.CostPrice) / newNumber;
                    }
                    newUnitPrice = item.UnitPrice;
                    break;

                case Mode.PurchaseOut://进货退货
                    newNumber = old.Number - item.Number;
                    if (newNumber <= 0)
                    {
                        newCostPrice = item.CostPrice;
                    }
                    else
                    {
                        newCostPrice = (old.Number * old.CostPrice - item.Number * item.CostPrice) / newNumber;
                    }
                    newUnitPrice = old.UnitPrice;
                    break;

                case Mode.SaleIn://销售退货
                    newNumber = old.Number + item.Number;
                    if (item.CostPrice <= 0)
                    {
                        newCostPrice = old.CostPrice;
                    }
                    else
                    {
                        newCostPrice = (old.Number * old.CostPrice + item.Number * item.CostPrice) / newNumber;
                    }
                    newUnitPrice = old.UnitPrice;
                    break;

                case Mode.SaleOut://销售出库
                    newNumber = old.Number - item.Number;
                    if (newNumber <= 0)
                    {
                        newCostPrice = old.CostPrice;
                    }
                    else
                    {
                        newCostPrice = (old.Number * old.CostPrice - item.Number * item.CostPrice) / newNumber;
                    }
                    newUnitPrice = old.UnitPrice;
                    break;
            }

            string sql = "Update Product Set [Number] = @number, CostPrice = @newCostPrice, UnitPrice = @unitPrice where ProductID = @productID And [Number] = @oldNumber";
            SqlParameter[] pars = 
            {
                    new SqlParameter("@number",DbType.UInt32),
                    new SqlParameter("@newCostPrice",DbType.Double),
                    new SqlParameter("@unitPrice",DbType.Double),
                    new SqlParameter("@productID",DbType.UInt32),
                    new SqlParameter("@oldNumber",DbType.UInt32),
            };
            pars[0].Value = newNumber;
            pars[1].Value = newCostPrice;
            pars[2].Value = newUnitPrice;
            pars[3].Value = old.ProductID;
            pars[4].Value = old.Number;
            return new TransactionModel<SqlParameter>(sql, pars);

        }

        public bool UpdateProductCostPrice(Product product)
        {
            string sql = "update Product Set CostPrice = @costPrice where ProductID = @productID";
            SqlParameter[] pars =
            {
                new SqlParameter("@costPrice",SqlDbType.Decimal),
                new SqlParameter("@productID",SqlDbType.Int),
            };

            pars[0].Value = product.CostPrice;
            pars[1].Value = product.ProductID;

            return helper.ExecuteSqlReturnInt(sql, pars) > 0;
        }

        public bool UpdateProductPY()
        {
            string sql = "update Product Set PY = @py where ProductID = @productID";

            foreach (var product in GetAllProduct())
            {
                SqlParameter[] pars = 
                {
                    new SqlParameter("@py",SqlDbType.VarChar,50),
                    new SqlParameter("@productID",DbType.UInt32),
                };

                pars[0].Value = product.ProductName.ConvertToCaptal();
                pars[1].Value = product.ProductID;
                helper.ExecuteSqlReturnInt(sql, pars);
            }
            return true;
        }
        #endregion

        #region 维修项目
        public bool SaveService(Model.Service service)
        {
            string sql = "";
            SqlParameter[] pars = null;
            if (service.ServiceID == -1)
            {
                if (GetServiceByName(service.ServiceName) == null)
                {
                    sql = "Insert Into Service (Category, ServiceName, UnitPrice) Values ( @category, @serviceName, @unitPrice)";
                    pars = new SqlParameter[]
                    {
                    new SqlParameter("@category",SqlDbType.VarChar,50),
                    new SqlParameter("@serviceName",SqlDbType.VarChar,50),
                    new SqlParameter("@unitPrice",DbType.Double),
                    };
                    pars[0].Value = service.Category;
                    pars[1].Value = service.ServiceName;
                    pars[2].Value = service.UnitPrice;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sql = "Update Service Set Category = @category, ServiceName = @serviceName, UnitPrice = @unitPrice where ServiceID = @serviceID";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@category",SqlDbType.VarChar,50),
                    new SqlParameter("@serviceName",SqlDbType.VarChar,50),
                    new SqlParameter("@unitPrice",DbType.Double),
                    new SqlParameter("@serviceID",DbType.UInt32),
                };
                pars[0].Value = service.Category;
                pars[1].Value = service.ServiceName;
                pars[2].Value = service.UnitPrice;
                pars[3].Value = service.ServiceID;
            }

            var result = helper.ExecuteSqlReturnInt(sql, pars);

            return result > 0;
        }
        #endregion

        #region 供应商
        public bool SaveSupplier(Supplier supplier)
        {
            string sql = "";
            SqlParameter[] pars = null;

            if (supplier.SupplierID == -1)
            {
                if (GetSupplierByName(supplier.SupplierName) == null)
                {
                    supplier.SupplierID = GetNewID("Supplier");
                    sql = "Insert Into Supplier (SupplierID, SupplierName, PY, Address, Contact, PhoneNumber) Values (@supplierID, @supplierName, @py, @address, @contact, @phoneNumber)";
                    pars = new SqlParameter[]
                    {
                        new SqlParameter("@supplierID",DbType.UInt32),
                        new SqlParameter("@supplierName",SqlDbType.VarChar,50),
                        new SqlParameter("@py",SqlDbType.VarChar,50),
                        new SqlParameter("@address",SqlDbType.VarChar,50),
                        new SqlParameter("@contact",SqlDbType.VarChar,50),
                        new SqlParameter("@phoneNumber",SqlDbType.VarChar,50),
                    };

                    pars[0].Value = supplier.SupplierID;
                    pars[1].Value = supplier.SupplierName;
                    pars[2].Value = supplier.PY;
                    pars[3].Value = supplier.Address;
                    pars[4].Value = supplier.Contact;
                    pars[5].Value = supplier.PhoneNumber;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                sql = "Update Supplier Set Address = @address, Contact = @contact, PhoneNumber = @phoneNumber where SupplierID = @supplierID";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@address",SqlDbType.VarChar,50),
                    new SqlParameter("@contact",SqlDbType.VarChar,50),
                    new SqlParameter("@phoneNumber",SqlDbType.VarChar,50),
                    new SqlParameter("@supplierID",DbType.UInt32),
                };

                pars[0].Value = supplier.Address;
                pars[1].Value = supplier.Contact;
                pars[2].Value = supplier.PhoneNumber;
                pars[3].Value = supplier.SupplierID;
            }

            var result = helper.ExecuteSqlReturnInt(sql, pars);

            return result > 0;

        }
        #endregion

        #region 费用
        public bool SaveCharge(Charge charge)
        {
            string sql = "";
            SqlParameter[] pars = null;
            if (charge.ChargeID == -1)
            {
                charge.ChargeID = GetNewID("Charge");
                sql = "Insert Into Charge (ChargeID, ChargeName, Unit, UnitPrice, [Number], PayDate, Category, AttendantName) Values (@chargeID, @chargeName, @unit, @unitPrice, @number, @payDate, @category, @attendantName)";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@chargeID",DbType.UInt32),
                    new SqlParameter("@chargeName",SqlDbType.VarChar,50),
                    new SqlParameter("@unit",SqlDbType.VarChar,50),
                    new SqlParameter("@unitPrice",DbType.Double),
                    new SqlParameter("@number",DbType.UInt32),
                    new SqlParameter("@payDate",DbType.Date),
                    new SqlParameter("@category",SqlDbType.VarChar,50),
                    new SqlParameter("@attendantName",SqlDbType.VarChar,50),
                };
                pars[0].Value = charge.ChargeID;
                pars[1].Value = charge.ChargeName;
                pars[2].Value = charge.Unit;
                pars[3].Value = charge.UnitPrice;
                pars[4].Value = charge.Number;
                pars[5].Value = charge.PayDate;
                pars[6].Value = charge.Category;
                pars[7].Value = charge.AttendantName;
            }

            else
            {
                sql = "Update Charge Set ChargeName = @chargeName, Unit= @unit, UnitPrice = @unitPrice, [Number] = @number, PayDate = @payDate, Category =  @category, AttendantName = @attendantName Where ChargeID = @chargeID";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@chargeName",SqlDbType.VarChar,50),
                    new SqlParameter("@unit",SqlDbType.VarChar,50),
                    new SqlParameter("@unitPrice",DbType.Double),
                    new SqlParameter("@number",DbType.UInt32),
                    new SqlParameter("@payDate",DbType.Date),
                    new SqlParameter("@category",SqlDbType.VarChar,50),
                    new SqlParameter("@attendantName",SqlDbType.VarChar,50),
                    new SqlParameter("@chargeID",DbType.UInt32),
                };

                pars[0].Value = charge.ChargeName;
                pars[1].Value = charge.Unit;
                pars[2].Value = charge.UnitPrice;
                pars[3].Value = charge.Number;
                pars[4].Value = charge.PayDate;
                pars[5].Value = charge.Category;
                pars[6].Value = charge.AttendantName;
                pars[7].Value = charge.ChargeID;
            }


            var result = helper.ExecuteSqlReturnInt(sql, pars);

            return result > 0;
        }
        #endregion

        #region 客户
        public bool SaveCustomer(Customer customer)
        {
            string sql = "";
            SqlParameter[] pars = null;
            if (customer.CustomerID == -1)
            {
                if (GetCustomerByName(customer.CustomerName) == null)
                {
                    customer.CustomerID = GetNewID("Customer");
                    sql = "Insert Into Customer (CustomerID, CustomerName, PY, Address, Contact, PhoneNumber) Values (@customerID, @customerName, @py, @address, @contact, @phoneNumber)";
                    pars = new SqlParameter[]
                    {
                        new SqlParameter("@customerID",DbType.UInt32),
                        new SqlParameter("@customerName",SqlDbType.VarChar,50),
                        new SqlParameter("@py",SqlDbType.VarChar,50),
                        new SqlParameter("@address",SqlDbType.VarChar,50),
                        new SqlParameter("@contact",SqlDbType.VarChar,50),
                        new SqlParameter("@phoneNumber",SqlDbType.VarChar,50),
                    };

                    pars[0].Value = customer.CustomerID;
                    pars[1].Value = customer.CustomerName;
                    pars[2].Value = customer.PY;
                    pars[3].Value = customer.Address;
                    pars[4].Value = customer.Contact;
                    pars[5].Value = customer.PhoneNumber;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sql = "Update Customer Set Address = @address, Contact = @contact, PhoneNumber = @phoneNumber where CustomerID = @customerID";
                pars = new SqlParameter[]
                {
                    new SqlParameter("@address",SqlDbType.VarChar,50),
                    new SqlParameter("@contact",SqlDbType.VarChar,50),
                    new SqlParameter("@phoneNumber",SqlDbType.VarChar,50),
                    new SqlParameter("@customerID",DbType.UInt32),
                };

                pars[0].Value = customer.Address;
                pars[1].Value = customer.Contact;
                pars[2].Value = customer.PhoneNumber;
                pars[3].Value = customer.CustomerID;
            }

            var result = helper.ExecuteSqlReturnInt(sql, pars);

            return result > 0;
        }
        #endregion
    }

}