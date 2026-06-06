using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Factories
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