# Book Manager (April 2025)

Book Manager is a console-based library management application built with C# and .NET.

This project was created to explore file persistence, structured data handling, and interactive console UI design. It highlights my ability to manage collections, serialise data to JSON, validate user input, and create a feature-rich but lightweight CRUD system within the console.

---

## Project Overview

The goal of Book Manager is to let users build and maintain a personal book library. Users can add, edit, delete, search, and sort books, as well as save their collection to disk. Data is stored in JSON format for persistence between runs, and the application also supports CSV export for external use.

The application demonstrates:

- Data serialisation and persistence using **System.Text.Json**  
- CRUD (Create, Read, Update, Delete) operations on in-memory collections  
- Input validation and safe handling of user input  
- Console-based menus with dynamic table formatting  
- File export and import techniques for interoperability  

---

## Features

- **Add, Edit, and Delete Books** – fully functional CRUD system  
- **Search and Sort** – flexible lookup and ordering of books  
- **JSON Persistence** – library automatically saved to disk  
- **CSV Export** – easily share your library with spreadsheets  
- **Sample Data Seeder** – quickly populate the application with test data  
- **Input Validation** – prevents crashes and ensures data integrity  
- **Interactive Console Menu** – intuitive, user-friendly navigation  

---

## Technologies Used

- **Language**: C#  
- **Framework**: .NET Console Application  
- **Libraries**: System.Text.Json, System.IO  
- **Concepts**: data serialisation, CRUD operations, collection management, input validation  

---

## How to Run

Clone the repository and run the application locally:  

```bash
git clone https://github.com/your-username/book-manager.git
cd book-manager
dotnet run