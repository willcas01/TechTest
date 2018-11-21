using NUnit.Framework;
    
namespace AnyCompany.Tests
{

    public class NunitTests
    {
        private AnyCompany.DAL _dal;

        [SetUp]
        public void SetUp()
        {
            _dal = new AnyCompany.DAL();
        }


        [Test]
        public void SaveOrderToDB_UnableToSave_ReturnsFalse()
        {
            var order = new Order();
            order.VAT = 1;
            // incomplete order will fail
            Assert.IsFalse(_dal.SaveOrderToDB(order), "Unable to save incomplete order.");
        }
        

        [Test]
        public void GetAllCustomerOrders_ReturnsOrders()
        {
            Assert.IsNotNull(_dal.GetAllCustomerOrders(),"Customer Orders found.");
        }

     
    }
}
