namespace AnyCompany
{
    public class OrderService
    {
        private readonly OrderRepository orderRepository = new OrderRepository();

        public double GetVAT(Customer customer)
        {
            double vat = 0d;

            if (customer.Country == "UK")
            {
                vat = 0.2d;
            }

            return vat;

        }
        public bool PlaceOrder(Order order, int customerId)
        {
            Customer customer = CustomerRepository.Load(customerId);

            if (order.Amount == 0)
                return false;

            order.VAT = GetVAT(customer);
            //if (customer.Country == "UK")
            //    order.VAT = 0.2d;
            //else
            //    order.VAT = 0;

            orderRepository.Save(order);

            return true;
        }
    }
}
