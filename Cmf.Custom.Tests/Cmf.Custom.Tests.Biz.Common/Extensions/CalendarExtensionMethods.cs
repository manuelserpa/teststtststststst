using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using System;

namespace Cmf.Custom.Tests.Biz.Common.Extensions
{
    public static class CalendarExtensionMethods
    {
        #region Calendar methods

        public static ShiftDefinitionShift GetShiftDefinitionShift(this Calendar calendar)
        {
            // determine shift definition
            ShiftDefinition shiftDefinition = null;
            ShiftDefinitionShift employeeShiftDefinitionShift = null;

            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Sunday":
                    shiftDefinition = calendar.DefaultSundayShiftDefinition;
                    break;
                case "Monday":
                    shiftDefinition = calendar.DefaultMondayShiftDefinition;
                    break;
                case "Tuesday":
                    shiftDefinition = calendar.DefaultTuesdayShiftDefinition;
                    break;
                case "Wednesday":
                    shiftDefinition = calendar.DefaultWednesdayShiftDefinition;
                    break;
                case "Thursday":
                    shiftDefinition = calendar.DefaultThursdayShiftDefinition;
                    break;
                case "Friday":
                    shiftDefinition = calendar.DefaultFridayShiftDefinition;
                    break;
                case "Saturday":
                    shiftDefinition = calendar.DefaultSaturdayShiftDefinition;
                    break;
            }

            // determine the shift code
            if (shiftDefinition != null)
            {
                shiftDefinition.Load();

                // capture current time
                DateTime currentTime = DateTime.Now;

                // look at each shift definition shift to match with current time
                foreach (ShiftDefinitionShift shiftDefinitionShift in shiftDefinition.ShiftsCollection)
                {
                    // get today as start and end dates
                    DateTime startDate = DateTime.Today;
                    DateTime endDate = DateTime.Today;

                    // assign the times from the shift to get full datetime values for comparing
                    startDate = startDate.Date + shiftDefinitionShift.StartTime.Value;
                    endDate = endDate.Date + shiftDefinitionShift.EndTime.Value;

                    // check if shift end time is less-than shift start time (indicating midnight shift)
                    if (shiftDefinitionShift.StartTime.Value >= shiftDefinitionShift.EndTime.Value)
                    {
                        if (currentTime.Hour > 0 && (currentTime <= endDate))
                        {
                            // decrement startDate back 1 day
                            startDate = startDate.AddDays(-1);
                        }
                        else
                        {
                            // increment endDate to next day
                            endDate = endDate.AddDays(1);
                        }
                    }

                    // now check if currentTime is between start and end
                    if ((currentTime >= startDate) && (currentTime <= endDate))
                    {
                        employeeShiftDefinitionShift = shiftDefinitionShift;
                        break;
                    }
                }
            }

            return employeeShiftDefinitionShift;
        }

        #endregion
    }
}
