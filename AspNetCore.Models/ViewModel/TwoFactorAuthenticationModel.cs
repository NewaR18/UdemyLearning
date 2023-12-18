using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class TwoFactorAuthenticationModel
    {
        public bool HasAuthenticator { get; set; }
        public int RecoveryCodesLeft { get; set; }
        [BindProperty]
        public bool Is2faEnabled { get; set; }
        public bool IsMachineRemembered { get; set; }
    }
}
