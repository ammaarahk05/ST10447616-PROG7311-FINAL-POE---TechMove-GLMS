using TechMoveLogisticSystem.Models;

namespace TechMoveLogisticSystem.Factories
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