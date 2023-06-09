# MarketUp - Digital Marketing Web Application
A Digital Marketing Learning Web Application designed for students and admins. It provides an interactive learning experience for individuals interested in mastering the field of digital marketing. Administrators are authorized to manage and monitor the learning content of the website. 

**YouTube Demonstration:** https://youtu.be/Lf7WL2TUAEA

## Project Specifications:
This project is fully mobile-responsive and is implemented using ASP.Net (C#), SQL Server, HTML, CSS, Bootstrap, JavaScript, and ApexCharts. Listed below are the list of specifications in this project:

1. **Users**
    - Unregistered user may optionally register as a student.
    - Registered customers and admins may log in with their credentials.
    - Admins may create new student or admin account.
    - Authenticated students may view their profile (summarizes their learning progress).
    - All users may log out.
2. **Modules**
    - Unregistered user, logged in students, and admins may view the existing digital marketing modules.
    - Modules are arranged in chronological order (by difficulty level).
    - Admin may create, update, or delete a module.
3. **Discussions** (Only for authenticated students / admin)
    - Contribute to discussions in each module.
4. **Lessons**
    - Unregistered user, logged in students, and admins may view the existing digital marketing lessons in each module.
    - Lessons are arranged in chronological order.
    - Admin may create, update, or delete a lesson.
5. **Quizzes** (Only for authenticated students / admin)
    - Students: Attempt quiz
    - Admins: Manage Quiz
6. **Admin Dashboard** (Admins)
    - View latest students' engagement and performance on the web application through an interactive dashboard in the landing page.

## Installation:
1. Clone this project to your local machine.
2. In your package manager console (Visual Studio), run `enable-migrations`
3. In your package manager console (Visual Studio), run `update-database`
4. Re-run the program.
