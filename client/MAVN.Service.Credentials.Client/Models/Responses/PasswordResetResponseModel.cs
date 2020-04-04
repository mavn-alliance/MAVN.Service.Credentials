using MAVN.Service.Credentials.Client.Enums;

namespace MAVN.Service.Credentials.Client.Models.Responses
{

        /// <summary>
        /// Holds information about Password Reset Procedure
        /// </summary>
        public class PasswordResetResponseModel
        {
            /// <summary>
            /// The Password Reset Identifier
            /// </summary>
            public string Identifier { get; set; }

            /// <summary>
            /// Holds information about any errors that were risen in the process
            /// </summary>
            public PasswordResetError ErrorCode { get; set; }
        }
   

}
