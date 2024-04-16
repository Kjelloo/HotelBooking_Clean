Feature: CreateBooking
	As a user
	I want to create a booking
	So that I can reserve a period for my needs
	
# Test case 1
Scenario Outline: Creating a booking within a valid booking period before an occupied period
	Given the fully occupied period from day <occupiedStartDate> to day <occupiedEndDate>
	When the user attempts to create a booking with start date <bookingStartDate> and end date <bookingEndDate>
	Then the booking should be created successfully

	Examples:
	  | occupiedStartDate | occupiedEndDate | bookingStartDate | bookingEndDate |
	  | 10 				  | 20 				| 2                | 7              |

# Test case 2
Scenario Outline: Creating a booking within a valid booking period after an occupied period
	Given the fully occupied period from day <occupiedStartDate> to day <occupiedEndDate>
	When the user attempts to create a booking with start date <bookingStartDate> and end date <bookingEndDate>
	Then the booking should be created successfully

	Examples:
	  | occupiedStartDate | occupiedEndDate | bookingStartDate  | bookingEndDate |
	  | 10                | 20              | 21                | 25             |

# Test case 3
Scenario Outline: Creating a booking within a valid booking period before and after an occupied period
	Given the fully occupied period from day <occupiedStartDate> to day <occupiedEndDate>
	When the user attempts to create a booking with start date <bookingStartDate> and end date <bookingEndDate>
	Then the booking should be rejected

	Examples:
	  | occupiedStartDate | occupiedEndDate | bookingStartDate  | bookingEndDate |
	  | 10                | 20              | 19                | 25             |

Scenario Outline: Start date before full period, end date in full period - booking denied
	# This scenario covers test case 4-5		
	Given the booking manager has the following occupied period:
	  | Occupied Start Date            | Occupied End Date            |
	  | <fullyOccupiedPeriodStartDate> | <fullyOccupiedPeriodEndDate> |
	When the user attempts to create a booking with start date "<startDate>" and end date "<endDate>"
	Then the booking should not be created

	Examples:
	  | fullyOccupiedPeriodStartDate | fullyOccupiedPeriodEndDate | startDate         | endDate                         |
	  | <todayPlusTenDays>           | <todayPlusTwentyDays>      | <beforeStartDate> | <firstDayInFullyOccupiedPeriod> |
	  | <todayPlusTenDays>           | <todayPlusTwentyDays>      | <beforeStartDate> | <lastDayInFullyOccupiedPeriod>  |

# Test case 6-7
Scenario Outline: Create a booking start in occupied period and end after the occupied period
	Given the fully occupied period from day <occupiedStartDate> to day <occupiedEndDate>
	When the user attempts to create a booking with start date <bookingStartDate> and end date <bookingEndDate>
	Then the booking should be rejected

	Examples:
	  | occupiedStartDate | occupiedEndDate | bookingStartDate | bookingEndDate |
	  | 10 				  | 20 				| 5    			   | 25 			|
	  | 10 				  | 20              | 19               | 25             |
    
Scenario Outline: Start date in full period, end date in full period - booking denied
	# This scenario covers test case 8-10
	Given the booking manager has the following occupied period:
	  | Occupied Start Date            | Occupied End Date            |
	  | <fullyOccupiedPeriodStartDate> | <fullyOccupiedPeriodEndDate> |
	When the user attempts to create a booking with start date "<startDate>" and end date "<endDate>"
	Then the booking should not be created

	Examples:  # Example 1
| fullyOccupiedPeriodStartDate | fullyOccupiedPeriodEndDate | startDate                            | endDate                             |
| <todayPlusTenDays>           | <todayPlusTwentyDays>      | <firstDayInFullyOccupiedPeriod>     | <firstDayInFullyOccupiedPeriod>     |

	Examples:  # Example 2
| fullyOccupiedPeriodStartDate | fullyOccupiedPeriodEndDate | startDate                            | endDate                             |
| <todayPlusTenDays>           | <todayPlusTwentyDays>      | <lastDayInFullyOccupiedPeriod>      | <lastDayInFullyOccupiedPeriod>      |

	Examples:  # Example 3
| fullyOccupiedPeriodStartDate | fullyOccupiedPeriodEndDate | startDate                            | endDate                             |
| <todayPlusTenDays>           | <todayPlusTwentyDays>      | <firstDayInFullyOccupiedPeriod>     | <lastDayInFullyOccupiedPeriod>      |