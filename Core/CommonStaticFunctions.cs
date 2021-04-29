using Core.Resources;
using System;
using System.Globalization;

namespace Core
{
    public static class CommonStaticFunctions
    {
        public static string Role_SuperAdmin = "SuperAdmin";
        public static string Role_Admin = "Admin";
        public static string Role_Employee = "Employee";
        public static string Role_Customer = "Customer";
        public static string ConvertGregToUmAlQura(DateTime? date)
        {
            DateTimeFormatInfo HijriDTFI;
            HijriDTFI = new CultureInfo("ar-SA", false).DateTimeFormat;
            HijriDTFI.Calendar = new UmAlQuraCalendar();
            return date.Value.Date.ToString("dd-MM-yyyy", HijriDTFI);
        }

        public static bool ValidateNID(string ID)
        {
            int li_index;
            int li_nidi_sum;
            int li_nid_last_digit;
            int li_nidi;

            int li_indexPlusOne;
            li_nidi_sum = 0;
            if (ID.Length != 10)
            {
                return false;
            }
            for (li_index = 0; li_index < 9; li_index++)
            {
                try
                {
                    li_nidi = int.Parse(ID.Substring(li_index, 1));
                }
                catch (Exception ex)
                {
                    return false;
                }
                li_indexPlusOne = li_index + 1;

                if (li_indexPlusOne % 2 != 0)
                {
                    li_nidi = li_nidi * 2;
                    if (li_nidi > 9)
                    {
                        li_nidi = 1 + (li_nidi % 10);
                    }
                }

                li_nidi_sum = li_nidi + li_nidi_sum;
            }

            li_nid_last_digit = li_nidi_sum % 10;

            if (li_nid_last_digit != 0)
            {
                li_nid_last_digit = 10 - li_nid_last_digit;
            }
            if (li_nid_last_digit == int.Parse(ID.Substring(9, 1)))
            {
                return true;
            }
            else
            {
                return (li_nid_last_digit == 10);
            }
        }

        public static string GetArabicState(string stateId)
        {
            switch (stateId)
            {
                case "1":
                    return Resource.State_New;
                case "2":
                    return Resource.State_In_progress;
                case "3":
                    return Resource.State_Closed;
                case "13":
                    return Resource.State_On_Hold;
                case "6":
                    return Resource.State_Resolved;
                default:
                    return Resource.State_New;
            }
        }
        public static string GetStateColor(string stateId)
        {
            switch (stateId)
            {
                case "1":
                    return "#BDC3C7";
                case "2":
                    return "#F39C12";
                case "3":
                    return "#34495E";
                case "13":
                    return "#C0392B";
                case "6":
                    return "#27AE60";
                default:
                    return "#BDC3C7";
            }
        }

        public static string ConvertImageToBase64(string imagePath)
        {
            try
            {
                byte[] b = System.IO.File.ReadAllBytes(imagePath);
                return "data:image/png;base64," + Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
        }
    }
}
