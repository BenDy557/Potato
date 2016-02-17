<?php
	
	//////////////////////////////////////////////////////////////////////
	/// Based Off http://php.net/manual/en/function.mysqli-connect.php ///
	//////////////////////////////////////////////////////////////////////
	
	// Create Connection
	$connection = mysqli_connect("127.0.0.1", "Username Here", "Password Here", "DB Name Here");

	// Validate Connection For Errors
	if (!$connection) 
	{
		echo "Error: Unable to connect to MySQL." . PHP_EOL;
		exit;
	}

	// Query The DB To Increment Potato Count
	mysqli_query($connection, "UPDATE PotatoManager SET TotalPotatoes=TotalPotatoes+1");
	
	// Close MySQL Connection
	mysqli_close($connection);
?>