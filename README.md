# MarketUp - Digital Marketing Web Application
A Digital Marketing Learning Web Application designed to provide an interactive learning experience for individuals interested in mastering the field of digital marketing. Whether you're a beginner looking to acquire fundamental knowledge or a seasoned professional aiming to enhance your skills, this platform offers a comprehensive curriculum to suit your needs.

**YouTube Demonstration:**

## Project Specifications:
This project is fully mobile-responsive and is implemented using ASP.Net (C#), SQL Server, HTML, CSS, Bootstrap, JavaScript, and ApexCharts. Listed below are the list of specifications in this project:

1. **Users**
    - Unregistered user may register as a student.
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
    - View latest students' engagement and performance on the web application in an interactive dashboard.

## Installation:
1. Clone this project to your local machine.
2. In your package manager console (Visual Studio), run `enable-migrations`
3. In your package manager console (Visual Studio), run `update-database`
4. Re-run the program.
