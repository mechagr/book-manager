# Book Manager (April 2025)

**Book Manager** is a console-based library management application built with C# and .NET. It allows users to maintain a personal book collection, demonstrating file persistence, structured data handling, and interactive console UI design.

This project showcases the ability to manage collections, serialise data to JSON, validate user input, and create a feature-rich yet lightweight CRUD system in the console.

---

## Project Overview

Book Manager helps users build and maintain a personal book library. You can:

- Add, edit, and delete books
- Search and sort your collection
- Save data persistently in JSON format
- Export your library to CSV for external use

The application demonstrates:

- **Data serialisation and persistence** with **System.Text.Json**
- **CRUD operations** on in-memory collections
- **Input validation** and safe handling of user input
- **Dynamic console-based menus** with table formatting
- **File export and import techniques** for interoperability

---

## Features

- **Add, Edit, and Delete Books** – fully functional CRUD system
- **Search and Sort** – flexible lookup and ordering of books
- **JSON Persistence** – automatic save and load from disk
- **CSV Export** – share your library with spreadsheets
- **Sample Data Seeder** – populate the application with test data
- **Input Validation** – prevents crashes and ensures data integrity
- **Interactive Console Menu** – intuitive and user-friendly navigation

---

## Technologies Used

- **Language:** C#
- **Framework:** .NET Console Application
- **Libraries:** System.Text.Json, System.IO
- **Concepts:** data serialisation, CRUD operations, collection management, input validation

---

## How to Run

Clone the repository and run the application locally:

```bash
git clone https://github.com/your-username/book-manager.git
cd book-manager
dotnet run
