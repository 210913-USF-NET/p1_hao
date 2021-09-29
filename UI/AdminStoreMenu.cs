using System;
using P0_M.Models;
using P0_M.BL;
using P0_M.DL;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace P0_M.UI
{
    public class AdminStoreMenu:IMenu
    {
        private Store _currentStore;
        private StoreBL _sbl;
        public AdminStoreMenu(StoreBL sbl,Store store){
            _sbl = sbl;
            _currentStore = store;
        }
        public void Start(){
            bool exit = false;
            do
            {
            Console.WriteLine($"{_currentStore.Name} Inventory");
            Console.WriteLine("[0] Add new Item to this Inventory");
            Console.WriteLine("[1] Adjust Current Item within this Inventory");
            Console.WriteLine("[2] View All the Item within this Inventory");
            Console.WriteLine("[3] View Order History for this store");
            Console.WriteLine("[<] Go back");
            Console.WriteLine("[x] Exit");
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "0":
                        AddNewItem();
                        break;
                    case "1":
                        AdjustItem();
                        break;
                    case "2":
                        ViewAllItem();
                        break;
                    case "3":
                        ViewStoreHistory();
                        break;
                    case "<":
                        exit = true;
                        break;
                    case "x":
                        Console.WriteLine("bye");
                        System.Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("?");
                        break;
                }
            } while (!exit);
        }

        private void ViewStoreHistory(){
            bool ViewHistory = true;
            Log.Information("Store view order history");
            do
            {
                Console.WriteLine("[0] Display Order by Date (latest to oldest)");
                Console.WriteLine("[1] Display Order by Date (oldest to latest)");
                Console.WriteLine("[2] Display Order by Cost (least expensive to most expensive)");
                Console.WriteLine("[3] Display Order by Cost (most expensive to least expensive)");
                Console.WriteLine("[<] Stop Display Order History");
                string historyInput = Console.ReadLine();
                List<Order> TempOrders = new List<Order>();
                switch (historyInput)
                {
                    case "0":
                        Console.WriteLine(DisplayOrders(DisplayOrderbyDate90(_sbl.GetAllOrderbyId(_currentStore.Id))));
                        ViewHistory = true;
                        break;
                    case "1":
                        Console.WriteLine(DisplayOrders(DisplayOrderbyDate09(_sbl.GetAllOrderbyId(_currentStore.Id))));
                        ViewHistory = true;
                        break;
                    case "2":
                        Console.WriteLine(DisplayOrders(DisplayOrderbyCost09(_sbl.GetAllOrderbyId(_currentStore.Id))));
                        ViewHistory = true;
                        break;
                    case "3":
                        Console.WriteLine(DisplayOrders(DisplayOrderbyCost90(_sbl.GetAllOrderbyId(_currentStore.Id))));
                        ViewHistory = true;
                        break;
                    case "<":
                        Console.WriteLine("Thank you.");
                        ViewHistory = true;
                        break;
                    default:
                        Console.WriteLine("?");
                        ViewHistory = false;
                        break;
                }
            } while (!ViewHistory);
        }

        private void ViewAllItem(){
            Console.WriteLine("Store Inventory");
            List<Inventory> currentInventory = _sbl.GetOneStoreById(_currentStore.Id).Inventory;
            
            if (currentInventory.Count == 0){
                Console.WriteLine("No Items");
            }
            for (var i = 0; i < currentInventory.Count; i++)
            {
                Inventory tempLineItem = currentInventory[i];

                Console.WriteLine($"[{i}] Name: {tempLineItem.Name}");
                Console.WriteLine($"Price: {tempLineItem.Price}");
                Console.WriteLine($"Quantity: {tempLineItem.Quantity}");
            }
        }
        private void AdjustItem(){
            Console.WriteLine("Store Inventory");
            List<Inventory> currentInventory = _sbl.GetOneStoreById(_currentStore.Id).Inventory;
            
            if (currentInventory.Count == 0){
                Console.WriteLine("No Items");
            }
            for (var i = 0; i < currentInventory.Count; i++)
            {
                Inventory tempLineItem = currentInventory[i];

                Console.WriteLine($"[{i}] Name: {tempLineItem.Name}");
                Console.WriteLine($"Price: {tempLineItem.Price}");
                Console.WriteLine($"Quantity: {tempLineItem.Quantity}");
            }
            enterAdJustItemNumber:
            Console.WriteLine("Select an item number you want to adjust");
            string customerInput = Console.ReadLine().ToLower();
            int itemResult = -1;
            try
            {
                itemResult = Int32.Parse(customerInput);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{customerInput}', reenter");
                goto enterAdJustItemNumber;
            }

            if(itemResult >= 0 & itemResult < currentInventory.Count){
                Console.WriteLine($"Item Name: {currentInventory[itemResult].Name}");
                Console.WriteLine($"Item Price: {currentInventory[itemResult].Price}");
                Console.WriteLine($"Item Quantity: {currentInventory[itemResult].Quantity}");
                Console.WriteLine("[0] Adjust Price");
                Console.WriteLine("[1] Adjust Quantity");
                switch (Console.ReadLine().ToLower())
                {
                    case "0":
                        enterNewPrice:
                        Console.WriteLine("Enter new price");
                        string newPrice = Console.ReadLine();
                        Decimal priceResult = -1;
                        try
                        {
                            priceResult = Decimal.Parse(newPrice);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Unable to parse '{newPrice}', reenter");
                            goto enterNewPrice;
                        }
                        if(priceResult>0){      
                            currentInventory[itemResult].Price = priceResult;
                            _sbl.UpdateInventory2(currentInventory[itemResult]);
                            Console.WriteLine("Changeing successful");
                            refresh();
                            
                        }else{
                            Console.WriteLine($"Bad price '{newPrice}', reenter");
                            goto enterNewPrice;
                        }
                        break;
                    case "1":
                        enterNewQuantity:
                        Console.WriteLine("Enter new quantity");
                        string newQuantity = Console.ReadLine();
                        int quantityResult = -1;
                        try
                        {
                            quantityResult = Int32.Parse(newQuantity);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Unable to parse '{newQuantity}', reenter");
                            goto enterNewQuantity;
                        }
                        if(quantityResult>0){      
                            currentInventory[itemResult].Quantity = quantityResult;
                            _sbl.UpdateInventory(currentInventory[itemResult],"");
                            currentInventory =  _sbl.GetOneStoreById(_currentStore.Id).Inventory;
                            Console.WriteLine("Changeing successful");
                            refresh();
                        }else{
                            Console.WriteLine($"Bad quantity '{newQuantity}', reenter");
                            goto enterNewQuantity;
                        }
                        break;
                    default:
                        Console.WriteLine("?");
                        break;
                }
            }else{
                Console.WriteLine("Input Error, reenter");
                goto enterAdJustItemNumber;
            }
            
        }

        private void AddNewItem(){
            Console.WriteLine("Enter Item Name:");
            string name = Console.ReadLine();
            enterPrice:
            Console.WriteLine("Enter Item Price:");
            string price = Console.ReadLine();
            decimal result = -1;
            try
            {
                result = Decimal.Parse(price);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{price}', reenter");
                goto enterPrice;
            }
            if(result<=0){
                Console.WriteLine($"Bad Price: '{price}', reenter");
                goto enterPrice;
            }
            enterQuantity:
            Console.WriteLine("Enter Item Quantity:");
            string quantity = Console.ReadLine();
            int qresult = -1;
            try
            {
                qresult = Int32.Parse(quantity);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{quantity}', reenter");
                goto enterQuantity;
            }
            if(qresult<0){
                Console.WriteLine($"Bad Quantity: '{quantity}', reenter");
                goto enterPrice;
            }

            _sbl.AddInventoryItem(new Inventory(name,result,qresult,_currentStore.Id));

            Console.WriteLine("Adding successful");
        }

        private void refresh(){
            
        }

        private List<Order> DisplayOrderbyDate09(List<Order>customerOrders){
            customerOrders.Sort((x, y) => DateTime.Compare(x.Time, y.Time));
            return customerOrders;
        }
        private List<Order> DisplayOrderbyDate90(List<Order>customerOrders){
            customerOrders.Sort((x, y) => DateTime.Compare(y.Time, x.Time));
            return customerOrders;
        }
        private List<Order> DisplayOrderbyCost09(List<Order>customerOrders){
            customerOrders.Sort((x,y) => x.Total.CompareTo(y.Total));
            return customerOrders;
        }
        private List<Order> DisplayOrderbyCost90(List<Order>customerOrders){
            customerOrders.Sort((x,y) => y.Total.CompareTo(x.Total));
            return customerOrders;
        }
        public StringBuilder DisplayOrders(List<Order>customerOrders){
            StringBuilder sb = new StringBuilder();
            sb.Append("Order Histroy\n");
            if (customerOrders.Count!=0){
                for (var i = 0; i < customerOrders.Count; i++)
                {
                    sb.Append($"Order Number: {i+1}\n");
                    sb.Append($"Order Time: {customerOrders[i].Time}\n");
                    sb.Append($"Order Location: {customerOrders[i].Location}\n");
                    
                    sb.Append($"Order Items: \n");
                    for(var j = 0; j<customerOrders[i].LineItems.Count;j++){
                        sb.Append($"Item Number: {j+1}\n");
                        sb.Append($"Item Name: {customerOrders[i].LineItems[j].Name}\n");
                        sb.Append($"Item Price: {customerOrders[i].LineItems[j].Price}\n");
                        sb.Append($"Item Quantity: {customerOrders[i].LineItems[j].Quantity}\n");
                    }
                    sb.Append("\n");
                }
            }else{
                sb.Append("You do not have any order");
            }
            return sb;
        }
    }
}