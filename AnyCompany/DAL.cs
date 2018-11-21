using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using NLog;


namespace AnyCompany
{
 public  class DAL
    {

        private OrderRepository orderRepository = new OrderRepository();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static SqlConnection DBConnect() {
            try
            {
                return new SqlConnection(@"Data Source=(local);Database=Customers;User Id=admin;Password=password;");
            }
            catch (SqlException sqlEx)
            {
                logger.Error("SQL Connection Error: " + sqlEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
                return null;
            }
        }


        public DataSet GetAllCustomerOrders()
        {
            var dsCustomerOrders = new DataSet("custOrders");

            using (var conn = DBConnect())
            {
                conn.Open();
                // Im using simple SQL select here to expose the join
                var stringbldr = new StringBuilder();
                stringbldr.AppendLine($"SELECT o.orderId, c.CustomerId, c.Name, c.Country, o.Amount");
                stringbldr.AppendLine(" FROM  Customers c");
                stringbldr.AppendLine(" INNER JOIN Orders o ON c.CustomerID = o.CustomerId");
                var adapter = new SqlDataAdapter(stringbldr.ToString(), conn);

                // alternatively stored proc could also be used and would be preferable in my view
                //SqlCommand cmd = new SqlCommand("usp_GetCustomerOrders", conn);
                //cmd.CommandType = CommandType.StoredProcedure;
                //var adapter adp = new SqlDataAdapter(cmd);

                adapter.Fill(dsCustomerOrders);
                    
            }

            if (!IsEmptyDS(dsCustomerOrders))
            {
                return dsCustomerOrders;
            }
            else
            {
                return null;
            }
        }


        bool IsEmptyDS(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                if (table.Rows.Count != 0) return false;

            return true;
        }

        public Customer GetCustomer(int customerId)
        {
            var customer = CustomerRepository.Load(customerId);
             return customer;
        }

        public bool SaveOrderToDB(Order order)
        {

            try
            {
                var conn = DBConnect();
                // using the command object so we can leverage the begin\commit transaction 
                var cmd = new SqlCommand();
                cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = "INSERT INTO Orders (OrderID, Amount, VAT,CustomerID) VALUES (@OrderId, @Amount, @VAT,@CustomerId)";
                cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                cmd.Parameters.AddWithValue("@Amount", order.Amount);
                cmd.Parameters.AddWithValue("@VAT", order.VAT);
                cmd.Parameters.AddWithValue("@CustomerID", order.CustomerId);
                cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
                return true;
            }
            catch (SqlException sqlEx)
            {
                logger.Error("SQLInsert Error: OrderId " + order.OrderId.ToString() + " " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Error: OrderId " + order.OrderId.ToString() + " " + ex.Message);
                return false;
            }

        }

    }
}
