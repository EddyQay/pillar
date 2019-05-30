using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.Utilities
{
    public class Constants
    {
        public const string TuringDbConnectionName = "TuringDbConnection";
        public const string NoFoundMessage = "No results were found matching your request";
        public const string BadRequestMessage = "Your request has invalid arguments";
        public const string BadRequestMessageZero = "Id cannot be less than 1";
        public const string InternalServerErrorMessage = "An unexpected error occured while processing your request";
        public const string AuthFailedErrorMessage = "Authorization failed; Invalid or expried token was found.";
        public const string AuthCodeEmptyErrorMessage = "Authorization failed; No valid token was found.";
        public const string DuplicateEmailMessage = "Email already exists";
        public const string DuplicateNameMessage = "Name already exists";
        public const int DefaultExpiryInHours = 24;
        public const string DefaultSortOrder = "category_id,ASC";
        public static readonly Dictionary<string, string> AllowedSorts = new Dictionary<string, string>() {
            {"name","ASC|DESC" },
            {"category_id","ASC|DESC" }
        };

        public enum SortOrderNames
        {
            /// <summary>
            /// In ascending order
            /// </summary>
            ASC,

            /// <summary>
            /// Descending order
            /// </summary>
            DESC
        }

        public const string ValidSortFileds_category_id = "category_id";
        public const string ValidSortFileds_name = "name";

        public enum OrderStatuses
        {
            PendingShipment,
            Shipped,
            Cancelled,
            Delievered
        }

        public enum ErrorCodes
        {
            /// <summary>
            /// An internal server error occured
            /// </summary>
            SVR_00,

            /// <summary>
            /// Empty authorization token code
            /// </summary>
            AUT_01,

            /// <summary>
            /// Invalid or Expired token found code
            /// </summary>
            AUT_02,

            /// <summary>
            /// Model validation error code
            /// </summary>
            PRM_02,

            /// <summary>
            /// Customer was not found
            /// </summary>
            USR_00,

            /// <summary>
            /// Email or password is invalid
            /// </summary>
            USR_01,

            /// <summary>
            /// Field(s) is/are required
            /// </summary>
            USR_02,

            /// <summary>
            /// The email is invalid
            /// </summary>
            USR_03,

            /// <summary>
            /// Email already exists
            /// </summary>
            USR_04,

            /// <summary>
            /// The email does not exist
            /// </summary>
            USR_05,

            /// <summary>
            /// Invalid phone number
            /// </summary>
            USR_06,

            /// <summary>
            /// Value is too long for [FIELD-NAME]
            /// </summary>
            USR_07,

            /// <summary>
            /// This is an invalid credit-card
            /// </summary>
            USR_08,

            /// <summary>
            /// The Shipping Region ID is not nummber
            /// </summary>
            USR_09,

            /// <summary>
            /// No category with this ID exists
            /// </summary>
            CAT_01,

            /// <summary>
            /// The ID is not number
            /// </summary>
            DEP_01,

            /// <summary>
            /// No department with this ID exists
            /// </summary>
            DEP_02,

            /// <summary>
            /// Tax information does not exist
            /// </summary>
            TAX_00,

            /// <summary>
            /// No tax information with this ID exists
            /// </summary>
            TAX_01,

            /// <summary>
            /// Generic error code for "entity" does exist
            /// </summary>
            ERR_01,

            /// <summary>
            /// Generic error code for "entity" does exist
            /// </summary>
            ERR_02,

            /// <summary>
            /// The order is not matched 'field,(DESC|ASC)'
            /// </summary>
            PAG_01,

            /// <summary>
            /// Field of order does not support sorting 
            /// </summary>
            PAG_02

        }
        
        public partial class ErrorMessages
        {
            public const string SVR_00 = "An internal server error occured";

            public const string PRM_02 = "Model validation error code";

            public const string AUT_01 = "Empty authorization token";
            
            public const string AUT_02 = "Invalid or Expired token found";

            public const string USR_00 = "Customer was not found";

            public const string USR_01 = "Email or password is invalid";

            public const string USR_02 = "Field(s) is/are required";
            
            public const string USR_03 = "The email is invalid";
            
            public const string USR_04 = "Email already exists";

            public const string USR_05 = "The email does not exist";
            
            public const string USR_06 = "Invalid phone number";

            /// <summary>
            /// Requires a field name to be passed as a string format parameter
            /// </summary>
            public const string USR_07 = "Value is too long for {0}";
            
            public const string USR_08 = "This is an invalid credit-card";
            
            public const string USR_09 = "The Shipping Region ID is not nummber";
            
            public const string CAT_01 = "No category with this ID exists";

            public const string DEP_01 = "The ID is not number";
            
            public const string DEP_02 = "No department with this ID exists";


            public const string TAX_01 = "No tax information with this ID exists";

            public const string ERR_01 = "No {0} with this ID exists";

            public const string ERR_02 = "Something unexpectedly happened while processing your request.";


            public const string PAG_01 = "The order format does not match 'field,(DESC|ASC). Use Ex. 'name,DESC'";

            public const string PAG_02 = "Field in 'order' does not support sorting. Consider using 'category_id' or 'name' ";
        }
    }
}
