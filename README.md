# Sensitive Data Storage Backend (C++)

A backend-oriented C++ project for securely storing and managing sensitive data.

The project focuses on security, clean architecture, and real-world backend patterns, including encryption, key derivation, database abstraction, and API design.  
It started as a CLI application and evolved into a REST-based backend system.


## Overview

This project is an educational backend system designed to simulate secure storage of sensitive user data.

It demonstrates:
- secure handling of user credentials
- encrypted storage of sensitive fields
- layered architecture
- database abstraction
- test-driven development
- gradual evolution from CLI to REST API


## Features

### Core functionality
- User registration, authentication, and deletion
- Secure storage of user-scoped sensitive records
- Create, update, delete, and list stored records
- SQLite database with automatic initialization
- Environment-based configuration
- Logging with configurable log path

### Security
- Password hashing using **PBKDF2** (100,000 iterations)
- **AES-256-GCM** encryption for sensitive data (OpenSSL)
- Key derivation from user password
- Clear separation between authentication and data encryption logic

### Interfaces
- CLI interface for interacting with the system
- REST API backend (JSON-based)

### Testing
- Unit tests using **Catch2**
- Test coverage for core components:
  - UserManager
  - AccountManager (data records)
  - DatabaseManager
  - PasswordHasher
  - EncryptionManager


## Environment variables

These variables configure the database and log file locations:

```bash
export PASSWORD_MANAGER_DB_PATH="$HOME/NewProject/PassManager2/data/passdb.sqlite"
export PASSWORD_MANAGER_LOG_PATH="$HOME/NewProject/PassManager2/log.txt"
```


## Build

A C++17-compatible compiler, CMake ≥ 3.14, SQLite3 and OpenSSL 3.x are required.

```bash
mkdir build
cd build
cmake ..
cmake --build .
```

After a successful build, binaries will be placed in the bin/ directory.


## Executables

CLI application

```bash
./bin/PasswordManager
```
Provides an interactive command-line interface for managing users and sensitive records.

Web backend service

```bash
./bin/PasswordManagerWeb
```
Runs an HTTP-based backend service suitable for integration with web or external clients.


## Running tests

Unit tests are built as part of the project.

```bash
cd build
ctest
``` 
or
```bash
cmake --build . --target test
```


## Roadmap

### completed
- CLI interface
- AES-GCM encryption
- User + Account management
- Environment configuration
- Catch2 tests

### in progress
-  REST API backend
-  Web interface
-  Improved error handling and observability