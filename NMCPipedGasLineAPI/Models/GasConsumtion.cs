using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NMCPipedGasLineAPI.Models;
using NPOI.SS.Util;

namespace NMCPipedGasLineAPI.Models
{
    public class GasConsumtion
    {

        public static String getBillReceiptNo(bool isBill, String societyCode, string BillNo)
        {
            int YY = ((DateTime.Now.Year) % 100);
            int monthValue = ((DateTime.Now.Month));
            String MM = ((monthValue < 10) ? "0" : "") + monthValue;

            // TODO: Warning!!!, inline IF is not supported ?
            Int64 lastBillNo;
            String XXXX;


            if (!string.IsNullOrEmpty(BillNo))
            {
                String lastChar = BillNo.Substring(BillNo.Length - 4);
                lastBillNo = Convert.ToInt64(lastChar);
            }
            else
            {
                lastBillNo = 0;
            }



            if (isBill)
            {
                lastBillNo = (lastBillNo + 1);
            }

            string str = lastBillNo.ToString();
            //XXXX = str.Substring((str.Length - 4), 4);
            XXXX = str;

            String receiptNo = (isBill ? "BW" : "R") + societyCode + YY + MM + "-" + "000" + XXXX;

            return receiptNo;
        }
        public static string getBillDate()
        {
            return DateTime.Now.Date.ToString("dd/mm/yyyy");

            // reading current date
            // Reading Date from New Bill
            // current date will show here
            // closingDate = String.valueOf(tvReadingDate.getTag(R.id.tag_inst_date));
        }

        //PreviousBillDate from customer
        //closingDate from current date
        //CreatedAt from customer
        public static string getBillPeriod(string PreviousBillDate, string closingDate, string CreatedAt)
        {
            String billPeriod;
            String previousBillDate;

            if (!string.IsNullOrEmpty(PreviousBillDate))
            {
                previousBillDate = PreviousBillDate;
                billPeriod = (previousBillDate + (" to " + closingDate));
            }
            else
            {
                previousBillDate = CreatedAt;
                billPeriod = (previousBillDate + (" to " + closingDate));
            }
            return billPeriod;
        }

        //dueDays company master coming on it
        //closingDate from current date
        public static string getDueDate(DateTime closingDate, int dueDays)
        {
            return Convert.ToDateTime(closingDate).AddDays(dueDays).ToString("dd/MM/yyyy");
        }



        //PreviousRedg  from customer
        //Enter Reading 
        public static decimal getCurrentSCM(decimal closingReading, decimal PreviousRedg)
        {
            return (closingReading - PreviousRedg);
        }
        // currentSCM get from getCurrentSCM
        //godownInputRate get from godown master 
        public static decimal getCurrentKGS(decimal currentSCM, decimal godownInputRate)
        {
            return (currentSCM * godownInputRate);
        }


        public static long getNumberOfDaysBetweenDates(DateTime previousDateTime, DateTime currentDateTime)
        {
            DateTime previousDate = Convert.ToDateTime(previousDateTime);
            DateTime currentDate = Convert.ToDateTime(currentDateTime);
            long diff = Convert.ToInt64((currentDate.Date - previousDate.Date).TotalDays);
            return diff;
        }

        public static long getNumberOfDays(DateTime previousDateTime, DateTime currentDateTime)
        {
            DateTime previousDate = Convert.ToDateTime(previousDateTime);
            DateTime currentDate = Convert.ToDateTime(currentDateTime);
            long diff = Convert.ToInt64((currentDate.Date - previousDate.Date).TotalDays) + 1;
            return diff;
        }

