using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class ResetPasswordModel
    {
        public readonly string Token;

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
