namespace SCustomers.Models
{
    public class AppConstants
    {
        //Authentication error codes
        public const int LockedAccountErrorCode = 100;
        public const int WrongUsernameErrorCode = 101;
        public const int WrongPasswordErrorCode = 102;
        public const int UnknownGrantTypeErrorCode = 104;
        //Authentication grant types
        public const string PasswordGrantType = "password";
        //user login types
        public const string FailedLoginType = "Failed Login";
        public const string SuccessLoginType = "Success Login";
        public const string LoginFailedReasonPwd = "mot de passe incorrect";
        public const string LoginFailedReasonLocked = "compte bloqué";
    }
}
