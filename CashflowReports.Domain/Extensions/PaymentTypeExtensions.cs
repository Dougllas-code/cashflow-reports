using CashFlow.Domain.Enums;
using CashFlow.Domain.Resources.Report;

namespace CashFlow.Domain.Extensions
{
    public static class PaymentTypeExtensions
    {
        public static string PaymentTypeToString(this PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.CreditCard => ResourceReportGenerationMessages.CREDIT_CARD,
                PaymentType.DebitCard => ResourceReportGenerationMessages.DEBIT_CARD,
                PaymentType.EletronicTransfer => ResourceReportGenerationMessages.ELETRONIC_TRANSFER,
                PaymentType.Cash => ResourceReportGenerationMessages.CASH,
                _ => string.Empty
            };
        }
    }
}
