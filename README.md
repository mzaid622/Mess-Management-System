# 🍽️ Mess Management System

<div align="center">

![Mess Management System](https://img.shields.io/badge/Mess-Management-purple?style=for-the-badge)
![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-512BD4?style=for-the-badge&logo=.net)
![Tailwind CSS](https://img.shields.io/badge/Tailwind-CSS-38B2AC?style=for-the-badge&logo=tailwind-css)
![Stripe](https://img.shields.io/badge/Stripe-Payment-008CDD?style=for-the-badge&logo=stripe)
![SQL Server](https://img.shields.io/badge/SQL-Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)

**A modern, full-featured mess management system built with ASP.NET Core MVC**

[Features](#-features) • [Tech Stack](#-tech-stack) • [Installation](#-installation) • [Usage](#-usage) • [Screenshots](#-screenshots) • [License](#-license)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Usage](#-usage)
- [API Documentation](#-api-documentation)
- [Screenshots](#-screenshots)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 🌟 Overview

The **Mess Management System** is a comprehensive web application designed to streamline mess operations for hostels, dormitories, and cafeterias. It provides separate interfaces for administrators and users, featuring menu management, attendance tracking, automated billing, and secure online payments through Stripe.

### Why This Project?

- ✅ **Automated Billing** - Generate monthly bills automatically based on attendance
- ✅ **Real-time Tracking** - Track daily attendance and menu items
- ✅ **Secure Payments** - Integrated Stripe payment gateway
- ✅ **Role-based Access** - Separate admin and user dashboards
- ✅ **Modern UI** - Beautiful, responsive design with Tailwind CSS
- ✅ **JWT Authentication** - Secure token-based authentication

---

## ✨ Features

### 🔐 Authentication & Authorization
- JWT-based secure authentication
- Role-based access control (Admin/User)
- Password hashing with SHA-256
- HTTP-only cookie storage for enhanced security

### 👨‍💼 Admin Features
- **Dashboard**
  - Real-time statistics (users, revenue, attendance)
  - Quick access to all modules
  - Monthly revenue tracking
  
- **Menu Management**
  - Add, edit, delete menu items
  - Set prices and types (Food/Drink)
  - Daily menu scheduling
  - Validation and duplicate prevention

- **User Management**
  - View all registered users
  - Edit user details and preferences
  - Role assignment
  - Delete users with validation

- **Attendance Management**
  - Mark daily attendance for all users
  - Real-time attendance updates
  - Auto-mark mandatory drinks
  - Visual attendance tracking

- **Billing System**
  - Automatic monthly bill generation
  - Mark bills as paid/unpaid
  - View bill history and statistics
  - Delete bills with confirmation
  - PDF invoice generation

### 👤 User Features
- **Home Dashboard**
  - View today's menu
  - See attendance status
  - Billing summary (paid/unpaid/total)
  - Monthly breakdown
  - Recent bills overview

- **Bill Management**
  - View complete bill history
  - Filter by status (paid/unpaid)
  - Monthly bill view
  - Detailed bill breakdown
  - Download PDF invoices

- **Payment Integration**
  - Secure Stripe checkout
  - Pay bills online
  - Payment confirmation
  - Transaction history

- **Attendance History**
  - View all attendance records
  - Filter by date/type
  - Statistics dashboard

---

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0 MVC
- **Language:** C# 12
- **Authentication:** JWT (JSON Web Tokens)
- **Database:** SQL Server (LocalDB)
- **ORM:** Entity Framework Core

### Frontend
- **CSS Framework:** Tailwind CSS 3.4
- **JavaScript:** jQuery 3.6
- **Icons:** Heroicons (SVG)
- **Styling:** Custom gradient themes

### Payment
- **Gateway:** Stripe Checkout
- **SDK:** Stripe.NET
- **Security:** Webhook validation

### Tools & Libraries
- **PDF Generation:** Custom HTML to PDF service
- **Password Hashing:** SHA-256
- **Validation:** Data Annotations + Server-side validation

---

## 🏗️ Architecture

```
Project Structure:
├── Controllers/
│   ├── AdminController.cs
│   ├── AdminAttendanceController.cs
│   ├── AdminMenuController.cs
│   ├── AdminUserController.cs
│   ├── LoginController.cs
│   ├── RegisterController.cs
│   ├── PaymentController.cs
│   ├── UserBillController.cs
│   └── UserMenuController.cs
├── Models/
│   ├── User.cs
│   ├── Menu.cs
│   ├── Bill.cs
│   ├── Attendance.cs
│   ├── ApplicationDbContext.cs
│   └── AdminAttendanceViewModel.cs
├── Services/
│   ├── JwtService.cs
│   ├── PdfService.cs
│   └── StripeService.cs
├── Views/
│   ├── Admin/
│   ├── AdminAttendance/
│   ├── AdminMenu/
│   ├── AdminUser/
│   ├── Login/
│   ├── Register/
│   ├── Payment/
│   ├── UserBill/
│   └── UserMenu/
└── wwwroot/
    ├── css/
    │   ├── tailwind.css
    │   └── styles.css
    └── js/
```

### Database Schema

```sql
Users
├── Id (PK)
├── FullName
├── Email (Unique)
├── PasswordHash
├── Role
├── Drink (bool)
└── Lunch (bool)

Menus
├── Id (PK)
├── Name
├── Price
├── Date
└── IsFood (bool)

Bills
├── Id (PK)
├── UserId (FK)
├── Amount
├── Date
└── Paid (bool)

Attendances
├── Id (PK)
├── UserId (FK)
├── MenuId (FK)
└── Attended (bool)
```

---

## 🚀 Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for Tailwind CSS)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
- [Stripe Account](https://stripe.com/) (for payments)

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/mess-management-system.git
cd mess-management-system
```

### Step 2: Install .NET Dependencies

```bash
dotnet restore
```

### Step 3: Install Tailwind CSS

```bash
npm install
```

### Step 4: Build Tailwind CSS

```bash
npm run build:css
```

### Step 5: Update Database Connection

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=Mess_Management_DB;Trusted_Connection=True;"
  }
}
```

### Step 6: Run Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 7: Configure Stripe Keys

Get your keys from [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys)

Update `appsettings.json`:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_YOUR_KEY_HERE",
    "SecretKey": "sk_test_YOUR_KEY_HERE",
    "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET"
  }
}
```

### Step 8: Run the Application

```bash
dotnet run
```

Navigate to `https://localhost:5001`

---

## ⚙️ Configuration

### JWT Settings

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKey_MinimumLength32Characters!",
    "Issuer": "MessManagementSystem",
    "Audience": "MessManagementSystemUsers",
    "ExpiryInHours": 24
  }
}
```

### Tailwind CSS Development

For development with auto-rebuild:

```bash
npm run watch:css
```

For production build:

```bash
npm run build:css
```

---

## 📖 Usage

### First Time Setup

1. **Register as First User** - Automatically becomes Admin
2. **Login as Admin**
3. **Add Menu Items** - Set up daily menus
4. **Create Users** - Add mess members
5. **Mark Attendance** - Track daily attendance
6. **Generate Bills** - Create monthly bills
7. **Users Pay Bills** - Via Stripe checkout

### Admin Workflow

```
Dashboard → Menu Management → Add Daily Menu
         ↓
    Attendance → Mark Attendance
         ↓
    Billing → Generate Monthly Bills
         ↓
    View Bills → Mark as Paid (manual) or Users Pay Online
```

### User Workflow

```
Login → Home → View Today's Menu & Attendance
      ↓
  Bill History → View Bills → Pay Online (Stripe)
      ↓
  Payment Success → Bill Marked as Paid
```

---

## 🔌 API Documentation

### Authentication Endpoints

#### POST /Login/Login_Page
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:** JWT Token in HTTP-only cookie

### Payment Endpoints

#### POST /Payment/CreateCheckoutSession
```json
{
  "billId": 1
}
```

**Response:** Redirect to Stripe Checkout

#### GET /Payment/Success?session_id={CHECKOUT_SESSION_ID}
**Response:** Payment confirmation page

#### POST /Payment/Webhook
**Stripe Webhook:** Handles checkout.session.completed events

---

## 📸 Screenshots

### Admin Dashboard
![Admin Dashboard](https://via.placeholder.com/800x450/7C3AED/FFFFFF?text=Admin+Dashboard)

### Menu Management
![Menu Management](https://via.placeholder.com/800x450/EC4899/FFFFFF?text=Menu+Management)

### Attendance Tracking
![Attendance](https://via.placeholder.com/800x450/10B981/FFFFFF?text=Attendance+Tracking)

### User Dashboard
![User Dashboard](https://via.placeholder.com/800x450/3B82F6/FFFFFF?text=User+Dashboard)

### Stripe Payment
![Stripe Payment](https://via.placeholder.com/800x450/6366F1/FFFFFF?text=Stripe+Payment)

### Bill History
![Bill History](https://via.placeholder.com/800x450/8B5CF6/FFFFFF?text=Bill+History)

---

## 🧪 Testing

### Test Cards (Stripe)

| Card Number | Purpose |
|-------------|---------|
| `4242 4242 4242 4242` | Success |
| `4000 0000 0000 0002` | Declined |
| `4000 0000 0000 9995` | Insufficient Funds |

**Test Details:**
- **Expiry:** Any future date (e.g., 12/34)
- **CVC:** Any 3 digits (e.g., 123)
- **ZIP:** Any 5 digits (e.g., 12345)

### Running Tests

```bash
dotnet test
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. **Commit your changes**
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/AmazingFeature
   ```
5. **Open a Pull Request**

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation comments
- Write unit tests for new features

---

## 🐛 Known Issues

- [ ] PDF generation requires Chrome/Chromium for best results
- [ ] Webhook validation requires HTTPS in production
- [ ] Date handling may vary based on timezone

---

## 🔮 Future Enhancements

- [ ] Email notifications for bills and payments
- [ ] SMS alerts for attendance
- [ ] Mobile app (React Native)
- [ ] Multi-language support
- [ ] Advanced reporting and analytics
- [ ] QR code-based attendance
- [ ] Integration with more payment gateways
- [ ] Dark mode theme

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024 Muhammad Zaid

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 📞 Contact

**Your Name**
- GitHub: [[@yourusername](https://github.com/yourusername)](https://github.com/mzaid622)
- Email: M.Zaid990447@gmail.com
- LinkedIn: [[Your LinkedIn](https://linkedin.com/in/yourprofile)](https://www.linkedin.com/in/zaid-mughal-a34785359/)

**Project Link:** [https://github.com/yourusername/mess-management-system](https://github.com/yourusername/mess-management-system)

---

## 🙏 Acknowledgments

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core)
- [Tailwind CSS](https://tailwindcss.com)
- [Stripe](https://stripe.com/docs)
- [Heroicons](https://heroicons.com)
- [jQuery](https://jquery.com)

---

<div align="center">

**⭐ Star this repo if you find it helpful! ⭐**

Made with ❤️ by [Muhammad Zaid]

</div>
