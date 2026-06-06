using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Factories
{
    public class InternationalContractFactory : IContractFactory
    {
        public Contract CreateContract()
        {
            return new Contract
            {
                Status = "Draft",
                ServiceLevel = "International"
            };
        }
    }
}