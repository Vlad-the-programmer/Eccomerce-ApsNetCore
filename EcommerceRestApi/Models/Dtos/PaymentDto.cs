namespace EcommerceRestApi.Models.Dtos
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public static PaymentDto ToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethodId = payment.PaymentMethodId
            };
        }
    }
}
