
using DigitalMarketing2.Models;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.CustomValidations
{
    public class RequiredAnswerAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int answerId = (int)value;
            return answerId != 0; // assumes answerId = 0 means no answer was selected
        }
    }
}
