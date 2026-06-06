namespace TechMoveLogisticSystem.Api.Strategies
{
    public interface IPaymentStrategy
    {
        double CalculatePayment(double amount);
    }
}