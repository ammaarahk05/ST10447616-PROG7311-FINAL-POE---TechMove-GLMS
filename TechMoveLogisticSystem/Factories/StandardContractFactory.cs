using TechMoveLogisticSystem.Models;

namespace TechMoveLogisticSystem.Factories
{
    public class StandardContractFactory : IContractFactory
    {
        public Contract CreateContract()
        {
            return new Contract
            {
                Status = "Draft",
                ServiceLevel = "Standard"
            };
        }
    }
}