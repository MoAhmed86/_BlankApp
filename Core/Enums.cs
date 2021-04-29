
namespace Core
{
    public class Enums
    {
        #region LookupItems

        public enum LookupItemCategories
        {
            /// <summary>
            /// حالة المشاركة
            /// </summary>
            ParticipationStatus = 1,
            /// <summary>
            /// المنصات
            /// </summary>
            Platforms = 2,
        }

        public enum LookupItems
        {
            OtherOutputs = 110,
            OtherBeneficiaries = 113,
        }

        #endregion

        public enum StatusCode
        {
            Undefined,

            OK,
            Found,
            NotFound,
            InternalError,
            Unauthorized,
            InvalidModel,
            Duplicated,

            Updated,
            SaveFailed,

            Activated,
            Deactivated,

            SendFailed,
        }
        public enum EmailTypes
        {
            NewPassword,
            ContactUs,
            SendToFriend,
            ProjectDelivered,
            NewRegistration,
            Generic,
        }
    }
}
