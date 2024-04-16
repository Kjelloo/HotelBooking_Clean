Feature: CreateBooking
	As a user
	I want to create a booking
	So that I can reserve a period for my needs

# Test case 1
Scenario Outline: Creating a booking within a valid booking period before an occupied period
	Given the booking manager has the following occupied period:
	  | Occupied Start Date | Occupied End Date |
	  | <occupiedStartDate> | <occupiedEndDate> |
	When the user attempts to create a booking with start date "<bookingStartDate>" and end date "<bookingEndDate>"
	Then the booking should be created successfully

	Examples:
	  | occupiedStartDate  | occupiedEndDate       | bookingStartDate   | bookingEndDate       |
	  | <todayPlusTenDays> | <todayPlusTwentyDays> | <todayPlusTwoDays> | <todayPlusSevenDays> |

# Test case 2
Scenario Outline: Creating a booking within a valid booking period after an occupied period
	Given the booking manager has the following occupied period:
	  | Occupied Start Date | Occupied End Date |
	  | <occupiedStartDate> | <occupiedEndDate> |
	When the user attempts to create a booking with start date "<bookingStartDate>" and end date "<bookingEndDate>"
	Then the booking should be created successfully

	Examples:
	  | occupiedStartDate  | occupiedEndDate       | bookingStartDate      | bookingEndDate            |
	  | <todayPlusTenDays> | <todayPlusTwentyDays> | <todayPlusTwentyDays> | <todayPlusTwentyFiveDays> |

# Test case 3
Scenario Outline: Creating a booking within a valid booking period before and after an occupied period
	Given the booking manager has the following occupied period:
	  | Occupied Start Date | Occupied End Date |
	  | <occupiedStartDate> | <occupiedEndDate> |
	When the user attempts to create a booking with start date "<bookingStartDate>" and end date "<bookingEndDate>"
	Then the booking should not be created

	Examples:
	  | occupiedStartDate  | occupiedEndDate       | bookingStartDate   | bookingEndDate            |
	  | <todayPlusTenDays> | <todayPlusTwentyDays> | <todayPlusTwoDays> | <todayPlusTwentyFiveDays> |

	
Scenario Outline: Start date before full period, end date in full period - booking denied
	# This scenario covers test case 4-5		
	Given the <start date> is before the fully occupied period
	And the <end date> is within the fully occupied period
	When a booking is made
	Then the booking should be denied
	
	Examples:
	  | start date          | end date                             |
	  | <before start date> | <first day in fully occupied period> |
	  | <before start date> | <last day in fully occupied period>  |
   	
# Test case 6-7
Scenario Outline: Create a booking start in occupied period and end after the occupied period
	Given the booking manager has the following occupied period:
	  | Occupied Start Date | Occupied End Date |
	  | <occupiedStartDate> | <occupiedEndDate> |
	When the user attempts to create a booking with start date "<bookingStartDate>" and end date "<bookingEndDate>"
	Then the booking should not be created

	Examples:
	  | occupiedStartDate  | occupiedEndDate       | bookingStartDate        | bookingEndDate            |
	  | <todayPlusTenDays> | <todayPlusTwentyDays> | <todayPlusFiveDays>     | <todayPlusTwentyFiveDays> |
	  | <todayPlusTenDays> | <todayPlusTwentyDays> | <todayPlusNineteenDays> | <todayPlusTwentyFiveDays> |
    
Scenario Outline: Start date in full period, end date in full period - booking denied
	# This scenario covers test case 8-10
	Given the <start date> is within the fully occupied period
	And the <end date> is within the fully occupied period
	When a booking is made
	Then the booking should be denied
	
	Examples:  # Eksempel 1
		| start date                           | end date                            |
		| <first day in fully occupied period> | <first day in fully occupied period> |

	Examples:  # Eksempel 2
		| start date                           | end date                           |
		| <last day in fully occupied period>  | <last day in fully occupied period>  |

	Examples:  # Eksempel 3
		| start date                           | end date                            |
		| <first day in fully occupied period> | <last day in fully occupied period>  |