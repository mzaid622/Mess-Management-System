# Mess Management System

# 🍽️ Mess Management System (Event-Based)

A **C# .NET MVC** project for managing a **Mess or Event System** — where users (students/teachers) can view menus, join meal groups, and check monthly bills, while admins manage menus, users, and payments efficiently.

---

## 🚀 Features

### 🔐 Roles & Rights Management
- **Admin**
  - Manage users and their roles.
  - Create, Edit, and Delete Menu items with price and date.
  - Enable/Disable menu items.
  - Approve bills and manage payments.
  - View full system reports.

- **User**
  - View daily/weekly menu.
  - Choose to join **Water & Tea group** or **Food group**.
  - View monthly bill report.
  - Request correction if any billing mistake occurs.

---

## 🧾 System Rules & Logic

1. **Menu Management**
   - Admin can create menu items with price and date.  
   - Actions: **Add**, **Edit**, **Delete**.  
   - Example: Menu 1 → Water & Tea.

2. **Groups**
   - There are two groups:
     - **Enable**
     - **Disable**
   - Water & Tea **cannot be disabled** (always enabled).  
   - Users can select:
     - Join **Water & Tea group**  
     - Join **Food group**  

   ✅ If “Food” is checked → the user is added to the food list.

3. **Billing Rules**
   - All users are charged for **Water & Tea**.
   - Only food group members who attended will be charged for **Food**.
   - Monthly bills are generated and shown as reports.

4. **Correction Requests**
   - Users can request correction in case of billing errors.
   - Admins can review and approve corrections.

5. **Monthly Report**
   - Displays item-wise details (water, tea, food).
   - Shows total monthly cost per user.

6. **Payment Method (New) 💳**
   - Multiple payment options:
     - Cash
     - Bank Transfer
     - EasyPaisa / JazzCash  
   - Payment status:
     - **Pending**
     - **Paid**
   - Admin can mark bills as **Paid** once confirmed.

7. **Super Admin**
   - Super Admin can reset payment status (`0`) for users who have already paid.

---

## 🛠️ Tech Stack

| Technology | Description |
|-------------|-------------|
| **ASP.NET MVC 5** | Framework used for web app |
| **C#** | Backend programming language |
| **Entity Framework** | ORM for database handling |
| **SQL Server / LocalDB** | Database |
| **Tailwind** | Frontend design |

---

📊 Future Enhancements
✅ Add Attendance Tracking
✅ Generate PDF bills
✅ Add Email Notifications
✅ Role-based dashboard analytics

---

💖 Author
👨‍💻 Muhammad Zaid Ashfaq
📍 UET Lahore | Data Science & Backend Developer
📫 LinkedIn
 • GitHub

