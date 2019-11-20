using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace southosting.Authorization
{
    public static class SouthostingOperations
    {
         public static OperationAuthorizationRequirement Create =   
          new OperationAuthorizationRequirement {Name=Constants.CreateOperationName};
        public static OperationAuthorizationRequirement Read = 
          new OperationAuthorizationRequirement {Name=Constants.ReadOperationName};  
        public static OperationAuthorizationRequirement Update = 
          new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public static OperationAuthorizationRequirement Submit = 
          new OperationAuthorizationRequirement {Name=Constants.SubmitOperationName};
        public static OperationAuthorizationRequirement Approve = 
          new OperationAuthorizationRequirement {Name=Constants.ApproveOperationName};
        public static OperationAuthorizationRequirement Reject = 
          new OperationAuthorizationRequirement {Name=Constants.RejectOperationName};
    }

    public static class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string SubmitOperationName = "Submit";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public const string AdministratorRole = "Administrator";
        public const string AccommodationOfficerRole = "AccommodationOfficer";
        public const string LandlordRole = "Landlord";
        public const string StudentRole = "Student";
    }

    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            // Admin can go anywhere
            Roles = Constants.AdministratorRole + "," + string.Join(",", roles);
        }
    }
}