# PriceTracker

A full-stack web application built with ASP.NET Core MVC that allows users to track products, monitor price changes over time, and manage associated tags and categories. Designed with modern UI using Tailwind CSS and dynamic JavaScript features like tag/category chips.

---

## Features

- **CRUD Operations**
  - Products: Create, Read, Update, Delete
  - Tags & Categories: Manage with real-time chips UI
- **Price Tracking**
  - Historical price chart
  - Automatically calculates average, max, min, and percent change
- **Email Notifications**
  - Sends emails when a product is added, updated, or deleted
- **Theme Settings**
  - Switch between light and dark mode (stored via user settings)
- **Soft Deletes**
  - Tags and categories are not permanently deleted, allowing recovery

---

## 🔧 Tech Stack

- **Frontend:** ASP.NET Razor Views, Tailwind CSS, Vanilla JavaScript
- **Backend:** ASP.NET Core MVC
- **Database:** Entity Framework Core with SQLite/SQL Server
- **Email:** SMTP integration (configurable in `appsettings.json`)
- **Other:** LINQ, ViewModels, Partial Views, TempData messages

---

## 🖼️ Screenshots

> - Home page
![Dashboard Screen](/img/ss1.jpg)
> - Products page
![Products Screen](/img/ss2.jpg)
> - Tags page
![Tags Screen](/img/ss3.jpg)
> - Categories page
![Categories Screen](/img/ss4.jpg)
> - Settings page
![Settings Screen](/img/ss5.jpg)

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server / SQLite
- (Optional) MailTrap or SMTP credentials

### Setup

1. Clone the repo:
   ```bash
   git clone https://github.com/yourusername/PriceTracker.git
   cd PriceTracker
2. Update your appsettings.json:
    ```json
    "SmtpSettings": {
      "Host": "smtp.example.com",
      "Port": 587,
      "Username": "your-email@example.com",
      "Password": "your-password"
    }
    ```
3. Apply migrations and run:
    ```bash
    dotnet ef database update
    dotnet run
    ```
4. Navigate to `https://localhost:<PORT>`.

###  Extra Features
- Modal-based deletion (no page refresh)

- Inline form validation

- Auto-reactivating soft-deleted tags when reused

- Clean chip-based input for tags and categories

### Challenges Overcome
- Chip-style Inputs: Managing multiple tag/category entries dynamically with proper model binding on POST.

- Soft Delete Reactivation: Ensured deleted tags/categories re-activate if reused.

- Email Integration: Set up custom SMTP and email formatting without external libraries.

### License
```
MIT License

Copyright (c) 2025 Dhruv

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