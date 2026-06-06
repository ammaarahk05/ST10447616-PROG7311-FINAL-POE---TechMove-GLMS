namespace TechMoveLogisticSystem.Api.DTOs
{
    public class CurrencyConversionRequestDto
    {
        //here, i send the USD amount from the frontend to be converted into ZAR
        public decimal UsdAmount { get; set; }
    }
}