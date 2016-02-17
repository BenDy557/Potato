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

	// Query The DB
	$query_value = mysqli_query($connection, "SELECT * FROM PotatoManager");
	// Output Data To Screen For C# To Read It.
	echo mysqli_fetch_assoc($query_value)['TotalPotatoes'];
	
	// Close MySQL Connection
	mysqli_close($connection);
?>