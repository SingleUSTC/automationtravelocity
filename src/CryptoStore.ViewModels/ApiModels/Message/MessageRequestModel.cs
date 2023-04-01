namespace CryptoStore.ViewModels.ApiModels.Message
{
    using System.ComponentModel.DataAnnotations;

    using static ValidationViewModels.Validation;
    using static ValidationViewModels.Validation.Message;

    public class MessageRequestModel
    {
        [Required(ErrorMessage = RequiredField)]
        [StringLength(MaxNameLenght, MinimumLength = MinNameLenght)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = RequiredField)]
        [StringLength(MaxNameLenght, MinimumLen