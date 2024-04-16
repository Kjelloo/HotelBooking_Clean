Feature: CreateBooking
	Create a booking for a hotel room

@mytag
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