        public static List<MonthYear> monthsBetweenDates(DateTime previousDateTime, DateTime currentDateTime)
        {
            List<MonthYear> monthValueList = new List<MonthYear>();
            DateTime previousDate = Convert.ToDateTime(previousDateTime);
            DateTime currentDate = Convert.ToDateTime(currentDateTime);

            //Calendar previousCalendar = new GregorianCalendar();
            //previousCalendar.setTime(previousDate);

            //Calendar currentCalendar = new GregorianCalendar();
            //currentCalendar.setTime(currentDate);
            //        currentCalendar.add(Calendar.DAY_OF_MONTH, -1);

            //int startMonth = previousDate.Month + 1;     //previousCalendar.get(Calendar.MONTH) + 1;
            //int endMonth = currentDate.Month -1; //currentCalendar.get(Calendar.MONTH) + 1;

            int startMonth = previousDate.Month;     //previousCalendar.get(Calendar.MONTH) + 1;
            int endMonth = currentDate.Month;

            int startYear = previousDate.Year;
            int endYear = currentDate.Year;

            if ((startMonth == endMonth) && (startYear == endYear))
            {
                MonthYear monthYear = new MonthYear();
                int todayDay = previousDate.Day;
                monthYear.setStartDay(todayDay);

                //monthYear.setMonth(currentDate.Month + 1);
                monthYear.setMonth(currentDate.Month);
                monthYear.setYear(currentDate.Year);
                monthYear.setTotalNoDays(DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                //monthYear.setBillingDays(getNumberOfDaysBetweenDates(previousDateTime, currentDateTime) + 1);
                monthYear.setBillingDays(getNumberOfDaysBetweenDates(previousDateTime, currentDateTime));
                monthValueList.Add(monthYear);
            }
            else
            {

                if ((startYear != endYear) && (startMonth == endMonth))
                {
                    MonthYear monthYear = new MonthYear();
                    //                int todayDay = currentCalendar.get(Calendar.DAY_OF_MONTH);
                    //                monthYear.setDay(todayDay);

                    //monthYear.setMonth(currentDate.Month + 1);
                    monthYear.setMonth(currentDate.Month);
                    monthYear.setYear(currentDate.Year);
                    monthYear.setTotalNoDays(DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    monthValueList.Add(monthYear);
                }

                if (startMonth != endMonth)
                {
                    MonthYear monthYear = new MonthYear();


                    // monthYear.setMonth(currentDate.Month + 1);
                    monthYear.setMonth(currentDate.Month);
                    monthYear.setYear(currentDate.Year);
                    monthYear.setTotalNoDays(DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    monthValueList.Add(monthYear);
                }

                while (previousDate < currentDate)
                {
                    MonthYear monthYear = new MonthYear();
                    //monthYear.setMonth(previousDate.Month + 1);
                    monthYear.setMonth(previousDate.Month);
                    monthYear.setYear(previousDate.Year);
                    monthYear.setTotalNoDays(DateTime.DaysInMonth(previousDate.Year, previousDate.Month));
                    if (!monthValueList.Contains(monthYear))
                    {
                        monthValueList.Add(monthYear);
                    }
                    previousDate = previousDate.AddMonths(1);
                }


                //for (previousDate; previousCalendar.before(currentCalendar); previousCalendar.add(Calendar.MONTH, 1))
                //{
                //    MonthYear monthYear = new MonthYear();
                //    monthYear.setMonth(previousDate.Month + 1);
                //    monthYear.setYear(previousDate.Year);
                //    monthYear.setTotalNoDays(DateTime.DaysInMonth(previousDate.Year, previousDate.Month));
                //    if (!monthValueList.Contains(monthYear))
                //    {
                //        monthValueList.Add(monthYear);
                //    }
                //}



                for (int i = 0; i < monthValueList.Count(); i++)
                {
                    MonthYear monthYear = monthValueList[i];
                    if ((monthYear.getMonth() == startMonth) && (monthYear.getYear() == startYear))
                    {
                        int todayDay = previousDate.Day;
                        monthYear.setStartDay(todayDay);
                        int remaining = monthYear.getTotalNoDays() - todayDay;
                        monthYear.setRemainingDays(remaining);
                        monthYear.setBillingDays(remaining + 1);
                    }
                    else if ((monthYear.getMonth() == endMonth) && (monthYear.getYear() == endYear))
                    {
                        int todayDay = currentDate.Day - 1;
                        monthYear.setStartDay(1);
                        monthYear.setRemainingDays(todayDay);
                        monthYear.setBillingDays(todayDay);
                    }
                    else
                    {
                        monthYear.setStartDay(1);
                        monthYear.setRemainingDays(monthYear.getTotalNoDays());
                        monthYear.setBillingDays(monthYear.getTotalNoDays());
                    }
                }
            }

            return removeDuplicates(monthValueList);
        }

        private static List<MonthYear> removeDuplicates(List<MonthYear> list)
        {
            var results = list
                     .Select(p => new { p.month, p.year, p.billingDays, p.startDay, p.totalNoDays, p.endDay, p.consumption, p.gasConsume, p.remainingDays })
                     .Distinct();


            var q = (from p in results select new MonthYear { month = p.month, year = p.year, billingDays = p.billingDays, startDay = p.startDay, totalNoDays = p.totalNoDays, endDay = p.endDay, consumption = p.consumption, gasConsume = p.gasConsume, remainingDays = p.remainingDays }).ToList();

            return q;
        }

        public static DateTime formateDateTogetPreviousRateEntry(DateTime dateTime)
        {
            return dateTime.AddDays(-1);
        }


        public static decimal gasConsumtionCal(List<StockItemGasRate> stockItemGasRateList, List<MonthYear> monthYearList, decimal currentKGS, DateTime previousBillDate, DateTime billDate)
        {
            int count = 1;
            decimal perDayConsume = 0;
            decimal gasConsume = 0;
          
            long totalNoOfBillingdays = getNumberOfDays(previousBillDate, billDate);
            if (totalNoOfBillingdays != 0)
            {
                perDayConsume = Math.Round(currentKGS, 3) / totalNoOfBillingdays;
            }
            else
            { perDayConsume = Math.Round(currentKGS, 3); }

            foreach (var itemGasRate in stockItemGasRateList)
            {
                // check if rate toDate is null or empty if not then claculate days between createDate and toDate else apply same rate on all remaining days else claculate days between createDate and toDate
                if (!string.IsNullOrEmpty(itemGasRate.CreatedDate.ToString()) && !string.IsNullOrEmpty(itemGasRate.ToCreatedDate.ToString()))
                {
                    long daysDifference;

                    if (count == 1)
                    {
                        if (count == stockItemGasRateList.Count())
                        {
                            (daysDifference) = getNumberOfDaysBetweenDates(previousBillDate, billDate);
                            daysDifference += 1;
                        }
                        else
                        {
                            daysDifference = getNumberOfDaysBetweenDates(previousBillDate, Convert.ToDateTime(itemGasRate.ToCreatedDate));
                            daysDifference += 1;
                        }
                    }
                    else
                    {
                        if (count == stockItemGasRateList.Count())
                        {
                            daysDifference = getNumberOfDaysBetweenDates(Convert.ToDateTime(itemGasRate.CreatedDate), billDate);
                            daysDifference += 1;
                        }
                        else
                        {
                            daysDifference = getNumberOfDaysBetweenDates(Convert.ToDateTime(itemGasRate.CreatedDate), Convert.ToDateTime(itemGasRate.ToCreatedDate));
                            daysDifference += 1;
                        }
                    }
                    if (daysDifference >= 0)
                    {
                        decimal consumption = perDayConsume * daysDifference;
                       //  decimal consumption = Math.Round(Math.Round(perDayConsume, 3) * daysDifference, 3);
                       decimal rate = Math.Round((itemGasRate.Rate / itemGasRate.weight), 2);
                        gasConsume = Math.Round(gasConsume + (consumption * rate), 2);
                        totalNoOfBillingdays = totalNoOfBillingdays - daysDifference;
                    }

                }
                else if (string.IsNullOrEmpty(itemGasRate.ToCreatedDate.ToString()))
                {
                    decimal consumption = Math.Round(Math.Round(perDayConsume, 3) * totalNoOfBillingdays, 3);
                    decimal rate = Math.Round(itemGasRate.Rate / itemGasRate.weight, 2);
                    gasConsume = Math.Round(gasConsume + (consumption * rate), 2);
                }
                count++;

            }
            return gasConsume;
        }


        //public static decimal gasConsumtionCal(List<StockItemGasRate> stockItemGasRateList, List<MonthYear> monthYearList, decimal currentKGS, DateTime previousBillDate, DateTime billDate)
        //{
        //    decimal perDayConsume = 0;
        //    decimal gasConsume = 0;
        //    long daysDifference = getNumberOfDaysBetweenDates(previousBillDate, billDate);
        //    if(daysDifference != 0) {
        //        perDayConsume = currentKGS / daysDifference;
        //    } else
        //    { perDayConsume = currentKGS; }

        //    foreach (var itemGasRate in stockItemGasRateList)
        //    {
        //        foreach (var MonthYear in monthYearList)
        //        {
        //            if (((MonthYear.month == Convert.ToInt16(itemGasRate.Ratemonth))
        //                        && (MonthYear.year == Convert.ToInt16(itemGasRate.RateYear))))
        //            {
        //                gasConsume = (((perDayConsume * MonthYear.billingDays)
        //                              * (itemGasRate.Rate / itemGasRate.weight)));
        //                MonthYear.setGasConsume(Convert.ToDouble(gasConsume));
        //                break;
        //            }

        //        }

        //    }

        //    decimal gasConsume1 = 0;
        //    foreach (var item in monthYearList)
        //    {
        //        gasConsume1 = gasConsume1 + Convert.ToDecimal(item.gasConsume);
        //    }

        //    return Math.Round(gasConsume1, 2);
        //}

        // gasConsume get from gasConsumtionCal 
        //currentKGS get from getCurrentKGS
        public static decimal getRate(decimal gasConsume, decimal currentKGS)
        {
            decimal rateIncTax = 0;
            if (currentKGS != 0)
            {
                rateIncTax = gasConsume / currentKGS;
            }
            else
            {
                rateIncTax = gasConsume;
            }
            return rateIncTax;
        }


        public static decimal getconsumeunit(decimal Rate, decimal currentKGS)
        {
            Rate = Math.Round(Rate, 2);
            decimal rateIncTax = currentKGS * Rate;
            return rateIncTax;
        }


        //from calculateServiceAndMinimumAmount retrun cgst,sgst
        public static decimal calculateGST(decimal godowServiceCharge)
        {
            decimal gst = (godowServiceCharge * 9) / 100;
            return gst;
        }

        public static decimal calculateGSTMin(decimal miniAmount)
        {
            decimal gst = miniAmount + (miniAmount * 18) / 100;
            return gst;
        }

        public static decimal getTotalAmount(
           decimal godowServiceCharge, decimal SGST, decimal gasConsume,
           decimal CGST, decimal miniPayableAmount, decimal delaychg)
        {
            // calculate total amount including Total Gas Consume + CGST + SGST + Service Charge
            if (gasConsume != 0)
            {
                decimal totalBillingAmount = gasConsume + SGST
                        + CGST + godowServiceCharge + delaychg;
                return totalBillingAmount;
            }
            else
            {
                decimal totalBillingAmount = gasConsume + SGST
                          + CGST + godowServiceCharge + miniPayableAmount + delaychg;
                return totalBillingAmount;
            }

        }
        //company  late fee
        public static decimal DelayAmount(decimal totalAmount)
        {
            return ((totalAmount * 18) / 100) + totalAmount;
        }

        public static double Delay(double totalAmount)
        {
            return ((totalAmount * 18) / 100);
        }


        public static decimal calculateDelayCharges(DateTime billDateTime, DateTime currentDateTime,
                                               int delayDays, decimal companyDelayCharges)
        {
            int delayDaysLimit = delayDays * 3;
            long daysDifference = Convert.ToInt32((currentDateTime.Date - billDateTime.Date).TotalDays);
            if (daysDifference > delayDaysLimit)
            {
                daysDifference = delayDaysLimit;
            }

            int modulo;
            try
            {
                modulo = (int)(daysDifference / delayDays);
            }
            catch (Exception e)
            {
                modulo = 0;
            }
            decimal delayCharge = companyDelayCharges * modulo;
            return delayCharge;
        }


        public static Tuple<decimal, decimal> ClosingBalance(decimal totalAmount, decimal customerDepositAmount, decimal customerPreviousDue)
        {
            decimal paymentDue;
            decimal ClosingBalance = 0;


            if (customerPreviousDue == 0 && customerDepositAmount == 0)
            {

                paymentDue = totalAmount;
            }
            else
            {
                if (customerPreviousDue != 0)
                {

                    paymentDue = totalAmount + customerPreviousDue;
                }
                else
                {

                    // if (Customer Deposit Amount - Billing Amount >= 0) then display Payment Due Amount = 0
                    if ((customerDepositAmount - totalAmount) >= 0)
                    {
                        paymentDue = 0;
                        ClosingBalance = customerDepositAmount - totalAmount;


                    }
                    else
                    {
                        // if (Customer Deposit Amount - Billing Amount < 0) then display
                        // Total Billing Amount = Billing Amount - Deposit
                        paymentDue = totalAmount - customerDepositAmount;
                        ClosingBalance = 0;
                    }
                }
            }

            return Tuple.Create(ClosingBalance, paymentDue);
        }


        public static DateTime getDueDate1(String formatPattern, DateTime billDate, int noOfDays)
        {

            DateTime date = new DateTime();
            if (billDate != null)
            {
                date = Convert.ToDateTime(billDate);

            }
            date = date.AddDays(noOfDays);

            return date;
        }

        /**
 * method used to calculate delay charges according to dates and return final string
 *
 * @param closingDate           bill closing date
 * @param delayDays             delay days interval
 * @param totalBillingAmount    total billing amount
 * @param customerPreviousDue   previous bill due amount
 * @param delayCharge           applicable delay charges from company master
 * @param customerDepositAmount customer previous deposit amount
 * @return final string to be display on UI

 */
        public static string calculateDelayDaysAmount(DateTime closingDate, int delayDays,
                                                             double totalBillingAmount, double customerPreviousDue,
                                                             double delayCharge, double customerDepositAmount)
        {


            //totalBillingAmount = 13416.94;
            // calculate delayCharges
            double delayCharge1 = delayCharge + Delay(delayCharge);
            double delayCharge2 = (delayCharge * 2) + Delay(delayCharge * 2);
            double delayCharge3 = (delayCharge * 3) + Delay(delayCharge * 3);

            // round off delayCharge
            delayCharge1 = Math.Round(delayCharge1);
            delayCharge2 = Math.Round(delayCharge2);
            delayCharge3 = Math.Round(delayCharge3);

            DateTime dueDate1 = getDueDate1("dd/MM/yyyy", closingDate, delayDays);
            double amountDue1;
            if ((customerDepositAmount - totalBillingAmount) >= 0)
            {
                amountDue1 = 0;
            }
            else
            {
                amountDue1 = totalBillingAmount + customerPreviousDue + delayCharge1 - customerDepositAmount;
            }

            // condition for if totalBillingAmount and customerDepositAmount both 0 but customerPreviousDue != 0 then calculate due (11 Dec 2019 by surendra)
            if (customerDepositAmount == 0 && totalBillingAmount == 0 && customerPreviousDue != 0)
            {
                amountDue1 = totalBillingAmount + customerPreviousDue + delayCharge1 - customerDepositAmount;
            }

            DateTime dueDate2 = getDueDate1("dd/MM/yyyy", dueDate1, delayDays);
            double amountDue2 = (amountDue1 - delayCharge1) + delayCharge2;

            DateTime dueDate3 = getDueDate1("dd/MM/yyyy", dueDate2, delayDays);
            double amountDue3 = (amountDue2 - delayCharge2) + delayCharge3;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("After ").Append(dueDate1.ToString("dd/MM/yyyy")).Append(" - ").Append("Rs")
                 .Append(Math.Round(amountDue1, 2)).Append("\n");
            stringBuilder.Append("After ").Append(dueDate2.ToString("dd/MM/yyyy")).Append(" - ").Append("Rs")
                             .Append(Math.Round(amountDue2, 2)).Append("\n");
            stringBuilder.Append("After ").Append(dueDate3.ToString("dd/MM/yyyy")).Append(" - ").Append("Rs")
                              .Append(Math.Round(amountDue3, 2)).Append("\n");

            string final = "";
            final = "After " + dueDate1.ToString("dd/MM/yyyy") + " - " + "Rs " + Convert.ToInt32(amountDue1);
            final = final + "," + "After " + dueDate2.ToString("dd/MM/yyyy") + " - " + "Rs " + Convert.ToInt32(amountDue2);
            final = final + "," + "After " + dueDate3.ToString("dd/MM/yyyy") + " - " + "Rs " + Convert.ToInt32(amountDue3);
            //return stringBuilder;
            return final;
        }




        public static double getPaymentDue(double totalBillingAmount, double customerPreviousDue)
        {
            return (totalBillingAmount + customerPreviousDue);
        }




        //customerPreviousDue get from customer PreviousDue
        // customerDepositAmount get from customer ClosingBalance
        public static decimal getBalanceDue(decimal customerPreviousDue, decimal customerDepositAmount, decimal totalBillingAmount)
        {
            decimal paymentDue = 0;
            if (customerPreviousDue == 0 && customerDepositAmount == 0)
            {
                paymentDue = totalBillingAmount;
            }
            else if (customerPreviousDue != 0)
            {
                paymentDue = totalBillingAmount + customerPreviousDue;
            }
            else if (customerDepositAmount != 0)
            {
                paymentDue = totalBillingAmount - customerDepositAmount;
            }

            return paymentDue;
        }


    }
}
