﻿using System;

namespace AppleAuth.TokenObjects
{
    /// <summary>
    ///Object containing information about the user. 
    /// </summary>
    public class UserInformation
    {
        /// <summary>
        /// Unique ID generated by Apple for your app.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Boolean value displaying if the user has verified email address.
        /// </summary>
        public string EmailVerified { get; set; }

        /// <summary>
        /// UTC date and time of authentication.
        /// </summary>
        public DateTime TimeOfAuthentication { get; set; }

        /// <summary>
        /// A unique value mitigates replay attacks and is present only if passed during the authorization request.
        /// Needs to be firstly set in the query string of the initial request.
        /// </summary>
        public string Nonce { get; set; }
    }
}
