namespace CryptoStore.Controllers.ApiControllers
{
    using CryptoStore.Infrastructure.Services;
    using CryptoStore.Services.Contracts;
    using CryptoStore.ViewModels.ApiModels.Payment;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class PaymemtController : ApiController
    {
        private readonly IPaymentService payment;
        private readonly ICurrentUserService currentUser;
        private readonly IServicesService serviceDetails;

        public PaymemtController(
            IPaymentService payment,
            ICurrentUserService currentUser,
            IServicesService serviceDetails) 
        {
            this.payment = payment;
            this.currentUser = currentUser;
       