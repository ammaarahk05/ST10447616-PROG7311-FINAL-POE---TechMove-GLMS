namespace TechMoveLogisticSystem.Strategies
{
    public class InternationalPaymentStrategy : IPaymentStrategy
    {
        public double CalculatePayment(double cost)
        {
            return cost * 1.10; // add 10% cost
        }
    }
}