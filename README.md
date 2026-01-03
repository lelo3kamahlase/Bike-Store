Bike Store Management System
A web-based management tool for bicycle retailers to track inventory, manage personnel, and analyze sales performance.

Key Features
Staff Performance Dashboard: * Ranking Table: Detailed list of staff members based on sales performance.
Bar Graph: Visual representation of performance metrics for quick comparison.
Staff Management: Full CRUD (Create, Read, Update, Delete) functionality to manage employee records.
Product Catalog: A dedicated page displaying bicycle names, categories, and pricing.

Tech Stack
Language: C#
Framework: ASP.NET (with Entity Framework)
Database: SQL Server (Stores all staff and product relational data)

Database Structure
The system relies on a SQL backend with the following core entities:
Staff: Names, roles, and performance stats.
Products: Bicycle inventory, including names and pricing.
Performance Data: Linked via SQL queries to generate real-time reports.

Setup
Clone the repository.
Run the SQL setup script to create the tables.
Update the Web.config connection string.
Build and run in Visual Studio.

Developed by: Bulelo Sibisi
