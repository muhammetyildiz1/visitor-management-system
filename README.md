# Visitor Management System

**Project Name:** Visitor Entry and Exit Management System  
**Course:** Advanced Programming Final Project  

This project was developed for use in the reception of a private company. It allows digital logging of visitor entries and exits, providing a structured system with easy access to historical records when needed.

---

## Project Overview

The system consists of two main components:  

1. **Windows Forms Application** (`form.exe`)  
   - Handles user interface for visitor entry and exit.  
   - Displays real-time updates on visitor information in dedicated panels.  
   - Maintains a log file (`log.txt`) via `logtutucu.exe` to record user activities.  
   - Uses threads to start a timer 1 second after the program launches.  
   - Registry access tracks the last user entry and exit for audit purposes.  

2. **Web API** (`webAPI.slnx`)  
   - Manages backend requests and data storage.  
   - Provides visitor information to the Windows Forms app.  

---

## Features

- Real-time visitor statistics and data panels.  
- Visitor table showing who is inside, entry and exit times, and visit details.  
- Textbox shortcuts:  
  - **❌** clears all input fields.  
  - **🔎** auto-fills visitor information based on previous records.  
- **🔄** refreshes and reorders the visitor list.  
- All logs (`logtutucu.exe` and `log.txt`) must be located in the same folder as `form.exe` (`form\form\bin\Debug`) for proper operation.

---

## Usage

1. Launch `form.exe` to open the Windows Forms application.  
2. Enter visitor details or use the search and auto-fill features.  
3. Click the entry or exit buttons to record the visit.  
4. Logs will automatically update in `log.txt`.  
5. Use the refresh button to update the visitor list.  

---

## License

This project is for educational purposes only.
