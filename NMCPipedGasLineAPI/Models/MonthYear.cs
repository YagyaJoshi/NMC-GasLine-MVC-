using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class MonthYear
    {
        public int month;
        public int year;
        public int totalNoDays;
        public int remainingDays;
        public double consumption;
        public double gasConsume;
        public long billingDays;
        public int startDay;
        public int endDay;

            public int getStartDay()
            {
                return startDay;
            }

            public void setStartDay(int startDay)
            {
                this.startDay = startDay;
            }

            public int getEndDay()
            {
                return endDay;
            }

            public void setEndDay(int endDay)
            {
                this.endDay = endDay;
            }

            public int getMonth()
            {
                return month;
            }

            public void setMonth(int month)
            {
                this.month = month;
            }

            public int getYear()
            {
                return year;
            }

            public void setYear(int year)
            {
                this.year = year;
            }

            public int getTotalNoDays()
            {
                return totalNoDays;
            }

            public void setTotalNoDays(int totalNoDays)
            {
                this.totalNoDays = totalNoDays;
            }

            public int getRemainingDays()
            {
                return remainingDays;
            }

            public void setRemainingDays(int remainingDays)
            {
                this.remainingDays = remainingDays;
            }

            public double getConsumption()
            {
                return consumption;
            }

            public void setConsumption(double consumption)
            {
                this.consumption = consumption;
            }

            public double getGasConsume()
            {
                return gasConsume;
            }

            public void setGasConsume(double gasConsume)
            {
                this.gasConsume = gasConsume;
            }

            public long getBillingDays()
            {
                return billingDays;
            }

            public void setBillingDays(long billingDays)
            {
                this.billingDays = billingDays;
            }


        //public Boolean equals(Object o)
        //{
        //    if (this == o) return true;
        //    if (o == null || m != o.getClass()) return false;
        //    MonthYear monthYear = (MonthYear)o;
        //    return month == monthYear.month &&
        //            year == monthYear.year;
        //}


        //public int hashCode()
        //{
        //    return Objects.hash(month, year);
        //}
    }

   
}