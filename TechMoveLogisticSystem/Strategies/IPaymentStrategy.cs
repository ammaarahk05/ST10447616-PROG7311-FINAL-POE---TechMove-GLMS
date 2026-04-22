namespace TechMoveLogisticSystem.Strategies
{
    public interface IPaymentStrategy
    {
        double CalculatePayment(double amount);
    }
}