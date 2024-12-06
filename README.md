# EfficiencyHub

  Manage your projects and tasks with ease. Organize, prioritize, and achieve your goals efficiently.

# Description 
  EfficiencyHub is a web application designed to help users organize and manage their projects and tasks. With features tailored for both individual users and administrators, the app ensures streamlined workflow management and enhanced productivity.

# Features
<ins>User Features:</ins>
+ Create, edit, and delete projects.
+ Manage tasks and assignments with due dates and statuses.
+ Set up reminders for important deadlines.
+ View a personalized dashboard with activity logs, reports and reminders.
  
<ins>Admin Features:</ins> 
* Manage all users in the system: assign or remove administrative roles.

# Technologies Used
+ Backend: ASP.NET Core 8
+ Frontend: Razor Pages, Bootstrap
+ Database: Microsoft SQL Server
+ Authentication & Authorization: Identity Framework
+ Unit Testing: xUnit
+ Dependency Injection: Built-in ASP.NET Core DI
    
# Installation Instructions
+ Clone the repository
+ Configure the database:
  Update the connection string in appsettings.json under the ConnectionStrings section.
     ```
          "AdminUser": {
                            "Email": ".........",
                            "Password": ".........."
                        }
     ```
+ Apply migrations
    
