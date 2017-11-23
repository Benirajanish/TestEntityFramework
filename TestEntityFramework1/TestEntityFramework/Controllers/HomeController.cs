


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

using TestEntityFramework.Models;

namespace TestEntityFrameWork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //SqlConnection c = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=northwind;Integrated Security=true;");

            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = c;
            //cmd.CommandText = "select * from customers";

            //c.Open();
            //cmd.ExecuteReader();



            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult ListCustomers()
        {
            // 1. Creating an object for the ORM 
            NorthwindEntities ORM = new NorthwindEntities();

            // 2. Load the data from the DbSet into a data structure 
            List<Customer> CustomerList = ORM.Customers.ToList();

            // 3. Filter the data (Optional)

            ViewBag.CustomerList = CustomerList;

            ViewBag.CountryList = ORM.Customers.Select(x => x.Country).Distinct().ToList();

            return View("CustomersView");

        }

        public ActionResult ListCustomersByCountry(string Country)
        {
            NorthwindEntities ORM = new NorthwindEntities();

            List<Customer> OutputList = new List<Customer>();
            ViewBag.CountryList = ORM.Customers.Select(x => x.Country).Distinct().ToList();
            //foreach (Customer CustomerRecord in ORM.Customers.ToList())
            //{
            //    if (CustomerRecord.Country!=null && CustomerRecord.Country.ToLower() == Country.ToLower())
            //    {
            //        OutputList.Add(CustomerRecord);
            //    }
            //}

            // LINQ Query syntax 
            //OutputList = (from CustomerRecord in ORM.Customers
            //where CustomerRecord.Country == Country
            //select CustomerRecord).ToList();


            // LINQ Method syntax 
            //OutputList = ORM.Customers.Where(x => x.Country == Country).ToList();


            OutputList = ORM.Customers.SqlQuery($"select * from customers where country=@param1",
                new SqlParameter("@param1", Country)).ToList();


            ViewBag.CustomerList = OutputList;

            return View("CustomersView");
        }


        public ActionResult ListCustomersBySearch(string SearchID)
        {
            NorthwindEntities ORM = new NorthwindEntities();

            List<Customer> OutputList = new List<Customer>();

            foreach (Customer CustomerRecord in ORM.Customers.ToList())
            {
                if (CustomerRecord.CustomerID.ToLower().Contains(SearchID.ToLower()))
                {
                    OutputList.Add(CustomerRecord);
                }
            }

            ViewBag.CustomerList = OutputList;

            return View("CustomersView");
        }

        public ActionResult DeleteCustomer(string CustomerID)
        {   // ToDO: Add exception handling for db exceptions
            //1. Find the record 
            NorthwindEntities ORM = new NorthwindEntities();

            // Find looks for a record based on the primary key
            Customer RecordToBeDeleted = ORM.Customers.Find(CustomerID);

            //2. Delete the record using the ORM
            if (RecordToBeDeleted != null)
            {
                ORM.Customers.Remove(RecordToBeDeleted);
                ORM.SaveChanges();

            }

            //3. Reload the list 

            return RedirectToAction("ListCustomers");
        }

        public ActionResult NewCustomerForm()
        {
            return View();
        }


        public ActionResult SaveCustomer(Customer NewCustomerRecord)
        {
            // 1. validation 

            if (ModelState.IsValid)
            {
                // 2. add the new record to the ORM, save changes to db
                NorthwindEntities ORM = new NorthwindEntities();

                ORM.Customers.Add(NewCustomerRecord);
                ORM.SaveChanges();

                // 3. showing the list of all customers
                return RedirectToAction("ListCustomers");

            }
            else
            {    // If validation fails 
                // go back to the form and show the list of errors!
                return View("NewCustomerForm");
            }
        }


        public ActionResult UpdateCustomer(string CustomerID)
        {
            //1. Find the customer by using the CustomerID
            NorthwindEntities ORM = new NorthwindEntities();

            Customer RecordToBeUpdated = ORM.Customers.Find(CustomerID);
            //2. Load the record into a ViewBag
            if (RecordToBeUpdated != null)
            {
                ViewBag.RecordToBeUpdated = RecordToBeUpdated;

                //3. Go the the view that has the update form
                return View("UpdateCustomerForm");

            }
            else
            {
                // ToDO: generate an error message! 
                return RedirectToAction("ListCustomers");
            }


        }

        public ActionResult SaveUpdatedCustomer(Customer RecordToBeUpdated)
        {   // Todo: Validation and exception handling 

            // 1. Find the original record 
            NorthwindEntities ORM = new NorthwindEntities();

            Customer temp = ORM.Customers.Find(RecordToBeUpdated.CustomerID);

            // 2. Do the update on that record, then save to the database
            temp.CompanyName = RecordToBeUpdated.CompanyName;
            temp.City = RecordToBeUpdated.City;
            temp.Country = RecordToBeUpdated.Country;

            ORM.Entry(temp).State = System.Data.Entity.EntityState.Modified;
            ORM.SaveChanges();
            //3. Load all the customer records 
            return RedirectToAction("ListCustomers");
        }
    }
}