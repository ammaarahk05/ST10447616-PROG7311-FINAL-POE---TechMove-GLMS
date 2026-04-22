namespace TechMoveLogisticSystem.Strategies
{
    public class MonthlyPaymentStrategy : IPaymentStrategy
    {
        public double CalculatePayment(double cost)
        {
            return cost; //doesnt divide by 12 anymoe
        }
    }
}