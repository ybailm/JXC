using Project.Extentions;
using Project.Model;
using Project.Service;
using Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service
{
    public partial interface ICommon
    {
        ObservableCollection<Supplier> GetAllSupplier();
        
        ObservableCollection<Manufacturer> GetAllManufacturer();
        
        ObservableCollection<Customer> GetAllCustomer();
        
        ObservableCollection<Model.Service> GetAllService();
        
        ObservableCollection<Attendant> GetAllAttendant();
        
        ObservableCollection<Product> GetAllProduct();
        
        ObservableCollection<Charge> GetAllCharge();

        ObservableCollection<Purchase> GetAllPurchaseList();
        ObservableCollection<Purchase> GetPurchaseListByPurchaseDate(DateTime dateFrom ,DateTime dateTo);
        ObservableCollection<PurchaseDetailItem> GetAllPurchaseDetailItem();
        ObservableCollection<PurchaseDetailItem> GetPurchaseDetailItemByPurchaseID(string PurchaseID);

        ObservableCollection<Sale> GetAllSaleList();
        ObservableCollection<Sale> GetSaleListBySaleDate(DateTime dateFrom ,DateTime dateTo);
        ObservableCollection<SaleDetailItem> GetAllSaleDetailItem();
        ObservableCollection<SaleDetailItem> GetSaleDetailItemBySaleID(string saleID);
        ObservableCollection<PurchaseDetailItem> GetAllPurchaseDetailItemByProduct(Product product);
        ObservableCollection<SaleDetailItem> GetAllSaleDetailItemByProduct(Product product);
        ObservableCollection<ServiceDetailItem> GetServiceDetailItemBySaleID(string saleID);

        Sale GetSaleByID(string saleID);
        Purchase GetPurchaseByID(string purchaseID);

        Attendant GetAttendantByID(int attendantID);

        Product GetProductByID(int productID);

        Product GetProductByPDM(string pdm);

        Supplier GetSupplierByID(int supplierID);

        Supplier GetSupplierByName(string supplierName);

        Manufacturer GetManufacturerByID(int manufacturerID);

        Model.Service GetServiceByID(int serviceID);

        Model.Service GetServiceByName(string serviceName);

        Customer GetCustomerByID(int customerID);

        Customer GetCustomerByName(string customerName);

        int GetNewID(string tableName, string columnName = null);

        string GetNewPurchaseID();

        string GetNewSaleID();      
    }
    public partial interface ICommon
    {
        #region 入库        
        bool InsertPurchase(Purchase purchase ,ObservableCollection<PurchaseDetailItem> purchaseItemCollection);
        bool UpdatePurchase(Purchase purchase,
            ObservableCollection<PurchaseDetailItem> purchaseItemCollection,
            Purchase oldPurchase, 
            ObservableCollection<PurchaseDetailItem> oldPurchaseItemCollection);
        bool ApprovePurchaseItem(Purchase purchase, ObservableCollection<PurchaseDetailItem> purchaseItemCollection);
        #endregion

        #region 销售
        bool InsertSale(Sale sale,
            ObservableCollection<SaleDetailItem> saleItemCollection,
            ObservableCollection<ServiceDetailItem> serviceItemCollection);

        bool DeleteSale(Sale sale);

        bool UpdateSale(Sale sale ,
            ObservableCollection<SaleDetailItem> saleItemCollection ,
            ObservableCollection<ServiceDetailItem> serviceItemCollection ,
            Sale oldSale, 
            ObservableCollection<SaleDetailItem> oldSaleItemCollection, 
            ObservableCollection<ServiceDetailItem> oldServiceItemCollection);
        bool ApproveSaleItem(Sale sale, ObservableCollection<SaleDetailItem> saleItemCollection); 
        #endregion

        #region 货品
        bool SaveProduct(Product product);
        bool UpdateProductCostPrice(Product product);
        bool UpdateProductPY();
        #endregion

        #region 维修项目
        bool SaveService(Model.Service service);
        #endregion

        #region 供应商
        bool SaveSupplier(Supplier supplier);        
        #endregion

        #region 费用
        bool SaveCharge(Charge charge);
        #endregion 费用

        #region 客户
        bool SaveCustomer(Customer customer);
        #endregion
    }
}