namespace TechMoveLogisticSystem.Strategies
{
    public class PaymentContext
    {
        private IPaymentStrategy _strategy;

        public void SetStrategy(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public double ExecutePayment(double amount)
        {
            return _strategy.CalculatePayment(amount);
        }
    }
}