# IP Address Management REST API Original Requirements
 
Create a simple IP Address Management REST API on top of any data store. It will include the ability to add IP Addresses by CIDR block and then either acquire or release IP addresses individually. Each IP address will have a status associated with it that is either “available” or “acquired”. 
 
The REST API must support four endpoint:
  * **Create IP addresses** - take in a CIDR block (e.g. 10.0.0.1/24) and add all IP addresses within that block to the data store with status “available”
  * **List IP addresses** - return all IP addresses in the system with their current status
  * **Acquire an IP** - set the status of a certain IP to “acquired”
  * **Release an IP** - set the status of a certain IP to “available”
 
# Overview and Use Notes

I built this API using ASP.NET Core 3.1 and SQLite.

\IpRestApi\Controllers\IpController.cs contains all of the primary functionality. 

 Please note that the IP_Management table is dropped and recreated with each launch of the application, so you’ll have to hit the Create IP Address endpoint before doing anything else. 

All errors will be logged to C:\Temp\.

# Testing it out

1.  Clone this repository
2.  Build the solution using Visual Studio, or on the  [command line](https://www.microsoft.com/net/core)  with  `dotnet build`.
3.  Run the project under IIS Express. The API will start up on  [https://localhost:44358/](https://localhost:44358/)  with  `dotnet run`.
4.  Use an HTTP client like  [Postman](https://www.getpostman.com/)  to create requests

	- For API endpoint details please view the [SwaggerHub API Documentation](https://app.swaggerhub.com/apis-docs/sethswango/ip-api-seth-swango/1.0.0#/)
	
	- An exported Postman collection that includes all the calls has been included in the root directory of the repo

